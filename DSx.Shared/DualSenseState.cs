using DualSenseAPI;

namespace DSx.Shared
{
    public struct DualSenseInputState
    {
        public DualSenseInputState(IDualSense ds)
        {
            IoMode = ds.IoMode;
            BatteryIsCharging = ds.InputState.BatteryStatus.IsCharging;
            BatteryIsFullyCharged = ds.InputState.BatteryStatus.IsFullyCharged;
            BatteryLevel = ds.InputState.BatteryStatus.Level;
            IsHeadPhoneConnected = ds.InputState.IsHeadphoneConnected;
            Accelerometer = ds.InputState.Accelerometer;
            Gyro = ds.InputState.Gyro;
            CircleButton = ds.InputState.CircleButton;
            CrossButton = ds.InputState.CrossButton;
            TriangleButton = ds.InputState.TriangleButton;
            SquareButton = ds.InputState.SquareButton;
            CreateButton = ds.InputState.CreateButton;
            MenuButton = ds.InputState.MenuButton;
            LogoButton = ds.InputState.LogoButton;
            MicButton = ds.InputState.MicButton;
            DPadUpButton = ds.InputState.DPadUpButton;
            DPadRightButton = ds.InputState.DPadRightButton;
            DPadDownButton = ds.InputState.DPadDownButton;
            DPadLeftButton = ds.InputState.DPadLeftButton;
            L1Button = ds.InputState.L1Button;
            L2 = ds.InputState.L2;
            L2Button = ds.InputState.L2Button;
            LeftStick = ds.InputState.LeftAnalogStick;
            LeftStickButton = ds.InputState.L3Button;
            R1Button = ds.InputState.R1Button;
            R2 = ds.InputState.R2;
            R2Button = ds.InputState.R2Button;
            RightStick = ds.InputState.RightAnalogStick;
            RightStickButton = ds.InputState.R3Button;
            Touch1Id = ds.InputState.Touchpad1.Id;
            Touch1 = ds.InputState.Touchpad1.IsDown;
            Touch1Position = new Vec2 { X = ds.InputState.Touchpad1.X, Y = ds.InputState.Touchpad1.Y };
            Touch2Id = ds.InputState.Touchpad2.Id;
            Touch2 = ds.InputState.Touchpad2.IsDown;
            Touch2Position = new Vec2 { X = ds.InputState.Touchpad2.X, Y = ds.InputState.Touchpad2.Y };
            TouchpadButton = ds.InputState.TouchpadButton;
        }
        
        public IoMode IoMode { get; set; }
        public bool BatteryIsCharging { get; set; }
        public bool BatteryIsFullyCharged { get; set; }
        public float BatteryLevel { get; set; }
        public bool IsHeadPhoneConnected { get; set; }

        public Vec3 Accelerometer { get; set; }
        public Vec3 Gyro { get; set; }
        
        public bool CircleButton { get; set; }
        public bool CrossButton { get; set; }
        public bool TriangleButton { get; set; }
        public bool SquareButton { get; set; }
        
        public bool CreateButton { get; set; }
        public bool MenuButton { get; set; }
        public bool LogoButton { get; set; }
        public bool MicButton { get; set; }
        
        public bool DPadUpButton { get; set; }
        public bool DPadRightButton { get; set; }
        public bool DPadDownButton { get; set; }
        public bool DPadLeftButton { get; set; }
        
        public bool L1Button { get; set; }
        public float L2 { get; set; }
        public bool L2Button { get; set; }
        public Vec2 LeftStick { get; set; }
        public bool LeftStickButton { get; set; }
        
        public bool R1Button { get; set; }
        public float R2 { get; set; }
        public bool R2Button { get; set; }
        public Vec2 RightStick { get; set; }
        public bool RightStickButton { get; set; }
        
        public byte Touch1Id { get; set; }
        public bool Touch1 { get; set; }
        public Vec2 Touch1Position { get; set; }
        public byte Touch2Id { get; set; }
        public bool Touch2 { get; set; }
        public Vec2 Touch2Position { get; set; }
        public bool TouchpadButton { get; set; }
    }
}