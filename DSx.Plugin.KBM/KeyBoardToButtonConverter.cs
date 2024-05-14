using DSx.Shared;

namespace DSx.Plugin.KBM;

public class KeyBoardToButtonConverter : IMappingConverter
{
    public object Convert(IDictionary<string, object> inputs, IDictionary<string, string> args, out Feedback feedback)
    {
        feedback = new Feedback();
        var buttonString = args["Button"];
        var button = Enum.Parse<Input.Button>(buttonString);
        return Input.IsButtonPressed(button);
    }
}