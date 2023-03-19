namespace DSx.Mapping
{
    public interface IMappingConverter
    {
        public object Convert(object[] inputs, string[] args, out object? feedback);
    }
}