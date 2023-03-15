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
            foreach (var controller in Enumerable.Range(0, _mapping.Count)
                         .Select<int, IVirtualGamepad>(i =>
                         {
                             return _mapping[(byte)i] switch
                             {
                                 ControllerType.DualShock => _manager.CreateDualShock4Controller(),
                                 ControllerType.XBox360 => _manager.CreateXbox360Controller(),
                             };
                         }))
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

        // private long _elapsed = 0;
        // private long _average = 1000;
        private void OnInputReceived(DualSense ds, DualSenseInputState inputState)
        {
            // var ms = _timer.ElapsedMilliseconds;
            var feedback = (Vec2)(_mapping.Map(inputState, _output) ?? new Vec2());
            _inputCollector.OnStateChanged(new Vector<float, float>(feedback.X, feedback.Y));

            foreach (var controller in _output) controller.SubmitReport();
            // var elapsed = _timer.ElapsedMilliseconds - ms;
            // _elapsed = elapsed > _elapsed ? elapsed : _elapsed;
            // _average = (_average*900 + elapsed*1000*100) / 1000;
            // System.Console.WriteLine($"{_elapsed} | {_average/1000} | {elapsed}");
        }

        private void OnButtonChanged(DualSense ds, DualSenseInputStateButtonDelta change)
        {

        }

        private string? OnCommandReceived(string command, string[] arguments)
        {
            command = command.ToLower();
            return command switch
            {
                // "sense" => Sense(_converter, arguments),
                // "deadzone" => Deadzone(_converter, arguments),
                "map" => Map(_mapping, arguments),
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

        private static string? Map(Mapping.Mapping mapping, string[] args)
        {
            if (args.Length < 4) return "Command 'map' accepts at least 4 arguments [id, input, output, global]";
            
            if (!byte.TryParse(args[0], out var id)) return $"Could not convert {args[0]} to id";
            if (!mapping.TryGetValue(id, out var controllerType)) return $"Id {id} is out of range";
            if (!Enum.TryParse<InputControl>(args[1], out var input)) return $"Could not convert {args[1]} to input";
            if (!bool.TryParse(args[3], out var global)) return $"Could not convert {args[3]} to global";
            
            var converter = args.Length >= 5 && Enum.TryParse<MappingConverter>(args[3], out var c) ? c : (MappingConverter?)null;

            var argumentCount = converter == null ? 4 : 5;
            var inputArguments = args.Skip(argumentCount)
                .TakeWhile(x => Enum.TryParse<InputControl>(x, out _))
                .Select(Enum.Parse<InputControl>)
                .ToList();
            argumentCount += inputArguments.Count;
            var arguments = args.Skip(argumentCount).ToList();

            switch (controllerType)
            {
                case ControllerType.DualShock when Enum.TryParse<DualShockControl>(args[2], out var dualShockOutput):
                    mapping.AddOrReplaceMapping(id, input, dualShockOutput, converter, inputArguments, arguments, global);
                    return null;
                case ControllerType.XBox360 when Enum.TryParse<XBox360Control>(args[2], out var xBox360Output):
                    mapping.AddOrReplaceMapping(id, input, xBox360Output, converter, inputArguments, arguments, global);
                    return null;
                default:
                    return $"Could not execute command 'map' with the given arguments: {string.Join(" | ", args)}";
            }
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
    }
}