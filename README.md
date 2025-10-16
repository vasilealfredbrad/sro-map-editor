# SRO Map Editor

A 3D map viewer and editor for Silkroad Online game files.

## Attribution

This project is based on the original SRO Map Editor source code from [elitepvpers](https://www.elitepvpers.com/forum/silkroad-online/pserver-guides-releases/4245552-re-release-sro-map-editor.html) by sro_cyklon. I have enhanced and improved the original codebase with additional features and bug fixes.

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

## Building the Solution

### Prerequisites
- **Visual Studio 2019 or later** (or Visual Studio Code with C# extension)
- **.NET Framework 4.0 or later** (included with Windows 10/11)
- **OpenTK** (included in project references)

### Build Steps
1. **Clone the repository**:
   ```bash
   git clone https://github.com/vasilealfredbrad/sro-map-editor.git
   cd sro-map-editor
   ```

2. **Open in Visual Studio**:
   - Open `SroMapEditor.csproj` in Visual Studio
   - Or use command line: `msbuild SroMapEditor.csproj`

3. **Build the solution**:
   - Press `Ctrl+Shift+B` in Visual Studio
   - Or run: `msbuild SroMapEditor.csproj /p:Configuration=Release`

4. **Run the application**:
   - The executable will be in `bin/Release/SroMapEditor.exe`
   - Make sure you have the required game files in `Data/` and `Map/` directories

## Features

- **3D Map Viewing**: Load and view Silkroad Online map files (.o2, .m, .t)
- **Object Management**: Add, edit, delete, and clone map objects
- **Navigation**: Mouse and keyboard controls for 3D navigation
- **NVM Support**: Load and save navigation mesh files (.nvm)
- **Multi-format Support**: Handles .bms, .bmt, .ddj, .bsr files
- **Enhanced Controls**: Improved mouse controls and camera movement
- **Bug Fixes**: Fixed common issues from the original release

## Improvements Made

### Enhanced Features
- **Fixed Mouse Controls**: Resolved the "mouse spinning like crazy" issue reported by users
- **Improved Camera Movement**: Better panning and rotation controls
- **Updated Dependencies**: Compatible with .NET Framework 4.0+ for better compatibility
- **Code Cleanup**: Removed unused code and improved structure
- **Better Error Handling**: More robust file loading and error management

### Security & Code Quality
- **No Security Issues**: Code is safe for public use
- **Clean Codebase**: Removed any potentially problematic code
- **Proper Attribution**: Credits original author and source
- **Open Source**: Full source code available for review and contribution

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
