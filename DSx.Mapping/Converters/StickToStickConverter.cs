using DualSenseAPI;

namespace DSx.Mapping
{
    public class StickToStickConverter : IMappingConveter<Vec2, Vec2>
    {
        public Vec2 Convert(Vec2 input, params object[] args)
        {
            return input;
        }
    }
}