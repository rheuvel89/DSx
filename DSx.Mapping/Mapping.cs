using System;
using System.Collections.Generic;
using DualSenseAPI;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.DualShock4;

namespace DSx.Mapping
{
    public class Mapping
    {
        private readonly IDictionary<byte, IList<IMappingAction>> _mapping;
        
        public Mapping(MappingConfiguration configuration)
        {
            _mapping = new Dictionary<byte, IList<IMappingAction>>();
            var basic = new DualShockMappingAction((i, o) => o.SetButtonState(DualShock4Button.Circle, i.InputState.CircleButton));
            if (_mapping.TryGetValue(0, out var mapping)) mapping.Add(basic);
            else _mapping.Add(0, new List<IMappingAction> { basic });
        }
        
        public void Map(DualSense input, IVirtualGamepad[] output)
        {
            byte modifierSelection = 0;
            var mapping = _mapping[modifierSelection];
            foreach (var action in mapping) action.Map(input, output[modifierSelection]);
        }
    }

    public interface IMappingAction
    {
        void Map(DualSense input, IVirtualGamepad output);
    }

    public class DualShockMappingAction : IMappingAction
    {
        private readonly Action<DualSense, IDualShock4Controller> _mappingAction;

        public DualShockMappingAction(Action<DualSense, IDualShock4Controller> mappingAction)
        {
            _mappingAction = mappingAction;
        }

        public void Map(DualSense input, IVirtualGamepad output)
        {
            _mappingAction(input, (IDualShock4Controller)output);
        }
    }

    public class XBox360MappingAction : IMappingAction
    {
        private readonly Action<DualSense, IXbox360Controller> _mappingAction;

        public XBox360MappingAction(Action<DualSense, IXbox360Controller> mappingAction)
        {
            _mappingAction = mappingAction;
        }

        public void Map(DualSense input, IVirtualGamepad output)
        {
            _mappingAction(input, (IXbox360Controller)output);
        }
    }
}