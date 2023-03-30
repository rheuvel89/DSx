namespace DSx.Shared
{
    public interface IMappingConverter
    {
        public object Convert(IDictionary<string, object> inputs, IDictionary<string, string> args, out Feedback feedback);
    }
}