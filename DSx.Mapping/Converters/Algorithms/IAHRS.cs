using DSx.Math;
using DualSenseAPI;

namespace DSx.Mapping
{
    public interface IAHRS
    {
        Vec2 Calculate(
            long timestamp,
            Vector<float, float, float> rAcc,
            Vector<float, float, float> rGyr,
            float sensitivity,
            float deadzone,
            bool reZero,
            out Feedback feedback);
    }
}