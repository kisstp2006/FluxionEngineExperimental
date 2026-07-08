# Fluxion Engine — Coding Conventions

Two languages, two conventions — each idiomatic for its ecosystem.
The rule of thumb: **C++ (engine) is snake_case, C# (editor) is .NET-style PascalCase.**

## C++ (Engine, FluxionTestFramework)

| Element | Convention | Example |
|---|---|---|
| Files | PascalCase, `.h` / `.cpp` | `GameObject.h`, `MathTypes.h` |
| Namespaces | lowercase, nested by module | `fluxion::ecs::transform` |
| Classes / structs | `snake_case` | `game_object`, `init_info` |
| Functions / methods | `snake_case` | `create_game_object`, `is_valid` |
| Private members | `_` prefix | `_id` |
| Locals / parameters | `snake_case`; short names for obvious types | `game_object_index`, `obj`, `info` |
| Constants (`constexpr`) | `snake_case` | `invalid_id`, `min_deleted_elements` |
| Type aliases | short lowercase | `flu32`, `flf32`, `v3`, `m4x4` |
| Macros | `SCREAMING_SNAKE_CASE` | `DEFINE_TYPED_ID`, `FLUXION_MATH_GENERIC` |

Additional rules:
- Includes are project-root relative (`"Common/Id.h"`, `"ECS/GameObject.h"`);
  same-folder includes may use the bare file name.
- Engine code never names `DirectX::` types directly — always the `math::` aliases.
- Internal (file-private) data lives in an anonymous namespace in the `.cpp`.
- Every test header defines a class named `engine_test` so `main.cpp` can stay unchanged.

## C# (FluxionEditor)

| Element | Convention | Example |
|---|---|---|
| Files | PascalCase, one main type per file | `GameObject.cs` |
| Namespaces | PascalCase, folder-aligned | `FluxionEditor.Foundation.Components` |
| Classes / interfaces | PascalCase, `I` prefix for interfaces | `MSGameObject`, `IMSComponent` |
| Methods / properties / events | PascalCase | `SelectedGameObjects`, `Refresh()` |
| Private fields | `_camelCase` | `_isEnabled`, `_enableUpdates` |
| Locals / parameters | `camelCase` | `msGameObject`, `gameObjects` |
| Event handlers | `On` + PascalCase, no underscores | `OnGameObjectListSelectionChanged` |
| XAML `x:Name` | PascalCase | `ProjectsListBox`, `InfoCheckBox` |

Additional rules:
- Public surface is PascalCase — no camelCase properties, even on internal classes.
- Event handler names in `.axaml` must be renamed together with the code-behind.
- Prefer commands (`ICommand`) for view logic; code-behind handlers only for
  view-local concerns (selection plumbing, dialogs).

## Shared vocabulary

The engine's entity concept is called a **game object** on both sides:
- C++: `game_object`, `game_object_id`, `create_game_object`
- C#: `GameObject`, `MSGameObject` (multi-select wrapper)

Never introduce `entity` / `game_entity` names — this engine calls them game objects.
