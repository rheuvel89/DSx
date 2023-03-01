using System.Collections.Concurrent;
using DSx.Math;

namespace DSx.Mapping
{
    public class TiltToJoystickConverter
    {
        private bool _active = true;
        private bool _toggled = false;
        private float _sensitivity = 1f;
        private float _deadzone = 0f;
        private Vector<float, float, float> _zAcc = new Vector<float, float, float>(0, 1, 0);
        private Vector<ConcurrentQueue<float>, ConcurrentQueue<float>, ConcurrentQueue<float>>? _aZero = null;

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
        
        public Vector<float, float> Convert(Vector<float, float, float> rAcc, bool reZero, bool toggle, out Vector<float, float> rumble)
        {
            if (!_toggled && toggle) _active = !_active;
            _toggled = toggle;
            if (!_active)
            {
                rumble = Vector<float, float>.Zero;
                return Vector<float, float>.Zero;
            }
            
            if (reZero)
            {
                _aZero ??= new Vector<ConcurrentQueue<float>, ConcurrentQueue<float>, ConcurrentQueue<float>>(
                    new ConcurrentQueue<float>(), new ConcurrentQueue<float>(), new ConcurrentQueue<float>());
                _zAcc = _aZero.AddAverage(rAcc, 10);
            }
            else if (_aZero != null) _aZero = null;

            var zRoll = System.Math.Atan2(_zAcc.Y, _zAcc.Z);
            var zPitch = System.Math.Atan2(-_zAcc.X, System.Math.Sqrt(_zAcc.Y * _zAcc.Y + _zAcc.Z * _zAcc.Z));
            var rRoll = System.Math.Atan2(rAcc.Y, rAcc.Z) - zRoll;
            var rPitch = System.Math.Atan2(-rAcc.X, System.Math.Sqrt(rAcc.Y * rAcc.Y + rAcc.Z * rAcc.Z)) - zPitch;

            rumble = Vector<float, float>.Zero;
            if (rPitch > 0.9 || rPitch < -0.9 || rRoll > 0.9 || rRoll < -0.9) rumble.X = rumble.Y = 0.01f;

            rPitch *= _sensitivity;
            rRoll *= _sensitivity;
            if (rPitch > -_deadzone && rPitch < _deadzone) rPitch = 0;
            else if (rPitch < 0) rPitch += _deadzone;
            else if (rPitch > 0) rPitch -= _deadzone;
            if (rRoll > -_deadzone && rRoll < _deadzone) rRoll = 0;
            else if (rRoll < 0) rRoll += _deadzone;
            else if (rRoll > 0) rRoll -= _deadzone;
            if (rPitch > 1) rPitch = 1;
            if (rPitch < -1) rPitch = -1;
            if (rRoll > 1) rRoll = 1;
            if (rRoll < -1) rRoll = -1;

            return new Vector<float, float>((float)rPitch, (float)rRoll);
        }
    }
}