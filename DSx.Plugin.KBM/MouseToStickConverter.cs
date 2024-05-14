using DSx.Shared;
using DualSenseAPI;

namespace DSx.Plugin.KBM;

public class MouseToStickConverter : IMappingConverter
{
    private int _maxX = 0;
    private int _maxY = 0;
    
    public object Convert(IDictionary<string, object> inputs, IDictionary<string, string> args, out Feedback feedback)
    {
        feedback = new Feedback();
        
        var position = Input.GetMousePosition();
        
        if (position.X > _maxX) _maxX = position.X;
        if (position.Y > _maxY) _maxY = position.Y;

        var xCenter = _maxX / 2;
        var yCenter = _maxY / 2;
        
        var max = Math.Min(_maxX - xCenter, _maxY - yCenter);

        var xPos = position.X - xCenter;
        var yPos = position.Y - yCenter;
        xPos = xPos < -max ? -max : xPos < max ? xPos : max;
        yPos = yPos < -max ? -max : yPos < max ? yPos : max;

        
        var x = (float)xPos / max;
        var y = (float)yPos / max;
        
        return new Vec2 { X = -x, Y = y };
    }
}