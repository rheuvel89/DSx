﻿using System.Collections.Generic;
using System.Diagnostics;
using DSx.Math;
using DSx.Shared;
using DualSenseAPI;

namespace DSx.Mapping
{
    public class TiltToStickConverter : IMappingConverter
    {
        private Stopwatch _timer;
        private bool _active = true;
        private bool _toggled = false;
        private float? _sensitivity;
        private float? _deadzone;
        private float? _gamma;
        private IAHRS? _algorithm;
        

        public TiltToStickConverter()
        {
            _timer = Stopwatch.StartNew();
            //_algorithm = new SimpleAHRS();
            //_algorithm = new CombinedAHRS(0.4f, 0.001f, 0.01f);
            //_algorithm = new MahonyAHRS((float)pollingInterval/1000 , 1f, 0f);
            _algorithm = new KalmanAHRS();
        }
        
        public object Convert(IDictionary<string, object> inputs, IDictionary<string, string> args, out Feedback feedback)
        {
            feedback = new Feedback();

            var acc = (Vec3)inputs["Accelerometer"];
            var gyro = (Vec3)inputs["Gyro"];
            var rezero = (bool)inputs["Zero"];
            var toggle = (bool)inputs["Toggle"];
            
            if (!_toggled && toggle) _active = !_active;
            _toggled = toggle;
            if (!_active) return new Vec2 { X = 0f, Y = 0f };
                
            _sensitivity ??= args.TryGetValue("Sensitivity", out var ss) && float.TryParse(ss, out var s) ? s : 1f;
            _deadzone ??= args.TryGetValue("Deadzone", out var sd) && float.TryParse(sd, out var d) ? d : 0f;
            _gamma ??= args.TryGetValue("Gamma", out var sg) && float.TryParse(sg, out var g) ? g : 1f;
            
            var rAcc = new Vector<float, float, float>(acc.X, acc.Y, acc.Z);
            var rGyr = new Vector<float, float, float>(gyro.X, gyro.Y, gyro.Z);
         
            var result = _algorithm.Calculate(_timer.ElapsedMilliseconds, rAcc.Normalize(), rGyr, _sensitivity.Value, _deadzone.Value, rezero, out feedback);
            
            return result.Deadzone(_deadzone.Value, DeadzoneMode.Center).Mutliply(_sensitivity.Value).Limit1(_gamma.Value);
        }
    }
}