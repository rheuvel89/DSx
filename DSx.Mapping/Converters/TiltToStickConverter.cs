using System.Collections.Generic;
using System.Diagnostics;
using DSx.Math;
using DualSenseAPI;

namespace DSx.Mapping
{
    public class TiltToStickConverter : IMappingConverter
    {
        private Stopwatch _timer;
        private bool _active = true;
        private bool _toggled = false;
        private float? _sensitivity;
        private float? _deadzone;
        private IAHRS? _algorithm;
        

        public TiltToStickConverter()
        {
            _timer = Stopwatch.StartNew();
            //_algorithm = new SimpleAHRS();
            //_algorithm = new CombinedAHRS(0.4f, 0.001f, 0.01f);
            //_algorithm = new MahonyAHRS((float)pollingInterval/1000 , 1f, 0f);
            _algorithm = new KalmanAHRS();
        }
        
        public object Convert(object[] inputs, IDictionary<string, string> args, out object? feedback)
        {
            feedback = null;

            var (acc, gyro) = ((Vec3, Vec3))inputs[0];
            var rezero = (bool)inputs[1];
            var toggle = (bool)inputs[2];
            
            if (!_toggled && toggle) _active = !_active;
            _toggled = toggle;
            if (!_active) return new Vec2 { X = 0f, Y = 0f };
                
            _sensitivity ??= args.TryGetValue("Sensitivity", out var ss) && float.TryParse(ss, out var s) ? s : 1f;
            _deadzone ??= args.TryGetValue("Deadzone", out var sd) && float.TryParse(sd, out var d) ? d : 0f;
            
            var rAcc = new Vector<float, float, float>(acc.X, acc.Y, acc.Z);
            var rGyr = new Vector<float, float, float>(gyro.X, gyro.Y, gyro.Z);
         
            var result = _algorithm.Calculate(_timer.ElapsedMilliseconds, rAcc.Normalize(), rGyr, _sensitivity.Value, _deadzone.Value, rezero, out var rumble);
            feedback = new Vec2 { X = rumble.X, Y = rumble.Y };
            
            return new Vec2 { X = -result.X, Y = result.Y }.Limit1();
        }
    }
}