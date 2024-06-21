using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DSx.Input;
using DSx.Mapping;
using DSx.Output;
using DSx.Shared;
using DualSenseAPI;
using DualSenseAPI.State;
using DXs.Common;
using Nefarius.ViGEm.Client;
using YamlDotNet.Serialization;
using DualSenseInputState = DSx.Shared.DualSenseInputState;

namespace DSx.Host
{
    public class Host : IApplication
    {
        private readonly IInputCollector _inputCollector;
        private readonly IOutputProcessor _outputProcessor;
        private readonly IList<IVirtualGamepad> _output;
        private readonly DSx.Console.Console _console;
        private readonly Stopwatch _timer;
        private readonly byte _count;
        private Mapping.Mapping _mapping;
        private IDictionary<string, Type> _converters;

        public Host(HostOptions options)
        {
            _inputCollector = options.Port != null
                ? new RemoteInputCollector(options.Port.Value)
                : new LocalInputCollector(options.PollingInterval);
            _outputProcessor = new LocalOutputProcessor();
            _output = new List<IVirtualGamepad>();
            _console = new Console.Console(_output, options.NoConsole);
            _count = options.Count;
            if (_count < 1 || _count > 4)
                throw new Exception("Invalid number of controllers to connect (must be between 1 and 4)");
            _timer = new Stopwatch();

            _converters = LoadConverters(options.PluginPath);
            _mapping = LoadMapping(options.MappingPath ?? "config.yaml", _converters);
        }

        public async Task Initialize()
        {
            if (_outputProcessor is LocalOutputProcessor lop) await lop.Initialize(_mapping);
            
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
        private void OnInputReceived(DualSenseInputState inputState)
        {
            // var ms = _timer.ElapsedMilliseconds;
            var feedback = _outputProcessor.Map(_mapping, inputState);

            feedback.Color = inputState.IoMode == IoMode.USB
                ? new Vec3()
                : new Vec3 { X = (10 - inputState.BatteryLevel) / 10, Y = inputState.BatteryLevel / 10 };
                
            _inputCollector.OnStateChanged(feedback);

            // var elapsed = _timer.ElapsedMilliseconds - ms;
            // _elapsed = elapsed > _elapsed ? elapsed : _elapsed;
            // _average = (_average*900 + elapsed*1000*100) / 1000;
            // System.Console.WriteLine($"{_elapsed} | {_average/1000} | {elapsed}");
        }

        private void OnButtonChanged(DualSenseInputStateButtonDelta change) { }

        private string? OnCommandReceived(string command, string[] arguments)
        {
            command = command.ToLower();
            return command switch
            {
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
            var converter = args[index++];
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

        private Mapping.Mapping LoadMapping(string mappingPath,
            IDictionary<string, Type> converters)
        {
            var yaml = File.ReadAllText(mappingPath);
            var mappingConfiguration =  new DeserializerBuilder()
                .WithTagMapping("!DualShock", typeof(DualShockControlConfiguration))
                .WithTagMapping("!XBox360", typeof(Xbox360ControlConfiguration))
                .Build()
                .Deserialize<MappingConfiguration>(yaml);

            var mapping = new Mapping.Mapping(mappingConfiguration, converters);
            return mapping;
        }

        private IDictionary<string, Type> LoadConverters(string? pluginPath)
        {
            var assemblies = new[] { typeof(ButtonToButtonConverter).Assembly }.AsEnumerable();
            if (pluginPath != null)
            {
                if (!Directory.Exists(pluginPath)) Directory.CreateDirectory(pluginPath);
                assemblies = assemblies.Concat(Directory.EnumerateFiles(pluginPath)
                    .Where(x => x.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                    .Select(Assembly.LoadFrom));
                
            }
            var types = assemblies
                .SelectMany(x => x.GetTypes())
                .Where(typeof(IMappingConverter).IsAssignableFrom)
                .Where(x => !x.IsAbstract)
                .Where(x => !x.IsInterface)
                .ToList();
            return types.ToDictionary(x => x.Name);
        }
    }
}