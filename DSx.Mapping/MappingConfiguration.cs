using System.Collections.Generic;

namespace DSx.Mapping
{
    public class MappingConfiguration
    {
        public List<ControllerConfiguration> Controllers { get; set; } //private set; }
    }

    public class ControllerConfiguration
    {
        public byte Id { get; set; } //private set; }
        public ControllerType ConrtollerType { get; set; } //private set; }
        public IList<InputControl>? Modifier { get; set; } //private set; }
        public IList<ControlConfiguration>? Mapping { get; set; } //private set; }
    }

    public abstract class ControlConfiguration
    {
        public InputControl Input { get; set; } //private set; }
        public MappingConverter Converter { get; set; } //private set; }
        public IList<string>? ConverterArguments { get; set; } //private set; }
    }

    public class Xbox360ControlConfiguration : ControlConfiguration
    {
        public XBox360Control Output { get; set; } //private set; }
    }

    public class DualShockControlConfiguration : ControlConfiguration
    {
        public DualShockControl Output { get; set; } //private set; }
    }

    public enum ControllerType
    {
        XBox360,
        DualShock
    }

    public enum XBox360Control
    {
        XButton,
        YButton,
        AButton,
        BButton,
    }

    public enum DualShockControl
    {
        TriangleButton,
        CircleButton,
        SquareButton,
        CrossButton
    }
    
    public enum InputControl
    {
        TriangleButton,
        CircleButton,
        SquareButton,
        CrossButton,
        LeftShoulder,
        RightShoulder,

    }
    
    public enum MappingConverter
    {
        ButtonToButtonConverter,
        InverseButtonToButtonConverter,
        TiltToJoystickConverter,
    }
}