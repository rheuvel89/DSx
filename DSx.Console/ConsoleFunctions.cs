using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using SystemConsole = System.Console;

namespace DSx.Console
{
    public class ConsoleFunctions
    {
        
        internal static void PrintState(IVirtualGamepad controller)
        {
            (controller switch
            {
                IXbox360Controller xb => PrintState(xb),
                IDualShock4Controller ds => PrintState(ds),
            }).GetAwaiter().GetResult();
        }
        
        internal static async Task PrintState(IXbox360Controller controller)
        {
            
        }
        
        internal static async Task PrintState(IDualShock4Controller controller)
        {
            var (left, top) = SystemConsole.GetCursorPosition();
            SystemConsole.SetCursorPosition(0, 0);
            SystemConsole.WriteLine($"Pitch {controller.LeftThumbX}".PadRight(SystemConsole.WindowWidth - 1));
            SystemConsole.WriteLine($"Roll  {controller.LeftThumbX}".PadRight(SystemConsole.WindowWidth - 1));
            SystemConsole.SetCursorPosition(left, top);
        }
    }
}