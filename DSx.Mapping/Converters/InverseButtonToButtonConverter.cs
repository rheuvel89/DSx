using System.Collections.Generic;

namespace DSx.Mapping
{
    public class InverseButtonToButtonConverter : IMappingConverter
    {
        public object Convert(object[] inputs, IDictionary<string, string> args, out object? feedback)
        {
            feedback = null;

            return !(bool)inputs[0];
        }
    }
}