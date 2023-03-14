namespace DSx.Mapping
{
    public interface IMappingConveter
    {
        public object Convert(object input, object[] inputArgs, string[] args, out object? feedback);
    }
}