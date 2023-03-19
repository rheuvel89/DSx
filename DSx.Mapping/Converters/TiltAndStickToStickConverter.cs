using System.Collections.Concurrent;
using System.Diagnostics;
using DSx.Math;
using DualSenseAPI;

namespace DSx.Mapping
{
    public class TiltAndStickToStickConverter : IMappingConverter
    {
        private Stopwatch _timer;
        private bool _active = true;
        private bool _toggled = false;
        private float _sensitivity = 1f;
        private float _deadzone = 0f;
        private IAHRS? _algorithm = null;
        

        public TiltAndStickToStickConverter()
        {
            _timer = Stopwatch.StartNew();
            //_algorithm = new SimpleAHRS();
            _algorithm = new CombinedAHRS(0.4f, 0.001f, 0.01f);
            //_algorithm = new MahonyAHRS((float)pollingInterval/1000 , 1f, 0f);
        }
        
        public float Sensitivity
        {
            get => _sensitivity;
            set => _sensitivity = value;
        }
        public float Deadzone
        {
            get => _deadzone;
            set => _deadzone = value;
        }
        
        public Vector<float, float, float> Convert(long timestamp, Vector<float, float, float> rAcc, Vector<float, float, float> rGyr, bool reZero, bool toggle, out Vector<float, float> rumble)
        {
            if (!_toggled && toggle) _active = !_active;
            _toggled = toggle;
            if (!_active)
            {
                rumble = Vector<float, float>.Zero;
                return Vector<float, float, float>.Zero;
            }

            return _algorithm.Calculate(timestamp, rAcc.Normalize(), rGyr.Normalize(), _sensitivity, _deadzone, reZero, out rumble);
        }

        public object Convert(object[] inputs, string[] args, out object? feedback)
        {
            feedback = null;

            var (acc, gyro) = ((Vec3, Vec3))inputs[0];
            var stick = (Vec2)inputs[1];
            var rezero = (bool)inputs[2];
            var toggle = (bool)inputs[3];
            
            if (!_toggled && toggle) _active = !_active;
            _toggled = toggle;
            if (!_active) return new Vec2 { X = 0f, Y = 0f };
                
            var alpha = args.Length >= 1 && float.TryParse(args[0], out var a) ? a : 1f;
            var beta = args.Length >= 2 && float.TryParse(args[1], out var b) ? b : 1f;
            var sense = args.Length >= 3 && float.TryParse(args[2], out var s) ? s : 1f;
            var dead = args.Length >= 4 && float.TryParse(args[3], out var d) ? d : 0f;
            
            var rAcc = new Vector<float, float, float>(acc.X, acc.Y, acc.Z);
            var rGyr = new Vector<float, float, float>(gyro.X, gyro.Y, gyro.Z);
         
            var result = _algorithm.Calculate(_timer.ElapsedMilliseconds, rAcc.Normalize(), rGyr.Normalize(), sense, dead, rezero, out var rumble);
            feedback = new Vec2 { X = rumble.X, Y = rumble.Y };

            var x = -result.X * alpha + stick.X * beta;
            var y = result.Y * alpha + stick.Y * beta;
            if (x < -1) x = -1; if (x > 1) x = 1;
            if (y < -1) y = -1; if (y > 1) y = 1;
            return new Vec2 { X = x, Y = y};
        }
    }
}