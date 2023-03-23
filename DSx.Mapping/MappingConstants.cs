using System;
using System.Collections.Generic;
using DualSenseAPI;
using DualSenseAPI.State;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.DualShock4;
using Nefarius.ViGEm.Client.Targets.Xbox360;

namespace DSx.Mapping
{
    public static class MappingConstants
    {
        public static readonly IDictionary<MappingConverter, Func<IMappingConverter>> MappingConveters =
            new Dictionary<MappingConverter, Func<IMappingConverter>>()
            {
                [MappingConverter.ButtonToButtonConverter] = () => new ButtonToButtonConverter(),
                [MappingConverter.InverseButtonToButtonConverter] = () => new InverseButtonToButtonConverter(),
                [MappingConverter.StickToStickConverter] = () => new StickToStickConverter(),
                [MappingConverter.TriggerToTriggerConverter] = () => new TriggerToTriggerConverter(),
                [MappingConverter.TiltToStickConverter] = () => new TiltToStickConverter(),
                [MappingConverter.TiltAndStickToStickConverter] = () => new TiltAndStickToStickConverter(),
                [MappingConverter.GyroToStickConverter] = () => new GyroToStickConverter(),
                [MappingConverter.GyroAndStickToStickConverter] = () => new GyroAndStickToStickConverter(),
            };
        
        public static readonly IDictionary<InputControl, Func<DualSenseInputState, object>> InputSelector = new Dictionary<InputControl, Func<DualSenseInputState, object>>
        {
            [InputControl.LeftStick] = i => i.LeftAnalogStick,
            [InputControl.LeftTrigger] = i => i.L2,
            [InputControl.LeftTriggerButton] = i => i.L2Button,
            [InputControl.LeftShoulder] = i => i.L1Button,
            [InputControl.LeftStickButton] = i => i.L3Button,
            [InputControl.RightStick] = i => i.RightAnalogStick,
            [InputControl.RightTrigger] = i => i.R2,
            [InputControl.RightTriggerButton] = i => i.R2Button,
            [InputControl.RightShoulder] = i => i.R1Button,
            [InputControl.RightStickButton] = i => i.R3Button,
            [InputControl.DPadNorth] = i => i.DPadUpButton && !i.DPadRightButton && !i.DPadDownButton && !i.DPadLeftButton,
            [InputControl.DPadNorthEast] = i => i.DPadUpButton && i.DPadRightButton && !i.DPadDownButton && !i.DPadLeftButton,
            [InputControl.DPadEast] = i => !i.DPadUpButton && i.DPadRightButton && !i.DPadDownButton && !i.DPadLeftButton,
            [InputControl.DPadSouthEast] = i => !i.DPadUpButton && i.DPadRightButton && i.DPadDownButton && !i.DPadLeftButton,
            [InputControl.DPadSouth] = i => !i.DPadUpButton && !i.DPadRightButton && i.DPadDownButton && !i.DPadLeftButton,
            [InputControl.DPadSouthWest] = i => !i.DPadUpButton && !i.DPadRightButton && i.DPadDownButton && i.DPadLeftButton,
            [InputControl.DPadWest] = i => !i.DPadUpButton && !i.DPadRightButton && !i.DPadDownButton && i.DPadLeftButton,
            [InputControl.DPadNorthWest] = i => i.DPadUpButton && !i.DPadRightButton && !i.DPadDownButton && i.DPadLeftButton,
            [InputControl.DPadNone] = i => !i.DPadUpButton && !i.DPadRightButton && !i.DPadDownButton && !i.DPadLeftButton,
            [InputControl.TriangleButton] = i => i.TriangleButton,
            [InputControl.CircleButton] = i => i.CircleButton,
            [InputControl.SquareButton] = i => i.SquareButton,
            [InputControl.CrossButton] = i => i.CrossButton,
            [InputControl.LogoButton] = i => i.LogoButton,
            [InputControl.CreateButton] = i => i.CreateButton,
            [InputControl.MenuButton] = i => i.MenuButton,
            [InputControl.MicButton] = i => i.MicButton,
            [InputControl.TouchButton] = i => i.TouchpadButton,
            [InputControl.Touch1] = i => i.Touchpad1,
            [InputControl.Touch1] = i => i.Touchpad2,
            [InputControl.Tilt] = i => (i.Accelerometer, i.Gyro),
        };
        
        public static readonly IDictionary<DualShockControl, Action<IDualShock4Controller, object>> DualShockAsigner = new Dictionary<DualShockControl, Action<IDualShock4Controller, object>>
        {
            [DualShockControl.LeftStick] = (o, v) =>
            {
                o.LeftThumbX = (byte)(-byte.MaxValue / 2 + ((Vec2)v).X * (byte.MaxValue / 2 - 1));
                o.LeftThumbY = (byte)(byte.MaxValue / 2 - ((Vec2)v).Y * (byte.MaxValue / 2 - 1));
            },
            [DualShockControl.LeftTrigger] = (o, v) => o.LeftTrigger = (byte)((float)v * byte.MaxValue),
            [DualShockControl.LeftTriggerButton] = (o, v) => o.SetButtonState(DualShock4Button.TriggerLeft, (bool)v),
            [DualShockControl.LeftShoulder] = (o, v) => o.SetButtonState(DualShock4Button.ShoulderLeft, (bool)v),
            [DualShockControl.LeftStickButton] = (o, v) => o.SetButtonState(DualShock4Button.ThumbLeft, (bool)v),
            [DualShockControl.RightStick] = (o, v) => 
            {
                o.RightThumbX = (byte)(byte.MaxValue / 2 - ((Vec2)v).X * byte.MaxValue / 2);
                o.RightThumbY = (byte)(byte.MaxValue / 2 - ((Vec2)v).Y * byte.MaxValue / 2);
            },
            [DualShockControl.RightTrigger] = (o, v) => o.RightTrigger = (byte)((float)v * byte.MaxValue),
            [DualShockControl.RightTriggerButton] = (o, v) => o.SetButtonState(DualShock4Button.TriggerRight, (bool)v),
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