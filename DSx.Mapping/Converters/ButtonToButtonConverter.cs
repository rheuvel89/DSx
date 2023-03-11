namespace DSx.Mapping
{
    public class ButtonToButtonConverter : IMappingConveter
    {
        public object Convert(object input, params string[] args)
        {
            return input;
        }
    }
    
    public class InverseButtonToButtonConverter : IMappingConveter
    {
        public object Convert(object input, params string[] args)
        {
            return !(bool)input;
        }
    }

    public interface IMappingConveter
    {
        public object Convert(object input, params string[] args);
    }
}