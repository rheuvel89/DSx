using System.Collections.Generic;
using DualSenseAPI.State;
using Nefarius.ViGEm.Client;

namespace DSx.Mapping
{
    public interface IMappingAction
    {
        IList<InputControl> Inputs { get; }
        MappingConverter Converter { get; }
        
        object Map(DualSenseInputState input, IVirtualGamepad output);
    }
}