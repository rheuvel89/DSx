using System.Runtime.InteropServices;

namespace DSx.Plugin.KBM;

public static class Input
{
    [DllImport("user32.dll")]
    static extern bool GetCursorPos(out CursorPosition cursorPosition);

    public struct CursorPosition
    {
        public int X;
        public int Y;
    }
    
    public static CursorPosition GetMousePosition()
    {
        CursorPosition pos;
        GetCursorPos(out pos);
        return pos;
    }
    
    [DllImport("user32.dll")]
    public static extern bool GetAsyncKeyState(int button);
    
    public static bool IsButtonPressed(Button button)
    {
        return GetAsyncKeyState((int)button);
    }
    
    // https://learn.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes?redirectedfrom=MSDN
    public enum Button
    {
        MOUSE_LEFT = 0x01,
        MOUSE_RIGHT = 0x02,
        MOUSE_MIDDLE = 0x04,
        
        KEY_SPACE = 0x20,
        KEY_A = 0x41,
        KEY_B = 0x42,
        KEY_C = 0x43,
        KEY_D = 0x44,
        KEY_E = 0x45,
        KEY_F = 0x46,
        KEY_G = 0x47,
        KEY_H = 0x48,
        KEY_I = 0x49,
        KEY_J = 0x4A,
        KEY_K = 0x4B,
        KEY_L = 0x4C,
        KEY_M = 0x4D,
        KEY_N = 0x4E,
        KEY_O = 0x4F,
        KEY_P = 0x50,
        KEY_Q = 0x51,
        KEY_R = 0x52,
        KEY_S = 0x53,
        KEY_T = 0x54,
        KEY_U = 0x55,
        KEY_V = 0x56,
        KEY_W = 0x57,
        KEY_X = 0x58,
        KEY_Y = 0x59,
        KEY_Z = 0x5A,
        NUMPAD_0 = 0x60,
        NUMPAD_1 = 0x61,
        NUMPAD_2 = 0x62,
        NUMPAD_3 = 0x63,
        NUMPAD_4 = 0x64,
        NUMPAD_5 = 0x65,
        NUMPAD_6 = 0x66,
        NUMPAD_7 = 0x67,
        NUMPAD_8 = 0x68,
        NUMPAD_9 = 0x69,

    }
}