    // See https://aka.ms/new-console-template for more information

    using System.Collections;
    using System.Collections.Concurrent;
    using System.Numerics;
    using DSx.Math;
    using Nefarius.ViGEm.Client.Targets.DualShock4;

Random rand = new Random();

//await StartIntercept();

//await StartVirtual(rand);
await Start();

static async Task StartIntercept()
{
    var controllers = DualSenseAPI.DualSense.EnumerateControllers().ToList();
    var ds4 = controllers.First();
    ds4.Acquire();
    // ds4.OnButtonStateChanged += (s, args) =>
    // {
    //     Console.WriteLine("Changed");
    //     Console.WriteLine($"{nameof(args.CircleButton)}: {args.CircleButton}");
    //     Console.WriteLine($"{nameof(args.CrossButton)}: {args.CrossButton}");
    //     Console.WriteLine($"{nameof(args.SquareButton)}: {args.SquareButton}");
    //     Console.WriteLine($"{nameof(args.TriangleButton)}: {args.TriangleButton}");
    //     Console.WriteLine("State");
    //     Console.WriteLine($"{nameof(s.InputState.CircleButton)}: {s.InputState.CircleButton}");
    //     Console.WriteLine($"{nameof(s.InputState.CrossButton)}: {s.InputState.CrossButton}");
    //     Console.WriteLine($"{nameof(s.InputState.SquareButton)}: {s.InputState.SquareButton}");
    //     Console.WriteLine($"{nameof(s.InputState.TriangleButton)}: {s.InputState.TriangleButton}");
    // };
    var accXMax = 0d;
    var accYMax = 0d;
    var accZMax = 0d;
    var accXBase = 0d;
    var accYBase = 0d;
    var accZBase = 0d;
    var gyrX = new Queue<float>();
    var gyrY = new Queue<float>();
    var gyrZ = new Queue<float>();
    var accX = new Queue<float>();
    var accY = new Queue<float>();
    var accZ = new Queue<float>();
    ds4.OnStatePolled += s =>
    {
        // Console.WriteLine($"{nameof(s.InputState.CircleButton)}: {s.InputState.CircleButton}");
        // Console.WriteLine($"{nameof(s.InputState.CrossButton)}: {s.InputState.CrossButton}");
        // Console.WriteLine($"{nameof(s.InputState.SquareButton)}: {s.InputState.SquareButton}");
        // Console.WriteLine($"{nameof(s.InputState.TriangleButton)}: {s.InputState.TriangleButton}");
        if (gyrX.Count > 16) gyrX.Dequeue(); gyrX.Enqueue(s.InputState.Gyro.X);
        if (gyrY.Count > 16) gyrY.Dequeue(); gyrY.Enqueue(s.InputState.Gyro.Y);
        if (gyrZ.Count > 16) gyrZ.Dequeue(); gyrZ.Enqueue(s.InputState.Gyro.Z);
        if (accX.Count > 16) accX.Dequeue(); accX.Enqueue(s.InputState.Accelerometer.X);
        if (accY.Count > 16) accY.Dequeue(); accY.Enqueue(s.InputState.Accelerometer.Y);
        if (accZ.Count > 16) accZ.Dequeue(); accZ.Enqueue(s.InputState.Accelerometer.Z);

        if (s.InputState.Accelerometer.X > accXMax) accXMax = s.InputState.Accelerometer.X;
        if (s.InputState.Accelerometer.Y > accYMax) accYMax = s.InputState.Accelerometer.Y;
        if (s.InputState.Accelerometer.Z > accZMax) accZMax = s.InputState.Accelerometer.Z;
        
        if (s.InputState.CircleButton)
        {
            accXBase = Math.Round(accX.Average());
            accYBase = Math.Round(accY.Average());
            accZBase = Math.Round(accZ.Average());
        }
       
        // Console.WriteLine($"{nameof(s.InputState.Gyro.X)}: {Math.Round(gyrX.Average())}".PadRight(Console.WindowWidth - 1));
        // Console.WriteLine($"{nameof(s.InputState.Gyro.Y)}: {Math.Round(gyrY.Average())}".PadRight(Console.WindowWidth - 1));
        // Console.WriteLine($"{nameof(s.InputState.Gyro.Z)}: {Math.Round(gyrZ.Average())}".PadRight(Console.WindowWidth - 1));
        // Console.WriteLine($"{nameof(s.InputState.Accelerometer.X)}: {Math.Round(accX.Average())}".PadRight(Console.WindowWidth - 1));
        // Console.WriteLine($"{nameof(s.InputState.Accelerometer.Y)}: {Math.Round(accY.Average())}".PadRight(Console.WindowWidth - 1));
        // Console.WriteLine($"{nameof(s.InputState.Accelerometer.Z)}: {Math.Round(accZ.Average())}".PadRight(Console.WindowWidth - 1));
        var x = Math.Round(accX.Average() - accXBase);
        var y = Math.Round(accY.Average() - accYBase);
        var z = Math.Round(accZ.Average() - accZBase);
                
        Print(x, -8000, 8000, 0, 'X');
        Print(y, -8000, 8000, 1, 'Y');
        Print(z, -8000, 8000, 2, 'Z');

        Console.SetCursorPosition(0, 11);
        
        Console.WriteLine($"{nameof(s.InputState.Accelerometer.X)}: {x} / {accXMax} ({x*360/accXMax})".PadRight(Console.WindowWidth - 1));
        Console.WriteLine($"{nameof(s.InputState.Accelerometer.Y)}: {y} / {accYMax} ({y*360/accYMax})".PadRight(Console.WindowWidth - 1));
        Console.WriteLine($"{nameof(s.InputState.Accelerometer.Z)}: {z} / {accZMax} ({z*360/accZMax})".PadRight(Console.WindowWidth - 1));
        Console.WriteLine($"{Math.Sqrt(x*x+y*y+z*z)}".PadRight(Console.WindowWidth - 1));
    };
    ds4.BeginPolling(33);
    while (true) await Task.Delay(1000);
}

