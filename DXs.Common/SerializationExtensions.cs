using System;
using System.IO;
using System.Reflection;
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
        where T : DualSenseInputState
        {
            var type = typeof(DualSenseInputState);
            var inputState = (DualSenseInputState)type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, Type.EmptyTypes).Invoke(Array.Empty<object>());

            var accVec = new Vec3();
            accVec.X = reader.ReadSingle();
            accVec.Y = reader.ReadSingle();
            accVec.Z = reader.ReadSingle();
            type.GetProperty(nameof(inputState.Accelerometer)).SetValue(inputState , accVec);

            var batteryStatus = new BatteryStatus();
            batteryStatus.IsCharging = reader.ReadBoolean(); 
            batteryStatus.IsFullyCharged = reader.ReadBoolean(); 
            batteryStatus.Level = reader.ReadSingle(); 
            type.GetProperty(nameof(inputState.BatteryStatus)).SetValue(inputState , batteryStatus);

            type.GetProperty(nameof(inputState.CircleButton)).SetValue(inputState , reader.ReadBoolean());
            type.GetProperty(nameof(inputState.CreateButton)).SetValue(inputState , reader.ReadBoolean());
            type.GetProperty(nameof(inputState.CrossButton)).SetValue(inputState , reader.ReadBoolean());
            type.GetProperty(nameof(inputState.DPadDownButton)).SetValue(inputState , reader.ReadBoolean());
            type.GetProperty(nameof(inputState.DPadLeftButton)).SetValue(inputState , reader.ReadBoolean());
            type.GetProperty(nameof(inputState.DPadRightButton)).SetValue(inputState , reader.ReadBoolean());
            type.GetProperty(nameof(inputState.DPadUpButton)).SetValue(inputState , reader.ReadBoolean());
            
            var gyrVec = new Vec3();
            gyrVec.X = reader.ReadSingle();
            gyrVec.Y = reader.ReadSingle();
            gyrVec.Z = reader.ReadSingle();
            type.GetProperty(nameof(inputState.Gyro)).SetValue(inputState , gyrVec);

            type.GetProperty(nameof(inputState.IsHeadphoneConnected)).SetValue(inputState , reader.ReadBoolean());
            type.GetProperty(nameof(inputState.L1Button)).SetValue(inputState , reader.ReadBoolean());
            type.GetProperty(nameof(inputState.L2)).SetValue(inputState , reader.ReadSingle());
            type.GetProperty(nameof(inputState.L2Button)).SetValue(inputState , reader.ReadBoolean());
            type.GetProperty(nameof(inputState.L3Button)).SetValue(inputState , reader.ReadBoolean());

            var lStickVec = new Vec2();
            lStickVec.X = reader.ReadSingle();
            lStickVec.Y = reader.ReadSingle();
            type.GetProperty(nameof(inputState.LeftAnalogStick)).SetValue(inputState , lStickVec);

            type.GetProperty(nameof(inputState.LogoButton)).SetValue(inputState , reader.ReadBoolean());
            type.GetProperty(nameof(inputState.MenuButton)).SetValue(inputState , reader.ReadBoolean());
            type.GetProperty(nameof(inputState.MicButton)).SetValue(inputState , reader.ReadBoolean());
            type.GetProperty(nameof(inputState.R1Button)).SetValue(inputState , reader.ReadBoolean());
            type.GetProperty(nameof(inputState.R2)).SetValue(inputState , reader.ReadSingle());
            type.GetProperty(nameof(inputState.R2Button)).SetValue(inputState , reader.ReadBoolean());
            type.GetProperty(nameof(inputState.R3Button)).SetValue(inputState , reader.ReadBoolean());
            
            var rStickVec = new Vec2();
            rStickVec.X = reader.ReadSingle();
            rStickVec.Y = reader.ReadSingle();
            type.GetProperty(nameof(inputState.RightAnalogStick)).SetValue(inputState , rStickVec);
            
            type.GetProperty(nameof(inputState.SquareButton)).SetValue(inputState , reader.ReadBoolean());

            var touchpad1 = new Touch();
            touchpad1.Id = reader.ReadByte();
            touchpad1.IsDown = reader.ReadBoolean();
            touchpad1.X = reader.ReadUInt32();
            touchpad1.Y = reader.ReadUInt32();
            type.GetProperty(nameof(inputState.Touchpad1)).SetValue(inputState , touchpad1);
            
            var touchpad2 = new Touch();
            touchpad2.Id = reader.ReadByte();
            touchpad2.IsDown = reader.ReadBoolean();
            touchpad2.X = reader.ReadUInt32();
            touchpad2.Y = reader.ReadUInt32();
            type.GetProperty(nameof(inputState.Touchpad2)).SetValue(inputState , touchpad2);
            
            type.GetProperty(nameof(inputState.TouchpadButton)).SetValue(inputState , reader.ReadBoolean());
            type.GetProperty(nameof(inputState.TriangleButton)).SetValue(inputState , reader.ReadBoolean());
            return (T)inputState;
        }
    }
}