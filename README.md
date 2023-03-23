# DSx: DualSense controller (re)mapping

!Warning! This is a work in progress. Not yet intended for actual release.

Default and example mapping can be found in the application folder ('[config.yaml](https://github.com/rheuvel89/DSx/blob/main/DSx.Client/config.yaml)') and on [GitHub](https://github.com/rheuvel89/DSx/blob/main/DSx.Client/config.yaml). An explanation of the different components of the mapping file can be found below.

# Id
Accepts incrementing *Byte* (0-255) values starting from 0.
```yaml
# Start of the file
Controllers:
  - Id: 0      # <--
    ConrtollerType: DualShock
#...
```

# Controller types
Accepts one of two types: **DualShock** or **XBox360**. Controller types should also be added as tag for each mapping (e.g. !DualShock).
```yaml
#...
    ConrtollerType: DualShock   # <--
    Modifier:
    Mapping:
      - !DualShock              # <-- (tag)
        Inputs:
        - LeftStick       
        Output: LeftStick
#...
```

# Modifiers
Accepts a list of input controls of type *Bool* e.g.:
```yaml
#...
- Id: 3
  ConrtollerType: XBox360
  Modifier:           # <--
    - LeftShoulder    # <--
    - RightShoulder   # <--
  Mapping:
    - !XBox360
#...
```

## Converters
Converters are required to indicate how to map an input to an output. Converters exists to straight map on input to an output and converters exist to map non similar types (e.g. tilt to stick), combine (tilt and stick to stick) or change (inverse button) input values before mapping to the output. All converters except a list of input controls a single output control and a dictionary of input arguments. Be careful tot supply input arguments in the right format and under the appropriate name. Arguments with incorrect names are silently omitted.

| Converter                      | Type in      | Type out | Input arguments                           | Additional arguments                     | Description                                                                        |
| ------------------------------ | ------------ | -------- | ----------------------------------------- | ---------------------------------------- | ---------------------------------------------------------------------------------- |
| ButtonToButtonConverter        | Bool         | Bool     | None                                      | None                                     | This converter does nothing                                                        |
| InverseButtonToButtonConverter | Bool         | Bool     | None                                      | None                                     | Maps off state to on and vice versa                                                |
| TiltToStickConverter           | (Vec3, Vec3) | Vec2     | (re)Zero (Bool input) Toggle (Bool input) | Sensitivity (decimal) Deadzone (decimal) | Maps gyroscope and accelerometer input for usage with sticks and other Vec2 output |

The following mapping snippet will change nothing:
```yaml
#...
    Mapping:
      - !XBox360
        Input: TriangleButton
        Output: YButton
        Converter: ButtonToButtonConverter   # <-- This converter does nothing
#...
```

```yaml
#...

  - Id: 1
    ConrtollerType: XBox360
    Modifier:
      - LeftShoulder
    Mapping:
      - !XBox360
        Global: true                      # <-- Map globally to have this always active
        Input: Tilt                       # <-- Input type (Vec3, Vec3)
        Output: LeftStick                 # <-- Output type (Vec2)
        Converter: TiltToStickConverter   # <-- Convert (Vec3, Vec3) to Vec2
        InputArguments:
          - TouchButton                   # <-- Input (button) for (re)zeroing the tilt
          - MicButton                     # <-- Input (button) for toggling tilt input on/off
        ConverterArguments:
          - 1,7                           # <-- Sensitivity (default 1,0) in decimal (1,0 or 1.0 depending on system settings)
          - 0,1                           # <-- Deadzone (default 0,0) in decimal (0,0 or 0.0 depending on system settings)
#...
```



## Controls
Input, types and their corresponding (default) mapping to DualShock or XBox360 controllers. Mapping multiple input controls to the same output for a given controller can lead to unexpected results. Use the provided converters to achieve combined mappings.

| Input              | Type         | DualShock          | XBox360          |
|--------------------|--------------|--------------------|------------------|
| LeftStick          | Bool         | LeftStick          | LeftStick        |    
| LeftTrigger        | Float        | LeftTrigger        | LeftTrigger      |    
| LeftTriggerButton  | Bool         | LeftTriggerButton  |                  |    
| LeftShoulder       | Bool         | LeftShoulder       | LeftShoulder     |
| LeftStickButton    | Bool         | LeftStickButton    | LeftStickButton  |
| RightStick         | Vec2         | RightStick         | RightStick       |
| RightTrigger       | Float        | RightTrigger       | RightTrigger     |
| RightTriggerButton | Bool         | RightTriggerButton |                  |
| RightShoulder      | Bool         | RightShoulder      | RightShoulder    |
| RightStickButton   | Bool         | RightStickButton   | RightStickButton |
| DPadNorth          | Bool         | DPadNorth          | DPadNorth        |
| DPadNorthEast      | Bool         | DPadNorthEast      | DPadNorthEast    |
| DPadEast           | Bool         | DPadEast           | DPadEast         |
| DPadSouthEast      | Bool         | DPadSouthEast      | DPadSouthEast    |
| DPadSouth          | Bool         | DPadSouth          | DPadSouth        |
| DPadSouthWest      | Bool         | DPadSouthWest      | DPadSouthWest    |
| DPadWest           | Bool         | DPadWest           | DPadWest         |
| DPadNorthWest      | Bool         | DPadNorthWest      | DPadNorthWest    |
| DPadNone           | Bool         | DPadNone           | DPadNone         |
| TriangleButton     | Bool         | TriangleButton     | YButton          |
| CircleButton       | Bool         | CircleButton       | BButton          |
| SquareButton       | Bool         | SquareButton       | XButton          |
| CrossButton        | Bool         | CrossButton        | AButton          |
| LogoButton         | Bool         |                    | GuideButton      | 
| CreateButton       | Bool         | ShareButton        | BackButton       |
| MenuButton         | Bool         | OptionButton       | StartButton      |
| MicButton          | Bool         |                    |                  |
| Touch1             | Vec2         |                    |                  |
| Touch2             | Vec2         |                    |                  |
| TouchButton        | Bool         |                    |                  |
| Tilt               | (Vec3, Vec3) |                    |                  |

The following mapping snippet will change nothing:
```yaml
#...
    Mapping:
      - !XBox360
        Inpust:
        - TriangleButton
        Output: YButton
#...
```

To swap the left and right stick use the following:
```yaml
#...
    Mapping:
      - !DualShock
        Input: RightStick
        Output: LeftStick
      - !DualShock
        Input: LeftStick
        Output: RightStick
#...
```

To unmap a specific input from it's default mapping (in case the input is used for something else) do not specify the output control:
```yaml
#...
    Mapping:
      - !DualShock
        Input: RightStick
                            # <-- Leave out or specify: 'Output: '
    Mapping:                # Next mapping
#...
```

## Global
Use the global property to indicate whether a specific mapping should be active only when the provider modifiers have been pressed (**false** or absent) or should always be active (**true**). A good usecase is mapping tilt input to one of stick of a second or other controller without losing the ability to map the other controls use a modifier. See Converters for an example.
