using System;
using System.Collections.Generic;
using System.Linq;
using DualSenseAPI;
using Nefarius.ViGEm.Client;

namespace DSx.Mapping
{
    public class Mapping
    {
        private readonly IDictionary<byte, ControllerType> _controllerTypes;
        private readonly IDictionary<byte, IDictionary<InputControl,IMappingAction>> _mapping;
        private readonly IDictionary<byte, Func<DualSense, bool>> _controllerSelectors;

        public Mapping(MappingConfiguration configuration)
        {
            _controllerTypes = new Dictionary<byte, ControllerType>();
            _mapping = new Dictionary<byte, IDictionary<InputControl, IMappingAction>>();
            _controllerSelectors = new Dictionary<byte, Func<DualSense, bool>>();

            byte index = 0;
            foreach (var controller in configuration.Controllers.OrderBy(x => x.Id))
            {
                _controllerTypes.Add(index, controller.ConrtollerType);
                
                var baseConfiguration = controller.ConrtollerType switch
                {
                    ControllerType.DualShock => BasicDualShockMapping,
                    ControllerType.XBox360 => BasicXBox360Mapping
                };
                var controllerMapping = new Dictionary<InputControl, IMappingAction>(baseConfiguration);
                
                foreach (var controlConfiguration in controller.Mapping)
                {
                    IMappingAction config = controlConfiguration switch
                    {
                        DualShockControlConfiguration d => MapDualShockAction(d.Input, d.Output, d.Converter, d.ConverterArguments),
                        Xbox360ControlConfiguration x => MapXBox360Action(x.Input, x.Output, x.Converter, x.ConverterArguments)
                    };
                    controllerMapping[controlConfiguration.Input] = config;
                }

                if (controller.Modifier != null) foreach (var modifier in controller.Modifier) controllerMapping.Remove(modifier);
                
                _mapping.Add(index, controllerMapping);

                Func<DualSense, bool> controllerSelector = i =>
                    controller.Modifier != null &&
                    controller.Modifier.All(m => (bool)MappingConstants.InputSelector[m](i)) &&
                    !configuration.Controllers.Any(c =>
                        c.Modifier != null &&
                        c.Modifier.All(m => (bool)MappingConstants.InputSelector[m](i)));
                _controllerSelectors.Add(index, controllerSelector);

                index += 1;
            }
        }
        
        public void Map(DualSense input, IList<IVirtualGamepad> output)
        {
            var id = _controllerSelectors.FirstOrDefault(kvp => kvp.Value?.Invoke(input) ?? false).Key;
            var mapping = _mapping[id];
            for (byte i = 0; i < output.Count; i++)
            {
                if (i == id) foreach (var action in mapping) action.Value.Map(input, output[id]);
                else output[i].ResetReport();
            }
        }
        
        private static DualShockMappingAction MapDualShockAction(
            InputControl input,
            DualShockControl output,
            MappingConverter? converter,
            IList<string>? arguments)
        {
            var selector = MappingConstants.InputSelector[input];
            var asigner = MappingConstants.DualShockAsigner[output];
            var selectedConverter = converter != null ? MappingConstants.MappingConveters[converter.Value] : null;
            var argumentArray = arguments?.ToArray() ?? Array.Empty<string>();
            return new DualShockMappingAction((i, o) =>
            {
                var value = selector(i);
                var result = selectedConverter != null ? selectedConverter.Convert(value, argumentArray) : value;
                asigner(o, result);
            });
        }
        
        private static XBox360MappingAction MapXBox360Action(
            InputControl input,
            XBox360Control output,
            MappingConverter? converter,
            IList<string>? arguments)
        {
            var selector = MappingConstants.InputSelector[input];
            var asigner = MappingConstants.XBox360Asigner[output];
            var selectedConverter = converter != null ? MappingConstants.MappingConveters[converter.Value] : null;
            var argumentArray = arguments?.ToArray() ?? Array.Empty<string>();
            return new XBox360MappingAction((i, o) =>
            {
                var value = selector(i);
                var result = selectedConverter != null ? selectedConverter.Convert(value, argumentArray) : value;
                asigner(o, result);
            });
        }
        
