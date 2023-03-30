using System.Collections.Generic;

namespace DSx.Mapping
{
    public abstract class ControlConfiguration
    {
        public IDictionary<string, InputControl> Inputs { get; set; } //private set; }
        public string Converter { get; set; } //private set; }
        public IDictionary<string, string>? Arguments { get; set; } //private set; }
        public bool? Global { get; set; }
    }
}