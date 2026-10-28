# Project Eclipse

Project Eclipse is a Unity 2D project whose goal is a survivor-like / bullet-heaven roguelite for Windows PC. It is currently in the **Project Foundation** phase: the repository provides the application scene flow, initial assembly boundaries, and development structure. Gameplay systems represented by reserved module folders are not implied to be implemented yet.

## Requirements

- Unity Editor **6000.3.27f1** (the repository version in `ProjectSettings/ProjectVersion.txt`)
- Git

The project uses C#, the Universal Render Pipeline (URP), and Unity's 2D Renderer.

## Developer setup

1. Clone the repository and enter it:

   ```text
   git clone <repository-url>
   cd Project-Eclipse
   ```

2. Install Unity Editor `6000.3.27f1` through Unity Hub.
3. In Unity Hub, choose **Add project from disk** and select the repository root (the directory containing `Assets`, `Packages`, and `ProjectSettings`).
4. Allow Unity to import the project and confirm that the Console contains no compile errors.
5. Open `Assets/_Game/Scenes/Boot.unity`. Use **Boot** as the application entry scene when running the full flow.

The enabled scenes in Build Settings are ordered as:

```text
Boot -> MainMenu -> Gameplay
```

`Boot` loads `MainMenu`; the Main Menu Play action loads `Gameplay`.

## Project layout

Project-owned gameplay code and content are organized primarily under
Assets/_Game.

Unity/template-level configuration assets currently remain under
Assets/Settings.

Third-party content belongs under Assets/ThirdParty.

Assets/Scenes currently contains the template SampleScene; it is not part of the enabled application flow. The application scenes are maintained under Assets/_Game/Scenes.

- `Art/` - project art assets.
- `Audio/` - audio assets.
- `Data/` - data assets and configuration.
- `Prefabs/` - reusable Unity prefabs.
- `Scenes/` - the `Boot`, `MainMenu`, and `Gameplay` scenes.
- `Scripts/` - runtime C# code, organized by responsibility.
- `Settings/` - project-owned Unity configuration assets.
- `Tests/EditMode/` and `Tests/PlayMode/` - Edit Mode and Play Mode test assemblies.

Within `Scripts`, `Core/` contains application bootstrap code and `UI/` contains the current menu controller. `Audio`, `Combat`, `Enemies`, `Loot`, `Player`, `Progression`, `Save`, `Weapons`, and `World` are currently reserved module folders; their presence does not mean those systems are implemented.

## Assembly boundaries

The current assembly dependency graph is:

```text
Eclipse.Core
`-- Eclipse.Gameplay
    `-- Eclipse.UI (also references Eclipse.Core directly)

Eclipse.Tests.EditMode --> Eclipse.Core, Eclipse.Gameplay, Eclipse.UI
Eclipse.Tests.PlayMode --> Eclipse.Core, Eclipse.Gameplay, Eclipse.UI
```

- `Eclipse.Core` has no project assembly references.
- `Eclipse.Gameplay` references only `Eclipse.Core`. Its assembly definition is at the `Scripts/` root, so script modules below that root belong to it unless a nearer assembly definition overrides it.
- `Eclipse.UI` references `Eclipse.Core` and `Eclipse.Gameplay`.
- Both test assemblies reference all three production assemblies. Edit Mode tests are Editor-only; both test assemblies use Unity's test assemblies reference and are not auto-referenced.

Keep dependencies flowing in these directions; do not make Core depend on Gameplay or UI.

## Code conventions

Follow the patterns already established in the repository:

- Use PascalCase for namespaces, types, methods, and C# filenames; keep one primary type per same-named file.
- Place code in the folder and assembly matching its responsibility.
- Use the assembly namespace (`Eclipse.Core`, `Eclipse.UI`) for code owned by a specialized assembly; the Gameplay assembly's root namespace is `Eclipse`.
- Keep Unity component APIs small and use the narrowest practical visibility. Existing components are sealed, and scene-name constants are private.
- Keep scene names consistent with their `.unity` filenames and Build Settings entries.

## Development workflow

Work branches are created from `development` and named `task/NNN-description`, for example `task/010-development-readme`.

- `main` - stable, verified milestone state.
- `development` - integration branch for completed tasks.
- `task/NNN-description` - one scoped roadmap task.

Each task must meet its acceptance criteria and the Definition of Done before it is merged into `development`. After every 10 tasks, complete a milestone checkpoint before promoting `development` to `main`.

See [CONTRIBUTING.md](CONTRIBUTING.md) for the authoritative branch workflow, checkpoint rule, commit guidance, and Definition of Done.
