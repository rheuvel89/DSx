using System.Collections.Concurrent;
using DSx.Math;
using DSx.Shared;
using DualSenseAPI;

namespace DSx.Mapping
{
    public class SimpleAHRS : IAHRS
    {
        
        private Vector<float, float, float> _zAcc = new Vector<float, float, float>(0, 1, 0);
        private Vector<ConcurrentQueue<float>, ConcurrentQueue<float>, ConcurrentQueue<float>>? _aZero = null;
        
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
            
            TryZero(rAcc, reZero);

            var zRoll = System.Math.Atan2(_zAcc.Y, _zAcc.Z);
            var zPitch = System.Math.Atan2(-_zAcc.X, System.Math.Sqrt(_zAcc.Y * _zAcc.Y + _zAcc.Z * _zAcc.Z));
            var rRoll = System.Math.Atan2(rAcc.Y, rAcc.Z) - zRoll;
            var rPitch = System.Math.Atan2(-rAcc.X, System.Math.Sqrt(rAcc.Y * rAcc.Y + rAcc.Z * rAcc.Z)) - zPitch;

            if (rPitch > 0.9 || rPitch < -0.9 || rRoll > 0.9 || rRoll < -0.9) feedback.Rumble = new Vec2 { X = 0.01f, Y = 0.01f };

            rPitch *= sensitivity;
            rRoll *= sensitivity;
            if (rPitch > -deadzone && rPitch < deadzone) rPitch = 0;
            else if (rPitch < 0) rPitch += deadzone;
            else if (rPitch > 0) rPitch -= deadzone;
            if (rRoll > -deadzone && rRoll < deadzone) rRoll = 0;
            else if (rRoll < 0) rRoll += deadzone;
            else if (rRoll > 0) rRoll -= deadzone;
            if (rPitch > 1) rPitch = 1;
            if (rPitch < -1) rPitch = -1;
            if (rRoll > 1) rRoll = 1;
            if (rRoll < -1) rRoll = -1;

            return new Vec2 { X = (float)rPitch, Y = (float)rRoll };
        }

        private void TryZero(Vector<float, float, float> rAcc, bool reZero)
        {
            if (reZero)
            {
                _aZero ??= new Vector<ConcurrentQueue<float>, ConcurrentQueue<float>, ConcurrentQueue<float>>(
                    new ConcurrentQueue<float>(), new ConcurrentQueue<float>(), new ConcurrentQueue<float>());
                _zAcc = _aZero.AddAverage(rAcc, 10);
            }
            else if (_aZero != null) _aZero = null;
        }
    }
}