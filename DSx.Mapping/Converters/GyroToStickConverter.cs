using System.Collections.Concurrent;
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
        
        public object Convert(object[] inputs, string[] args, out object? feedback)
        {
            feedback = null;

            var (acc, gyro) = ((Vec3, Vec3))inputs[0];
            var toggle = (bool)inputs[1];
            
            if (!_toggled && toggle) _active = !_active;
            _toggled = toggle;
            if (!_active) return new Vec2 { X = 0f, Y = 0f };
            
            _factorX ??= args.Length >= 1 && float.TryParse(args[0], out var fx) ? fx : 1f;
            _factorY ??= args.Length >= 2 && float.TryParse(args[1], out var fy) ? fy : 1f;

            var elapsed = -_timestamp + (_timestamp = _timer.ElapsedMilliseconds);
            _xQueue.Enqueue( (float)(System.Math.Sign(gyro.Y+ gyro.Z) * System.Math.Sqrt(gyro.Y * gyro.Y + gyro.Z * gyro.Z) * ((float)elapsed / 1000)));
            _yQueue.Enqueue(gyro.X * ((float)elapsed / 1000));
            
            _timestamp = _timer.ElapsedMilliseconds;

            if (_xQueue.Count > 10) _xQueue.TryDequeue(out _);
            if (_yQueue.Count > 10) _yQueue.TryDequeue(out _);

            var x = (float)System.Math.Tanh(_xQueue.ToArray().Average() * _factorX.Value);
            var y = (float)System.Math.Tanh(_yQueue.ToArray().Average() * _factorY.Value);
            
            return new Vec2 { X = x, Y = y };
        }
    }
}