using DualSenseAPI;

namespace DSx.Mapping
{
    public struct Feedback
    {
        public Feedback()
        {
            Rumble = new Vec2();
            MicLed = MicLed.Off;
            Color = new Vec3();
        }
        
        public Vec2 Rumble { get; set; }
        public MicLed MicLed { get; set; }
        public Vec3 Color { get; set; }
        
        public static Feedback operator +(Feedback a, Feedback b)
            => new Feedback
            {
                Rumble = new Vec2
                {
                    X = Math.Max(a.Rumble.X, b.Rumble.X),
                    Y = Math.Max(a.Rumble.Y, b.Rumble.Y),
                },
                Color = new Vec3
                {
                    X = (a.Color.X + b.Color.X)/2,
                    Y = (a.Color.Y + b.Color.Y)/2,
                    Z = (a.Color.Z + b.Color.Z)/2,
                },
                MicLed = (MicLed)Math.Max((int)a.MicLed,(int)b.MicLed)
            };
    }
}