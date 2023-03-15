using System;
using DualSenseAPI;
using DualSenseAPI.State;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;

namespace DSx.Mapping
{
    public class DualShockMappingAction : IMappingAction
    {
        private readonly Func<DualSenseInputState, IDualShock4Controller, object> _mappingAction;

        public DualShockMappingAction(Func<DualSenseInputState, IDualShock4Controller, object> mappingAction)
        {
            _mappingAction = mappingAction;
        }

        public object Map(DualSenseInputState input, IVirtualGamepad output)
        {
            return _mappingAction(input, (IDualShock4Controller)output);
        }
    }
}