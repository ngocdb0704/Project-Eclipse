# Project Eclipse

Project Eclipse is a 2D survivor-like / bullet-heaven roguelite inspired by the gameplay loop of games such as Vampire Survivors.

## Technology

- Engine: Unity 6.3 LTS
- Language: C#
- Rendering: Universal Render Pipeline (URP)
- Renderer: Unity 2D Renderer
- Initial target platform: Windows PC

## Project Status

Current development phase: Project Foundation.

The project is being developed incrementally using a milestone-based roadmap with verifiable acceptance criteria.

## Development Workflow

Project Eclipse uses a task-based Git workflow.

- `main` — stable milestone/release branch
- `development` — active integration branch
- `task/<number>-<description>` — individual roadmap tasks

Example:

```text
development
└── task/003-folder-structure

Each task must satisfy its acceptance criteria before being merged into development.

After every 10 tasks, a milestone checkpoint is performed before changes are promoted to main.

See [CONTRIBUTING.md](CONTRIBUTING.md) for the complete development workflow.
```