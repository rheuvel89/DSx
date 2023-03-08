using System;

namespace DSx.Math
{
    public class Vector<Tx, Ty, Tz>
    {
        public static Vector<Tx, Ty, Tz> Zero => new Vector<Tx, Ty, Tz>(default(Tx), default(Ty), default(Tz));

        [Obsolete("Serialization only")]
        private Vector() { }
        public Vector(Tx x, Ty y, Tz z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public Tx X { get; set; }
        public Ty Y { get; set; }
        public Tz Z { get; set; }

        public override string ToString()
        {
            return $"[{$"{X:n2}",7}, {$"{Y:n2}",7}, {$"{Z:n2}",7}]";
        }
    }
    public class Vector<Tx, Ty>
    {
        public static Vector<Tx, Ty> Zero => new Vector<Tx, Ty>(default(Tx), default(Ty));
        
        [Obsolete("Serialization only")]
        private Vector() { }
        public Vector(Tx x, Ty y)
        {
            X = x;
            Y = y;
        }
        public Tx X { get; set; }
        public Ty Y { get; set; }

        public override string ToString()
        {
            return $"[{$"{X:n2}",7}, {$"{Y:n2}",7}]";
        }
    }
}