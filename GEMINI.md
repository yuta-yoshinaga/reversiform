# GEMINI.md
This file provides guidance to GEMINI when working with code in this repository.

## High-Level Architecture

This repository contains a C# Windows Forms application that implements the Reversi game algorithm.

*   **`ReversiForm.sln`**: The main solution file for the project.
*   **`ReversiForm/`**: This directory contains the primary source code for the Reversi application.
    *   **`ReversiForm/Model/`**: Expected to contain the core logic and algorithms for the Reversi game.
    *   **`FormMain.cs`**: Defines the main user interface form of the application.
    *   **`SettingForm.cs`**: Defines the settings user interface form.
    *   **`Program.cs`**: The entry point of the application.

## Common Development Tasks

### Build
To build the project, use the .NET CLI:
```bash
dotnet build ReversiForm.sln
```

### Run
To run the application after building, or to build and run in one step:
```bash
dotnet run --project ReversiForm/ReversiForm.csproj
```

### Testing and Linting
No explicit testing framework or linting configuration was found in the repository. Standard C# compilation will provide basic error checking.
