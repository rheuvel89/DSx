# DSx: DualSense controller (re)mapping

# Id
Accepts incrementing *Byte* (0-255) values starting from 0.

# Controller types
Accepts one of two types: **DualShock** or **XBox360**. Controller types should also be added as tag for each mapping (e.g. !DualShock).

# Modifiers
Accepts a list of input controls of type *Bool* e.g.:
```yaml
Modifier:
- LeftShoulder
- RightShoulder
```

## Controls
Input, types and their corresponding (default) mapping to DualShock or XBox360 controllers

| Input            | Type          | DualShock        | XBox360         |
| ---------------- | ------------- | ---------------- | --------------- |
| LeftStick        | Bool          | LeftStick        |LeftStick        |    
| LeftTrigger      | Float         | LeftTrigger      |LeftTrigger      |    
| LeftShoulder     | Bool          | LeftShoulder     |LeftShoulder     |
| LeftStickButton  | Bool          | LeftStickButton  |LeftStickButton  |
| RightStick       | Vec2          | RightStick       |RightStick       |
| RightTrigger     | Float         | RightTrigger     |RightTrigger     |
| RightShoulder    | Bool          | RightShoulder    |RightShoulder    |
| RightStickButton | Bool          | RightStickButton |RightStickButton |
| DPadNorth        | Bool          | DPadNorth        |DPadNorth        |
| DPadNorthEast    | Bool          | DPadNorthEast    |DPadNorthEast    |
| DPadEast         | Bool          | DPadEast         |DPadEast         |
| DPadSouthEast    | Bool          | DPadSouthEast    |DPadSouthEast    |
| DPadSouth        | Bool          | DPadSouth        |DPadSouth        |
| DPadSouthWest    | Bool          | DPadSouthWest    |DPadSouthWest    |
| DPadWest         | Bool          | DPadWest         |DPadWest         |
| DPadNorthWest    | Bool          | DPadNorthWest    |DPadNorthWest    |
| DPadNone         | Bool          | DPadNone         |DPadNone         |
| TriangleButton   | Bool          | TriangleButton   |YButton          |
| CircleButton     | Bool          | CircleButton     |BButton          |
| SquareButton     | Bool          | SquareButton     |XButton          |
| CrossButton      | Bool          | CrossButton      |AButton          |
| LogoButton       | Bool          |                  |GuideButton      | 
| CreateButton     | Bool          | ShareButton      |BackButton       |
| MenuButton       | Bool          | OptionButton     |StartButton      |
| MicButton        | Bool          |                  |                 |
| Touch1           | Vec2          |                  |                 |
| Touch2           | Vec2          |                  |                 |
| TouchButton      | Bool          |                  |                 |
| Tilt             | (Vec3, Vec3)  |                  |                 |

## Converters
Converters to map non similar input (e.g. tilt to stick) or change input values before mapping to output.

| Converter                      | Type in      | Type out | Input arguments                           | Additional arguments                     | Description                                                                        |
| ------------------------------ | ------------ | -------- | ----------------------------------------- | ---------------------------------------- | ---------------------------------------------------------------------------------- |
| ButtonToButtonConverter        | Bool         | Bool     | None                                      | None                                     | This converter does nothing                                                        |
| InverseButtonToButtonConverter | Bool         | Bool     | None                                      | None                                     | Maps off state to on and vice versa                                                |
| TiltToStickConverter           | (Vec3, Vec3) | Vec2     | (re)Zero (Bool input) Toggle (Bool input) | Sensitivity (decimal) Deadzone (decimal) | Maps gyroscope and accelerometer input for usage with sticks and other Vec2 output |