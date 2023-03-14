namespace DSx.Mapping
{
    public class InverseButtonToButtonConverter : IMappingConveter
    {
        public object Convert(object input, object[] inputArgs, string[] args, out object? feedback)
        {
            feedback = null;

            return !(bool)input;
        }
    }
}