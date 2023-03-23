using System.Collections.Generic;
using DualSenseAPI;

namespace DSx.Mapping
{
    public class StickToStickConverter : IMappingConverter
    {
        public object Convert(object[] inputs, IDictionary<string, string> args, out object? feedback)
        {
            feedback = null;

            var deadzone = args.TryGetValue("Deadzone", out var sd) && float.TryParse(sd, out var d) ? d : 0f;
            var input = (Vec2)inputs[0];

            return input.Deadzone(deadzone, DeadzoneMode.Center);
        }
    }
}