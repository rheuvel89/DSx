namespace DSx.Mapping
{
    public class ButtonToButtonConverter : IMappingConverter
    {
        public object Convert(object[] input, string[] args, out object? feedback)
        {
            feedback = null;

            return input[0];
        }
    }
}