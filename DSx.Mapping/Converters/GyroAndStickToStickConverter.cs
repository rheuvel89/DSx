using System.Collections.Generic;
using DualSenseAPI;

namespace DSx.Mapping
{
    public class GyroAndStickToStickConverter : IMappingConverter
    {
        private readonly GyroToStickConverter _innerConverter;
        private float? _alphaX;
        private float? _alphaY;
        private float? _betaX;
        private float? _betaY;

        public GyroAndStickToStickConverter()
        {
            _innerConverter = new GyroToStickConverter();
        }
        
        public object Convert(IDictionary<string, object> inputs, IDictionary<string, string> args, out Feedback feedback)
        {
            feedback = new Feedback();
            
            var stick = (Vec2)inputs["Stick"];

            _alphaX ??= args.TryGetValue("AlphaX", out var sax) && float.TryParse(sax, out var ax) ? ax : 1f;
            _alphaY ??= args.TryGetValue("AlphaY", out var say) && float.TryParse(say, out var ay) ? ay : 1f;
            _betaX ??= args.TryGetValue("BetaX", out var sbx) && float.TryParse(sbx, out var bx) ? bx : 1f;
            _betaY ??= args.TryGetValue("BetaY", out var sby) && float.TryParse(sby, out var by) ? by : 1f;

            var output = (Vec2)_innerConverter.Convert(inputs, args, out feedback);
            
            output.X = output.X * _alphaX.Value + stick.X * _betaX.Value;
            output.Y = output.Y * _alphaY.Value + stick.Y * _betaX.Value;

            return output.Limit1();
        }
    }
}