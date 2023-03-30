using DualSenseAPI.State;
using DualSenseInputState = DSx.Shared.DualSenseInputState;

namespace DSx.Input
{
    public delegate void InputReceivedHandler(DualSenseInputState state);
    public delegate void ButtonChangedHandler(DualSenseInputStateButtonDelta delta);
}