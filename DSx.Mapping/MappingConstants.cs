using System;
using System.Collections.Generic;
using DualSenseAPI;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.DualShock4;
using Nefarius.ViGEm.Client.Targets.Xbox360;

namespace DSx.Mapping
{
    public static class MappingConstants
    {
        public static readonly IDictionary<MappingConverter, IMappingConveter> MappingConveters =
            new Dictionary<MappingConverter, IMappingConveter>()
            {
                [MappingConverter.ButtonToButtonConverter] = new ButtonToButtonConverter(),
                [MappingConverter.InverseButtonToButtonConverter] = new InverseButtonToButtonConverter(),
                [MappingConverter.TiltToStickConverter] = new TiltToStickConverter(),
            };
        
        public static readonly IDictionary<InputControl, Func<DualSense, object>> InputSelector = new Dictionary<InputControl, Func<DualSense, object>>
        {
            [InputControl.LeftStick] = i => i.InputState.LeftAnalogStick,
            [InputControl.LeftTrigger] = i => i.InputState.L2,
            [InputControl.LeftShoulder] = i => i.InputState.L1Button,
            [InputControl.LeftStickButton] = i => i.InputState.L3Button,
            [InputControl.RightStick] = i => i.InputState.RightAnalogStick,
            [InputControl.RightTrigger] = i => i.InputState.R2,
            [InputControl.RightShoulder] = i => i.InputState.R1Button,
            [InputControl.RightStickButton] = i => i.InputState.R3Button,
            [InputControl.DPadNorth] = i => i.InputState.DPadUpButton && !i.InputState.DPadRightButton && !i.InputState.DPadDownButton && !i.InputState.DPadLeftButton,
            [InputControl.DPadNorthEast] = i => i.InputState.DPadUpButton && i.InputState.DPadRightButton && !i.InputState.DPadDownButton && !i.InputState.DPadLeftButton,
            [InputControl.DPadEast] = i => !i.InputState.DPadUpButton && i.InputState.DPadRightButton && !i.InputState.DPadDownButton && !i.InputState.DPadLeftButton,
            [InputControl.DPadSouthEast] = i => !i.InputState.DPadUpButton && i.InputState.DPadRightButton && i.InputState.DPadDownButton && !i.InputState.DPadLeftButton,
            [InputControl.DPadSouth] = i => !i.InputState.DPadUpButton && !i.InputState.DPadRightButton && i.InputState.DPadDownButton && !i.InputState.DPadLeftButton,
            [InputControl.DPadSouthWest] = i => !i.InputState.DPadUpButton && !i.InputState.DPadRightButton && i.InputState.DPadDownButton && i.InputState.DPadLeftButton,
            [InputControl.DPadWest] = i => !i.InputState.DPadUpButton && !i.InputState.DPadRightButton && !i.InputState.DPadDownButton && i.InputState.DPadLeftButton,
            [InputControl.DPadNorthWest] = i => i.InputState.DPadUpButton && !i.InputState.DPadRightButton && !i.InputState.DPadDownButton && i.InputState.DPadLeftButton,
            [InputControl.DPadNone] = i => !i.InputState.DPadUpButton && !i.InputState.DPadRightButton && !i.InputState.DPadDownButton && !i.InputState.DPadLeftButton,
            [InputControl.TriangleButton] = i => i.InputState.TriangleButton,
            [InputControl.CircleButton] = i => i.InputState.CircleButton,
            [InputControl.SquareButton] = i => i.InputState.SquareButton,
            [InputControl.CrossButton] = i => i.InputState.CrossButton,
            [InputControl.LogoButton] = i => i.InputState.LogoButton,
            [InputControl.CreateButton] = i => i.InputState.CreateButton,
            [InputControl.MenuButton] = i => i.InputState.MenuButton,
            [InputControl.MicButton] = i => i.InputState.MicButton,
            [InputControl.TouchButton] = i => i.InputState.TouchpadButton,
            [InputControl.Touch1] = i => i.InputState.Touchpad1,
            [InputControl.Touch1] = i => i.InputState.Touchpad2,
            [InputControl.Tilt] = i => (i.InputState.Accelerometer, i.InputState.Gyro),
        };
        
