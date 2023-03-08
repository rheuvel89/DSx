using DualSenseAPI;
using DualSenseAPI.State;

namespace DSx.Input
{
    public delegate void InputReceivedHandler(DualSense sender, DualSenseInputState state);
    public delegate void ButtonChangedHandler(DualSense sender, DualSenseInputStateButtonDelta delta);
}