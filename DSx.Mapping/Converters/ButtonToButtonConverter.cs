using System.Collections.Generic;

namespace DSx.Mapping
{
    public class ButtonToButtonConverter : IMappingConverter
    {
        public object Convert(object[] inputs, IDictionary<string, string> args, out object? feedback)
        {
            feedback = null;

            return inputs[0];
        }
    }
}