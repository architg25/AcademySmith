# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

AcademySmith is a game mod for "Let's School" (Academy Smith), a Unity-based simulation game. This mod provides enhanced tooling and cheats for the game including currency manipulation, time control, character attribute modification, and milestone unlocking.

**Target Framework**: .NET Framework 4.8
**Language**: C#
**Build System**: MSBuild (via .csproj)
**Current Version**: 0.0.9

**⚠️ CRITICAL: Windows-Only Project**
This project targets .NET Framework 4.8, which is **Windows-only** and cannot be built on macOS or Linux. You must use a Windows machine, Windows VM, or remote Windows environment to build this mod. The game "Let's School" is also Windows-only.

## Building the Project

**Prerequisites (Windows Only)**:
- Windows OS (native, VM, or remote)
- .NET Framework 4.8 Developer Pack installed
- "Let's School" game installed (for assembly references)

The project requires game assemblies from "Let's School" installation. The .csproj references assemblies from:
`E:\steam\steamapps\common\Let's School\LetsSchool_Data\Managed\`

**Build Commands (Windows only)**:
```bash
# Using dotnet CLI
dotnet build AcademySmith.csproj -c Debug    # Debug build
dotnet build AcademySmith.csproj -c Release  # Release build

# Using MSBuild directly
msbuild AcademySmith.csproj /p:Configuration=Debug
msbuild AcademySmith.csproj /p:Configuration=Release
```

**Important Notes**:
- The project uses hardcoded paths to game assemblies - update these in the .csproj to match your installation
- On macOS/Linux: This **will not build**. Use a Windows VM or remote Windows machine.
- Output: `Academy Smith.dll` in `bin/Debug/` or `bin/Release/`

## Architecture

### Entry Point
- **Main.cs**: Defines the mod entry point via `BootableModFunctionBase`
  - Registers/unregisters the `Modules` game module on enable/disable
  - Uses `[assembly:ModEntry]` attribute to mark script entry point

### Core Module
- **Modules.cs**: The main game module implementing `M_GameModuleBase`
  - Implements multiple game interfaces for save/load functionality:
    - `IArchiveableModule` - Save/load data
    - `IArchiveBeforeLoadProcessModule` / `IArchiveAfterLoadProcessModule`
    - `IArchiveBeforeSaveProcessModule` / `IArchiveAfterSaveProcessModule`
  - Contains a nested `Mono` class (MonoBehaviour) that handles all UI rendering via Unity's IMGUI system
  - Manages UI windows for FPS, currency, time manipulation, character modification, etc.

### Key Dependencies
The mod relies on several game framework namespaces:
- `ModFrameworkNs.Publish` - Mod framework core
- `ProjectSchoolModNs.Publish` - Game-specific mod APIs
- `FrameworkNs.*` - Game framework utilities
- `Lanka.*` / `ProjectSchoolNs.*` - Game logic systems

## Code Patterns

### UI Windows
The `Mono` class uses Unity's IMGUI (`OnGUI`) to render multiple draggable windows:
- Main menu window (toggled with Home/Delete keys)
- FPS display, currency/points, time controls, character attributes, etc.
- Each window has its own `Rect` for position/size and bool flag for visibility
- Window content is drawn via `Draw*Window(int windowID)` methods

### Language Support
Bilingual support (Chinese/English) controlled by `language` field (0=Chinese, 1=English):
- UI strings selected based on `language` value
- Uses ternary operators extensively: `language == 1 ? "English" : "中文"`

### Game Module Interaction
Access to game systems via singleton pattern:
```csharp
Module<SchoolModule>.Instance.AddMoney(amount, MoneyUseType.other);
Module<TimeModule>.Instance.JumpTimeByDay(days);
Module<CharacterModule>.Instance.AttendanceList;
```

## Development Notes

### Path Configuration
Before building, update assembly reference paths in `AcademySmith.csproj` to match your game installation directory. All references currently point to `E:\steam\steamapps\common\Let's School\`.

### Installation
Place compiled DLL in the game's mod folder:
- Windows: `C:\Users\<username>\AppData\Roaming\Academy Smith\Mods`

### Save Data Structure
The mod persists UI configuration via `SaveData` class:
- Language preference
- Font size
- Window dimensions (width/height)
- Marked with `[ArchiveDataIdentity("Mod.TestModule")]`
