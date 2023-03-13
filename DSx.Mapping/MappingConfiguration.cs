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
        LeftStick,
        LeftTrigger,
        LeftShoulder,
        LeftStickButton,

        RightStick,
        RightTrigger,
        RightShoulder,
        RightStickButton,
        
        DPadNorth,
        DPadNorthEast,
        DPadEast,
        DPadSouthEast,
        DPadSouth,
        DPadSouthWest,
        DPadWest,
        DPadNorthWest,
        DPadNone,
        
        YButton,
        BButton,
        XButton,
        AButton,

        BackButton,
        GuideButton,
        StartButton,
    }

    public enum DualShockControl
    {
        LeftStick,
        LeftTrigger,
        LeftShoulder,
        LeftStickButton,

        RightStick,
        RightTrigger,
        RightShoulder,
        RightStickButton,
        
        DPadNorth,
        DPadNorthEast,
        DPadEast,
        DPadSouthEast,
        DPadSouth,
        DPadSouthWest,
        DPadWest,
        DPadNorthWest,
        DPadNone,
        
        TriangleButton,
        CircleButton,
        SquareButton,
        CrossButton,

        ShareButton,
        OptionButton,
    }
    
    public enum InputControl
    {
        LeftStick,
        LeftTrigger,
        LeftShoulder,
        LeftStickButton,

        RightStick,
        RightTrigger,
        RightShoulder,
        RightStickButton,
        
        DPadNorth,
        DPadNorthEast,
        DPadEast,
        DPadSouthEast,
        DPadSouth,
        DPadSouthWest,
        DPadWest,
        DPadNorthWest,
        DPadNone,
        
        TriangleButton,
        CircleButton,
        SquareButton,
        CrossButton,

        LogoButton,
        CreateButton,
        MenuButton,
        MicButton,
        
        Touch1,
        Touch2,
        TouchButton,
        
        Tilt
    }
    
    public enum MappingConverter
    {
        ButtonToButtonConverter,
        InverseButtonToButtonConverter,
        TiltToJoystickConverter,
    }
}