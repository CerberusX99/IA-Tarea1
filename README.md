# Guard Unity Script

This repository contains a Unity script for controlling a guard NPC in a game environment. The script includes functionalities such as detection of objects within a vision cone, following a target object, and attacking when detected.

## Code Overview

- `Guard.cs`: Main script file containing the implementation of the guard NPC behavior.
  - `Start()`: Initializes the guard's start position.
  - `Update()`: Handles continuous detection of objects and state transitions.
  - `DetectObjects()`: Detects objects within the guard's vision cone.
  - `OnDrawGizmos()`: Draws the guard's vision cone for debugging purposes.
  - `StateC()`: Manages the guard's different states (Normal, Alert, Attack).
  - `NormalS()`, `AlertS()`, `AttackS()`: Implement behaviors for each state.
  - `SavePosition()`, `LoadPosition()`: Save and load guard's position for state persistence.
  - `MoveToSavedPosition()`, `MoveToStartPosition()`: Coroutine functions for smooth movement.

## Additional Information

### References

- [OpenWebinars - Unity 3D](https://www.youtube.com/watch?v=TLq_wSJVYys&ab_channel=OpenWebinars)
- [Dave/GameDevelopment - Unity Tutorial](https://www.youtube.com/watch?v=UjkSFoLxesw&ab_channel=Dave%2FGameDevelopment)
- [ThegamedevTraum - Unity Tutorial](https://www.youtube.com/watch?v=MtLCmBG_pYo&list=LL&index=1&ab_channel=ThegamedevTraum)
- [GameDev Resources - Point-to-Click Movement with the New Input System](https://gamedev-resources.com/point-to-click-movement-with-the-new-input-system-cinemachine/)

### Additional Credits

- Dios (Add any additional credits or acknowledgments here)
- Codigos de proyectos anteriores (Reference any previous project codes used)

