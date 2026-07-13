---
name: contentstack-model-generator
description: CLI entry, CMA and OAuth flow, HTTP layer, and C# code generation pipeline for the Contentstack Model Generator tool.
---

# Contentstack Model Generator – product and runtime flow

## When to use

- Changing **CLI flags**, validation, or exit codes.
- Modifying **authentication** (authtoken vs OAuth / PKCE) or **CMA** calls.
- Adjusting **HTTP** behavior or headers.
- Changing **generated C#** (models, helpers, converters, modular blocks, groups).

## Runtime flow (high level)

Entry: [Program.cs](../../contentstack.model.generator/Program.cs) calls `CommandLineApplication.ExecuteAsync<ModelGenerator>`. Uncaught exceptions print `Messages.UnexpectedError` and return exit code `EXCEPTION` (`2`).

Orchestration: [ModelGenerator.cs](../../contentstack.model.generator/ModelGenerator.cs) — `[Command]` class `ModelGenerator`, method **`OnExecute`**.

### Phases in `ModelGenerator.OnExecute` (order matters)

1. **Validate auth** — If `--oauth`: require client id and redirect URI (and optional secret). Else: require `--authtoken` (`-A`).
2. **Build `ContentstackOptions`** — API key, host, branch, OAuth fields, optional scopes list.
3. **OAuth only** — `HandleOAuthFlow`: PKCE verifier → authorization URL → user pastes authorization code → `OAuthService.ExchangeCodeForTokenAsync` → set `Authorization` Bearer (and tokens) on `options`.
4. **`ContentstackClient`** — Construct with `ContentstackOptions`; constructor sets headers (`api_key`, `authtoken` **or** `Authorization`, optional `branch`) and `Config` from host/version.
5. **`GetStack()`** — Validates connectivity and loads stack metadata (`StackResponse`).
6. **Output path** — Resolve `-p` / cwd; append **`/Models`**; create directory if needed.
7. **Paginated fetch** — `getContentTypes` and `getGlobalFields` with `skip` in steps of **100** until all rows fetched; both append into **`_contentTypes`**.
8. **Shared generated files** — `CreateEmbeddedObjectClass`, `CreateLinkClass`, `CreateHelperClass`, `CreateStringHelperClass`, `CreateDisplayAttributeClass` (helpers/converters used by emitted types).
9. **Per content type** — `CreateFile` for each `Contenttype` in `_contentTypes` (handles modular blocks, groups, RTE references via nested `Create*` methods).
10. **Finish** — Success messages; `OpenFolderatPath` opens the `Models` folder in the OS shell.
11. **OAuth cleanup** — `OAuthService.LogoutAsync` (failures logged as warnings, non-fatal).

Exit codes: `Program.OK` (`0`), `Program.ERROR` (`1`) on user/API/OAuth failures; `Program.EXCEPTION` (`2`) only from `Program.Main` on unexpected exceptions.

## Layer map

| Layer | Responsibility | Main paths |
| --- | --- | --- |
| **CLI bootstrap** | Parse args, top-level exception handling | [Program.cs](../../contentstack.model.generator/Program.cs) |
| **Command + codegen** | Options, auth, fetch, emit C# text | [ModelGenerator.cs](../../contentstack.model.generator/ModelGenerator.cs) |
| **CMA client** | Stack, content types, global fields URLs; header assembly; JSON deserialize | [ContentstackClient.cs](../../contentstack.model.generator/CMA/ContentstackClient.cs) |
| **HTTP** | GET requests, query string, `Authorization` vs custom headers | [HTTPRequestHandler.cs](../../contentstack.model.generator/CMA/HTTPRequestHandler.cs) |
| **OAuth** | PKCE, authorize URL, token exchange, logout | [OAuthService.cs](../../contentstack.model.generator/CMA/OAuth/OAuthService.cs), [ContentstackOptions.cs](../../contentstack.model.generator/CMA/ContentstackOptions.cs) |
| **DTOs** | Content types, fields, stack/API JSON shapes; OAuth token/app JSON | [Model/](../../contentstack.model.generator/Model/) — including [OAuth.cs](../../contentstack.model.generator/Model/OAuth.cs) (`Contentstack.Model.Generator.Model`) |

### Debugging API errors

`ContentstackClient.GetContentstackException` is written around **`WebException`**. [HTTPRequestHandler](../../contentstack.model.generator/CMA/HTTPRequestHandler.cs) uses **`HttpClient`** and may throw **`HttpRequestException`**. Parsed CMA error JSON from `GetContentstackException` may not apply on every failure path — check the **actual exception type** when debugging HTTP failures.

## Extension points

| Goal | Where to change |
| --- | --- |
| New CLI flag | `[Option]` properties on `ModelGenerator`; consume in `OnExecute`; document in [README.md](../../README.md). |
| User-facing CLI strings | [Messages.cs](../../contentstack.model.generator/Messages.cs) |
| New header or API call | `ContentstackClient` methods and/or `HTTPRequestHandler.ProcessRequest`; keep header rules consistent with OAuth vs authtoken. |
| Field type → C# mapping | `GetDatatype`, `GetDatatypeForField`, `GetDatatypeForContentType` in `ModelGenerator`. |
| New global emitted type | Add a `Create*` method and invoke it in the same phase as related helpers (order matters for dependencies). |
| Modular blocks / groups | `CreateModularBlocks`, `CreateGroup`, `CreateModularBlockConverter`, `CreateModularBlockEnum`, etc. |

### File overwrite behavior

`shouldCreateFile` prompts when a file exists unless `--force` (`-f`) is set; returns `null` to skip writing.

User-facing OAuth and install/flags: [OAuth.md](../../OAuth.md), [README.md](../../README.md) (consumer apps need NuGets for generated code: **Contentstack.Core**, **Contentstack.Utils**, **Newtonsoft.Json**, **Markdig** where markdown helpers are used).

If this document grows unwieldy, consider splitting **only** the emit pipeline (modular blocks, groups, `GetDatatype*`) into a separate `code-generation-pipeline` skill and link it from here.
