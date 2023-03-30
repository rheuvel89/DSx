using DSx.Mapping;
using DualSenseAPI.State;

namespace DSx.Input
{
    public delegate void InputReceivedHandler(DualSenseState sender, DualSenseInputState state);
    public delegate void ButtonChangedHandler(DualSenseState sender, DualSenseInputStateButtonDelta delta);
}