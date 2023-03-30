using System.Collections.Generic;
using DSx.Shared;

namespace DSx.Mapping
{
    public class TriggerToTriggerConverter : IMappingConverter
    {
        public object Convert(IDictionary<string, object> inputs, IDictionary<string, string> args, out Feedback feedback)
        {
            feedback = new Feedback();

            return inputs["Trigger"];
        }
    }
}