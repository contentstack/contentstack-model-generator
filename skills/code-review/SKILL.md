---
name: code-review
description: PR checklist, branch policy, versioning notes, and security scan context for this repository.
---

# Code review – Contentstack Model Generator

## When to use

- Opening or reviewing a pull request.
- Checking release readiness (version, changelog, package metadata).
- Ensuring changes align with automated security and branch rules.

## Instructions

### Branch policy

- PRs **into `master`** must be **from `staging`** per [.github/workflows/check-branch.yml](../../.github/workflows/check-branch.yml). Verify head/base before approving merges to `master`.
- If the PR does not target `master`, the same rule may not apply; still follow team conventions for default branch.

### Review checklist

- **Build and tests:** `dotnet build` and `dotnet test` on `contentstack.model.generator/contentstack.model.generator.sln` succeed (CI does not run `dotnet test`; see [dev-workflow/SKILL.md](../dev-workflow/SKILL.md)).
- **Release versioning:** For releases, keep `[VersionOption]` in [ModelGenerator.cs](../../contentstack.model.generator/ModelGenerator.cs) (CLI `--version`), **`PackageVersion` / `ReleaseVersion`** in [contentstack.model.generator.csproj](../../contentstack.model.generator/contentstack.model.generator.csproj), and **[CHANGELOG.md](../../CHANGELOG.md)** aligned.
- **Auth paths:** Traditional `authtoken` vs `--oauth` remain mutually consistent; no accidental logging of secrets.
- **Codegen output:** Changes to `ModelGenerator` or templates do not break emitted class shapes without **[CHANGELOG.md](../../CHANGELOG.md)** and version bump in **[contentstack.model.generator.csproj](../../contentstack.model.generator/contentstack.model.generator.csproj)** when releasing.
- **Dependencies:** New package references are justified; Snyk/SCA will scan on PRs ([sca-scan.yml](../../.github/workflows/sca-scan.yml)).
- **Security-sensitive code:** HTTP, OAuth, and error handling reviewed for information disclosure and robust failure modes.

### Automated security analysis

- **Snyk (SCA):** [.github/workflows/sca-scan.yml](../../.github/workflows/sca-scan.yml) — dependency vulnerabilities.
- **CodeQL:** [.github/workflows/codeql-analysis.yml](../../.github/workflows/codeql-analysis.yml) — static analysis for C#.

Treat new findings from these workflows as blockers until triaged or waived with documented rationale.

### Product and licensing

- **License** and **copyright** in packaged output follow `contentstack.model.generator.csproj` and [LICENSE](../../LICENSE) / packaged `LICENSE.txt` as applicable.
- **Security:** Report vulnerabilities per [SECURITY.md](../../SECURITY.md) (not via public GitHub issues).