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
            var selectedConverter = converter != null ? MappingConstants.MappingConveters[converter] : null;
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
            var selectedConverter = converter != null ? MappingConstants.MappingConveters[converter] : null;
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
        
        // private IDictionary<InputControl, IMappingAction> BasicDualShockMapping = new Dictionary<InputControl, IMappingAction>
        // {
        //     [InputControl.LeftStick] = MapDualShockAction(InputControl.LeftStick, DualShockControl.LeftStick, null, null, null), 
        //     [InputControl.LeftTrigger] = MapDualShockAction(InputControl.LeftTrigger, DualShockControl.LeftTrigger, null, null, null), 
        //     [InputControl.LeftShoulder] = MapDualShockAction(InputControl.LeftShoulder, DualShockControl.LeftShoulder, null, null, null), 
        //     [InputControl.LeftStickButton] = MapDualShockAction(InputControl.LeftStickButton, DualShockControl.LeftStickButton, null, null, null), 
        //     [InputControl.RightStick] = MapDualShockAction(InputControl.RightStick, DualShockControl.RightStick, null, null, null), 
        //     [InputControl.RightTrigger] = MapDualShockAction(InputControl.RightTrigger, DualShockControl.RightTrigger, null, null, null),
        //     [InputControl.RightShoulder] = MapDualShockAction(InputControl.RightShoulder, DualShockControl.RightShoulder, null, null, null), 
        //     [InputControl.RightStickButton] = MapDualShockAction(InputControl.RightStickButton, DualShockControl.RightStickButton, null, null, null),
        //     [InputControl.DPadNorth] = MapDualShockAction(InputControl.DPadNorth, DualShockControl.DPadNorth, null, null, null), 
        //     [InputControl.DPadNorthEast] = MapDualShockAction(InputControl.DPadNorthEast, DualShockControl.DPadNorthEast, null, null, null), 
        //     [InputControl.DPadEast] = MapDualShockAction(InputControl.DPadEast, DualShockControl.DPadEast, null, null, null), 
        //     [InputControl.DPadSouthEast] = MapDualShockAction(InputControl.DPadSouthEast, DualShockControl.DPadSouthEast, null, null, null), 
        //     [InputControl.DPadSouth] = MapDualShockAction(InputControl.DPadSouth, DualShockControl.DPadSouth, null, null, null), 
        //     [InputControl.DPadSouthWest] = MapDualShockAction(InputControl.DPadSouthWest, DualShockControl.DPadSouthWest, null, null, null), 
        //     [InputControl.DPadWest] = MapDualShockAction(InputControl.DPadWest, DualShockControl.DPadWest, null, null, null), 
        //     [InputControl.DPadNorthWest] = MapDualShockAction(InputControl.DPadNorthWest, DualShockControl.DPadNorthWest, null, null, null), 
        //     [InputControl.DPadNone] = MapDualShockAction(InputControl.DPadNone, DualShockControl.DPadNone, null, null, null), 
        //     [InputControl.TriangleButton] = MapDualShockAction(InputControl.TriangleButton, DualShockControl.TriangleButton, null, null, null), 
        //     [InputControl.CircleButton] = MapDualShockAction(InputControl.CircleButton, DualShockControl.CircleButton, null, null, null), 
        //     [InputControl.SquareButton] = MapDualShockAction(InputControl.SquareButton, DualShockControl.SquareButton, null, null, null), 
        //     [InputControl.CrossButton] = MapDualShockAction(InputControl.CrossButton, DualShockControl.CrossButton, null, null, null), 
        //     [InputControl.CreateButton] = MapDualShockAction(InputControl.CreateButton, DualShockControl.ShareButton, null, null, null), 
        //     [InputControl.MenuButton] = MapDualShockAction(InputControl.MenuButton, DualShockControl.OptionButton, null, null, null), 
        // };
        //
        // private IDictionary<InputControl, IMappingAction> BasicXBox360Mapping = new Dictionary<InputControl, IMappingAction>
        // {
        //     [InputControl.LeftStick] = MapXBox360Action(InputControl.LeftStick, XBox360Control.LeftStick, null, null, null), 
        //     [InputControl.LeftTrigger] = MapXBox360Action(InputControl.LeftTrigger, XBox360Control.LeftTrigger, null, null, null), 
        //     [InputControl.LeftShoulder] = MapXBox360Action(InputControl.LeftShoulder, XBox360Control.LeftShoulder, null, null, null), 
        //     [InputControl.LeftStickButton] = MapXBox360Action(InputControl.LeftStickButton, XBox360Control.LeftStickButton, null, null, null), 
        //     [InputControl.RightStick] = MapXBox360Action(InputControl.RightStick, XBox360Control.RightStick, null, null, null), 
        //     [InputControl.RightTrigger] = MapXBox360Action(InputControl.RightTrigger, XBox360Control.RightTrigger, null, null, null),
        //     [InputControl.RightShoulder] = MapXBox360Action(InputControl.RightShoulder, XBox360Control.RightShoulder, null, null, null), 
        //     [InputControl.RightStickButton] = MapXBox360Action(InputControl.RightStickButton, XBox360Control.RightStickButton, null, null, null),
        //     [InputControl.DPadNorth] = MapXBox360Action(InputControl.DPadNorth, XBox360Control.DPadNorth, null, null, null), 
        //     [InputControl.DPadNorthEast] = MapXBox360Action(InputControl.DPadNorthEast, XBox360Control.DPadNorthEast, null, null, null), 
        //     [InputControl.DPadEast] = MapXBox360Action(InputControl.DPadEast, XBox360Control.DPadEast, null, null, null), 
        //     [InputControl.DPadSouthEast] = MapXBox360Action(InputControl.DPadSouthEast, XBox360Control.DPadSouthEast, null, null, null), 
        //     [InputControl.DPadSouth] = MapXBox360Action(InputControl.DPadSouth, XBox360Control.DPadSouth, null, null, null), 
        //     [InputControl.DPadSouthWest] = MapXBox360Action(InputControl.DPadSouthWest, XBox360Control.DPadSouthWest, null, null, null), 
        //     [InputControl.DPadWest] = MapXBox360Action(InputControl.DPadWest, XBox360Control.DPadWest, null, null, null), 
        //     [InputControl.DPadNorthWest] = MapXBox360Action(InputControl.DPadNorthWest, XBox360Control.DPadNorthWest, null, null, null), 
        //     [InputControl.DPadNone] = MapXBox360Action(InputControl.DPadNone, XBox360Control.DPadNone, null, null, null), 
        //     [InputControl.TriangleButton] = MapXBox360Action(InputControl.TriangleButton, XBox360Control.YButton, null, null, null), 
        //     [InputControl.CircleButton] = MapXBox360Action(InputControl.CircleButton, XBox360Control.BButton, null, null, null), 
        //     [InputControl.SquareButton] = MapXBox360Action(InputControl.SquareButton, XBox360Control.XButton, null, null, null), 
        //     [InputControl.CrossButton] = MapXBox360Action(InputControl.CrossButton, XBox360Control.AButton, null, null, null), 
        //     [InputControl.CreateButton] = MapXBox360Action(InputControl.CreateButton, XBox360Control.BackButton, null, null, null), 
        //     [InputControl.LogoButton] = MapXBox360Action(InputControl.MenuButton, XBox360Control.GuideButton, null, null, null), 
        //     [InputControl.MenuButton] = MapXBox360Action(InputControl.MenuButton, XBox360Control.StartButton, null, null, null), 
        // };

        public int Count => _controllerMapping.Count;

        public ControllerType this[byte index] => _controllerTypes[index];

        public bool TryGetValue(byte id, out ControllerType controllerType)
        {
            return _controllerTypes.TryGetValue(id, out controllerType);
        }
    }
}