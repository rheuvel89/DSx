using System.Collections.Generic;
using System.Linq;
using DualSenseAPI;

namespace DSx.Mapping
{
    public abstract class ButtonAndConverter<TInner, TReturn> : IMappingConverter
    where TInner : IMappingConverter, new()
    where TReturn : struct
    {
        private TInner _innerConverter = new();
        public object Convert(object[] inputs, IDictionary<string, string> args, out object? feedback)
        {
            feedback = null;
            
            var button = (bool)inputs[0];

            inputs = inputs.Skip(1).ToArray();

            return button
                ? _innerConverter.Convert(inputs, args, out feedback)
                : default(TReturn);
        }
    }
    
    public abstract class ButtonAndConverter<TInnerTrue, TInnerFalse, TReturn> : IMappingConverter
    where TInnerTrue : IMappingConverter, new()
    where TInnerFalse : IMappingConverter, new()
    {
        private TInnerTrue _innerTrueConverter = new();
        private TInnerTrue _innerFalseConverter = new();
        
        public object Convert(object[] inputs, IDictionary<string, string> args, out object? feedback)
        {
            feedback = null;
            
            var button = (bool)inputs[0];

            inputs = inputs.Skip(1).ToArray();

            return button
                ? _innerTrueConverter.Convert(inputs, args, out feedback)
                : _innerFalseConverter.Convert(inputs, args, out feedback);
        }
    }

    public class ButtonAndStickToStickConverter : ButtonAndConverter<StickToStickConverter, Vec2>
    { }
    
    public class ButtonAndTriggerToTriggerConverter : ButtonAndConverter<TriggerToTriggerConverter, float>
    { }
    
    public class ButtonAndTiltToStickConverter : ButtonAndConverter<TiltToStickConverter, Vec2>
    { }
    
    public class ButtonAndGyroToStickConverter : ButtonAndConverter<GyroToStickConverter, Vec2>
    { }
    
    public class ButtonAndGyroAndStickToStickConverter : ButtonAndConverter<GyroAndStickToStickConverter, StickToStickConverter, Vec2>
    { }
    
    public class ButtonAndTiltAndStickToStickConverter : ButtonAndConverter<TiltAndStickToStickConverter, StickToStickConverter, Vec2>
    { }
}