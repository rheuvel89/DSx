using System.Collections.Concurrent;
using System.Linq;
using DSx.Math;
using DualSenseAPI;

namespace DSx.Mapping
{
    public class CombinedAHRS : IAHRS
    {
        private Vector<float, float> _reading = Vector<float, float>.Zero;
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

        public Vec2 Calculate(
            long timestamp,
            Vector<float, float, float> rAcc,
            Vector<float, float, float> rGyr,
            float sensitivity,
            float deadzone,
            bool reZero,
            out Feedback feedback)
        {
            feedback = new Feedback();

            var delta = timestamp - _previousTimestamp;
            _previousTimestamp = timestamp;
            _zAcc = TryZero(rAcc, _aZero, reZero) ?? _zAcc;
            if (reZero) _reading = Vector<float, float>.Zero; 

            var zPitch = System.Math.Atan2(_zAcc.Y, _zAcc.Z);
            var zRoll = System.Math.Atan2(-_zAcc.X, System.Math.Sqrt(_zAcc.Y * _zAcc.Y + _zAcc.Z * _zAcc.Z));
            var aPitch = System.Math.Atan2(rAcc.Y, rAcc.Z);
            var aRoll = System.Math.Atan2(-rAcc.X, System.Math.Sqrt(rAcc.Y * rAcc.Y + rAcc.Z * rAcc.Z));

            var gPitch = _reading.X + rGyr.X * _epsilon * delta;
            var gRoll = _reading.Y + rGyr.Y * _gamma * delta;

            var beta = 1 - _alpha;
            _reading.X = (float)(_alpha * gPitch + beta * aRoll);  
            _reading.Y = (float)(_alpha * gRoll + beta * (aPitch - zPitch));  

            if (_reading.X > 0.9 || _reading.X < -0.9 || _reading.Y > 0.9 || _reading.Y < -0.9) feedback.Rumble = new Vec2 { X = 0.01f, Y = 0.01f };

            _reading.X *= sensitivity;
            _reading.Y *= sensitivity;
            if (_reading.Length() < deadzone) _reading.X = _reading.Y = 0;
            else _reading = _reading.Subtract(_reading.Normalize().Multiply(deadzone));
            if (_reading.X > 1) _reading.X = 1;
            if (_reading.X < -1) _reading.X = -1;
            if (_reading.Y > 1) _reading.Y = 1;
            if (_reading.Y < -1) _reading.Y = -1;

            return new Vec2 { X = _reading.X, Y = _reading.Y, };
        }

        private Vector<float, float, float>? TryZero(Vector<float, float, float> r,
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