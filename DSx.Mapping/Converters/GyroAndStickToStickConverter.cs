using System.Collections.Concurrent;
using System.Diagnostics;
using DSx.Math;
using DualSenseAPI;

namespace DSx.Mapping
{
    public class GyroAndStickToStickConverter : IMappingConverter
    {
        private Stopwatch _timer;
        private bool _active = true;
        private bool _toggled = false;

        public GyroAndStickToStickConverter()
        {
            _timer = Stopwatch.StartNew();
        }
        
        public object Convert(object[] inputs, string[] args, out object? feedback)
        {
            feedback = null;

            var (acc, gyro) = ((Vec3, Vec3))inputs[0];
            var stick = (Vec2)inputs[1];
            var toggle = (bool)inputs[2];
            
            if (!_toggled && toggle) _active = !_active;
            _toggled = toggle;
            if (!_active) return new Vec2 { X = stick.X, Y = stick.Y };

            return new Vec2 { X = (float)System.Math.Sqrt(gyro.Y * gyro.Y + gyro.Z * gyro.Z) / 1000, Y = gyro.X / 1000 };
        }
    }
}