using System.Collections.Generic;
using System.Linq;

namespace DSx.Mapping
{
    public class OrButtonToButtonConverter : IMappingConverter
    {
        public object Convert(IDictionary<string, object> inputs, IDictionary<string, string> args, out Feedback feedback)
        {
            feedback = new Feedback();

            return inputs.Values.Cast<bool>().Any(b => b);
        }
    }
}