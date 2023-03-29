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
using YamlDotNet.Serialization;

namespace DSx.Client
{
    public class Client : IApplication
    {
        private readonly ViGEmClient _manager;
        private readonly InputCollector _inputCollector;
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
                                 ControllerType.DualShock => _manager.CreateDualShock4Controller(0x7331, (ushort)i),
                                 ControllerType.XBox360 => _manager.CreateXbox360Controller(0x7331, (ushort)i),
                             };
                         }))
            {
                _output.Add(controller);
                controller.Connect();
                await Task.Delay(TimeSpan.FromSeconds(1));
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
        private void OnInputReceived(DualSense? ds, DualSenseInputState inputState)
        {
            // var ms = _timer.ElapsedMilliseconds;
            var feedback = _mapping.Map(inputState, _output);

            if (ds != null)
            {
                feedback.Color = ds.IoMode == IoMode.USB
                    ? new Vec3()
                    : new Vec3 { X = (10 - ds.InputState.BatteryStatus.Level) / 10, Y = ds.InputState.BatteryStatus.Level / 10 };
            }
            _inputCollector.OnStateChanged(feedback);

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
                "remove" => Remove(_mapping, arguments),
                _ => $"Command {command} not recognized"
            };
        }

        private static string? Map(Mapping.Mapping mapping, string[] args)
        {
            if (args.Length < 5) return "Command 'map' accepts at least the following arguments: id, converter, inputs, output, global";

            var index = 0;
            if (!byte.TryParse(args[index++], out var id)) return $"Could not convert {args[index-1]} to id";
            if (!mapping.TryGetValue(id, out var controllerType)) return $"Id {id} is out of range";
            if (!Enum.TryParse<MappingConverter>(args[index++], out var converter)) return $"Could not convert {args[index-1]} to input";
            var inputs = args.Skip(index)
                .Select(x => x.Split(":"))
                .TakeWhile(x => x.Length == 2 && Enum.TryParse<InputControl>(x[1], out _))
                .ToDictionary(x => x[0], x => Enum.Parse<InputControl>(x[1]));
            index += inputs.Count;
            var outputIndex = index++;
            if (!bool.TryParse(args[index++], out var global)) return $"Could not convert {args[index-1]} to global";

            var arguments = args.Skip(index)
                .Select(x => x.Split(":"))
                .TakeWhile(x => x.Length == 2)
                .ToDictionary(x => x[0], x => x[1]);

            switch (controllerType)
            {
                case ControllerType.DualShock when Enum.TryParse<DualShockControl>(args[outputIndex], out var dualShockOutput):
                    mapping.AddOrReplaceMapping(id, converter, inputs, dualShockOutput, arguments, global);
                    return null;
                case ControllerType.XBox360 when Enum.TryParse<XBox360Control>(args[outputIndex], out var xBox360Output):
                    mapping.AddOrReplaceMapping(id, converter, inputs, xBox360Output, arguments, global);
                    return null;
                default:
                    return $"Could not execute command 'map' with the given arguments: {string.Join(" | ", args)}";
            }
        }

        private static string? Remove(Mapping.Mapping mapping, string[] args)
        {
            if (args.Length != 3) return "Command 'remove' accepts exactly 3 arguments: id, output, global";

            var index = 0;
            if (!byte.TryParse(args[index++], out var id)) return $"Could not convert {args[0]} to id";
            if (!mapping.TryGetValue(id, out var controllerType)) return $"Id {id} is out of range";
            var outputIndex = index++;
            if (!bool.TryParse(args[index++], out var global)) return $"Could not convert {args[index-1]} to global";
            
            switch (controllerType)
            {
                case ControllerType.DualShock when Enum.TryParse<DualShockControl>(args[outputIndex], out var dualShockOutput):
                    mapping.RemoveMapping(id, dualShockOutput, global);
                    return null;
                case ControllerType.XBox360 when Enum.TryParse<XBox360Control>(args[outputIndex], out var xBox360Output):
                    mapping.RemoveMapping(id, xBox360Output, global);
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