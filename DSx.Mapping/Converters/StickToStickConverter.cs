using DualSenseAPI;

namespace DSx.Mapping
{
    public class StickToStickConverter : IMappingConveter
    {
        public object Convert(object input, object[] inputArgs, string[] args, out object? feedback)
        {
            feedback = null;

            return input;
        }
    }
}