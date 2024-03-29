# Project YOMI (Working Title)

[![Build](https://github.com/cboveda/ProjectYOMI/actions/workflows/main.yml/badge.svg)](https://github.com/cboveda/ProjectYOMI/actions/workflows/main.yml) [![codecov](https://codecov.io/gh/cboveda/ProjectYOMI/branch/main/graph/badge.svg?token=NAMSFMFB5I)](https://codecov.io/gh/cboveda/ProjectYOMI)

## Vision

An online multiplayer, synchronous turn-based fighting game for iOS and Web with colorful visuals, uniquely stylized characters, and social features such as player profiles, leaderboards, and character customization.

## Links

- [Project Management](https://github.com/users/cboveda/projects/3/views/1)
- [Design Documents](https://github.com/cboveda/ProjectYOMI/wiki)
- [Dev Journal](https://dev.to/cboveda/series/22535)

## Tools

This project is being built on the Unity Engine, with Unity Netcode for GameObjects for the networking programming, and Unity Gaming Services handling the network services. Blender and Adobe Illustrator will be used for assets, with placeholders pulled from Mixamo until production assets are finalized.

- [Unity v2021.3.20f1](https://unity.com/)
- [Unity Netcode for GameObjects](https://docs-multiplayer.unity3d.com/netcode/current/about/index.html)
- [Unity Gaming Services](https://unity.com/solutions/gaming-services)
- [Blender v3.4.1](https://www.blender.org/download/)
- [Adobe Illustrator](https://www.adobe.com/creativecloud/products/illustrator.html)
- [Mixamo](https://www.mixamo.com/)

## Project

### Branch naming

`<work item ID>-<title>`

### Code style

Following [C# Coding Conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)

### Structure

General assets are grouped by asset type, while context-specific assets are grouped by context.

```C#
Assets
├── Art  // For general art assets
|   ├── Materials
|   ├── Models
|   ├── Music
|   └── Sound
├── External
├── Levels  // Anything related to game design
|   ├── Characters
|   |   ├── Character1  // Assets grouped by context
|   |   |   ├── Sound
|   |   |   ├── Prefab
|   |   |   ├── Animation
|   |   |   └── Data
|   |   └── ...
|   ├── Moves
|   ├── Prefabs
|   └── Scenes
├── Plugins
├── Resources  // To be used sparingly
├── Scripts  // Scripts grouped by context
|   ├── CharcterSelect
|   ├── Core
|   |   ├── Data
|   |   ├── Installers
|   |   └── Networking
|   ├── Gameplay
|   └── MainMenu
└── Tests
```

### Quality Policy

All merges to the main branch shall pass automated build testing and unit testing, and shall not decrease total code coverage by greater than 5%.
