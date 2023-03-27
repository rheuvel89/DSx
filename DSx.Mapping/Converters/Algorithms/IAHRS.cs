using DSx.Math;

namespace DSx.Mapping
{
    public interface IAHRS
    {
        Vector<float, float, float> Calculate(
            long timestamp,
            Vector<float, float, float> rAcc,
            Vector<float, float, float> rGyr,
            float sensitivity,
            float deadzone,
            bool reZero,
            out Feedback feedback);
    }
}