# Project YOMI (Working Title)

## Vision

An online multiplayer, synchronous turn-based fighting game for iOS and Web with colorful visuals, uniquely stylized characters, and social features such as player profiles, leaderboards, and character customization.

## Tools

This project is being built on the Unity Engine, with Unity Netcode for GameObjects for the networking programming, and Unity Gaming Services handling the network services. Blender and Adobe Illustrator will be used for assets, with placeholders pulled from Mixamo until production assets are finalized.

- [Unity v2021.3.20f1](https://unity.com/)
- [Unity Netcode for GameObjects](https://docs-multiplayer.unity3d.com/netcode/current/about/index.html)
- [Unity Gaming Services](https://unity.com/solutions/gaming-services)
- [Blender v3.4.1](https://www.blender.org/download/)
- [Adobe Illustrator](https://www.adobe.com/creativecloud/products/illustrator.html)
- [Mixamo](https://www.mixamo.com/)

## Project

### Management

This project is being tracked through GitHub Projects, [here](https://github.com/users/cboveda/projects/3/views/1).

### Workflow

This repository utilizes trunk-based development to more easily faciliate continuous integration. Even though I am currently the sole developer of this project, rather than committing directly to main, short-lived feature branches and pull requests into main will be used to trigger pre-integration automated build tests and practice what it would be like to work on a larger team.

### Conventions

#### Branch naming syntax

`<work item ID>-<title>`

#### Code standards 

Following [C# Coding Conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)

#### Project Structure

General assets are grouped by asset type, while context specific assets are grouped by context.

```js
Assets
├── Art  // For general art assets
|   ├── Materials
|   ├── Models
|   ├── Music
|   └── Sound
├── External
├── Levels  // Anything related to game design
|   ├── Characters
|   |   ├── [1_Character]  // Assets grouped by context
|   |   |   ├── [Sound]
|   |   |   ├── [Prefab]
|   |   |   ├── [Animation]
|   |   |   └── [Data]
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

#### Quality policy

All merges to the main branch shall pass automated build testing and unit testing, with a minimum code coverage percentage to be defined once the CI and testing automations have been implemented (see [here](https://github.com/users/cboveda/projects/3/views/1?filterQuery=milestone%3A%22CI+and+Testing%22) for related tasks).
