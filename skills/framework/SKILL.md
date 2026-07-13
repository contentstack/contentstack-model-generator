---
name: framework
description: Third-party and platform libraries used by the tool—CLI, JSON, HTTP, and CMA base URL config—not business logic in ModelGenerator.
---

# Framework – Contentstack Model Generator

## When to use

- Changing **command-line parsing** (McMaster), **JSON** settings, or **HTTP** behavior.
- Understanding how **CMA base URL** is built from host/protocol (`Config` / `ContentstackOptions`).
- Adding or upgrading **NuGet dependencies** on the tool project (with SCA/CodeQL in mind).
- **Not** for end-to-end product flow — see [contentstack-model-generator/SKILL.md](../contentstack-model-generator/SKILL.md).

## What this repo uses (tool project)

Declared in [contentstack.model.generator.csproj](../../contentstack.model.generator/contentstack.model.generator.csproj):

| Area | Library / API | Where it shows up |
| --- | --- | --- |
| **CLI** | **McMaster.Extensions.CommandLineUtils** | [Program.cs](../../contentstack.model.generator/Program.cs) `CommandLineApplication.ExecuteAsync<ModelGenerator>`; `[Command]`, `[Option]`, `[VersionOption]` on [ModelGenerator.cs](../../contentstack.model.generator/ModelGenerator.cs). |
| **JSON** | **Newtonsoft.Json** | Deserialization in [ContentstackClient.cs](../../contentstack.model.generator/CMA/ContentstackClient.cs) (`JsonConvert`, `JObject`); generated code templates in `ModelGenerator` also emit Newtonsoft attributes. |
| **HTTP** | **`System.Net.Http.HttpClient`** (via [HTTPRequestHandler.cs](../../contentstack.model.generator/CMA/HTTPRequestHandler.cs)) | GET requests with query string; maps `Authorization` / `api_key` / `authtoken` headers; sets `x-user-agent`. No Polly/retry stack in-repo—retries are not a built-in feature here. |
| **CMA URL** | [Config.cs](../../contentstack.model.generator/CMA/Config.cs) | Internal `Config` holds protocol, host, port, API version; combined into `BaseUrl` for stack/content_types/global_fields paths used by `ContentstackClient`. |

## Conventions

- **CLI:** Prefer existing McMaster patterns (`[Option]`, `CommandOptionType`, validation attributes) on `ModelGenerator`; keep `--help` and README in sync when adding flags.
- **JSON:** Respect `JsonSerializerSettings` on `ContentstackClient` when changing deserialize behavior; generated models assume Newtonsoft on the **consumer** side too (see [AGENTS.md](../../AGENTS.md) consumer packages).
- **HTTP:** Changes to TLS, timeouts, or default `HttpClient` lifetime should stay centralized in `HTTPRequestHandler` (or the type that owns the client) to avoid duplicated policy.

## Packaging

The tool is shipped as a **.NET global tool** (`PackAsTool`). Commands and CI paths: [dev-workflow/SKILL.md](../dev-workflow/SKILL.md).
