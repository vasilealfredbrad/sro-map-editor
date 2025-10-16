# SRO Map Editor

A 3D map viewer and editor for Silkroad Online game files.

## Inspiration

This project was inspired by the need to visualize and edit Silkroad Online map data for private server development and modding. The editor provides a 3D interface to work with the game's complex map file structure, making it easier to understand and modify the game world.

## Getting Game Files

To use this editor, you need Silkroad Online game files:

1. **Extract from PK2 archives** using tools like PK2Extractor
2. **Place files in correct directories**:
   - `Data/` - Contains .bms, .bmt, .ddj, .bsr files
   - `Map/` - Contains .o2, .m, .t files
3. **File structure should match**:
   ```
   Data/
   ├── prim/mesh/     # 3D models (.bms)
   ├── prim/mtrl/     # Materials (.bmt)
   └── res/           # Resources (.ddj, .bsr)
   Map/
   ├── [Y]/[X].o2     # Object files
   ├── [Y]/[X].m      # Terrain files
   └── [Y]/[X].t      # Texture files
   ```

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