        private IDictionary<InputControl, IMappingAction> BasicDualShockMapping = new Dictionary<InputControl, IMappingAction>
        {
            [InputControl.LeftStick] = MapDualShockAction(InputControl.LeftStick, DualShockControl.LeftStick, null, null), 
            [InputControl.LeftTrigger] = MapDualShockAction(InputControl.LeftTrigger, DualShockControl.LeftTrigger, null, null), 
            [InputControl.LeftShoulder] = MapDualShockAction(InputControl.LeftShoulder, DualShockControl.LeftShoulder, null, null), 
            [InputControl.LeftStickButton] = MapDualShockAction(InputControl.LeftStickButton, DualShockControl.LeftStickButton, null, null), 
            [InputControl.RightStick] = MapDualShockAction(InputControl.RightStick, DualShockControl.RightStick, null, null), 
            [InputControl.RightTrigger] = MapDualShockAction(InputControl.RightTrigger, DualShockControl.RightTrigger, null, null),
            [InputControl.RightShoulder] = MapDualShockAction(InputControl.RightShoulder, DualShockControl.RightShoulder, null, null), 
            [InputControl.RightStickButton] = MapDualShockAction(InputControl.RightStickButton, DualShockControl.RightStickButton, null, null),
            [InputControl.DPadNorth] = MapDualShockAction(InputControl.DPadNorth, DualShockControl.DPadNorth, null, null), 
            [InputControl.DPadNorthEast] = MapDualShockAction(InputControl.DPadNorthEast, DualShockControl.DPadNorthEast, null, null), 
            [InputControl.DPadEast] = MapDualShockAction(InputControl.DPadEast, DualShockControl.DPadEast, null, null), 
            [InputControl.DPadSouthEast] = MapDualShockAction(InputControl.DPadSouthEast, DualShockControl.DPadSouthEast, null, null), 
            [InputControl.DPadSouth] = MapDualShockAction(InputControl.DPadSouth, DualShockControl.DPadSouth, null, null), 
            [InputControl.DPadSouthWest] = MapDualShockAction(InputControl.DPadSouthWest, DualShockControl.DPadSouthWest, null, null), 
            [InputControl.DPadWest] = MapDualShockAction(InputControl.DPadWest, DualShockControl.DPadWest, null, null), 
            [InputControl.DPadNorthWest] = MapDualShockAction(InputControl.DPadNorthWest, DualShockControl.DPadNorthWest, null, null), 
            [InputControl.DPadNone] = MapDualShockAction(InputControl.DPadNone, DualShockControl.DPadNone, null, null), 
            [InputControl.TriangleButton] = MapDualShockAction(InputControl.TriangleButton, DualShockControl.TriangleButton, null, null), 
            [InputControl.CircleButton] = MapDualShockAction(InputControl.CircleButton, DualShockControl.CircleButton, null, null), 
            [InputControl.SquareButton] = MapDualShockAction(InputControl.SquareButton, DualShockControl.SquareButton, null, null), 
            [InputControl.CrossButton] = MapDualShockAction(InputControl.CrossButton, DualShockControl.CrossButton, null, null), 
            [InputControl.CreateButton] = MapDualShockAction(InputControl.CreateButton, DualShockControl.ShareButton, null, null), 
            [InputControl.MenuButton] = MapDualShockAction(InputControl.MenuButton, DualShockControl.OptionButton, null, null), 
        };
        
        private IDictionary<InputControl, IMappingAction> BasicXBox360Mapping = new Dictionary<InputControl, IMappingAction>
        {
            [InputControl.LeftStick] = MapXBox360Action(InputControl.LeftStick, XBox360Control.LeftStick, null, null), 
            [InputControl.LeftTrigger] = MapXBox360Action(InputControl.LeftTrigger, XBox360Control.LeftTrigger, null, null), 
            [InputControl.LeftShoulder] = MapXBox360Action(InputControl.LeftShoulder, XBox360Control.LeftShoulder, null, null), 
            [InputControl.LeftStickButton] = MapXBox360Action(InputControl.LeftStickButton, XBox360Control.LeftStickButton, null, null), 
            [InputControl.RightStick] = MapXBox360Action(InputControl.RightStick, XBox360Control.RightStick, null, null), 
            [InputControl.RightTrigger] = MapXBox360Action(InputControl.RightTrigger, XBox360Control.RightTrigger, null, null),
            [InputControl.RightShoulder] = MapXBox360Action(InputControl.RightShoulder, XBox360Control.RightShoulder, null, null), 
            [InputControl.RightStickButton] = MapXBox360Action(InputControl.RightStickButton, XBox360Control.RightStickButton, null, null),
            [InputControl.DPadNorth] = MapXBox360Action(InputControl.DPadNorth, XBox360Control.DPadNorth, null, null), 
            [InputControl.DPadNorthEast] = MapXBox360Action(InputControl.DPadNorthEast, XBox360Control.DPadNorthEast, null, null), 
            [InputControl.DPadEast] = MapXBox360Action(InputControl.DPadEast, XBox360Control.DPadEast, null, null), 
            [InputControl.DPadSouthEast] = MapXBox360Action(InputControl.DPadSouthEast, XBox360Control.DPadSouthEast, null, null), 
            [InputControl.DPadSouth] = MapXBox360Action(InputControl.DPadSouth, XBox360Control.DPadSouth, null, null), 
            [InputControl.DPadSouthWest] = MapXBox360Action(InputControl.DPadSouthWest, XBox360Control.DPadSouthWest, null, null), 
            [InputControl.DPadWest] = MapXBox360Action(InputControl.DPadWest, XBox360Control.DPadWest, null, null), 
            [InputControl.DPadNorthWest] = MapXBox360Action(InputControl.DPadNorthWest, XBox360Control.DPadNorthWest, null, null), 
            [InputControl.DPadNone] = MapXBox360Action(InputControl.DPadNone, XBox360Control.DPadNone, null, null), 
            [InputControl.TriangleButton] = MapXBox360Action(InputControl.TriangleButton, XBox360Control.YButton, null, null), 
            [InputControl.CircleButton] = MapXBox360Action(InputControl.CircleButton, XBox360Control.BButton, null, null), 
            [InputControl.SquareButton] = MapXBox360Action(InputControl.SquareButton, XBox360Control.XButton, null, null), 
            [InputControl.CrossButton] = MapXBox360Action(InputControl.CrossButton, XBox360Control.AButton, null, null), 
            [InputControl.CreateButton] = MapXBox360Action(InputControl.CreateButton, XBox360Control.BackButton, null, null), 
            [InputControl.LogoButton] = MapXBox360Action(InputControl.MenuButton, XBox360Control.GuideButton, null, null), 
            [InputControl.MenuButton] = MapXBox360Action(InputControl.MenuButton, XBox360Control.StartButton, null, null), 
        };

        public int Count => _mapping.Count;

        public ControllerType this[byte index] => _controllerTypes[index];
    }
}