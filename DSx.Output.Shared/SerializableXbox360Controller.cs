using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.Xbox360;

namespace DSx.Output;

public class SerializableXbox360Controller : IXbox360Controller
{
    
    public void Connect() { }

    public void Disconnect() { }

    public void SetButtonState(int index, bool pressed) => ButtonState = (ushort)(pressed ? ButtonState | (1 << index) : ButtonState & ~(1 << index));

    public void SetAxisValue(int index, short value)
    {
        switch (index)
        {
            case 0: LeftThumbX = value; break;
            case 1: LeftThumbY = value; break;
            case 2: RightThumbX = value; break;
            case 3: RightThumbY = value; break;
        }
    }

    public void SetSliderValue(int index, byte value) 
    {
        switch (index)
        {
            case 0: LeftTrigger = value; break;
            case 1: RightTrigger = value; break;
        }
    }

    public void ResetReport()
    {
        ButtonState = 0;
        LeftThumbX = 0;
        LeftThumbY = 0;
        RightThumbX = 0;
        RightThumbY = 0;
        LeftTrigger = 0;
        RightTrigger = 0;
    }

    public void SubmitReport()
    {
        AutoSubmitReport = true;
    }

    public int ButtonCount { get; }
    public int AxisCount { get; }
    public int SliderCount { get; }
    public bool AutoSubmitReport { get; set; }
    
    public void SetButtonState(Xbox360Button button, bool pressed) => SetButtonState(button.Id, pressed);
    
    public void SetAxisValue(Xbox360Axis axis, short value) => SetAxisValue(axis.Id, value);

    public void SetSliderValue(Xbox360Slider slider, byte value) => SetSliderValue(slider.Id, value);

    public void SetButtonsFull(ushort buttons)
    {
        ButtonState = buttons;
    }

    public int UserIndex => 0;

    private byte _leftTrigger = 0;
    public ref byte LeftTrigger => ref _leftTrigger;

    private byte _rightTrigger = 0;
    public ref byte RightTrigger => ref _rightTrigger;

    private short _leftThumbX = 0;
    public ref short LeftThumbX => ref _leftThumbX;

    private short _leftThumbY = 0;
    public ref short LeftThumbY => ref _leftThumbY;

    private short _rightThumbX = 0;
    public ref short RightThumbX => ref _rightThumbX;

    private short _rightThumbY = 0;
    public ref short RightThumbY => ref _rightThumbY;

    public ushort _buttonState = 0;
    public ref ushort ButtonState => ref _buttonState;

    public event Xbox360FeedbackReceivedEventHandler? FeedbackReceived;
    
}