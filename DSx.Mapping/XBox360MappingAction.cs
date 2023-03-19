using System;
using System.Collections.Generic;
using DualSenseAPI.State;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;

namespace DSx.Mapping
{
    public class XBox360MappingAction : IMappingAction
    {
        private readonly Func<DualSenseInputState, IXbox360Controller, object> _mappingAction;

        public XBox360MappingAction(IList<InputControl> inputs, XBox360Control output, MappingConverter converter, Func<DualSenseInputState, IXbox360Controller, object> mappingAction)
        {
            Inputs = inputs;
            Output = output;
            Converter = converter;
            _mappingAction = mappingAction;
        }

        public IList<InputControl> Inputs { get; }
        public XBox360Control Output { get; }
        public MappingConverter Converter { get; }

        public object Map(DualSenseInputState input, IVirtualGamepad output)
        {
            return _mappingAction(input, (IXbox360Controller)output);
        }
    }
}