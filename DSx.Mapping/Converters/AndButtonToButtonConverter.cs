using System.Collections.Generic;
using System.Linq;
using DSx.Shared;

namespace DSx.Mapping
{
    public class AndButtonToButtonConverter : IMappingConverter
    {
        public object Convert(IDictionary<string, object> inputs, IDictionary<string, string> args, out Feedback feedback)
        {
            feedback = new Feedback();

            return inputs.Values.Cast<bool>().All(b => b);
        }
    }
}