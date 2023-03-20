using System.Linq;
using DualSenseAPI;

namespace DSx.Mapping
{
    public class GyroAndStickToStickConverter : IMappingConverter
    {
        private readonly GyroToStickConverter _innerConverter;

        public GyroAndStickToStickConverter()
        {
            _innerConverter = new GyroToStickConverter();
        }
        
        public object Convert(object[] inputs, string[] args, out object? feedback)
        {
            feedback = null;
            
            var tilt = inputs[0];
            var stick = (Vec2)inputs[1];
            var toggle = inputs[2];

            var alphaX = args.Length >= 1 && float.TryParse(args[0], out var ax) ? ax : 1f;
            var alphaY = args.Length >= 2 && float.TryParse(args[1], out var ay) ? ay : 1f;
            var betaX = args.Length >= 3 && float.TryParse(args[2], out var bx) ? bx : 1f;
            var betaY = args.Length >= 4 && float.TryParse(args[3], out var by) ? by : 1f;

            inputs = new[] { tilt, toggle };
            args = args.Skip(4).ToArray(); 
            var output = (Vec2)_innerConverter.Convert(inputs, args, out feedback);
            
            output.X = output.X * alphaX + stick.X * betaX;
            output.Y = output.Y * alphaY + stick.Y * betaY;
            if (output.X < -1) output.X = -1; if (output.X > 1) output.X = 1;
            if (output.Y < -1) output.Y = -1; if (output.Y > 1) output.Y = 1;

            return output;
        }
    }
}