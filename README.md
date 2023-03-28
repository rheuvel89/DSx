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
#...
```

## Converters
Converters are required to indicate how to map an input (or multiple inputs) to an output. Converters exists to straight map on input to an output and converters exist to map non similar types (e.g. tilt to stick), combine (tilt and stick to stick) or change (inverse button) input values before mapping to the output. All converters except a dictionary of input controls a single output control and a dictionary of arguments. Be careful tot supply input controls with the correct type and arguments in the right format. Both input controls and arguments must be supplied under the appropriate name. Arguments with incorrect names are silently omitted.

| Converter                             | Inputs        | Output   | Arguments                                | Description                                                                        |
| ------------------------------------- | ------------- | -------- | ---------------------------------------- | ---------------------------------------------------------------------------------- |
| ButtonToButtonConverter               | Button: Bool  |          |                                          |                                                                                    |      
| InverseButtonToButtonConverter        | Button: Bool  |          |                                          |                                                                                    |             
| AndButtonToButtonConverter            | ButtonX: Bool* |          |                                          |                                                                                    |         
| OrButtonToButtonConverter             | ButtonX: Bool* |          |                                          |                                                                                    |        
| StickToStickConverter                 | Stick: Vec2   |          |                                          |                                                                                    |    
| ButtonAndStickToStickConverter        | Button: Bool <br> Stick: Vec2 |          |                                          |                                                                                    |             
| TriggerToTriggerConverter             | Trigger: Float |          |                                          |                                                                                    |        
| ButtonAndTriggerToTriggerConverter    | Button: Bool <br> Trigger: Float |          |                                          |                                                                                    |                 
| TiltToStickConverter                  | Tilt: (Vec3, Vec3) |          |                                          |                                                                                    |   
| ButtonAndTiltToStickConverter         |               |          |                                          |                                                                                    |            
| TiltAndStickToStickConverter          |               |          |                                          |                                                                                    |           
| ButtonAndTiltAndStickToStickConverter |               |          |                                          |                                                                                    |                    
| GyroToStickConverter                  |               |          |                                          |                                                                                    |   
| ButtonAndGyroToStickConverter         |               |          |                                          |                                                                                    |            
| GyroAndStickToStickConverter          |               |          |                                          |                                                                                    |           
| ButtonAndGyroAndStickToStickConverter |               |          |                                          |                                                                                    |                    

The following mapping snippet will simply map the triangle button to the corresponding button on the XBox360 controller:
```yaml
#...
    Mapping:
      - !XBox360
        Converter: ButtonToButtonConverter   # <--
        Input:
          Button: TriangleButton
        Output: YButton
#...
```

## Controls (input and output)
The input property contains a dictionary (list of names and the corresponding input controls) to indicate which input control should be used for a specific funtion in the converter. The output accepts a single output control of the type of controller that is assigned via the ControllerType property and tag. Be sure to match the types of input and output controls to the types required by the converter.
Input, types and their corresponding (default) mapping to DualShock or XBox360 controllers. Mapping multiple input controls toargy the same output for a given controller can lead to unexpected results. Use the provided converters to achieve combined mappings or map one type to another.

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

One of the most simple converters is the ButtonToButtonConverter, accepting only on input and mapping it directly to the output.
```yaml
#...
    Mapping:
      - !XBox360
        Converter: ButtonToButtonConverter  
        Input:
          Button: TriangleButton            # <-- there is only one input required, the button to map from
        Output: YButton
#...
```

More complex converters often use multiple input controls to conrole different aspects of the converter.
```yaml
#...
- !DualShock
        Global: true
        Inputs:
          Tilt: Tilt           # <-- Combined gyro and accelerometer input
          Stick: RightStick    # <-- The stick to combine with
          Zero: TouchButton    # <-- Button to reset the zero position
          Toggle: CreateButton # <-- Button to toggle tilt input on/off (and therefore switch to stick only)
        Output: RightStick
        Converter: TiltAndStickToStickConverter
#...
```

# Arguments
The arguments are a list of names and values and can be used to finetune the behaviour of a converter. Some converters accept no arguments (ButtonToButtonConverter) while the more complex converters accept multiple arguments, each with their own effect on the output of the converter. All arguments have a default value (if omitted) which can be found in the converters table.
```yaml
- !DualShock
        Global: true
        Inputs:
          Tilt: Tilt
          Stick: RightStick
          Zero: TouchButton
          Toggle: CreateButton
        Output: RightStick
        Converter: TiltAndStickToStickConverter
        Arguments:
          AlphaX: 1,0       # <-- Weight of the tilt contribution in the X-axis
          AlphaY: 1,0       # <-- Weight of the tilt contribution in the Y-axis
          BetaX: 1,0        # <-- Weight of the stick contribution in the X-axis
          BetaY: 1,0        # <-- Weight of the stick contribution in the Y-axis
          Sensitivity: 1,7  # <-- Sensitivity of the tilt input (higher is more sensitive)
          Deadzone: 0,1     # <-- Deadzone of the tilt input 
```

## Global
Use the global property to indicate whether a specific mapping should be active only when the provider modifiers have been pressed (**false** or absent) or should always be active (**true**). A good usecase is mapping tilt input to one of stick of a second or other controller without losing the ability to map the other controls use a modifier. See Converters for an example.
