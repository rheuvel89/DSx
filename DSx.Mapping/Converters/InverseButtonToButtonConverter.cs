using System.Collections.Generic;

namespace DSx.Mapping
{
    public class InverseButtonToButtonConverter : IMappingConverter
    {
        public object Convert(IDictionary<string, object> inputs, IDictionary<string, string> args, out Feedback feedback)
        {
            feedback = new Feedback();

            return !(bool)inputs["Button"];
        }
    }
}