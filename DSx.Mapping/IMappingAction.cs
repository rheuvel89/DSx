using System.Collections.Generic;
using DSx.Shared;
using Nefarius.ViGEm.Client;

namespace DSx.Mapping
{
    public interface IMappingAction
    {
        IDictionary<string, InputControl> Inputs { get; }
        string Converter { get; }
        
        Feedback Map(DualSenseInputState input, IVirtualGamepad output);
    }
}