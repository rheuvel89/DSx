using System;
using System.Collections.Generic;
using System.Linq;
using DSx.Shared;
using Nefarius.ViGEm.Client;

namespace DSx.Mapping
{
    public class Mapping
    {
        private readonly IDictionary<string, Type> _converters;
        private readonly IDictionary<byte, ControllerType> _controllerTypes;
        private readonly IDictionary<byte, IDictionary<int, IMappingAction>> _globalMapping;
        private readonly IDictionary<byte, IDictionary<int, IMappingAction>> _controllerMapping;
        private readonly IDictionary<byte, Func<DualSenseInputState, bool>> _controllerSelectors;

        public Mapping(MappingConfiguration configuration, IDictionary<string, Type> converters)
        {
            _converters = converters;
            _controllerTypes = new Dictionary<byte, ControllerType>();
            _globalMapping = new Dictionary<byte, IDictionary<int, IMappingAction>>();
            _controllerMapping = new Dictionary<byte, IDictionary<int, IMappingAction>>();
            _controllerSelectors = new Dictionary<byte, Func<DualSenseInputState, bool>>();

            byte index = 0;
            foreach (var controller in configuration.Controllers.OrderBy(x => x.Id))
            {
                _controllerTypes.Add(index, controller.ConrtollerType);
                
                var globalMapping = new Dictionary<int,IMappingAction>();
                var controllerMapping = new Dictionary<int,IMappingAction>();
                
                foreach (var controlConfiguration in controller.Mapping)
                {
                    (int, IMappingAction) config = controlConfiguration switch
                    {
                        DualShockControlConfiguration d => ((int)d.Output, MapDualShockAction(d.Inputs ?? new Dictionary<string, InputControl>(), d.Output, d.Converter, d.Arguments)),
                        Xbox360ControlConfiguration x => ((int)x.Output ,MapXBox360Action(x.Inputs ?? new Dictionary<string, InputControl>(), x.Output, x.Converter, x.Arguments))
                    };
                    if (controlConfiguration.Global == true) globalMapping.Add(config.Item1, config.Item2);
                    else controllerMapping.Add(config.Item1, config.Item2);
                }

                _globalMapping.Add(index, globalMapping);
                _controllerMapping.Add(index, controllerMapping);

                Func<DualSenseInputState, bool> controllerSelector = i =>
                    controller.Modifier != null &&
                    controller.Modifier.All(m => (bool)MappingConstants.InputSelector[m](i)) &&
                    !configuration.Controllers.Any(c =>
                        c.Id != controller.Id &&
                        c.Modifier != null &&
                        c.Modifier.All(m => (bool)MappingConstants.InputSelector[m](i)));
                _controllerSelectors.Add(index, controllerSelector);

                index += 1;
            }
        }
        
        public Feedback Map(DualSenseInputState input, IList<IVirtualGamepad> output)
        {
            Feedback feedback = new Feedback(); 
            
            var id = _controllerSelectors.FirstOrDefault(kvp => kvp.Value?.Invoke(input) ?? false).Key;
            for (byte i = 0; i < output.Count; i++)
            {
                if (i == id) foreach (var action in _controllerMapping[i]) feedback = action.Value.Map(input, output[i]) + feedback;
                else output[i].ResetReport();
                foreach (var action in _globalMapping[i]) feedback = action.Value.Map(input, output[i]) + feedback;
            }

            return feedback;
        }

        public void AddOrReplaceMapping(byte controllerId, string converter, IDictionary<string, InputControl> inputs,
            DualShockControl output, IDictionary<string, string> arguments, bool global = false)
        {
            var mapping = global ? _globalMapping : _controllerMapping;
            mapping[controllerId][(int)output] = MapDualShockAction(inputs, output, converter, arguments);
        }

        public void AddOrReplaceMapping(byte controllerId, string converter, IDictionary<string, InputControl> inputs,
            XBox360Control output, IDictionary<string, string> arguments, bool global = false)
        {
            var mapping = global ? _globalMapping : _controllerMapping;
            mapping[controllerId][(int)output] = MapXBox360Action(inputs, output, converter, arguments);
        }

        public void RemoveMapping(byte controllerId, DualShockControl output, bool global)
        {
            var mapping = global ? _globalMapping : _controllerMapping;
            mapping[controllerId].Remove((int)output);
        }

        public void RemoveMapping(byte controllerId, XBox360Control output, bool global)
        {
            var mapping = global ? _globalMapping : _controllerMapping;
            mapping[controllerId].Remove((int)output);
        }
        
        private DualShockMappingAction MapDualShockAction(
            IDictionary<string, InputControl> inputs,
            DualShockControl output,
            string converter,
            IDictionary<string, string>? arguments)
        {
            var selectors = inputs.ToDictionary(x => x.Key, x => MappingConstants.InputSelector[x.Value]);
            var asigner = MappingConstants.DualShockAsigner[output];
            var selectedConverter = MappingConstants.ConveterInvoker(_converters[converter]);
            var argumentArray = arguments ?? new Dictionary<string, string>();
            return new DualShockMappingAction(inputs, output, converter, (i, o) =>
            {
                Feedback feedback = new Feedback();
                var values = selectors.ToDictionary(x => x.Key, x => x.Value(i));
                var result = selectedConverter.Convert(values, argumentArray, out feedback);
                asigner(o, result);
                return feedback;
            });
        }
        
        private XBox360MappingAction MapXBox360Action(
            IDictionary<string, InputControl> inputs,
            XBox360Control output,
            string converter,
            IDictionary<string, string>? arguments)
        {
            var selectors = inputs.ToDictionary(x => x.Key, x => MappingConstants.InputSelector[x.Value]);
            var asigner = MappingConstants.XBox360Asigner[output];
            var selectedConverter = MappingConstants.ConveterInvoker(_converters[converter]);
            var argumentArray = arguments ?? new Dictionary<string, string>();
            return new XBox360MappingAction(inputs, output, converter, (i, o) =>
            {
                Feedback feedback = new Feedback();
                var values = selectors.ToDictionary(x => x.Key, x => x.Value(i));
                var result = selectedConverter.Convert(values, argumentArray, out feedback);
                asigner(o, result);
                return feedback;
            });
        }
        
        public int Count => _controllerMapping.Count;

        public ControllerType this[byte index] => _controllerTypes[index];

        public bool TryGetValue(byte id, out ControllerType controllerType)
        {
            return _controllerTypes.TryGetValue(id, out controllerType);
        }
    }
}