using System;
using DSx.Math;
using DualSenseAPI;

namespace DSx.Mapping
{
    public class KalmanAHRS : IAHRS
    {
        private int _initializing = 500;
        private Kalman _roll = new Kalman();
        private Kalman _pitch = new Kalman();

        private long _timestamp;

        private const double RAD_TO_DEG = 180 / System.Math.PI;
        private const double GYRO_TO_RATE = 0.003;

        private double _driftX = 0;
        private double _driftY = 0;
        private float _zero = 0;
        
        public Vector<float, float, float> Calculate(long timestamp, Vector<float, float, float> rAcc, Vector<float, float, float> rGyr, float sensitivity, float deadzone, bool reZero,
            out Feedback feedback)
        {
            feedback = new Feedback();
            
            var dt = (float)(timestamp - _timestamp) / 1000;
            _timestamp = timestamp;
            
            var pitch  = System.Math.Atan2(rAcc.Y, rAcc.Z) * RAD_TO_DEG + 90;
            if (pitch < 0) pitch += 360;
            var roll = System.Math.Atan(-rAcc.X / System.Math.Sqrt(rAcc.Y * rAcc.Y + rAcc.Z * rAcc.Z)) * RAD_TO_DEG;

            var rateX = rGyr.X * GYRO_TO_RATE;
            var rateY = System.Math.Sqrt(rGyr.Y*rGyr.Y + rGyr.Z*rGyr.Z) * GYRO_TO_RATE;

            if (_initializing-- > 0)
            {
                feedback.MicLed = MicLed.Pulse;
                
                _driftX = _driftX * 0.99 + rateX * 0.01;
                _driftY = _driftY * 0.99 + rateY * 0.01;
                Console.SetCursorPosition(0,0);
                Console.WriteLine($"{_driftX/dt} \t {_driftY/dt}");

                _pitch.SetAngle((float)pitch);
                _roll.SetAngle((float)roll);
                
                return new Vector<float, float, float>((float)pitch, (float)roll, 0);
            }
            
            var kalmanPitch = _pitch.GetAngle((float)pitch, (float)(rateX-_driftX), dt)/45;
            var kalmanRoll = _roll.GetAngle((float)roll, (float)(rateY-_driftY), dt)/45;

            if (reZero) _zero = _zero * 0.9f + kalmanPitch * 0.1f;

            // Console.SetCursorPosition(0,0);
            // Console.WriteLine($"{kalmanRoll} \t {kalmanPitch - _zero}");

            var x = kalmanRoll.Limit1();
            var y = (kalmanPitch - _zero).Limit1();
            
            return new Vector<float, float, float>(x, y, 0);
        }
    }

    public class Kalman
    {
        private float _qAngle = 0.01f; // Decrease if slow (trusting angle too much)
        private float _qBias = 0.8f; // Increase with drift
        private float _rMeasure = 0.03f; // High = slow, small = overshoot/noisy
        
        private float _angle;
        private float _bias;
        private float _rate;

        private float[,] _p = new float[2, 2];

        public float GetAngle(float newAngle, float newRate, float dt)
        {
            _rate = newRate - _bias;
            _angle += dt * _rate;

            _p[0, 0] += dt * (dt * _p[1, 1] - _p[0, 1] - _p[1, 0] + _qAngle);
            _p[0, 1] -= dt * _p[1, 1];
            _p[1, 0] -= dt * _p[1, 1];
            _p[1, 1] += _qBias * dt;

            var s = _p[0, 0] + _rMeasure;
            
            var k = new float[2];
            k[0] = _p[0, 0] / s;
            k[1] = _p[1, 0] / s;

            var y = newAngle - _angle;
            _angle += k[0] * y;
            _bias += k[1] * y;

            float p00 = _p[0, 0];
            float p01 = _p[0, 1];

            _p[0, 0] -= k[0] * p00;
            _p[0, 1] -= k[0] * p01;
            _p[1, 0] -= k[1] * p00;
            _p[1, 1] -= k[1] * p01;

            return _angle;
        }

        public void SetAngle(float angle)
        {
            _angle = angle;
        }
    }
}