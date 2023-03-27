using System.Collections.Generic;

namespace DSx.Mapping
{
    public class TriggerToTriggerConverter : IMappingConverter
    {
        public object Convert(IDictionary<string, object> inputs, IDictionary<string, string> args, out object? feedback)
        {
            feedback = null;

            return inputs["Trigger"];
        }
    }
}