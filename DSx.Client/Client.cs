using System.Diagnostics;
using System.Reactive.Linq;
using CoreDX.vJoy.Wrapper;
using DSx.Mapping;
using DSx.Math;
using DSx.VJoy;
using DualSenseAPI;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.DualShock4;
using Nefarius.ViGEm.Client.Targets.Xbox360;

namespace DSx.Client
{
    public class Client
    {
        private uint _pollingInterval;
        private TiltToJoystickConverter _converter;
        private Stopwatch _timer = Stopwatch.StartNew();

        public Client(uint pollingInterval)
        {
            _pollingInterval = pollingInterval;
            _converter = new TiltToJoystickConverter(pollingInterval);
        }
        
        public async Task Start()
        {
            var controllers = DualSense.EnumerateControllers().ToList();
            var dsi1 = controllers.FirstOrDefault();
            dsi1?.Acquire();

            using var vigemManager = new Nefarius.ViGEm.Client.ViGEmClient();
            var output = new[]
            {
                vigemManager.CreateDualShock4Controller(),
                vigemManager.CreateDualShock4Controller(),
                vigemManager.CreateDualShock4Controller(),
                vigemManager.CreateDualShock4Controller()
            };
            foreach (var controller in output) controller.Connect();
            
            using var vJoymanager = VJoyControllerManager.GetManager();
            var joysticks = vJoymanager.EnumerateControllers().ToList();

            using var _ = Observable.Interval(TimeSpan.FromMilliseconds(333)).Subscribe(OnScreenRefresh);

            dsi1.OnStatePolled += s => OnStatePolled(s, output);
            dsi1.BeginPolling(_pollingInterval);
            
            string? input = null;
            while (true)
            {
                input = Console.ReadLine();
                var split = input.Split(" ");
                var command = split.First();
                var args = split.Skip(1).ToArray();
                if (CommandActions.TryGetValue(command, out var action)) action(_converter, args);
            }
        }

        private void OnScreenRefresh(long time)
        {
            
        }
        
        private void OnStatePolled(DualSense ds, IDualShock4Controller[] output)
        {
            var timestamp = _timer.ElapsedMilliseconds;
            foreach (var controller in output) controller.ResetReport();
            switch (ds.InputState.L1Button, ds.InputState.R1Button)
            {
                case (false, false): CopyState(ds, output[0], false, true); break;
                case (true, false): CopyState(ds, output[1], false, false); break;
                case (false, true): CopyState(ds, output[2], false, false); break;
                case (true, true): CopyState(ds, output[3], false, false); break;
            }

            var reZero = ds.InputState.Touchpad1.IsDown && !ds.InputState.Touchpad2.IsDown && ds.InputState.TouchpadButton;
            var toggle = ds.InputState.Touchpad1.IsDown && ds.InputState.Touchpad2.IsDown && ds.InputState.TouchpadButton;
            var rAcc = new Vector<float, float, float>(
                ds.InputState.Accelerometer.X,
                ds.InputState.Accelerometer.Y,
                ds.InputState.Accelerometer.Z
            );
            var rGyr = new Vector<float, float, float>(
                ds.InputState.Gyro.X,
                ds.InputState.Gyro.Y,
                ds.InputState.Gyro.Z
            );
            var pitchAndRoll = _converter.Convert(timestamp, rAcc, rGyr, reZero, toggle, out var rumble);
            
            Console.SetCursorPosition(0, 0);
            Console.WriteLine($"Pitch {pitchAndRoll.X}".PadRight(Console.WindowWidth - 1));
            Console.WriteLine($"Roll  {pitchAndRoll.Y}".PadRight(Console.WindowWidth - 1));
            Console.WriteLine($"Yaw   {pitchAndRoll.Z}".PadRight(Console.WindowWidth - 1));
            
            output[1].LeftThumbX = (byte)(byte.MaxValue / 2 - pitchAndRoll.X * byte.MaxValue / 2);
            output[1].LeftThumbY = (byte)(byte.MaxValue / 2 - pitchAndRoll.Y * byte.MaxValue / 2);
            
            ds.OutputState.LeftRumble = rumble.X;
            ds.OutputState.RightRumble = rumble.Y;

            foreach (var controller in output) controller.SubmitReport();
        }

        IDictionary<string, Action<TiltToJoystickConverter, string[]>> CommandActions =
            new Dictionary<string, Action<TiltToJoystickConverter, string[]>>
        {
            ["sense"] = Sense,
            ["deadzone"] = Deadzone,
        };

        private static void Sense(TiltToJoystickConverter converter, string[] args)
        {
            if (args.Length != 1 || !float.TryParse(args[0], out var sense)) return;
            converter.Sensitivity = sense;
        }

        private static void Deadzone(TiltToJoystickConverter converter, string[] args)
        {
            if (args.Length != 1 || !float.TryParse(args[0], out var deadzone)) return;
            converter.Deadzone = deadzone;
        }

