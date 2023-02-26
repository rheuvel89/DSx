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
    
        public static float Angle(this Vector<float, float, float> source, Vector<float, float, float> value, AngleRepresentation rep = AngleRepresentation.Radians)
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

        public static Vector<float, float, float> Add(this Vector<float, float, float> source,
            Vector<float, float, float> value)
        {
            return new Vector<float, float, float>(source.X + value.X, source.Y + value.Y, source.Z + value.Z);
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
    }
}