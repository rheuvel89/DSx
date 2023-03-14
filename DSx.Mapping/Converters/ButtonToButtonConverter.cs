namespace DSx.Mapping
{
    public class ButtonToButtonConverter : IMappingConveter
    {
        public object Convert(object input, object[] inputArgs, string[] args, out object? feedback)
        {
            feedback = null;

            return input;
        }
    }
}