using System;
using System.Collections.Generic;
using DSx.Shared;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;

namespace DSx.Mapping
{
    public class DualShockMappingAction : IMappingAction
    {
        private readonly Func<DualSenseInputState, IDualShock4Controller, Feedback> _mappingAction;

        public DualShockMappingAction(IDictionary<string, InputControl> inputs, DualShockControl output, string converter, Func<DualSenseInputState, IDualShock4Controller, Feedback> mappingAction)
        {
            Inputs = inputs;
            Output = output;
            Converter = converter;
            _mappingAction = mappingAction;
        }

        public IDictionary<string, InputControl> Inputs { get; }
        public DualShockControl Output { get; }
        public string Converter { get; }

        public Feedback Map(DualSenseInputState input, IVirtualGamepad output)
        {
            return _mappingAction(input, (IDualShock4Controller)output);
        }
    }
}