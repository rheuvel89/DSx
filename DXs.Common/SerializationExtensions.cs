using System;
using System.IO;
using System.Reflection;
using DSx.Mapping;
using DSx.Math;
using DualSenseAPI;
using DualSenseAPI.State;

namespace DXs.Common
{
    public static class SerializationExtensions
    {
        public static byte[] Serialize(this DualSenseInputState source, long order)
        {
            using var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);
            writer.Write(order);
            writer.Write(source.Accelerometer.X);
            writer.Write(source.Accelerometer.Y);
            writer.Write(source.Accelerometer.Z);
            writer.Write(source.BatteryStatus.IsCharging);
            writer.Write(source.BatteryStatus.IsFullyCharged);
            writer.Write(source.BatteryStatus.Level);
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
            writer.Write(source.IsHeadphoneConnected);
            writer.Write(source.L1Button);
            writer.Write(source.L2);
            writer.Write(source.L2Button);
            writer.Write(source.L3Button);
            writer.Write(source.LeftAnalogStick.X);
            writer.Write(source.LeftAnalogStick.Y);
            writer.Write(source.LogoButton);
            writer.Write(source.MenuButton);
            writer.Write(source.MicButton);
            writer.Write(source.R1Button);
            writer.Write(source.R2);
            writer.Write(source.R2Button);
            writer.Write(source.R3Button);
            writer.Write(source.RightAnalogStick.X);
            writer.Write(source.RightAnalogStick.Y);
            writer.Write(source.SquareButton);
            writer.Write(source.Touchpad1.Id);
            writer.Write(source.Touchpad1.IsDown);
            writer.Write(source.Touchpad1.X);
            writer.Write(source.Touchpad1.Y);
            writer.Write(source.Touchpad2.Id);
            writer.Write(source.Touchpad2.IsDown);
            writer.Write(source.Touchpad2.X);
            writer.Write(source.Touchpad2.Y);
            writer.Write(source.TouchpadButton);
            writer.Write(source.TriangleButton);
            return stream.ToArray();
        }

