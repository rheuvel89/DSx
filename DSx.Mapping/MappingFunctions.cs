// using System;

using System;
using DualSenseAPI;

namespace DSx.Mapping
{
    public static class MappingFunctions
    {
        public static float Limit1(this float f, float c)
        {
            // var x = f * c;
            // return
            //     (System.MathF.Pow(System.MathF.E, x) - System.MathF.Pow(System.MathF.E, -x))
            //     /
            //     (System.MathF.Pow(System.MathF.E, x) + System.MathF.Pow(System.MathF.E, -x));
            var x = MathF.Sign(f)*MathF.Pow(MathF.Abs(f), c);
            return x < -1 ? -1 : x > 1 ? 1 : x;

        }
        
        public static float Limit1(this float f)
        {
            return f < -1 ? -1 : f > 1 ? 1 : f;
        }

        public static Vec2 Deadzone(this Vec2 input, float deadzone, DeadzoneMode mode)
        {
            var returnValue = new Vec2(); 
            var oneOver = 1 / (1-deadzone);
            
            if (mode == DeadzoneMode.Cross)
            {
                returnValue.X = input.X <= deadzone && input.X >= -deadzone 
                    ? 0 
                    : input.X < 0 ? input.X + deadzone : input.X - deadzone;
                returnValue.Y = input.Y <= deadzone && input.Y >= -deadzone 
                    ? 0 
                    : input.Y < 0 ? input.Y + deadzone : input.Y - deadzone;
                returnValue = returnValue.Limit1();
            }

            if (mode == DeadzoneMode.Center)
            {
                var length = input.Magnitude();
                returnValue = length <= deadzone
                    ? returnValue
                    : input.Subtract(input.Normalize().Mutliply(deadzone))
                        .Mutliply(oneOver)
                        .Limit1();
            }

            return returnValue;
        }

        public static Vec2 Mutliply(this Vec2 input, float factor)
        {
            return new Vec2
            {
                X = input.X * factor,
                Y = input.Y * factor,
            };
        }

        public static Vec2 Subtract(this Vec2 a, Vec2 b)
        {
            return new Vec2
            {
                X = a.X - b.X,
                Y = a.Y - b.Y,
            };
        }

        public static Vec2 Limit1(this Vec2 a)
        {
            return new Vec2
            {
                X = a.X.Limit1(),
                Y = a.Y.Limit1()
            };
        }

        public static Vec2 Limit1(this Vec2 a, float c)
        {
            return new Vec2
            {
                X = a.X.Limit1(c),
                Y = a.Y.Limit1(c)
            };
        }
    }
}