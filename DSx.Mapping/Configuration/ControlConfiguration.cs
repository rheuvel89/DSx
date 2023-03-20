using System.Collections.Generic;

namespace DSx.Mapping
{
    public abstract class ControlConfiguration
    {
        public IList<InputControl> Inputs { get; set; } //private set; }
        public MappingConverter Converter { get; set; } //private set; }
        public IList<string>? ConverterArguments { get; set; } //private set; }
        public bool? Global { get; set; }
    }
}