static void Print(double value, float min, float max, int pos, char id)
{
    var size = Math.Max(Math.Abs(min), Math.Abs(max));
    if (value < -size) value += size;
    if (value > size) value -= size;

    var range = Enumerable.Range(0, 5);
    foreach (var i in range)
    {
        if (value > 0 && i < value * 5 / size)
        {
            Console.SetCursorPosition(pos, 5-i);
            Console.Write('♦');
        } else
        {
            Console.SetCursorPosition(pos, 5-i);
            Console.Write(' ');
        }

        if (value < 0 && i > value * 5  / size)
        {
            Console.SetCursorPosition(pos, 5+i);
            Console.Write('♦');
        }
        else
        {
            Console.SetCursorPosition(pos, 5+i);
            Console.Write(' ');
        }
    }
    Console.SetCursorPosition(pos, 5+5);
    Console.Write(id);
}

static async Task StartVirtual(Random rand)
{
    using var client = new Nefarius.ViGEm.Client.ViGEmClient();
    using var ds4_1 = client.CreateDualShock4Controller();
    using var ds4_2 = client.CreateDualShock4Controller();
    Console.WriteLine("Created...");
    ds4_1.Connect();
    ds4_2.Connect();
    Console.WriteLine("Connected...");
    while (true)
    {
        ds4_1.SetButtonState(DualShock4Button.Circle, rand.Next(0, 5) == 0);
        ds4_1.SetButtonState(DualShock4Button.Cross, rand.Next(0, 5) == 0);
        ds4_1.SetButtonState(DualShock4Button.Square, rand.Next(0, 5) == 0);
        ds4_1.SetButtonState(DualShock4Button.Triangle, rand.Next(0, 5) == 0);
        ds4_2.SetButtonState(DualShock4Button.Circle, rand.Next(0, 5) == 0);
        ds4_2.SetButtonState(DualShock4Button.Cross, rand.Next(0, 5) == 0);
        ds4_2.SetButtonState(DualShock4Button.Square, rand.Next(0, 5) == 0);
        ds4_2.SetButtonState(DualShock4Button.Triangle, rand.Next(0, 5) == 0);
        ds4_1.SubmitReport();
        ds4_2.SubmitReport();
        Console.WriteLine("Submitted...");
        await Task.Delay(333);
    }
    Console.ReadLine();    
}

static async Task Start()
{
    var controllers = DualSenseAPI.DualSense.EnumerateControllers().ToList();
    var ds4 = controllers.First();
    
    using var client = new Nefarius.ViGEm.Client.ViGEmClient();
    var xb1 = client.CreateXbox360Controller();
    ds4.Acquire();
    xb1.Connect();

    var zero = Vector<float, float, float>.Zero;
    var max = Vector<float, float, float>.Zero;
    Vector<ConcurrentQueue<float>, ConcurrentQueue<float>, ConcurrentQueue<float>> avg = null;

    ds4.OnStatePolled += s =>
    {
        var reading = new Vector<float, float, float>(s.InputState.Accelerometer.X, s.InputState.Accelerometer.Y, s.InputState.Accelerometer.Z);
        
        if (s.InputState.Touchpad1.IsDown && s.InputState.Touchpad2.IsDown && s.InputState.TouchpadButton)
        {
            avg ??= new Vector<ConcurrentQueue<float>, ConcurrentQueue<float>, ConcurrentQueue<float>>(new ConcurrentQueue<float>(), new ConcurrentQueue<float>(), new ConcurrentQueue<float>());
            if (avg.X.Count > 3) avg.X.TryDequeue(out var _);
            if (avg.Y.Count > 3) avg.Y.TryDequeue(out var _);
            if (avg.Z.Count > 3) avg.Z.TryDequeue(out var _);
            avg.X.Enqueue(s.InputState.Accelerometer.X);
            avg.Y.Enqueue(s.InputState.Accelerometer.Y);
            avg.Z.Enqueue(s.InputState.Accelerometer.Z);
            zero = new Vector<float, float, float>(0, avg.Y.Average(), avg.Z.Average()).Normalize();
        } else if (avg != null) avg = null;
        
        Console.Clear();
        Console.WriteLine($"Raw: {reading}");
        Console.WriteLine($"Zero: {zero}");
        Console.WriteLine($"Normalized zero: {zero.Normalize()}");
        Console.WriteLine($"Length zero: {zero.Length()}");
        Console.WriteLine($"Angle: {zero.Angle(reading, AngleRepresentation.Degrees)}");
        Console.WriteLine($"Angle X: {zero.Angle(reading.Project(zero, new Vector<float, float, float>(1, 0, 0).Cross(zero)), AngleRepresentation.Degrees)}");
        Console.WriteLine($"Angle Y: {zero.Angle(reading.Project(new Vector<float, float, float>(1, 0, 0), zero.Cross(new Vector<float, float, float>(1, 0, 0))), AngleRepresentation.Degrees)}");
        Console.WriteLine($"Angle Z: {zero.Angle(reading.Project(new Vector<float, float, float>(1, 0, 0), zero), AngleRepresentation.Degrees)}");
        
    };
    ds4.BeginPolling(100);
    while (true) await Task.Delay(1000);
}