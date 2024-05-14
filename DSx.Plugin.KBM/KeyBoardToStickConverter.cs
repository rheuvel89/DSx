using DSx.Shared;
using DualSenseAPI;

namespace DSx.Plugin.KBM;

public class KeyBoardToStickConverter : IMappingConverter
{
    public object Convert(IDictionary<string, object> inputs, IDictionary<string, string> args, out Feedback feedback)
    {
        feedback = new Feedback();
        var negativeXButtonString = args["NegativeXButton"];
        var positiveXButtonString = args["PositiveXButton"];
        var negativeYButtonString = args["NegativeYButton"];
        var positiveYButtonString = args["PositiveYButton"];
        
        var negativeXButton = Enum.Parse<Input.Button>(negativeXButtonString);
        var positiveXButton = Enum.Parse<Input.Button>(positiveXButtonString);
        var negativeYButton = Enum.Parse<Input.Button>(negativeYButtonString);
        var positiveYButton = Enum.Parse<Input.Button>(positiveYButtonString);
        
        var x = Input.IsButtonPressed(positiveXButton) ? 1 : 0;
        x -= Input.IsButtonPressed(negativeXButton) ? 1 : 0;
        var y = Input.IsButtonPressed(negativeYButton) ? 1 : 0;
        y -= Input.IsButtonPressed(positiveYButton) ? 1 : 0;
        
        return new Vec2 { X = x, Y = y };
    }
}