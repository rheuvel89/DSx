using DSx.Shared;
using DualSenseAPI;

namespace DSx.Input.Shared;

public static class SerializationExtensions
{
    public static void Serialize(this BinaryWriter writer, DualSenseInputState source)
    {
        writer.Write(source.Accelerometer.X);
        writer.Write(source.Accelerometer.Y);
        writer.Write(source.Accelerometer.Z);
        writer.Write(source.BatteryIsCharging);
        writer.Write(source.BatteryIsFullyCharged);
        writer.Write(source.BatteryLevel);
        writer.Write(source.CircleButton);
        writer.Write(source.CreateButton);
        writer.Write(source.CrossButton);
        writer.Write(source.DPadDownButton);
        writer.Write(source.DPadLeftButton);
        writer.Write(source.DPadRightButton);
        writer.Write(source.DPadUpButton);
        writer.Write(source.Gyro.X);
        writer.Write(source.Gyro.Y);
        writer.Write(source.Gyro.Z);
        writer.Write(source.IsHeadPhoneConnected);
        writer.Write(source.L1Button);
        writer.Write(source.L2);
        writer.Write(source.L2Button);
        writer.Write(source.LeftStickButton);
        writer.Write(source.LeftStick.X);
        writer.Write(source.LeftStick.Y);
        writer.Write(source.LogoButton);
        writer.Write(source.MenuButton);
        writer.Write(source.MicButton);
        writer.Write(source.R1Button);
        writer.Write(source.R2);
        writer.Write(source.R2Button);
        writer.Write(source.RightStickButton);
        writer.Write(source.RightStick.X);
        writer.Write(source.RightStick.Y);
        writer.Write(source.SquareButton);
        writer.Write(source.Touch1Id);
        writer.Write(source.Touch1);
        writer.Write(source.Touch1Position.X);
        writer.Write(source.Touch1Position.Y);
        writer.Write(source.Touch2Id);
        writer.Write(source.Touch2);
        writer.Write(source.Touch2Position.X);
        writer.Write(source.Touch2Position.Y);
        writer.Write(source.TouchpadButton);
        writer.Write(source.TriangleButton);
        writer.Write((int)source.IoMode);
    }

    public static void Serialize(this BinaryWriter writer, Feedback feedback)
    {
        writer.Write(feedback.Rumble.X);
        writer.Write(feedback.Rumble.Y);
        writer.Write(feedback.Color.X);
        writer.Write(feedback.Color.Y);
        writer.Write(feedback.Color.Z);
        writer.Write((int)feedback.MicLed);
    }

    public static DualSenseInputState DeserializeInputState(this BinaryReader reader)
    {
        var accelerometerX = reader.ReadSingle();
        var accelerometerY = reader.ReadSingle();
        var accelerometerZ = reader.ReadSingle();
        var batteryIsCharging = reader.ReadBoolean();
        var batteryIsFullyCharged = reader.ReadBoolean();
        var batteryLevel = reader.ReadSingle();
        var circleButton = reader.ReadBoolean();
        var createButton = reader.ReadBoolean();
        var crossButton = reader.ReadBoolean();
        var dPadDownButton = reader.ReadBoolean();
        var dPadLeftButton = reader.ReadBoolean();
        var dPadRightButton = reader.ReadBoolean();
        var dPadUpButton = reader.ReadBoolean();
        var gyroX = reader.ReadSingle();
        var gyroY = reader.ReadSingle();
        var gyroZ = reader.ReadSingle();
        var isHeadPhoneConnected = reader.ReadBoolean();
        var l1Button = reader.ReadBoolean();
        var l2 = reader.ReadSingle();
        var l2Button = reader.ReadBoolean();
        var leftStickButton = reader.ReadBoolean();
        var leftStickX = reader.ReadSingle();
        var leftStickY = reader.ReadSingle();
        var logoButton = reader.ReadBoolean();
        var menuButton = reader.ReadBoolean();
        var micButton = reader.ReadBoolean();
        var r1Button = reader.ReadBoolean();
        var r2 = reader.ReadSingle();
        var r2Button = reader.ReadBoolean();
        var rightStickButton = reader.ReadBoolean();
        var rightStickX = reader.ReadSingle();
        var rightStickY = reader.ReadSingle();
        var squareButton = reader.ReadBoolean();
        var touch1Id = reader.ReadByte();
        var touch1 = reader.ReadBoolean();
        var touch1PositionX = reader.ReadSingle();
        var touch1PositionY = reader.ReadSingle();
        var touch2Id = reader.ReadByte();
        var touch2 = reader.ReadBoolean();
        var touch2PositionX = reader.ReadSingle();
        var touch2PositionY = reader.ReadSingle();
        var touchpadButton = reader.ReadBoolean();
        var triangleButton = reader.ReadBoolean();
        var ioMode = (IoMode)reader.ReadInt32();

        return new DualSenseInputState
        {
            IoMode = ioMode,
            BatteryIsCharging = batteryIsCharging,
            BatteryIsFullyCharged = batteryIsFullyCharged,
            BatteryLevel = batteryLevel,
            Accelerometer = new Vec3
            {
                X = accelerometerX,
                Y = accelerometerY,
                Z = accelerometerZ
            },
            IsHeadPhoneConnected = isHeadPhoneConnected,
            Gyro = new Vec3
            {
                X = gyroX,
                Y = gyroY,
                Z = gyroZ
            },
            CircleButton = circleButton,
            CrossButton = crossButton,
            TriangleButton = triangleButton,
            SquareButton = squareButton,
            CreateButton = createButton,
            MenuButton = menuButton,
            LogoButton = logoButton,
            MicButton = micButton,
            DPadUpButton = dPadUpButton,
            DPadRightButton = dPadRightButton,
            DPadDownButton = dPadDownButton,
            DPadLeftButton = dPadLeftButton,
            L1Button = l1Button,
            L2 = l2,
            L2Button = l2Button,
            LeftStick = new Vec2
            {
                X = leftStickX,
                Y = leftStickY
            },
            LeftStickButton = leftStickButton,
            R1Button = r1Button,
            R2 = r2,
            R2Button = r2Button,
            RightStick = new Vec2
            {
                X = rightStickX,
                Y = rightStickY
            },
            RightStickButton = rightStickButton,
            Touch1Id = touch1Id,
            Touch1 = touch1,
            Touch1Position = new Vec2
            {
                X = touch1PositionX,
                Y = touch1PositionY
            },
            Touch2Id = touch2Id,
            Touch2 = touch2,
            Touch2Position = new Vec2
            {
                X = touch2PositionX,
                Y = touch2PositionY
            },
            TouchpadButton = touchpadButton,
        };
    }

    public static Feedback DeserializeFeedback(this BinaryReader reader)
    {
        var rumbleX = reader.ReadSingle();
        var rumbleY = reader.ReadSingle();

        var colorX = reader.ReadSingle();
        var colorY = reader.ReadSingle();
        var colorZ = reader.ReadSingle();

        var mic = (MicLed)reader.ReadInt32();

        return new Feedback
        {
            Rumble = new Vec2
            {
                X = rumbleX,
                Y = rumbleY
            },
            Color = new Vec3
            {
                X = colorX,
                Y = colorY,
                Z = colorZ
            },
            MicLed = mic
        };
    }
}