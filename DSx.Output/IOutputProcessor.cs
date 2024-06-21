using DSx.Shared;
using Nefarius.ViGEm.Client;

namespace DSx.Output;

public interface IOutputProcessor
{
    IList<IVirtualGamepad> Output { get; }
    
    Task Initialize(Mapping.Mapping mapping);
    void ProcessOutput();
    void Reset();
}