using System;
using DualSenseAPI;
using DualSenseAPI.State;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;

namespace DSx.Mapping
{
    public class XBox360MappingAction : IMappingAction
    {
        private readonly Func<DualSenseInputState, IXbox360Controller, object> _mappingAction;

        public XBox360MappingAction(Func<DualSenseInputState, IXbox360Controller, object> mappingAction)
        {
            _mappingAction = mappingAction;
        }

        public object Map(DualSenseInputState input, IVirtualGamepad output)
        {
            return _mappingAction(input, (IXbox360Controller)output);
        }
    }
}