# Contentstack Model Generator – Agent guide

**Universal entry point** for contributors and AI agents. Detailed conventions live in **`skills/*/SKILL.md`**.

## What this repo is

| Field | Detail |
| --- | --- |
| **Name:** | [contentstack/contentstack-model-generator](https://github.com/contentstack/contentstack-model-generator) |
| **Purpose:** | A **.NET global tool** (`contentstack.model.generator`) that connects to the Content Management API (CMA), fetches content types and global fields from a stack, and **emits C# model classes** (and helpers) under a `Models` output folder. |
| **Out of scope (if any):** | This package is a **CLI code generator**, not a runtime Contentstack SDK for apps. Product install and CLI flags are documented in [README.md](README.md). |

## Tech stack (at a glance)

| Area | Details |
| --- | --- |
| **Language** | C# — tool targets **.NET 7** (`net7.0`); test project targets **.NET 9** (`net9.0`). |
| **Build** | .NET SDK — solution [contentstack.model.generator/contentstack.model.generator.sln](contentstack.model.generator/contentstack.model.generator.sln); main project [contentstack.model.generator/contentstack.model.generator.csproj](contentstack.model.generator/contentstack.model.generator.csproj) (`PackAsTool`, `PackOnBuild`). |
| **Tests** | xUnit — project [contentstack.model.generator.tests/contentstack.model.generator.tests.csproj](contentstack.model.generator.tests/contentstack.model.generator.tests.csproj); Moq; coverlet.collector. |
| **Lint / coverage** | Built-in .NET analyzers; coverlet for coverage in tests. No repo-wide `dotnet format` / StyleCop config in-tree at time of writing. |
| **Tool dependencies** | [contentstack.model.generator.csproj](contentstack.model.generator/contentstack.model.generator.csproj): McMaster.Extensions.CommandLineUtils, Newtonsoft.Json; HTTP via [HTTPRequestHandler.cs](contentstack.model.generator/CMA/HTTPRequestHandler.cs) (`HttpClient`). |
| **Consumer projects (generated `Models/`)** | Emitted C# references **Contentstack.Core**, **Contentstack.Utils**, **Newtonsoft.Json**, and **Markdig** (e.g. generated `ContentstackStringExtension`). Those NuGet packages must be added to the **application that uses the generated code**, not to the generator tool project. |

## Commands (quick reference)

| Command type | Command |
| --- | --- |
| **Restore** | `dotnet restore contentstack.model.generator/contentstack.model.generator.sln` |
| **Build** | `dotnet build contentstack.model.generator/contentstack.model.generator.sln` |
| **Test** | `dotnet test contentstack.model.generator/contentstack.model.generator.sln` |
| **Pack (tool NuGet)** | From [contentstack.model.generator/](contentstack.model.generator/): CI uses `dotnet pack -c Release -o out` ([nuget-publish.yml](.github/workflows/nuget-publish.yml)), producing `.nupkg` under `contentstack.model.generator/out/`. Local builds also use **`PackOnBuild`** and **`PackageOutputPath`** (`./nupkg`), so packages may appear under `contentstack.model.generator/nupkg/` unless you override with `-o`. |
| **Install globally (users)** | See [README.md](README.md) (`dotnet tool install -g contentstack.model.generator`). |

**CI / automation:** [.github/workflows/](.github/workflows/) — e.g. branch check ([check-branch.yml](.github/workflows/check-branch.yml)), SCA ([sca-scan.yml](.github/workflows/sca-scan.yml)), CodeQL ([codeql-analysis.yml](.github/workflows/codeql-analysis.yml)), NuGet publish on release ([nuget-publish.yml](.github/workflows/nuget-publish.yml)). **No workflow currently runs `dotnet test`** — run tests locally before opening a PR (see [skills/dev-workflow/SKILL.md](skills/dev-workflow/SKILL.md)).

## Where the documentation lives: skills

| Skill | Path | What it covers |
| --- | --- | --- |
| **Dev workflow** | [skills/dev-workflow/SKILL.md](skills/dev-workflow/SKILL.md) | Branches, CI, build/test/pack commands, solution paths. |
| **Testing** | [skills/testing/SKILL.md](skills/testing/SKILL.md) | Test layout, xUnit/Moq/coverlet, test file map, secrets/credentials. |
| **Code review** | [skills/code-review/SKILL.md](skills/code-review/SKILL.md) | PR checklist, branch policy, security scans. |
| **C# / .NET** | [skills/csharp-dotnet/SKILL.md](skills/csharp-dotnet/SKILL.md) | TFMs, namespaces, project layout conventions. |
| **Framework (libraries)** | [skills/framework/SKILL.md](skills/framework/SKILL.md) | McMaster CLI, Newtonsoft.Json, HTTP client, CMA `Config` / base URL—third-party and platform glue. |
| **Product (CLI + CMA + codegen)** | [skills/contentstack-model-generator/SKILL.md](skills/contentstack-model-generator/SKILL.md) | End-to-end runtime flow, layers, extension points. |

Quick links to each **SKILL.md**: [skills/README.md](skills/README.md).

## Using Cursor (optional)

If you use **Cursor**, [.cursor/rules/README.md](.cursor/rules/README.md) points to **[AGENTS.md](AGENTS.md)** so editor-specific config does not duplicate the canonical docs.
