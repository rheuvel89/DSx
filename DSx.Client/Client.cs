using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DSx.Input;
using DSx.Mapping;
using DSx.Math;
using DualSenseAPI;
using DualSenseAPI.State;
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
        private readonly byte _count;

        public Client(ClientOptions options)
        {
             _manager = new ViGEmClient();
             _inputCollector = !string.IsNullOrWhiteSpace(options.Server) && options.Port != null
                ? new RemoteInputCollector(options.Server, options.Port.Value)
                : new LocalInputCollector(options.PollingInterval);
            _converter = new TiltToJoystickConverter();
            _output = new List<IVirtualGamepad>();
            _console = new Console.Console(_output, options.NoConsole);
            _count = options.Count;
            if (_count < 1 || _count > 4) throw new Exception("Invalid number of controllers to connect (must be between 1 and 4)");
            _timer = new Stopwatch();
        }

        public async Task Initialize(object mapping)
        {
            foreach(var controller in  Enumerable.Range(0, _count)
                .Select(i => _manager.CreateDualShock4Controller((ushort)i, (ushort)i)))
            {
                _output.Add(controller);
                controller.Connect();
            }
            
            _inputCollector.OnInputReceived += OnInputReceived;
            _inputCollector.OnButtonChanged += OnButtonChanged;

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
            var activeId = (ds.InputState.L1Button, ds.InputState.R1Button) switch
            {
                (false, false) when _count >= 1 => 0,
                (true, false) when _count >= 2 => 1,
                (false, true) when _count >= 3 => 2,
                (true, true) when _count >= 4 => 3,
                _ => 0,
            };
            MappingFunctions.CopyState(ds, _output[activeId], false, activeId == 0);
            for (int id = 0 ; id < _output.Count ; id++) if (id != activeId) _output[id].ResetReport();

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

            if (_count >= 2)
            {
                MappingFunctions.MapGyro(pitchAndRoll, _output[1]);
                ds.OutputState.LeftRumble = rumble.X;
                ds.OutputState.RightRumble = rumble.Y;

            }
            
            foreach (var controller in _output) controller.SubmitReport();
        }

        private void OnButtonChanged(DualSense ds, DualSenseInputStateButtonDelta change)
        {
            
        }
        
        private string? OnCommandReceived(string command, string[] arguments)
        {
            command = command.ToLower();
            return command switch
            {
                "sense" => Sense(_converter, arguments),
                "deadzone" => Deadzone(_converter, arguments),
                _ => $"Command {command} not recognized"
            };
        }

        private static string? Sense(TiltToJoystickConverter converter, string[] args)
        {
            if (args.Length != 1 || !float.TryParse(args[0], out var sense)) return "Command 'sense' accepts 1 argument (decimal)";
            converter.Sensitivity = sense;
            return null;
        }

        private static string? Deadzone(TiltToJoystickConverter converter, string[] args)
        {
            if (args.Length != 1 || !float.TryParse(args[0], out var deadzone)) return "Command 'deadzone' accepts 1 argument (decimal)";
            converter.Deadzone = deadzone;
            return null;
        }

    }
}