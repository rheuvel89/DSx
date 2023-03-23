using System.Collections.Generic;
using DualSenseAPI;

namespace DSx.Mapping
{
    public class StickToStickConverter : IMappingConverter
    {
        public object Convert(object[] inputs, IDictionary<string, string> args, out object? feedback)
        {
            feedback = null;

            return inputs[0];
        }
    }
}