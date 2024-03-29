﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DSx.Shared;
using DualSenseAPI;

namespace DSx.Mapping
{
    public class GyroToStickConverter : IMappingConverter
    {
        private Stopwatch _timer;
        private int _initializing = 500;
        private long _timestamp = 0;
        private bool _active = true;
        private bool _toggled = false;
        private float? _gammaX;
        private float? _gammaY;
        private float? _epsilonX;
        private float? _epsilonY;
        
        private float _driftX = 0;
        private float _driftY = 0;

        private ConcurrentQueue<float> _xQueue;
        private ConcurrentQueue<float> _yQueue;

        public GyroToStickConverter()
        {
            _timer = Stopwatch.StartNew();
            _xQueue = new ConcurrentQueue<float>();
            _yQueue = new ConcurrentQueue<float>();
        }
        
        public object Convert(IDictionary<string, object> inputs, IDictionary<string, string> args, out Feedback feedback)
        {
            feedback = new Feedback();

            var gyro = (Vec3)inputs["Gyro"];
            var toggle = (bool)inputs["Toggle"];
            
            if (!_toggled && toggle) _active = !_active;
            _toggled = toggle;
            if (!_active) return new Vec2 { X = 0f, Y = 0f };
            
            _gammaX ??= args.TryGetValue("GammaX", out var sgx) && float.TryParse(sgx, out var gx) ? gx : 1f;
            _gammaY ??= args.TryGetValue("GammaY", out var sgy) && float.TryParse(sgy, out var gy) ? gy : 1f;
            _epsilonX ??= args.TryGetValue("EpsilonX", out var sex) && float.TryParse(sex, out var ex) ? ex : 1f;
            _epsilonY ??= args.TryGetValue("EpsilonY", out var sey) && float.TryParse(sey, out var ey) ? ey : 1f;

            var delta = -_timestamp + (_timestamp = _timer.ElapsedMilliseconds);
            _xQueue.Enqueue( (float)(System.Math.Sign(gyro.Y+ gyro.Z) * System.Math.Sqrt(gyro.Y * gyro.Y + gyro.Z * gyro.Z) * ((float)delta / 1000)));
            _yQueue.Enqueue(gyro.X * ((float)delta / 1000));
            
            _timestamp = _timer.ElapsedMilliseconds;

            if (_xQueue.Count > 10) _xQueue.TryDequeue(out _);
            if (_yQueue.Count > 10) _yQueue.TryDequeue(out _);

            var x = (_xQueue.ToArray().Average() * _epsilonX.Value).Limit1(_gammaX.Value);
            var y = (_yQueue.ToArray().Average() * _epsilonY.Value).Limit1(_gammaY.Value);
            
            
            if (_initializing-- > 0)
            {
                feedback.MicLed = MicLed.Pulse;
                
                _driftX = _driftX * 0.99f + x * 0.01f;
                _driftY = _driftY * 0.99f + y * 0.01f;
            }
            
            return new Vec2 { X = x - _driftX, Y = y - _driftY };
        }
    }
}