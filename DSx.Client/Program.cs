// See https://aka.ms/new-console-template for more information

using System.Collections.Concurrent;
using System.Reactive.Linq;
using DSx.Math;

Random rand = new Random();

await Start();

float _sense = 1;

static async Task Start()
{
    var controllers = DualSenseAPI.DualSense.EnumerateControllers().ToList();
    var ds4 = controllers.First();
    
    using var client = new Nefarius.ViGEm.Client.ViGEmClient();
    var xb1 = client.CreateXbox360Controller();
    ds4.Acquire();
    xb1.Connect();

    var zero = Vector<float, float, float>.Zero;
    Vector<ConcurrentQueue<float>, ConcurrentQueue<float>, ConcurrentQueue<float>> rAcc =  new Vector<ConcurrentQueue<float>, ConcurrentQueue<float>, ConcurrentQueue<float>>(
        new ConcurrentQueue<float>(), new ConcurrentQueue<float>(), new ConcurrentQueue<float>()
    );
    Vector<ConcurrentQueue<float>, ConcurrentQueue<float>, ConcurrentQueue<float>> rGy =  new Vector<ConcurrentQueue<float>, ConcurrentQueue<float>, ConcurrentQueue<float>>(
        new ConcurrentQueue<float>(), new ConcurrentQueue<float>(), new ConcurrentQueue<float>()
    );
    Vector<ConcurrentQueue<float>, ConcurrentQueue<float>, ConcurrentQueue<float>> rEst =  new Vector<ConcurrentQueue<float>, ConcurrentQueue<float>, ConcurrentQueue<float>>(
        new ConcurrentQueue<float>(), new ConcurrentQueue<float>(), new ConcurrentQueue<float>()
    );
    Vector<ConcurrentQueue<float>, ConcurrentQueue<float>, ConcurrentQueue<float>> rZero = null;
    var rPitch = 0d; var rRoll = 0d;
    

    using var _ = Observable.Interval(TimeSpan.FromMilliseconds(333)).Subscribe(_ =>
    {
        Console.Clear();
        Console.WriteLine($"Pitch: {rPitch}");
        Console.WriteLine($"Roll: {rRoll}");
    });
    
    ds4.OnStatePolled += s =>
    {
        rAcc.AddAverage(new Vector<float, float, float>(s.InputState.Accelerometer.X, s.InputState.Accelerometer.Y, s.InputState.Accelerometer.Z), 1);
        if (s.InputState.Touchpad1.IsDown && s.InputState.Touchpad2.IsDown && s.InputState.TouchpadButton)
        {
            rZero ??= new Vector<ConcurrentQueue<float>, ConcurrentQueue<float>, ConcurrentQueue<float>>(new ConcurrentQueue<float>(), new ConcurrentQueue<float>(), new ConcurrentQueue<float>());
            zero = rZero.AddAverage(rAcc.Index(0), 10);
        } else if (rZero != null) rZero = null;

        var r = rAcc.Index(0);
        var zRoll = Math.Atan2(zero.Y, zero.Z);
        var zPitch = Math.Atan2(-zero.X, Math.Sqrt(zero.Y*zero.Y+zero.Z*zero.Z));
        rRoll = Math.Atan2(r.Y, r.Z) - zRoll;
        rPitch = Math.Atan2(-r.X, Math.Sqrt(r.Y*r.Y+r.Z*r.Z)) - zPitch;
        if (rPitch > 1) rPitch = 1;
        if (rPitch < -1) rPitch = -1;
        if (rRoll > 1) rRoll = 1;
        if (rRoll < -1) rRoll = -1;

        if (rPitch > 0.8 || rPitch < -0.8 || rRoll > 0.8 || rRoll < -0.8)
            s.OutputState.LeftRumble = s.OutputState.RightRumble = 0.01f;
        else s.OutputState.LeftRumble = s.OutputState.RightRumble = 0f;

        xb1.LeftThumbX = (short)(rPitch * short.MaxValue);
        xb1.LeftThumbY = (short)(rRoll * short.MaxValue);;
        
        xb1.SubmitReport();
    };
    ds4.BeginPolling(10);
    while (true) await Task.Delay(1000);
}