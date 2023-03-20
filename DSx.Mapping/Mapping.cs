using System;
using System.Collections.Generic;
using System.Linq;
using DualSenseAPI.State;
using Nefarius.ViGEm.Client;

namespace DSx.Mapping
{
    public class Mapping
    {
        private readonly IDictionary<byte, ControllerType> _controllerTypes;
        private readonly IDictionary<byte, IList<IMappingAction>> _globalMapping;
        private readonly IDictionary<byte, IList<IMappingAction>> _controllerMapping;
        private readonly IDictionary<byte, Func<DualSenseInputState, bool>> _controllerSelectors;

        public Mapping(MappingConfiguration configuration)
        {
            _controllerTypes = new Dictionary<byte, ControllerType>();
            _globalMapping = new Dictionary<byte, IList<IMappingAction>>();
            _controllerMapping = new Dictionary<byte, IList<IMappingAction>>();
            _controllerSelectors = new Dictionary<byte, Func<DualSenseInputState, bool>>();

            byte index = 0;
            foreach (var controller in configuration.Controllers.OrderBy(x => x.Id))
            {
                _controllerTypes.Add(index, controller.ConrtollerType);
                
                var globalMapping = new List<IMappingAction>();
                var controllerMapping = new List<IMappingAction>();
                
                foreach (var controlConfiguration in controller.Mapping)
                {
                    IMappingAction config = controlConfiguration switch
                    {
                        DualShockControlConfiguration d => MapDualShockAction(d.Inputs, d.Output, d.Converter, d.ConverterArguments),
                        Xbox360ControlConfiguration x => MapXBox360Action(x.Inputs, x.Output, x.Converter, x.ConverterArguments)
                    };
                    if (controlConfiguration.Global == true) globalMapping.Add(config);
                    else controllerMapping.Add(config);
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
        
        public object? Map(DualSenseInputState input, IList<IVirtualGamepad> output)
        {
            object? feedback = null; 
            
            var id = _controllerSelectors.FirstOrDefault(kvp => kvp.Value?.Invoke(input) ?? false).Key;
            for (byte i = 0; i < output.Count; i++)
            {
                if (i == id) foreach (var action in _controllerMapping[i]) feedback = action.Map(input, output[i]) ?? feedback;
                else output[i].ResetReport();
                foreach (var action in _globalMapping[i]) feedback = action.Map(input, output[i]) ?? feedback;
            }

            return feedback;
        }

        public void AddOrReplaceMapping(byte controllerId, InputControl input, DualShockControl output,
            MappingConverter? converter, IList<InputControl> inputArguments, IList<string> arguments, bool global = false)
        {
            throw new NotImplementedException();
            // var mapping = global ? _globalMapping : _controllerMapping;
            // mapping[controllerId][input] = MapDualShockAction(input, output, converter, inputArguments, arguments);
        }

        public void AddOrReplaceMapping(byte controllerId, InputControl input, XBox360Control output,
            MappingConverter? converter, IList<InputControl> inputArguments, IList<string> arguments, bool global = false)
        {
            throw new NotImplementedException();
            // var mapping = global ? _globalMapping : _controllerMapping;
            // mapping[controllerId][input] = MapXBox360Action(input, output, converter, inputArguments, arguments);
        }
        
        private static DualShockMappingAction MapDualShockAction(
            IList<InputControl> inputs,
            DualShockControl output,
            MappingConverter converter,
            IList<string>? arguments)
        {
            var selectors = inputs.Select(x => MappingConstants.InputSelector[x]).ToArray();
            var asigner = MappingConstants.DualShockAsigner[output];
            var selectedConverter = converter != null ? MappingConstants.MappingConveters[converter]() : null;
            var argumentArray = arguments?.ToArray() ?? Array.Empty<string>();
            return new DualShockMappingAction(inputs, output, converter, (i, o) =>
            {
                object? feedback = null;
                var values = selectors.Select(x => x(i)).ToArray();
                var result = selectedConverter.Convert(values, argumentArray, out feedback);
                asigner(o, result);
                return feedback;
            });
        }
        
        private static XBox360MappingAction MapXBox360Action(
            IList<InputControl> inputs,
            XBox360Control output,
            MappingConverter converter,
            IList<string>? arguments)
        {
            var selectors = inputs.Select(x => MappingConstants.InputSelector[x]).ToArray();
            var asigner = MappingConstants.XBox360Asigner[output];
            var selectedConverter = converter != null ? MappingConstants.MappingConveters[converter]() : null;
            var argumentArray = arguments?.ToArray() ?? Array.Empty<string>();
            return new XBox360MappingAction(inputs, output, converter, (i, o) =>
            {
                object? feedback = null;
                var values = selectors.Select(x => x(i)).ToArray();
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