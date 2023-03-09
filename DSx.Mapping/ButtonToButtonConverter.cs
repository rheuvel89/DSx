namespace DSx.Mapping
{
    public class ButtonToButtonConverter : IMappingConveter<bool, bool>
    {
        public bool Convert(bool input, params object[] args)
        {
            return input;
        }
    }
    public class InverseButtonToButtonConverter : IMappingConveter<bool, bool>
    {
        public bool Convert(bool input, params object[] args)
        {
            return !input;
        }
    }

    public interface IMappingConveter<TIn, TOut> : IMappingConveter
    {
        public TOut Convert(TIn input, params object[] args);
    }
    public interface IMappingConveter
    {
    }
}