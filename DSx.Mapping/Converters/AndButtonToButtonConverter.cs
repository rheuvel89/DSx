using System.Collections.Generic;
using System.Linq;

namespace DSx.Mapping
{
    public class AndButtonToButtonConverter : IMappingConverter
    {
        public object Convert(object[] inputs, IDictionary<string, string> args, out object? feedback)
        {
            feedback = null;

            return inputs.Cast<bool>().All(b => b);
        }
    }
}