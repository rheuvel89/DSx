using DualSenseAPI;
using Nefarius.ViGEm.Client;

namespace DSx.Mapping
{
    public interface IMappingAction
    {
        void Map(DualSense input, IVirtualGamepad output);
    }
}