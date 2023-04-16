# DSx: DualSense controller (re)mapping
This project aims to create a versatile and adaptable mapping engine for the Dual Sense controller by emulating Dual Shock and/or XBox360 controllers using the [ViGEm bus driver](https://github.com/ViGEm/ViGEmBus). The ViGEm bus driver should be installed before running this project.

Default and example mapping can be found in the application folder ('[config.yaml](https://github.com/rheuvel89/DSx/blob/main/DSx.Client/config.yaml)') and on [GitHub](https://github.com/rheuvel89/DSx/blob/main/DSx.Client/config.yaml). An explanation of the different components of the mapping file can be found below.

Although practically usable, this project is far from complete. Use it entirely AT YOUR OWN RISK. No guarantees whatsoever are given.

# Command line
The command line is largely self explanitory. Use the 'Client' verb on the machine where the controllers will be emulated. Input will be collected on the same machine unless a '--Port' is specified, in which case it will try to collect the input from a remote instance running with the verb 'Host' and both '--Port' and '--Server' specified. Be sure to open and forward the specified port and port + 1 (e.g. 12345 and 12346) for the UDP protocol. Forwarding to the 'Client' is essential for proper functioning. To enjoy feedback to the controller forward the ports also to the 'Host'. Further command line help is available by providing '--help' to either the 'Client' or 'Host' verb.

# Extensibility
Custom mapping converter plugins can be build by referencing DSx.Shared.dll and DualSenseAPI v1.0.2 or higher. Converters should implement the IMappingConverter interface and provide a parameterless (default) constructor. An example can be found in the [DSx.Plugin](https://github.com/rheuvel89/DSx/tree/main/DSx.Plugin) project. Plugins pose a serious security risk with respect to arbitrary code execution, so only use plugins that you know and trust. Plugins will only be loaded when the '--PluginPath' option is provided. It will then attempt to load all DLLs in the given path and scan them for implementation of IMappingConverter. These implementations can be used with their class names in the mapping configuration.

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

| Converter                             | Inputs                                                                 | Output | Arguments                                                                                               | Description                                                                                                                                             |
| ------------------------------------- | ---------------------------------------------------------------------- | ------ | ------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------- |
| ButtonToButtonConverter               | Button: Bool                                                           | Bool   |                                                                                                         | Use this for straigth button to button mapping.                                                                                                         |      
| InverseButtonToButtonConverter        | Button: Bool                                                           | Bool   |                                                                                                         | Inverts button input True -> False, False -> True                                                                                                       |             
| AndButtonToButtonConverter            | ButtonX: Bool*                                                         | Bool   |                                                                                                         | Accepts a list of Bool inputs of arbitrary length. Performs logical AND on the entire list. I.e. If one button is False it returns False.               |         
| OrButtonToButtonConverter             | ButtonX: Bool*                                                         | Bool   |                                                                                                         | Accepts a list of Bool inputs of arbitrary length. Performs logical OR on the entire list. I.e. If one button is True it returns True.                  | 
| StickToStickConverter                 | Stick: Vec2                                                            | Vec2   |                                                                                                         | Use this for straigth stick to stick mapping.                                                                                                           |    
| ButtonAndStickToStickConverter        | Button: Bool <br> Stick: Vec2                                          | Vec2   |                                                                                                         | Enables the given stick when the button is pressed.                                                                                                     |             
| TriggerToTriggerConverter             | Trigger: Float                                                         | Float  |                                                                                                         | Use this for straigth trigger to trigger mapping.                                                                                                       |        
| ButtonAndTriggerToTriggerConverter    | Button: Bool <br> Trigger: Float                                       | Float  |                                                                                                         | Enables the given trigger when the button is pressed.                                                                                                   |                 
| TiltToStickConverter                  | Accelerometer: Vec3 <br> Gyro: Vec3 <br> Zero: Bool <br> Toggle: Bool  | Vec2   | Sensitivity: Float <br> Deadzone: Float <br> Gamma: Float                                               | Converts accelerometer and gyro input to tilt that can be mapped to a stick. Hold **Zero** to recenter the tilt, **Toggle** turn tilt input on and off. |   
| ButtonAndTiltToStickConverter         | Button: Bool <br> Stick: Vec2                                          | Vec2   | ***TiltToStickConverter\****                                                                            | Enables the tilt to stick mapping when the button is pressed.                                                                                           |            
| TiltAndStickToStickConverter          | Accelerometer: Vec3 <br> Gyro: Vec3 <br> Stick: Vec2                   | Vec2   | ***TiltToStickConverter\**** <br> AlphaX: Float <br> AlphaY: Float <br> BetaX: Float <br> BetaY: Float  | Additively combines tilt and stick input so that it can be mapped to a stick.                                                                           |           
| ButtonAndTiltAndStickToStickConverter | Button: Bool <br> Accelerometer: Vec3 <br> Gyro: Vec3 <br> Stick: Vec2 | Vec2   | ***TiltAndStickToStickConverter\****                                                                    | Enable tilt input when the button is pressed, otherwise behaves as a stick to stick mapping.                                                            |                    
| GyroToStickConverter                  | Gyro: Vec3                                                             | Vec2   | GammaX: Float <br> GammaY: Float <br> EpsilonX: Float <br> EpsilonY                                     | Converts gyro input to a joystick mouse that can be mapped to a stick.                                                                                  |   
| ButtonAndGyroToStickConverter         | Button: Bool <br> Gyro: Vec3                                           | Vec2   | ***GyroToStickConverter\****                                                                            | Enables the gyro to stick mapping when the button is pressed.                                                                                           |            
| GyroAndStickToStickConverter          | Gyro: Vec3 <br> Stick: Vec2                                            | Vec2   | ***GyroToStickConverter\**** <br> AlphaX: Float <br> AlphaY: Float <br> BetaX: Float <br> BetaY: Float  | Additively combines gyro and stick input so that it can be mapped to a stick.                                                                           |           
| ButtonAndGyroAndStickToStickConverter | Button: Bool <br> Gyro: Vec3 <br> Stick: Vec2                          | Vec2   | ***GyroAndStickToStickConverter\****                                                                    | Enable gyro input when the button is pressed, otherwise behaves as a stick to stick mapping.                                                            |                    

**Arguments**

Sensitivity: Linear sensitivity for tilt to stick conversions. Default: 1,0. Range: 0,0 -> $\infty$.

Deadzone: Circular deadzone for tilt to stick conversion. Default 0,0. Range 0,0 -> 1,0.

Alpha/Beta: Coefficients for adding two inputs. I.e. $(Input_1*\alpha + Input_2*\beta)$. Default 1,0. Range: -$\infty$ -> $\infty$.

Gamma: Curvature parameter for tilt and gyro conversions. I.e. $(Input^\gamma)$. Default 1,0. Range: 0,0 -> $\infty$.

Epsilon: Gyro time-correction factor. This factor is dependent on the gyro sensor and polling interval.

<br/>

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

| Input                 | Type   | DualShock          | XBox360          |
| --------------------- | ------ | ------------------ | ---------------- |
| LeftStick             | Bool   | LeftStick          | LeftStick        |    
| LeftTrigger           | Float  | LeftTrigger        | LeftTrigger      |    
| LeftTriggerButton     | Bool   | LeftTriggerButton  |                  |    
| LeftShoulder          | Bool   | LeftShoulder       | LeftShoulder     |
| LeftStickButton       | Bool   | LeftStickButton    | LeftStickButton  |
| RightStick            | Vec2   | RightStick         | RightStick       |
| RightTrigger          | Float  | RightTrigger       | RightTrigger     |
| RightTriggerButton    | Bool   | RightTriggerButton |                  |
| RightShoulder         | Bool   | RightShoulder      | RightShoulder    |
| RightStickButton      | Bool   | RightStickButton   | RightStickButton |
| DPadNorth             | Bool   | DPadNorth          | DPadNorth        |
| DPadNorthEast         | Bool   | DPadNorthEast      | DPadNorthEast    |
| DPadEast              | Bool   | DPadEast           | DPadEast         |
| DPadSouthEast         | Bool   | DPadSouthEast      | DPadSouthEast    |
| DPadSouth             | Bool   | DPadSouth          | DPadSouth        |
| DPadSouthWest         | Bool   | DPadSouthWest      | DPadSouthWest    |
| DPadWest              | Bool   | DPadWest           | DPadWest         |
| DPadNorthWest         | Bool   | DPadNorthWest      | DPadNorthWest    |
| DPadNone              | Bool   | DPadNone           | DPadNone         |
| TriangleButton        | Bool   | TriangleButton     | YButton          |
| CircleButton          | Bool   | CircleButton       | BButton          |
| SquareButton          | Bool   | SquareButton       | XButton          |
| CrossButton           | Bool   | CrossButton        | AButton          |
| LogoButton            | Bool   |                    | GuideButton      | 
| CreateButton          | Bool   | ShareButton        | BackButton       |
| MenuButton            | Bool   | OptionButton       | StartButton      |
| MicButton             | Bool   |                    |                  |
| Touch1Id              | Byte   |                    |                  |
| Touch1                | Bool   |                    |                  |
| Touch1Position        | Vec2   |                    |                  |
| Touch2Id              | Byte   |                    |                  |
| Touch2                | Bool   |                    |                  |
| Touch2Position        | Vec2   |                    |                  |
| TouchButton           | Bool   |                    |                  |
| Accelerometer         | Vec3   |                    |                  |
| Gyro                  | Vec3   |                    |                  |
| IoMode                | IoMode |                    |                  |
| IsBatteryCharging     | Bool   |                    |                  |
| IsBatteryFullyCharged | Bool   |                    |                  |
| BatteryLevel          | Float  |                    |                  |
| IsHeadPhoneConnected  | Bool   |                    |                  |

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
          Accelerometer: Accelerometer  # <-- Accelerometer input
          Gyro: Gyro                    # <-- Gyro input
          Stick: RightStick             # <-- The stick to combine with
          Zero: TouchButton             # <-- Button to reset the zero position
          Toggle: CreateButton          # <-- Button to toggle tilt input on/off (and therefore switch to stick only)
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

# Attribution
This project uses [ViGEm.NET](https://github.com/ViGEm/ViGEm.NET) for emulating virtual Dual Shock and XBox360 controllers.

This project uses [DualSense-Windows](https://github.com/Ohjurot/DualSense-Windows) for collecting Dual Sense input.
