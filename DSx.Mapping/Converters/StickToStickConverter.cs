using DualSenseAPI;

namespace DSx.Mapping
{
    public class StickToStickConverter : IMappingConverter
    {
        public object Convert(object[] inputs, string[] args, out object? feedback)
        {
            feedback = null;

            return inputs[0];
        }
    }
    public class TriggerToTriggerConverter : IMappingConverter
    {
        public object Convert(object[] inputs, string[] args, out object? feedback)
        {
            feedback = null;

            return inputs[0];
        }
    }
}