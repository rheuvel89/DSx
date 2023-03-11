using System;
using DualSenseAPI;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;

namespace DSx.Mapping
{
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
}