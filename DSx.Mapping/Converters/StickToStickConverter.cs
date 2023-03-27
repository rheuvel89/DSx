using System.Collections.Generic;
using DualSenseAPI;

namespace DSx.Mapping
{
    public class StickToStickConverter : IMappingConverter
    {
        public object Convert(IDictionary<string, object> inputs, IDictionary<string, string> args, out Feedback feedback)
        {
            feedback = new Feedback();

            var deadzone = args.TryGetValue("Deadzone", out var sd) && float.TryParse(sd, out var d) ? d : 0f;
            var input = (Vec2)inputs["Stick"];

            return input.Deadzone(deadzone, DeadzoneMode.Center);
        }
    }
}