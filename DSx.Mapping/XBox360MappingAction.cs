using System;
using DualSenseAPI;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;

namespace DSx.Mapping
{
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