namespace DSx.Mapping
{
    public class TriggerToTriggerConverter : IMappingConverter
    {
        public object Convert(object[] inputs, string[] args, out object? feedback)
        {
            feedback = null;

            return inputs[0];
        }
    }
}