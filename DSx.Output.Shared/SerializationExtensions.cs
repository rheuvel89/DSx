using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;

namespace DSx.Output.Shared;

public static class SerializationExtensions
{
    public static void Serialize(this BinaryWriter writer, IVirtualGamepad source)
    {
        switch (source)
        {
            case IXbox360Controller xbox360Controller:
                writer.Write((ushort)0);
                writer.Serialize(xbox360Controller);
                break;
            case IDualShock4Controller dualShock4Controller:
                writer.Write((ushort)1);
                throw new NotImplementedException();
                writer.Serialize(dualShock4Controller);
                break;
        }
    }

    public static void Serialize(this BinaryWriter writer, IXbox360Controller source)
    {
        writer.Write(source.ButtonState);
        writer.Write(source.LeftTrigger);
        writer.Write(source.RightTrigger);
        writer.Write(source.LeftThumbX);
        writer.Write(source.LeftThumbY);
        writer.Write(source.RightThumbX);
        writer.Write(source.RightThumbY);
    }

    public static IVirtualGamepad DeserializeVirtualGamepad(this BinaryReader reader)
    {
        var type = reader.ReadUInt16();
        return type switch
        {
            0 => reader.DeserializeXbox360Controller(),
            1 => throw new NotImplementedException(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public static IXbox360Controller DeserializeXbox360Controller(this BinaryReader reader)
    {
        var buttonState = reader.ReadUInt16();
        var leftTrigger = reader.ReadByte();
        var rightTrigger = reader.ReadByte();
        var leftThumbX = reader.ReadInt16();
        var leftThumbY = reader.ReadInt16();
        var rightThumbX = reader.ReadInt16();
        var rightThumbY = reader.ReadInt16();

        return new SerializableXbox360Controller()
        {
            ButtonState = buttonState,
            LeftTrigger = leftTrigger,
            RightTrigger = rightTrigger,
            LeftThumbX = leftThumbX,
            LeftThumbY = leftThumbY,
            RightThumbX = rightThumbX,
            RightThumbY = rightThumbY
        };
    }
}