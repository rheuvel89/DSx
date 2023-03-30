using System;
using System.Collections.Generic;
using DSx.Shared;
using DualSenseAPI.State;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using DualSenseInputState = DSx.Shared.DualSenseInputState;

namespace DSx.Mapping
{
    public class XBox360MappingAction : IMappingAction
    {
        private readonly Func<DualSenseInputState, IXbox360Controller, Feedback> _mappingAction;

        public XBox360MappingAction(IDictionary<string, InputControl> inputs, XBox360Control output, MappingConverter converter, Func<DualSenseInputState, IXbox360Controller, Feedback> mappingAction)
        {
            Inputs = inputs;
            Output = output;
            Converter = converter;
            _mappingAction = mappingAction;
        }

        public IDictionary<string, InputControl> Inputs { get; }
        public XBox360Control Output { get; }
        public MappingConverter Converter { get; }

        public Feedback Map(DualSenseInputState input, IVirtualGamepad output)
        {
            return _mappingAction(input, (IXbox360Controller)output);
        }
    }
}