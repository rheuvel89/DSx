using System;
using System.Collections.Concurrent;
using System.Linq;

namespace DSx.Math
{
    public static class Extensions
    {
        public static Vector<float, float, float> Normalize(this Vector<float, float, float> source)
        {
            var length = source.Length();
            return new Vector<float, float, float>(source.X / length, source.Y / length, source.Z / length);
        }
    
        public static float Length(this Vector<float, float, float> source)
        {
            return (float)System.Math.Sqrt(source.X * source.X + source.Y * source.Y + source.Z * source.Z);
        }
        public static float Dot(this Vector<float, float, float> source, Vector<float, float, float> value)
        {
            return source.X * value.X + source.Y * value.Y + source.Z * value.Z;
        }
    
        public static Vector<float, float, float> Cross(this Vector<float, float, float> source, Vector<float, float, float> value)
        {
            var x = source.Y * value.Z - source.Z * value.Y;
            var y = -(source.X * value.Z - source.Z * value.X);
            var z = source.X * value.Y - source.Y * value.X;
            return new Vector<float, float, float>(x, y, z);
        }

        public static float Angle(this Vector<float, float, float> source,
            Vector<float, float, float> value,
            AngleRepresentation rep = AngleRepresentation.Radians)
        {
            var dot = source.Dot(value);
            var ls = source.Length();
            var lv = value.Length();
            var angle = System.Math.Acos(dot / (ls * lv));
            return rep switch
            {
                AngleRepresentation.Radians => (float)angle,
                AngleRepresentation.Degrees => (float)(angle * 180 / System.Math.PI),
                AngleRepresentation.Unit => (float)(angle / (System.Math.PI * 2))
            };
        }

        public static Vector<float, float, float> Project(this Vector<float, float, float> source,
            Vector<float, float, float> p1,
            Vector<float, float, float> p2
        )
        {
            var normal = p1.Cross(p2);
            var dot = source.Dot(normal);
            var length = normal.Length();
            var factor = dot / length;
            var orthogonal = new Vector<float, float, float>(factor * normal.X, factor * normal.Y, factor * normal.Z);
            return source.Subtract(orthogonal);
        }

        public static float DirCos(this Vector<float, float, float> source,
            Vector<float, float, float> axis,
            AngleRepresentation rep = AngleRepresentation.Radians
        )
        {
            axis = axis.Normalize();
            var dot = source.Dot(axis);
            var length = source.Length();
            var angle = System.Math.Acos(dot / length);
            return rep switch
            {
                AngleRepresentation.Radians => (float)angle,
                AngleRepresentation.Degrees => (float)(angle * 180 / System.Math.PI),
                AngleRepresentation.Unit => (float)(angle / (System.Math.PI * 2))
            };
        }

        public static float DirSin(this Vector<float, float, float> source,
            Vector<float, float, float> axis,
            AngleRepresentation rep = AngleRepresentation.Radians
        )
        {
            axis = axis.Normalize();
            var dot = source.Dot(axis);
            var length = source.Length();
            var angle = System.Math.Asin(dot / length);
            return rep switch
            {
                AngleRepresentation.Radians => (float)angle,
                AngleRepresentation.Degrees => (float)(angle * 180 / System.Math.PI),
                AngleRepresentation.Unit => (float)(angle / (System.Math.PI * 2))
            };
        }

        public static float SignedAngle(this Vector<float, float, float> source,
            Vector<float, float, float> p1,
            Vector<float, float, float> p2,
            AngleRepresentation rep = AngleRepresentation.Radians
        )
        {
            p1 = p1.Normalize();
            p2 = p2.Normalize();
            var dot1 = source.Dot(p1);
            var dot2 = source.Dot(p2);
            var length = source.Length();
            var angle = System.Math.Atan2(dot2, dot1);
            return rep switch
            {
                AngleRepresentation.Radians => (float)angle,
                AngleRepresentation.Degrees => (float)(angle * 180 / System.Math.PI),
                AngleRepresentation.Unit => (float)(angle / (System.Math.PI * 2))
            };
        }
        
        public static Vector<float, float, float> ToSpherical(this Vector<float, float, float> source, 
            AngleRepresentation representation = AngleRepresentation.Radians)
        {
            var r = source.Length();
            var theha = System.Math.Acos(source.X / r);
            var phi = System.Math.Sign(source.Z) * System.Math.Acos(source.Y / System.Math.Sqrt(source.Y * source.Y + source.Z * source.Z));
            return representation switch
            {
                AngleRepresentation.Radians => new Vector<float, float, float>(r, (float)theha, (float)phi),
                AngleRepresentation.Degrees => new Vector<float, float, float>(r, (float)(theha * 180 / System.Math.PI), (float)(phi * 180 / System.Math.PI)),
                AngleRepresentation.Unit => new Vector<float, float, float>(r, (float)(theha / System.Math.PI), (float)(phi / System.Math.PI)),
            };
        }
        
        public static Vector<float, float, float> Add(this Vector<float, float, float> source,
            Vector<float, float, float> value)
        {
            return new Vector<float, float, float>(source.X + value.X, source.Y + value.Y, source.Z + value.Z);
        }

        public static Vector<float, float, float> Multiply(this Vector<float, float, float> source, float factor)
        {
            return new Vector<float, float, float>(source.X * factor, source.Y * factor, source.Z * factor);
        }

        public static Vector<float, float, float> Subtract(this Vector<float, float, float> source,
            Vector<float, float, float> value)
        {
            return new Vector<float, float, float>(source.X - value.X, source.Y - value.Y, source.Z - value.Z);
        }

        public static Vector<float, float, float> Negate(this Vector<float, float, float> source)
        {
            return new Vector<float, float, float>(-source.X, -source.Y, -source.Z);
        }

        public static Vector<U, U, U> Apply<T, U>(this Vector<T, T, T> source, Func<T, U> func)
        {
            return new Vector<U, U, U>(func(source.X), func(source.Y), func(source.Z));
        }

        
        
        
        
        public static Vector<float, float, float> AddAverage(this Vector<ConcurrentQueue<float>, ConcurrentQueue<float>, ConcurrentQueue<float>>? source,
            Vector<float, float, float> value,
            int maxCount)
        {
            if (source.X.Count > maxCount) source.X.TryDequeue(out _);
            if (source.Y.Count > maxCount) source.Y.TryDequeue(out _);
            if (source.Z.Count > maxCount) source.Z.TryDequeue(out _);
            source.X.Enqueue(value.X);
            source.Y.Enqueue(value.Y);
            source.Z.Enqueue(value.Z);
            return new Vector<float, float, float>(source.X.Average(), source.Y.Average(), source.Z.Average());
        }
        public static Vector<float, float, float> Index(this Vector<ConcurrentQueue<float>, ConcurrentQueue<float>, ConcurrentQueue<float>> source,
            int index, bool reverse = false)
        {
            if (reverse) return new Vector<float, float, float>(source.X.Reverse().ToArray()[index], source.Y.Reverse().ToArray()[index], source.Z.Reverse().ToArray()[index]);
            else return new Vector<float, float, float>(source.X.ToArray()[index], source.Y.ToArray()[index], source.Z.ToArray()[index]);
        }
    }
}