using DualSenseAPI;
using DualSenseAPI.State;
using Nefarius.ViGEm.Client;

namespace DSx.Mapping
{
    public interface IMappingAction
    {
        object Map(DualSenseInputState input, IVirtualGamepad output);
    }
}