        private static void CopyState(DualSense input, IXbox360Controller output)
        {
            output.LeftThumbX = (short)(input.InputState.LeftAnalogStick.X * short.MaxValue);
            output.LeftThumbY = (short)(input.InputState.LeftAnalogStick.Y * short.MaxValue);
            output.LeftTrigger = (byte)(input.InputState.L2 * byte.MaxValue); 
            output.SetButtonState(Xbox360Button.LeftShoulder, input.InputState.L1Button);
            output.SetButtonState(Xbox360Button.LeftThumb, input.InputState.L3Button);
            
            output.RightThumbX = (short)(input.InputState.RightAnalogStick.X * short.MaxValue);
            output.RightThumbY = (short)(input.InputState.RightAnalogStick.Y * short.MaxValue);
            output.RightTrigger = (byte)(input.InputState.R2 * byte.MaxValue);
            output.SetButtonState(Xbox360Button.RightShoulder, input.InputState.R1Button);
            output.SetButtonState(Xbox360Button.RightThumb, input.InputState.R3Button);
            
            output.SetButtonState(Xbox360Button.Up, input.InputState.DPadUpButton);
            output.SetButtonState(Xbox360Button.Right, input.InputState.DPadRightButton);
            output.SetButtonState(Xbox360Button.Down, input.InputState.DPadDownButton);
            output.SetButtonState(Xbox360Button.Left, input.InputState.DPadLeftButton);
            
            output.SetButtonState(Xbox360Button.Y, input.InputState.TriangleButton);
            output.SetButtonState(Xbox360Button.B, input.InputState.CircleButton);
            output.SetButtonState(Xbox360Button.A, input.InputState.CrossButton);
            output.SetButtonState(Xbox360Button.X, input.InputState.SquareButton);
            
            output.SetButtonState(Xbox360Button.Guide, input.InputState.LogoButton);
            output.SetButtonState(Xbox360Button.Back, input.InputState.CreateButton);
            output.SetButtonState(Xbox360Button.Start, input.InputState.MenuButton);
        }

        private static void CopyState(DualSense input, IDualShock4Controller output, bool mapShoulder = true, bool mapSticks = true)
        {
            if (mapSticks) output.LeftThumbX = (byte)(byte.MaxValue / 2 - input.InputState.LeftAnalogStick.X * byte.MaxValue / 2);
            if (mapSticks) output.LeftThumbY = (byte)(byte.MaxValue / 2 - input.InputState.LeftAnalogStick.Y * byte.MaxValue / 2);
            output.LeftTrigger = (byte)(input.InputState.L2 * byte.MaxValue); 
            if (mapShoulder) output.SetButtonState(DualShock4Button.ShoulderLeft, input.InputState.L1Button);
            output.SetButtonState(DualShock4Button.ThumbLeft, input.InputState.L3Button);
            
            if (mapSticks) output.RightThumbX = (byte)(byte.MaxValue / 2 - input.InputState.RightAnalogStick.X * byte.MaxValue / 2);
            if (mapSticks) output.RightThumbY = (byte)(byte.MaxValue / 2 - input.InputState.RightAnalogStick.Y * byte.MaxValue / 2);
            output.RightTrigger = (byte)(input.InputState.R2 * byte.MaxValue);
            if (mapShoulder) output.SetButtonState(DualShock4Button.ShoulderRight, input.InputState.R1Button);
            output.SetButtonState(DualShock4Button.ThumbRight, input.InputState.R3Button);

            var dpadDir = (input.InputState.DPadUpButton, input.InputState.DPadRightButton,
                    input.InputState.DPadDownButton, input.InputState.DPadLeftButton) switch
                {
                    (false, false, false, false) => DualShock4DPadDirection.None,
                    (true, false, false, false) => DualShock4DPadDirection.North,
                    (false, true, false, false) => DualShock4DPadDirection.East,
                    (false, false, true, false) => DualShock4DPadDirection.South,
                    (false, false, false, true) => DualShock4DPadDirection.West,
                    (true, true, false, false) => DualShock4DPadDirection.Northeast,
                    (true, false, true, false) => DualShock4DPadDirection.None,
                    (true, false, false, true) => DualShock4DPadDirection.Northwest,
                    (false, true, true, false) => DualShock4DPadDirection.Southeast,
                    (false, true, false, true) => DualShock4DPadDirection.None,
                    (false, false, true, true) => DualShock4DPadDirection.Southwest,
                    (true, true, true, false) => DualShock4DPadDirection.East,
                    (false, true, true, true) => DualShock4DPadDirection.South,
                    (true, false, true, true) => DualShock4DPadDirection.West,
                    (true, true, false, true) => DualShock4DPadDirection.North,
                    (true, true, true, true) => DualShock4DPadDirection.Southwest,
                };
            output.SetDPadDirection(dpadDir);
            
            output.SetButtonState(DualShock4Button.Triangle, input.InputState.TriangleButton);
            output.SetButtonState(DualShock4Button.Circle, input.InputState.CircleButton);
            output.SetButtonState(DualShock4Button.Cross, input.InputState.CrossButton);
            output.SetButtonState(DualShock4Button.Square, input.InputState.SquareButton);
            
            output.SetButtonState(DualShock4Button.Share, input.InputState.CreateButton);
            output.SetButtonState(DualShock4Button.Options, input.InputState.MenuButton);
        }
    }
}