        public static readonly IDictionary<DualShockControl, Action<IDualShock4Controller, object>> DualShockAsigner = new Dictionary<DualShockControl, Action<IDualShock4Controller, object>>
        {
            [DualShockControl.LeftStick] = (o, v) =>
            {
                o.LeftThumbX = (byte)(-byte.MaxValue / 2 + ((Vec2)v).X * (byte.MaxValue / 2 - 1));
                o.LeftThumbY = (byte)(byte.MaxValue / 2 - ((Vec2)v).Y * (byte.MaxValue / 2 - 1));
            },
            [DualShockControl.LeftTrigger] = (o, v) => o.LeftTrigger = (byte)((float)v * byte.MaxValue),
            [DualShockControl.LeftShoulder] = (o, v) => o.SetButtonState(DualShock4Button.ShoulderLeft, (bool)v),
            [DualShockControl.LeftStickButton] = (o, v) => o.SetButtonState(DualShock4Button.ThumbLeft, (bool)v),
            [DualShockControl.RightStick] = (o, v) => 
            {
                o.RightThumbX = (byte)(byte.MaxValue / 2 - ((Vec2)v).X * byte.MaxValue / 2);
                o.RightThumbY = (byte)(byte.MaxValue / 2 - ((Vec2)v).Y * byte.MaxValue / 2);
            },
            [DualShockControl.RightTrigger] = (o, v) => o.RightTrigger = (byte)((float)v * byte.MaxValue),
            [DualShockControl.RightShoulder] = (o, v) => o.SetButtonState(DualShock4Button.ShoulderRight, (bool)v),
            [DualShockControl.RightStickButton] = (o, v) => o.SetButtonState(DualShock4Button.ThumbRight, (bool)v),
            [DualShockControl.DPadNorth] = (o, v) =>
            {
                if ((bool)v) o.SetDPadDirection(DualShock4DPadDirection.North);
            },
            [DualShockControl.DPadNorthEast] = (o, v) =>
            {
                if ((bool)v) o.SetDPadDirection(DualShock4DPadDirection.Northeast);
            },
            [DualShockControl.DPadEast] = (o, v) =>
            {
                if ((bool)v) o.SetDPadDirection(DualShock4DPadDirection.East);
            },
            [DualShockControl.DPadSouthEast] = (o, v) => 
            {
                if ((bool)v) o.SetDPadDirection(DualShock4DPadDirection.Southeast);
            },
            [DualShockControl.DPadSouth] = (o, v) => 
            {
                if ((bool)v) o.SetDPadDirection(DualShock4DPadDirection.South);
            },
            [DualShockControl.DPadSouthWest] = (o, v) => 
            {
                if ((bool)v) o.SetDPadDirection(DualShock4DPadDirection.Southwest);
            },
            [DualShockControl.DPadWest] = (o, v) => 
            {
                if ((bool)v) o.SetDPadDirection(DualShock4DPadDirection.West);
            },
            [DualShockControl.DPadNorthWest] = (o, v) => 
            {
                if ((bool)v) o.SetDPadDirection(DualShock4DPadDirection.Northwest);
            },
            [DualShockControl.DPadNone] = (o, v) => 
            {
                if ((bool)v) o.SetDPadDirection(DualShock4DPadDirection.None);
            },
            [DualShockControl.TriangleButton] = (o, v) => o.SetButtonState(DualShock4Button.Triangle, (bool)v),
            [DualShockControl.CircleButton] = (o, v) => o.SetButtonState(DualShock4Button.Circle, (bool)v),
            [DualShockControl.SquareButton] = (o, v) => o.SetButtonState(DualShock4Button.Square, (bool)v),
            [DualShockControl.CrossButton] = (o, v) => o.SetButtonState(DualShock4Button.Cross, (bool)v),
            [DualShockControl.ShareButton] = (o, v) => o.SetButtonState(DualShock4Button.Share, (bool)v),
            [DualShockControl.OptionButton] = (o, v) => o.SetButtonState(DualShock4Button.Options, (bool)v),
        };
        
