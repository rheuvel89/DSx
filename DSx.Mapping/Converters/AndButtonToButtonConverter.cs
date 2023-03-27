using System.Collections.Generic;
using System.Linq;

namespace DSx.Mapping
{
    public class AndButtonToButtonConverter : IMappingConverter
    {
        public object Convert(IDictionary<string, object> inputs, IDictionary<string, string> args, out Feedback feedback)
        {
            feedback = new Feedback();

            return inputs.Cast<bool>().All(b => b);
        }
    }
}