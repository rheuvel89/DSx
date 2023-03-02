using System.Collections.Concurrent;
using System.Linq;
using DSx.Math;

namespace DSx.Mapping
{
    public class CombinedAHRS : IAHRS
    {
        private Vector<float, float, float> _reading = Vector<float, float, float>.Zero;
        private Vector<float, float, float> _zAcc = new Vector<float, float, float>(0, 1, 0);
        private Vector<ConcurrentQueue<float>, ConcurrentQueue<float>, ConcurrentQueue<float>> _aZero =
            new Vector<ConcurrentQueue<float>, ConcurrentQueue<float>, ConcurrentQueue<float>>(
                new ConcurrentQueue<float>(), new ConcurrentQueue<float>(), new ConcurrentQueue<float>());

        private float _alpha;
        private readonly float _epsilon;
        private readonly float _gamma;
        private long _previousTimestamp = 0;

        public CombinedAHRS(float alpha, float epsilon, float gamma)
        {
            _alpha = alpha;
            _epsilon = epsilon;
            _gamma = gamma;
        }

        public Vector<float, float, float> Calculate(
            long timestamp,
            Vector<float, float, float> rAcc,
            Vector<float, float, float> rGyr,
            float sensitivity,
            float deadzone,
            bool reZero,
            out Vector<float, float> rumble)
        {
            var delta = timestamp - _previousTimestamp;
            _previousTimestamp = timestamp;
            _zAcc = TryZero(rAcc, _aZero, reZero) ?? _zAcc;
            if (reZero) _reading = Vector<float, float, float>.Zero; 

            var zRoll = System.Math.Atan2(_zAcc.Y, _zAcc.Z);
            var zPitch = System.Math.Atan2(-_zAcc.X, System.Math.Sqrt(_zAcc.Y * _zAcc.Y + _zAcc.Z * _zAcc.Z));
            var aRoll = System.Math.Atan2(rAcc.Y, rAcc.Z);
            var aPitch = System.Math.Atan2(-rAcc.X, System.Math.Sqrt(rAcc.Y * rAcc.Y + rAcc.Z * rAcc.Z));

            var gPitch = _reading.X + rGyr.X * _epsilon * delta;
            var gRoll = _reading.Y + rGyr.Y * _gamma * delta;

            var beta = 1 - _alpha;
            _reading.X = (float)(_alpha * gPitch + beta * (aPitch - zPitch));  
            _reading.Y = (float)(_alpha * gRoll + beta * (aRoll - zRoll));  

            rumble = Vector<float, float>.Zero;
            if (_reading.X > 0.9 || _reading.X < -0.9 || _reading.Y > 0.9 || _reading.Y < -0.9) rumble.X = rumble.Y = 0.01f;

            _reading.X *= sensitivity;
            _reading.Y *= sensitivity;
            if (_reading.X > -deadzone && _reading.X < deadzone) _reading.X = 0;
            else if (_reading.X < 0) _reading.X += deadzone;
            else if (_reading.X > 0) _reading.X -= deadzone;
            if (_reading.Y > -deadzone && _reading.Y < deadzone) _reading.Y = 0;
            else if (_reading.Y < 0) _reading.Y += deadzone;
            else if (_reading.Y > 0) _reading.Y -= deadzone;
            if (_reading.X > 1) _reading.X = 1;
            if (_reading.X < -1) _reading.X = -1;
            if (_reading.Y > 1) _reading.Y = 1;
            if (_reading.Y < -1) _reading.Y = -1;

            return _reading;
        }

        private Vector<float, float, float> TryZero(Vector<float, float, float> r,
            Vector<ConcurrentQueue<float>, ConcurrentQueue<float>, ConcurrentQueue<float>> z, bool reZero)
        {
            Vector<float, float, float> zero = null;
            if (reZero) zero = z.AddAverage(r, 10);
            else if (z.X.Any())
            {
                z.X.Clear();
                z.Y.Clear();
                z.Z.Clear();
            }

            return zero;
        }
    }
}