using System;
using DSx.Math;
using DualSenseAPI.State;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.DualShock4;
using Nefarius.ViGEm.Client.Targets.Xbox360;

namespace DSx.Mapping
{
    public static class MappingFunctions
    {
        public static void CopyState(DualSenseInputState input, IVirtualGamepad output, bool mapShoulders = true,
            bool mapSticks = true)
        {
            switch (output)
            {
                case IXbox360Controller xOutput: CopyState(input, xOutput); break;
                case IDualShock4Controller dOutput: CopyState(input, dOutput, mapShoulders, mapSticks); break;
                default: throw new Exception($"Controller of type {output.GetType()} not recognized");
            }
        }
        
        public static void CopyState(DualSenseInputState input, IXbox360Controller output)
        {
            output.LeftThumbX = (short)(input.LeftAnalogStick.X * short.MaxValue);
            output.LeftThumbY = (short)(input.LeftAnalogStick.Y * short.MaxValue);
            output.LeftTrigger = (byte)(input.L2 * byte.MaxValue); 
            output.SetButtonState(Xbox360Button.LeftShoulder, input.L1Button);
            output.SetButtonState(Xbox360Button.LeftThumb, input.L3Button);
            
            output.RightThumbX = (short)(input.RightAnalogStick.X * short.MaxValue);
            output.RightThumbY = (short)(input.RightAnalogStick.Y * short.MaxValue);
            output.RightTrigger = (byte)(input.R2 * byte.MaxValue);
            output.SetButtonState(Xbox360Button.RightShoulder, input.R1Button);
            output.SetButtonState(Xbox360Button.RightThumb, input.R3Button);
            
            output.SetButtonState(Xbox360Button.Up, input.DPadUpButton);
            output.SetButtonState(Xbox360Button.Right, input.DPadRightButton);
            output.SetButtonState(Xbox360Button.Down, input.DPadDownButton);
            output.SetButtonState(Xbox360Button.Left, input.DPadLeftButton);
            
            output.SetButtonState(Xbox360Button.Y, input.TriangleButton);
            output.SetButtonState(Xbox360Button.B, input.CircleButton);
            output.SetButtonState(Xbox360Button.A, input.CrossButton);
            output.SetButtonState(Xbox360Button.X, input.SquareButton);
            
            output.SetButtonState(Xbox360Button.Guide, input.LogoButton);
            output.SetButtonState(Xbox360Button.Back, input.CreateButton);
            output.SetButtonState(Xbox360Button.Start, input.MenuButton);
        }

        public static void CopyState(DualSenseInputState input, IDualShock4Controller output, bool mapShoulders = true, bool mapSticks = true)
        {
            if (mapSticks) output.LeftThumbX = (byte)(byte.MaxValue / 2 - input.LeftAnalogStick.X * byte.MaxValue / 2);
            if (mapSticks) output.LeftThumbY = (byte)(byte.MaxValue / 2 - input.LeftAnalogStick.Y * byte.MaxValue / 2);
            output.LeftTrigger = (byte)(input.L2 * byte.MaxValue); 
            if (mapShoulders) output.SetButtonState(DualShock4Button.ShoulderLeft, input.L1Button);
            output.SetButtonState(DualShock4Button.ThumbLeft, input.L3Button);
            
            if (mapSticks) output.RightThumbX = (byte)(byte.MaxValue / 2 - input.RightAnalogStick.X * byte.MaxValue / 2);
            if (mapSticks) output.RightThumbY = (byte)(byte.MaxValue / 2 - input.RightAnalogStick.Y * byte.MaxValue / 2);
            output.RightTrigger = (byte)(input.R2 * byte.MaxValue);
            if (mapShoulders) output.SetButtonState(DualShock4Button.ShoulderRight, input.R1Button);
            output.SetButtonState(DualShock4Button.ThumbRight, input.R3Button);

            var dpadDir = (input.DPadUpButton, input.DPadRightButton,
                    input.DPadDownButton, input.DPadLeftButton) switch
                {
                    (false, false, false, false) => DualShock4DPadDirection.None,
                    (true, false, false, false) => DualShock4DPadDirection.North,
                    (false, true, false, false) => DualShock4DPadDirection.East,
                    (false, false, true, false) => DualShock4DPadDirection.South,
                    (false, false, false, true) => DualShock4DPadDirection.West,
                    (true, true, false, false) => DualShock4DPadDirection.Northeast,
                    (true, false, true, false) => DualShock4DPadDirection.None,
                    (true, false, false, true) => DualShock4DPadDirection.Northwest,
                    (false, true, true, false) => DualShock4DPadDirection.Southeast,
                    (false, true, false, true) => DualShock4DPadDirection.None,
                    (false, false, true, true) => DualShock4DPadDirection.Southwest,
                    (true, true, true, false) => DualShock4DPadDirection.East,
                    (false, true, true, true) => DualShock4DPadDirection.South,
                    (true, false, true, true) => DualShock4DPadDirection.West,
                    (true, true, false, true) => DualShock4DPadDirection.North,
                    (true, true, true, true) => DualShock4DPadDirection.Southwest,
                };
            output.SetDPadDirection(dpadDir);
            
            output.SetButtonState(DualShock4Button.Triangle, input.TriangleButton);
            output.SetButtonState(DualShock4Button.Circle, input.CircleButton);
            output.SetButtonState(DualShock4Button.Cross, input.CrossButton);
            output.SetButtonState(DualShock4Button.Square, input.SquareButton);
            
            output.SetButtonState(DualShock4Button.Share, input.CreateButton);
            output.SetButtonState(DualShock4Button.Options, input.MenuButton);
        }
 
        public static void MapGyro(Vector<float, float, float> value, IVirtualGamepad output)
        {
            switch (output)
            {
                case IXbox360Controller xOutput: MapGyro(value, xOutput); break;
                case IDualShock4Controller dOutput: MapGyro(value, dOutput); break;
                default: throw new Exception($"Controller of type {output.GetType()} not recognized");
            }
        }
        
        public static void MapGyro(Vector<float, float, float> value, IXbox360Controller output)
        {
            output.LeftThumbX = (short)(value.X * short.MaxValue);
            output.LeftThumbY = (short)(value.Y * short.MaxValue);      
        }
        
        public static void MapGyro(Vector<float, float, float> value, IDualShock4Controller output)
        {
            output.LeftThumbX = (byte)(byte.MaxValue / 2 - value.X * byte.MaxValue / 2);
            output.LeftThumbY = (byte)(byte.MaxValue / 2 - value.Y * byte.MaxValue / 2);
        }
        
    }
}