public class State
{
    public State()
    {
        Sense = 1f;
        Deadzone = 0f;
    }
    public State(float sense, float deadzone)
    {
        Sense = sense;
        Deadzone = deadzone;
    }

    public float Sense { get; set; }
    public float Deadzone { get; set; }
}