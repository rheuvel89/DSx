using System.Collections.Generic;

namespace DSx.Mapping
{
    public class MappingConstants
    {
        public readonly IDictionary<MappingConverter, IMappingConveter> MappingConveters =
            new Dictionary<MappingConverter, IMappingConveter>()
            {
                [MappingConverter.ButtonToButtonConverter] = new ButtonToButtonConverter(),
                [MappingConverter.InverseButtonToButtonConverter] = new InverseButtonToButtonConverter(),
                [MappingConverter.TiltToJoystickConverter] = new TiltToJoystickConverter(),
            };
    }
}