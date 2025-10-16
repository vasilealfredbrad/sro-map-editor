# SRO Map Editor

A 3D map viewer and editor for Silkroad Online game files.

## Features

- **3D Map Viewing**: Load and view Silkroad Online map files (.o2, .m, .t)
- **Object Management**: Add, edit, delete, and clone map objects
- **Navigation**: Mouse and keyboard controls for 3D navigation
- **NVM Support**: Load and save navigation mesh files (.nvm)
- **Multi-format Support**: Handles .bms, .bmt, .ddj, .bsr files

## Controls

### Mouse
- **Right-click + Drag**: Rotate camera
- **Left-click**: Select objects
- **Mouse Wheel**: Zoom in/out

### Keyboard
- **W/A/S/D**: Move camera
- **Q/E**: Move up/down
- **R**: Reset view
- **+/-**: Zoom in/out

## Requirements

- .NET Framework 4.8
- OpenTK 1.x
- Silkroad Online game files in `Data/` and `Map/` directories

## Usage

1. Place Silkroad Online game files in the `Data/` and `Map/` directories
2. Run `SroMapEditor.exe`
3. Click "Open Map" to load a map file
4. Use mouse and keyboard to navigate the 3D view

## File Structure

```
SroMapEditor/
├── Data/           # Game data files (.bms, .bmt, .ddj, .bsr)
├── Map/            # Map files (.o2, .m, .t)
└── bin/Debug/      # Compiled executable
```

## License

This project is for educational purposes only.
