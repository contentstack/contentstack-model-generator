---
name: dev-workflow
description: Branches, CI, and dotnet restore/build/test/pack for the Contentstack Model Generator solution.
---

# Dev workflow – Contentstack Model Generator

## When to use

- Setting up a local build or verifying changes before a PR.
- Understanding which GitHub Actions run on PRs or releases.
- Packing or publishing the global tool NuGet package.
- Anything about **how the CLI and codegen behave** at runtime — see [contentstack-model-generator/SKILL.md](../contentstack-model-generator/SKILL.md).

## Instructions

### Repository layout

- **Solution:** `contentstack.model.generator/contentstack.model.generator.sln`
- **Tool (executable + pack):** `contentstack.model.generator/contentstack.model.generator.csproj` (`PackAsTool`, `net7.0`).
- **Tests:** `contentstack.model.generator.tests/contentstack.model.generator.tests.csproj` (`net9.0`).

### Commands (from repository root)

| Step | Command |
| --- | --- |
| Restore | `dotnet restore contentstack.model.generator/contentstack.model.generator.sln` |
| Build | `dotnet build contentstack.model.generator/contentstack.model.generator.sln` |
| Test | `dotnet test contentstack.model.generator/contentstack.model.generator.sln` |
| Pack Release | `cd contentstack.model.generator && dotnet pack -c Release -o out` (output `.nupkg` under `contentstack.model.generator/out/`; CI uses similar `dotnet pack` from that directory). |

Package version and metadata: see `PackageVersion` / `ReleaseVersion` in `contentstack.model.generator/contentstack.model.generator.csproj` (avoid duplicating version numbers in prose here).

**CI and unit tests:** No workflow under [.github/workflows/](../../.github/workflows/) runs `dotnet test`. Run `dotnet test contentstack.model.generator/contentstack.model.generator.sln` **locally** before relying on a PR.

**Talisman:** [.talismanrc](../../.talismanrc) may configure secret scanning for this repo. If it references workflow paths that no longer exist (e.g. `.github/workflows/secrets-scan.yml`), update ignores or restore the workflow in a follow-up change.

### Branches and PRs

- Workflow [.github/workflows/check-branch.yml](../../.github/workflows/check-branch.yml): pull requests **into `master`** must come **from `staging`** (other combinations get a failing check and PR comment).
- Plan feature work accordingly: integrate to `staging` first unless your team documents an exception.

### CI and security workflows (overview)

| Workflow | Role |
| --- | --- |
| [check-branch.yml](../../.github/workflows/check-branch.yml) | Enforces `staging` → `master` for PRs targeting `master`. |
| [sca-scan.yml](../../.github/workflows/sca-scan.yml) | `dotnet restore` + Snyk on .NET assets. |
| [codeql-analysis.yml](../../.github/workflows/codeql-analysis.yml) | CodeQL for C#. |
| [nuget-publish.yml](../../.github/workflows/nuget-publish.yml) | On **release created**: pack and push NuGet package(s). |
| [policy-scan.yml](../../.github/workflows/policy-scan.yml), [issues-jira.yml](../../.github/workflows/issues-jira.yml) | Org/process automation as configured. |

### PR expectations (summary)

- Run **build + test** locally before opening or updating a PR.
- For user-visible behavior or releases, coordinate **version** and **[CHANGELOG.md](../../CHANGELOG.md)** with maintainers (see [code-review/SKILL.md](../code-review/SKILL.md)).
