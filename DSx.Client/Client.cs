using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DSx.Input;
using DSx.Mapping;
using DSx.Math;
using DualSenseAPI;
using DXs.Common;
using Nefarius.ViGEm.Client;

namespace DSx.Client
{
    public class Client : IApplication
    {
        private readonly ViGEmClient _manager;
        private readonly InputCollector _inputCollector; 
        private readonly TiltToJoystickConverter _converter;
        private readonly IList<IVirtualGamepad> _output;
        private readonly DSx.Console.Console _console;
        private readonly Stopwatch _timer;

        public Client(ClientOptions options)
        {
             _manager = new ViGEmClient();
             _inputCollector = !string.IsNullOrWhiteSpace(options.Server) && options.Port != null
                ? new RemoteInputCollector(options.Server, options.Port.Value)
                : new LocalInputCollector(options.PollingInterval);
            _converter = new TiltToJoystickConverter();
            _output = new List<IVirtualGamepad>();
            _console = new Console.Console(_output);
            _timer = new Stopwatch();
        }

        public async Task Initialize(object mapping)
        {
            foreach(var controller in  Enumerable.Range(0, 4)
                .Select(_ => _manager.CreateDualShock4Controller()))
            {
                _output.Add(controller);
                controller.Connect();
            }
            
            _inputCollector.OnInputReceived += OnInputReceived;

            _console.OnCommandReceived += OnCommandReceived;
        }
        
        public async Task Start()
        {
            await Initialize(null);

            _timer.Start();

            await _inputCollector.Start();
            await _console.Attach();
        }
        
        private void OnInputReceived(DualSense ds)
        {
            var timestamp = _timer.ElapsedMilliseconds;
            foreach (var controller in _output) controller.ResetReport();
            switch (ds.InputState.L1Button, ds.InputState.R1Button)
            {
                case (false, false): MappingFunctions.CopyState(ds, _output[0], false, true); break;
                case (true, false): MappingFunctions.CopyState(ds, _output[1], false, false); break;
                case (false, true): MappingFunctions.CopyState(ds, _output[2], false, false); break;
                case (true, true): MappingFunctions.CopyState(ds, _output[3], false, false); break;
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
            
            MappingFunctions.MapGyro(pitchAndRoll, _output[1]);
            
            ds.OutputState.LeftRumble = rumble.X;
            ds.OutputState.RightRumble = rumble.Y;

            foreach (var controller in _output) controller.SubmitReport();
        }

        private string OnCommandReceived(string command, string[] arguments)
        {
            return null;
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

    }
}