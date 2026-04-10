---
name: csharp-dotnet
description: .NET TFMs, namespaces, and source layout conventions for Contentstack Model Generator.
---

# C# / .NET – Contentstack Model Generator

## When to use

- Adding new `.cs` files or projects.
- Resolving confusion between tool and test target frameworks.
- Matching existing namespace and folder patterns.

## Instructions

### Target frameworks

| Project | TFM | Notes |
| --- | --- | --- |
| `contentstack.model.generator` | `net7.0` | Global tool executable; `PackAsTool` in csproj. |
| `contentstack.model.generator.tests` | `net9.0` | `ImplicitUsings` and `Nullable` enabled in test csproj only. |

When adding code to the **tool**, assume **.NET 7** language/API surface unless you raise the TFM in the csproj.

### Namespaces and folders

- **CLI entry and codegen:** `contentstack.model.generator` — e.g. [Program.cs](../../contentstack.model.generator/Program.cs), [ModelGenerator.cs](../../contentstack.model.generator/ModelGenerator.cs). Root namespace in csproj is `Contentstack.Model.Generator`; some types use `contentstack.model.generator` / `contentstack.CMA` style — **follow the file you are editing**.
- **CMA / HTTP / OAuth:** [contentstack.model.generator/CMA/](../../contentstack.model.generator/CMA/) — `ContentstackClient`, `HTTPRequestHandler`, `ContentstackOptions`, `OAuth/OAuthService`.
- **JSON DTOs / models:** [contentstack.model.generator/Model/](../../contentstack.model.generator/Model/) — e.g. `Contenttype`, `Field`, stack responses.

### Conventions for new code

- Prefer **async** patterns consistent with existing CMA methods (`async Task`, `await`).
- Use **Newtonsoft.Json** attributes and patterns already used in generated and hand-written models unless migrating the whole project.
- Avoid **public API** expansion on the tool’s CLI without updating [README.md](../../README.md) and the `[VersionOption]` / package version in csproj when releasing.
