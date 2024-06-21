using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;

namespace DXs.Common;

public static class VirtualGamepadExtensions
{
    public static void Update<T>(this T destination, T source)
    where T: IVirtualGamepad
    {
        switch (destination)
        {
            case IXbox360Controller xbox360Destination when source is IXbox360Controller xbox360Source:
                xbox360Destination.Update(xbox360Source);
                break;
            case IDualShock4Controller dualShock4Destination when source is IDualShock4Controller dualShock4Source:
                dualShock4Destination.Update(dualShock4Source);
                break;
        }
    }
    
    public static void Update(this IXbox360Controller destination, IXbox360Controller source)
    {
        for (var i = 0; i < destination.ButtonCount; i++) destination.SetButtonState(i, (source.ButtonState & (1 << i)) != 0);
        destination.LeftTrigger = source.LeftTrigger;
        destination.RightTrigger = source.RightTrigger;
        destination.LeftThumbX = source.LeftThumbX;
        destination.LeftThumbY = source.LeftThumbY;
        destination.RightThumbX = source.RightThumbX;
        destination.RightThumbY = source.RightThumbY;
    }
    
    public static void Update(this IDualShock4Controller destination, IDualShock4Controller source)
    {
        
    }
}