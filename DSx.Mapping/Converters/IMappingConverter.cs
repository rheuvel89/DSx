using System.Collections.Generic;
using DSx.Shared;

namespace DSx.Mapping
{
    public interface IMappingConverter
    {
        public object Convert(IDictionary<string, object> inputs, IDictionary<string, string> args, out Feedback feedback);
    }
}