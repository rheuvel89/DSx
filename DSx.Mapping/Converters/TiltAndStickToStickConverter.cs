using System.Linq;
using DualSenseAPI;

namespace DSx.Mapping
{
    public class TiltAndStickToStickConverter : IMappingConverter
    {
        private readonly TiltToStickConverter _innerConverter;
        private float? _alphaX;
        private float? _alphaY;
        private float? _betaX;
        private float? _betaY;


        public TiltAndStickToStickConverter()
        {
            _innerConverter = new TiltToStickConverter();
        }
        
        public object Convert(object[] inputs, string[] args, out object? feedback)
        {
            feedback = null;

            var tilt = inputs[0];
            var stick = (Vec2)inputs[1];
            var rezero = inputs[2];
            var toggle = inputs[3];
            
            _alphaX ??= args.Length >= 1 && float.TryParse(args[0], out var ax) ? ax : 1f;
            _alphaY ??= args.Length >= 2 && float.TryParse(args[1], out var ay) ? ay : 1f;
            _betaX ??= args.Length >= 3 && float.TryParse(args[2], out var bx) ? bx : 1f;
            _betaY ??= args.Length >= 4 && float.TryParse(args[3], out var by) ? by : 1f;
            
            inputs = new[] { tilt, rezero, toggle };
            args = args.Skip(4).ToArray(); 
            var output = (Vec2)_innerConverter.Convert(inputs, args, out feedback);
            
            output.X = output.X * _alphaX.Value + stick.X * _betaX.Value;
            output.Y = output.Y * _alphaX.Value + stick.Y * _betaX.Value;
            if (output.X < -1) output.X = -1; if (output.X > 1) output.X = 1;
            if (output.Y < -1) output.Y = -1; if (output.Y > 1) output.Y = 1;

            return output;
        }
    }
}