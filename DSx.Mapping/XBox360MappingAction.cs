using System;
using DualSenseAPI;
using DualSenseAPI.State;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;

namespace DSx.Mapping
{
    public class XBox360MappingAction : IMappingAction
    {
        private readonly Action<DualSenseInputState, IXbox360Controller> _mappingAction;

        public XBox360MappingAction(Action<DualSenseInputState, IXbox360Controller> mappingAction)
        {
            _mappingAction = mappingAction;
        }

        public void Map(DualSenseInputState input, IVirtualGamepad output)
        {
            _mappingAction(input, (IXbox360Controller)output);
        }
    }
}