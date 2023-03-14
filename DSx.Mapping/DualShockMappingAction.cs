using System;
using DualSenseAPI;
using DualSenseAPI.State;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;

namespace DSx.Mapping
{
    public class DualShockMappingAction : IMappingAction
    {
        private readonly Action<DualSenseInputState, IDualShock4Controller> _mappingAction;

        public DualShockMappingAction(Action<DualSenseInputState, IDualShock4Controller> mappingAction)
        {
            _mappingAction = mappingAction;
        }

        public void Map(DualSenseInputState input, IVirtualGamepad output)
        {
            _mappingAction(input, (IDualShock4Controller)output);
        }
    }
}