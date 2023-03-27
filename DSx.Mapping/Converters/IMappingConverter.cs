using System.Collections.Generic;

namespace DSx.Mapping
{
    public interface IMappingConverter
    {
        public object Convert(IDictionary<string, object> inputs, IDictionary<string, string> args, out object? feedback);
    }
}