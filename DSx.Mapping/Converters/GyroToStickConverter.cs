using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DualSenseAPI;

namespace DSx.Mapping
{
    public class GyroToStickConverter : IMappingConverter
    {
        private Stopwatch _timer;
        private long _timestamp = 0;
        private bool _active = true;
        private bool _toggled = false;
        private float? _factorX;
        private float? _factorY;

        private ConcurrentQueue<float> _xQueue;
        private ConcurrentQueue<float> _yQueue;

        public GyroToStickConverter()
        {
            _timer = Stopwatch.StartNew();
            _xQueue = new ConcurrentQueue<float>();
            _yQueue = new ConcurrentQueue<float>();
        }
        
        public object Convert(object[] inputs, IDictionary<string, string> args, out object? feedback)
        {
            feedback = null;

            var (acc, gyro) = ((Vec3, Vec3))inputs[0];
            var toggle = (bool)inputs[1];
            
            if (!_toggled && toggle) _active = !_active;
            _toggled = toggle;
            if (!_active) return new Vec2 { X = 0f, Y = 0f };
            
            _factorX ??= args.TryGetValue("GammaX", out var sgx) && float.TryParse(sgx, out var gx) ? gx : 1f;
            _factorY ??= args.TryGetValue("GammaY", out var sgy) && float.TryParse(sgy, out var gy) ? gy : 1f;

            var delta = -_timestamp + (_timestamp = _timer.ElapsedMilliseconds);
            _xQueue.Enqueue( (float)(System.Math.Sign(gyro.Y+ gyro.Z) * System.Math.Sqrt(gyro.Y * gyro.Y + gyro.Z * gyro.Z) * ((float)delta / 1000)));
            _yQueue.Enqueue(gyro.X * ((float)delta / 1000));
            
            _timestamp = _timer.ElapsedMilliseconds;

            if (_xQueue.Count > 10) _xQueue.TryDequeue(out _);
            if (_yQueue.Count > 10) _yQueue.TryDequeue(out _);

            var x = (float)System.Math.Tanh(_xQueue.ToArray().Average() * _factorX.Value);
            var y = (float)System.Math.Tanh(_yQueue.ToArray().Average() * _factorY.Value);
            
            return new Vec2 { X = x, Y = y };
        }
    }
}