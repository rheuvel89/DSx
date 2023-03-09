using System.Collections.Concurrent;
using DSx.Math;

namespace DSx.Mapping
{
    public class TiltToJoystickConverter : IMappingConveter
    {
        private bool _active = true;
        private bool _toggled = false;
        private float _sensitivity = 1f;
        private float _deadzone = 0f;
        private IAHRS? _algorithm = null;

        public TiltToJoystickConverter()
        {
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
    }
}