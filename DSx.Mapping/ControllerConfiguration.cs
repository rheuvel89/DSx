using System.Collections.Generic;

namespace DSx.Mapping
{
    public class ControllerConfiguration
    {
        public byte Id { get; set; } //private set; }
        public ControllerType ConrtollerType { get; set; } //private set; }
        public IList<InputControl>? Modifier { get; set; } //private set; }
        public IList<ControlConfiguration>? Mapping { get; set; } //private set; }
    }
}