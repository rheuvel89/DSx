using System.Collections.Generic;

namespace DSx.Mapping
{
    public abstract class ControlConfiguration
    {
        public InputControl Input { get; set; } //private set; }
        public MappingConverter Converter { get; set; } //private set; }
        public IList<InputControl>? InputArguments { get; set; } //private set; }
        public IList<string>? ConverterArguments { get; set; } //private set; }
        public bool? Global { get; set; }
    }
}