        public static readonly IDictionary<XBox360Control, Action<IXbox360Controller, object>> XBox360Asigner = new Dictionary<XBox360Control, Action<IXbox360Controller, object>>
        {
            [XBox360Control.LeftStick] = (o, v) =>
            {
                o.LeftThumbX = (short)(((Vec2)v).X * short.MaxValue);
                o.LeftThumbY = (short)(((Vec2)v).Y * short.MaxValue);
            },
            [XBox360Control.LeftTrigger] = (o, v) => o.LeftTrigger = (byte)((float)v * byte.MaxValue),
            [XBox360Control.LeftShoulder] = (o, v) => o.SetButtonState(Xbox360Button.LeftShoulder, (bool)v),
            [XBox360Control.LeftStickButton] = (o, v) => o.SetButtonState(Xbox360Button.LeftThumb, (bool)v),
            [XBox360Control.RightStick] = (o, v) => 
            {
                o.RightThumbX = (short)(((Vec2)v).X * short.MaxValue);
                o.RightThumbY = (short)(((Vec2)v).Y * short.MaxValue);
            },
            [XBox360Control.RightTrigger] = (o, v) => o.RightTrigger = (byte)((float)v * byte.MaxValue),
            [XBox360Control.RightShoulder] = (o, v) => o.SetButtonState(Xbox360Button.RightShoulder, (bool)v),
            [XBox360Control.RightStickButton] = (o, v) => o.SetButtonState(Xbox360Button.RightThumb, (bool)v),
            [XBox360Control.DPadNorth] = (o, v) =>
            {
                if (!(bool)v) return;
                o.SetButtonState(Xbox360Button.Up, (bool)v);
                o.SetButtonState(Xbox360Button.Right, !(bool)v);
                o.SetButtonState(Xbox360Button.Down, !(bool)v);
                o.SetButtonState(Xbox360Button.Left, !(bool)v);
            },
            [XBox360Control.DPadNorthEast] = (o, v) =>
            {
                if (!(bool)v) return;
                o.SetButtonState(Xbox360Button.Up, (bool)v);
                o.SetButtonState(Xbox360Button.Right, (bool)v);
                o.SetButtonState(Xbox360Button.Down, !(bool)v);
                o.SetButtonState(Xbox360Button.Left, !(bool)v);
            },
            [XBox360Control.DPadEast] = (o, v) =>
            {
                if (!(bool)v) return;
                o.SetButtonState(Xbox360Button.Up, !(bool)v);
                o.SetButtonState(Xbox360Button.Right, (bool)v);
                o.SetButtonState(Xbox360Button.Down, !(bool)v);
                o.SetButtonState(Xbox360Button.Left, !(bool)v);
            },
            [XBox360Control.DPadSouthEast] = (o, v) =>
            {
                if (!(bool)v) return;
                o.SetButtonState(Xbox360Button.Up, !(bool)v);
                o.SetButtonState(Xbox360Button.Right, (bool)v);
                o.SetButtonState(Xbox360Button.Down, (bool)v);
                o.SetButtonState(Xbox360Button.Left, !(bool)v);
            },
            [XBox360Control.DPadSouth] = (o, v) =>
            {
                if (!(bool)v) return;
                o.SetButtonState(Xbox360Button.Up, !(bool)v);
                o.SetButtonState(Xbox360Button.Right, !(bool)v);
                o.SetButtonState(Xbox360Button.Down, (bool)v);
                o.SetButtonState(Xbox360Button.Left, !(bool)v);
            },
            [XBox360Control.DPadSouthWest] = (o, v) =>
            {
                if (!(bool)v) return;
                o.SetButtonState(Xbox360Button.Up, !(bool)v);
                o.SetButtonState(Xbox360Button.Right, !(bool)v);
                o.SetButtonState(Xbox360Button.Down, (bool)v);
                o.SetButtonState(Xbox360Button.Left, (bool)v);
            },
            [XBox360Control.DPadWest] = (o, v) =>
            {
                if (!(bool)v) return;
                o.SetButtonState(Xbox360Button.Up, !(bool)v);
                o.SetButtonState(Xbox360Button.Right, !(bool)v);
                o.SetButtonState(Xbox360Button.Down, !(bool)v);
                o.SetButtonState(Xbox360Button.Left, (bool)v);
            },
            [XBox360Control.DPadNorthWest] = (o, v) =>
            {
                if (!(bool)v) return;
                o.SetButtonState(Xbox360Button.Up, (bool)v);
                o.SetButtonState(Xbox360Button.Right, !(bool)v);
                o.SetButtonState(Xbox360Button.Down, !(bool)v);
                o.SetButtonState(Xbox360Button.Left, (bool)v);
            },
            [XBox360Control.DPadNone] = (o, v) =>
            {
                if (!(bool)v) return;
                o.SetButtonState(Xbox360Button.Up, !(bool)v);
                o.SetButtonState(Xbox360Button.Right, !(bool)v);
                o.SetButtonState(Xbox360Button.Down, !(bool)v);
                o.SetButtonState(Xbox360Button.Left, !(bool)v);
            },
            [XBox360Control.YButton] = (o, v) => o.SetButtonState(Xbox360Button.Y, (bool)v),
            [XBox360Control.BButton] = (o, v) => o.SetButtonState(Xbox360Button.B, (bool)v),
            [XBox360Control.XButton] = (o, v) => o.SetButtonState(Xbox360Button.X, (bool)v),
            [XBox360Control.AButton] = (o, v) => o.SetButtonState(Xbox360Button.A, (bool)v),
            [XBox360Control.BackButton] = (o, v) => o.SetButtonState(Xbox360Button.Back, (bool)v),
            [XBox360Control.GuideButton] = (o, v) => o.SetButtonState(Xbox360Button.Guide, (bool)v),
            [XBox360Control.StartButton] = (o, v) => o.SetButtonState(Xbox360Button.Start, (bool)v),
        };
    }
}