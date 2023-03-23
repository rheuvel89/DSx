using System.Collections.Generic;

namespace DSx.Mapping
{
    public interface IMappingConverter
    {
        public object Convert(object[] inputs, IDictionary<string, string> args, out object? feedback);
    }
}