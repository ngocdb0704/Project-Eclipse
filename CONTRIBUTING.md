# Contributing to Project Eclipse

Project Eclipse follows a task-based Git workflow aligned with the project's development roadmap.

## Branch Strategy

### `main`

`main` represents the stable state of the project.

Only verified milestone changes should be merged into `main`.

Direct feature development should not normally occur on this branch.

### `development`

`development` is the integration branch for completed development tasks.

Individual task branches are merged into `development` after their acceptance criteria have been verified.

After a milestone checkpoint is completed successfully, `development` may be merged into `main`.

### Task Branches

Each roadmap task should normally use its own branch.

Naming convention:

`task/<task-number>-<short-description>`

Examples:

`task/003-folder-structure`

`task/004-assembly-definitions`

`task/018-health-component`

Branches should be created from the latest `development` branch.

## Development Workflow

1. Update `development`.
2. Create a branch for the current task.
3. Implement only the scope defined by that task.
4. Verify all acceptance criteria.
5. Ensure Unity compiles without errors.
6. Review the changed files.
7. Commit the task.
8. Merge the task branch into `development`.
9. Delete the completed task branch when appropriate.

Every 10 tasks, run the milestone checkpoint before promoting `development` to `main`.

## Commit Convention

Use concise Conventional Commit-style messages where practical.

Examples:

`chore: initialize Unity 2D URP project`

`chore: create project folder structure`

`feat: implement player movement`

`feat: add enemy chase behavior`

`fix: reset pooled enemy health`

`test: add damage calculation tests`

`docs: document development workflow`

Recommended prefixes:

- `feat:` — gameplay or product feature
- `fix:` — bug fix
- `chore:` — infrastructure or maintenance
- `refactor:` — internal restructuring without behavior change
- `test:` — tests
- `docs:` — documentation
- `perf:` — performance optimization

## Definition of Done

A task is considered complete only when:

- The project compiles successfully.
- Unity Console contains no compile errors.
- The task requirements are implemented.
- All acceptance criteria are verified.
- Existing functionality still works.
- Files are located in the correct module/folder.
- No unrelated refactoring was introduced.
- Gameplay configuration is not unnecessarily hard-coded.
- Generated Unity folders are not committed.
- Tests required by the task pass.

## Unity Generated Files

The following directories must not be committed:

- `Library/`
- `Temp/`
- `Logs/`
- `Obj/`
- `Build/`
- `Builds/`
- `UserSettings/`

The repository `.gitignore` contains the authoritative ignore rules.

## Project Version

Use the Unity Editor version defined by:

`ProjectSettings/ProjectVersion.txt`

Do not manually edit this file to change Unity versions.

Unity version upgrades must be deliberate project-level changes.

## Scope Discipline

Each task should modify only files necessary for its requirements.

Do not implement future roadmap tasks early simply because they are related.

Avoid unrelated refactoring while completing a task.

This keeps commits small, reviewable, and easy to revert.