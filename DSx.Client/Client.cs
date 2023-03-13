using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DSx.Input;
using DSx.Mapping;
using DSx.Math;
using DualSenseAPI;
using DualSenseAPI.State;
using DXs.Common;
using Nefarius.ViGEm.Client;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace DSx.Client
{
    public class Client : IApplication
    {
        private readonly ViGEmClient _manager;
        private readonly InputCollector _inputCollector;
        private readonly TiltToStickConverter _converter;
        private readonly IList<IVirtualGamepad> _output;
        private readonly DSx.Console.Console _console;
        private readonly Stopwatch _timer;
        private readonly byte _count;
        private Mapping.Mapping _mapping;

        public Client(ClientOptions options)
        {
            _manager = new ViGEmClient();
            _inputCollector = options.Port != null
                ? new RemoteInputCollector(options.Port.Value)
                : new LocalInputCollector(options.PollingInterval);
            _converter = new TiltToStickConverter();
            _output = new List<IVirtualGamepad>();
            _console = new Console.Console(_output, options.NoConsole);
            _count = options.Count;
            if (_count < 1 || _count > 4)
                throw new Exception("Invalid number of controllers to connect (must be between 1 and 4)");
            _timer = new Stopwatch();

            _mapping = LoadMapping(options.MappingPath ?? "config.yaml");
        }

        public async Task Initialize()
        {
            foreach (var controller in Enumerable.Range(0, _count)
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
            await Initialize();

            _timer.Start();

            await _inputCollector.Start();
            await _console.Attach();
        }

        private void OnInputReceived(DualSense ds, DualSenseInputState inputState)
        {
            // var timestamp = _timer.ElapsedMilliseconds;
            // var activeId = (inputState.L1Button, inputState.R1Button) switch
            // {
            //     (false, false) when _count >= 1 => 0,
            //     (true, false) when _count >= 2 => 1,
            //     (false, true) when _count >= 3 => 2,
            //     (true, true) when _count >= 4 => 3,
            //     _ => 0,
            // };
            // MappingFunctions.CopyState(inputState, _output[activeId], false, activeId == 0);
            // for (int id = 0; id < _output.Count; id++)
            //     if (id != activeId)
            //         _output[id].ResetReport();
            //
            // var reZero = inputState.Touchpad1.IsDown && !inputState.Touchpad2.IsDown && inputState.TouchpadButton;
            // var toggle = inputState.Touchpad1.IsDown && inputState.Touchpad2.IsDown && inputState.TouchpadButton;
            // var rAcc = new Vector<float, float, float>(
            //     inputState.Accelerometer.X,
            //     inputState.Accelerometer.Y,
            //     inputState.Accelerometer.Z
            // );
            // var rGyr = new Vector<float, float, float>(
            //     inputState.Gyro.X,
            //     inputState.Gyro.Y,
            //     inputState.Gyro.Z
            // );
            // var pitchAndRoll = _converter.Convert(timestamp, rAcc, rGyr, reZero, toggle, out var rumble);
            //
            // MappingFunctions.MapGyro(pitchAndRoll, _output[1]);
            // _inputCollector.OnStateChanged(rumble);
            
            _mapping.Map(ds, _output);

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

        private static string? Sense(TiltToStickConverter converter, string[] args)
        {
            if (args.Length != 1 || !float.TryParse(args[0], out var sense))
                return "Command 'sense' accepts 1 argument (decimal)";
            converter.Sensitivity = sense;
            return null;
        }

        private static string? Deadzone(TiltToStickConverter converter, string[] args)
        {
            if (args.Length != 1 || !float.TryParse(args[0], out var deadzone))
                return "Command 'deadzone' accepts 1 argument (decimal)";
            converter.Deadzone = deadzone;
            return null;
        }

        private Mapping.Mapping LoadMapping(string mappingPath)
        {
            var yaml = File.ReadAllText(mappingPath);
            var mappingConfiguration =  new DeserializerBuilder()
                .WithTagMapping("!DualShock", typeof(DualShockControlConfiguration))
                .WithTagMapping("!XBox360", typeof(Xbox360ControlConfiguration))
                .Build()
                .Deserialize<MappingConfiguration>(yaml);

            var mapping = new Mapping.Mapping(mappingConfiguration);
            return mapping;
        }


        public class PopulationNodeResolver : INodeTypeResolver
        {
            public bool Resolve(NodeEvent? nodeEvent, ref Type currentType)
            {
                if (nodeEvent.Tag.IsEmpty) return false;
                var returnValue =  currentType != (currentType = nodeEvent.Tag.Value switch
                {
                    "!DualShock" => typeof(DualShockControlConfiguration),
                    "!XBox360" => typeof(Xbox360ControlConfiguration),
                    _ => currentType
                });
                return returnValue;
            }
        }
    }
}