# Contributing to Project Eclipse

Project Eclipse uses a task-based Git workflow. Start with the developer setup and repository layout in [README.md](README.md).

## Branch strategy

- `main` is the stable branch. Only verified milestone changes should be merged into it; feature development should not normally occur here.
- `development` integrates completed tasks. A task is merged only after its acceptance criteria and the Definition of Done are verified.
- `task/NNN-description` contains one roadmap task and is created from the latest `development`. Use a zero-padded task number and a short kebab-case description, for example `task/003-folder-structure`, `task/010-development-readme`, or `task/018-health-component`.

## Task workflow

1. Update `development` and create the task branch from it.
2. Implement only the task's defined scope; do not implement later roadmap work early.
3. Verify every acceptance criterion and run the relevant tests.
4. Confirm the project compiles and the Unity Console has no compile errors.
5. Review the changed files and commit the task.
6. Merge the completed task into `development` and delete the task branch when appropriate.

### Milestone checkpoints

After every 10 tasks (for example, after Task 010), perform a milestone checkpoint before promoting `development` to `main`. At the checkpoint, verify the integrated milestone's acceptance criteria, compilation, relevant tests, scene flow, and changed files. Promote to `main` only when the checkpoint succeeds.

## Commit convention

Use concise Conventional Commit-style messages where practical:

```text
chore: initialize Unity 2D URP project
feat: implement player movement
fix: reset pooled enemy health
test: add damage calculation tests
docs: document development workflow
```

Recommended prefixes are `feat:`, `fix:`, `chore:`, `refactor:`, `test:`, `docs:`, and `perf:`.

## Definition of Done

A task is complete only when all applicable items below are true:

- The task requirements are implemented and every acceptance criterion is verified.
- The project compiles successfully and the Unity Console contains no compile errors.
- All tests required by the task pass, and existing functionality still works.
- Files are in the correct module/folder and respect the existing assembly boundaries.
- No unrelated refactoring or future roadmap functionality was introduced.
- Gameplay configuration is not unnecessarily hard-coded.
- Generated Unity folders are not committed.
- The final changed-file review contains only intended, task-scoped changes.

## Unity version and generated files

Use Unity Editor `6000.3.27f1`, as defined by `ProjectSettings/ProjectVersion.txt`. Do not edit that file manually; Unity upgrades must be deliberate project-level changes.

Do not commit generated directories such as `Library/`, `Temp/`, `Logs/`, `Obj/`, `Build/`, `Builds/`, or `UserSettings/`. The repository `.gitignore` is authoritative.