        public static T Deserialize<T>(this BinaryReader reader)
        {
            var type = typeof(T);
            var value = (T)(
                type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, Type.EmptyTypes)?.Invoke(Array.Empty<object>()) ??
                type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, Type.EmptyTypes).Invoke(Array.Empty<object>())
            );
            
            switch (value)
            {
                case DualSenseInputState v: reader.Deserialize(v); break;
                case Feedback v: reader.Deserialize(v); break;
            }

            return value;
        }

        public static void Deserialize(this BinaryReader reader, DualSenseInputState value)
        {
            var type = typeof(DualSenseInputState);
            
            var accVec = new Vec3();
            accVec.X = reader.ReadSingle();
            accVec.Y = reader.ReadSingle();
            accVec.Z = reader.ReadSingle();
            type.GetProperty(nameof(value.Accelerometer)).SetValue(value , accVec);

            var batteryStatus = new BatteryStatus();
            batteryStatus.IsCharging = reader.ReadBoolean(); 
            batteryStatus.IsFullyCharged = reader.ReadBoolean(); 
            batteryStatus.Level = reader.ReadSingle(); 
            type.GetProperty(nameof(value.BatteryStatus)).SetValue(value , batteryStatus);

            type.GetProperty(nameof(value.CircleButton)).SetValue(value , reader.ReadBoolean());
            type.GetProperty(nameof(value.CreateButton)).SetValue(value , reader.ReadBoolean());
            type.GetProperty(nameof(value.CrossButton)).SetValue(value , reader.ReadBoolean());
            type.GetProperty(nameof(value.DPadDownButton)).SetValue(value , reader.ReadBoolean());
            type.GetProperty(nameof(value.DPadLeftButton)).SetValue(value , reader.ReadBoolean());
            type.GetProperty(nameof(value.DPadRightButton)).SetValue(value , reader.ReadBoolean());
            type.GetProperty(nameof(value.DPadUpButton)).SetValue(value , reader.ReadBoolean());
            
            var gyrVec = new Vec3();
            gyrVec.X = reader.ReadSingle();
            gyrVec.Y = reader.ReadSingle();
            gyrVec.Z = reader.ReadSingle();
            type.GetProperty(nameof(value.Gyro)).SetValue(value , gyrVec);

            type.GetProperty(nameof(value.IsHeadphoneConnected)).SetValue(value , reader.ReadBoolean());
            type.GetProperty(nameof(value.L1Button)).SetValue(value , reader.ReadBoolean());
            type.GetProperty(nameof(value.L2)).SetValue(value , reader.ReadSingle());
            type.GetProperty(nameof(value.L2Button)).SetValue(value , reader.ReadBoolean());
            type.GetProperty(nameof(value.L3Button)).SetValue(value , reader.ReadBoolean());

            var lStickVec = new Vec2();
            lStickVec.X = reader.ReadSingle();
            lStickVec.Y = reader.ReadSingle();
            type.GetProperty(nameof(value.LeftAnalogStick)).SetValue(value , lStickVec);

            type.GetProperty(nameof(value.LogoButton)).SetValue(value , reader.ReadBoolean());
            type.GetProperty(nameof(value.MenuButton)).SetValue(value , reader.ReadBoolean());
            type.GetProperty(nameof(value.MicButton)).SetValue(value , reader.ReadBoolean());
            type.GetProperty(nameof(value.R1Button)).SetValue(value , reader.ReadBoolean());
            type.GetProperty(nameof(value.R2)).SetValue(value , reader.ReadSingle());
            type.GetProperty(nameof(value.R2Button)).SetValue(value , reader.ReadBoolean());
            type.GetProperty(nameof(value.R3Button)).SetValue(value , reader.ReadBoolean());
            
            var rStickVec = new Vec2();
            rStickVec.X = reader.ReadSingle();
            rStickVec.Y = reader.ReadSingle();
            type.GetProperty(nameof(value.RightAnalogStick)).SetValue(value , rStickVec);
            
            type.GetProperty(nameof(value.SquareButton)).SetValue(value , reader.ReadBoolean());

            var touchpad1 = new Touch();
            touchpad1.Id = reader.ReadByte();
            touchpad1.IsDown = reader.ReadBoolean();
            touchpad1.X = reader.ReadUInt32();
            touchpad1.Y = reader.ReadUInt32();
            type.GetProperty(nameof(value.Touchpad1)).SetValue(value , touchpad1);
            
            var touchpad2 = new Touch();
            touchpad2.Id = reader.ReadByte();
            touchpad2.IsDown = reader.ReadBoolean();
            touchpad2.X = reader.ReadUInt32();
            touchpad2.Y = reader.ReadUInt32();
            type.GetProperty(nameof(value.Touchpad2)).SetValue(value , touchpad2);
            
            type.GetProperty(nameof(value.TouchpadButton)).SetValue(value , reader.ReadBoolean());
            type.GetProperty(nameof(value.TriangleButton)).SetValue(value , reader.ReadBoolean());
        }


        public static byte[] Serialize(this Feedback feedback, long order)
        {
            using var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);
            
            writer.Write(order);
            
            writer.Write(feedback.Rumble.X);
            writer.Write(feedback.Rumble.Y);
            writer.Write(feedback.Color.X);
            writer.Write(feedback.Color.Y);
            writer.Write(feedback.Color.Z);
            writer.Write((int)feedback.MicLed);
            return stream.ToArray();
        }

        public static void Deserialize(this BinaryReader reader, Feedback value)
        {
            var rumble = new Vec2();
            rumble.X = reader.ReadSingle();
            rumble.Y = reader.ReadSingle();
            value.Rumble = rumble;

            var color = new Vec3();
            color.X = reader.ReadSingle();
            color.Y = reader.ReadSingle();
            color.Z = reader.ReadSingle();
            value.Color = color;
                
            var mic = reader.ReadInt32();
            value.MicLed = (MicLed)mic;
        }

    }
}