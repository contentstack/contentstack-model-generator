---
name: testing
description: xUnit test layout, how to add new tests, tooling, test file map, and credentials policy for Contentstack Model Generator.
---

# Testing – Contentstack Model Generator

## When to use

- Adding or fixing unit/integration tests for OAuth, HTTP, CMA client, options, or codegen.
- Running tests locally with coverage expectations.
- Deciding how to handle API keys, tokens, or secrets in tests.

## Instructions

### Test project

- **Path:** `contentstack.model.generator.tests/contentstack.model.generator.tests.csproj`
- **Framework:** `net9.0`
- **Packages:** xUnit, Moq, `Microsoft.NET.Test.Sdk`, `coverlet.collector`, `Microsoft.AspNetCore.Mvc.Testing` (see csproj for versions).
- **`Microsoft.AspNetCore.Mvc.Testing`:** Referenced in the test csproj but **not used** by any current `.cs` test file (no `WebApplicationFactory` / similar). Safe candidate for removal in a dedicated change, or keep if you plan integration-style web tests.
- **Internals:** Main project exposes internals to the test assembly via `InternalsVisibleTo` in `contentstack.model.generator/contentstack.model.generator.csproj`.

### Run tests

From repository root:

```bash
dotnet test contentstack.model.generator/contentstack.model.generator.sln
```

With coverage (example using coverlet collector — adjust flags to match your local tooling):

```bash
dotnet test contentstack.model.generator/contentstack.model.generator.sln --collect:"XPlat Code Coverage"
```

### HTML test + coverage report

`Scripts/generate_test_report.py` (stdlib-only, no extra Python packages) turns a `.trx` run plus its Cobertura coverage file into a single self-contained HTML report (pass/fail summary, per-class breakdown with failure details, and a code coverage table). Mirrors the pattern used in the `contentstack-dotnet` sibling repo's `Scripts/generate_enhanced_html_report.py`.

```bash
dotnet test contentstack.model.generator.tests/contentstack.model.generator.tests.csproj \
    --logger "trx;LogFileName=test-results.trx" \
    --collect:"XPlat Code Coverage" \
    --results-directory contentstack.model.generator.tests/TestResults

python3 Scripts/generate_test_report.py
```
Run with no arguments, it auto-discovers the newest `.trx` and `coverage.cobertura.xml` under `contentstack.model.generator.tests/TestResults/`. Pass explicit paths (`python3 Scripts/generate_test_report.py <trx> <coverage.xml>`) to target a specific run. Output is `test-report_<timestamp>.html` at the repo root — gitignored, local artifact only.

Run a **single test** or class (examples):

```bash
dotnet test contentstack.model.generator/contentstack.model.generator.sln --filter "FullyQualifiedName~ContentstackClientTests"
dotnet test contentstack.model.generator/contentstack.model.generator.sln --filter "FullyQualifiedName~Constructor_ShouldInitializeWithOAuthOptions"
```

### Adding new tests (step by step)

#### 1. Pick where the test lives

| If you are testing… | Prefer |
| --- | --- |
| Existing area in the map below | Add methods to the **existing** `*Tests.cs` file (keeps related cases together). |
| A new subsystem (new public type, new vertical) | Add a **new** file under `contentstack.model.generator.tests/`, e.g. `MyFeatureTests.cs`, and update the **Test file map** table in this skill in the same PR. |
| `internal` types in the main project | Rely on **`InternalsVisibleTo`** (`contentstack.model.generator.tests`) — you can reference internals from the test assembly without changing production code for visibility. |

#### 2. Namespace and class shape

- **Namespace:** `contentstack.model.generator.tests` (same as existing tests).
- **Class name:** suffix **`Tests`**, e.g. `ContentstackClientTests`, `OAuthServiceTests`.
- **Regions (optional):** In large files, use `#region` blocks (e.g. `#region Property Tests`) like [ModelGeneratorTests.cs](../../contentstack.model.generator.tests/ModelGeneratorTests.cs).

#### 3. Test method naming

Follow patterns already used in this repo:

- **`MethodName_Scenario_ExpectedOutcome`** — e.g. `Constructor_ShouldInitializeWithOAuthOptions`, `GenerateCodeVerifier_ShouldReturnValidBase64UrlEncodedString`.
- Or **`Feature_ShouldBehavior`** — e.g. `ApiKey_ShouldBeRequired`, `OAuthScopesParsing_ShouldParseCorrectly`.

Names should read clearly in test runners and failure output.

#### 4. xUnit attributes

| Use | When |
| --- | --- |
| `[Fact]` | Single scenario, no input matrix. |
| `[Theory]` + `[InlineData(...)]` | Parameterized cases (multiple inputs → same assertion shape). |
| `[Theory]` + `[MemberData(nameof(MyData))]` | Larger or shared datasets (use when InlineData becomes unwieldy). |

**Exceptions:** use `Assert.Throws<TException>(() => ...)` (see `Constructor_ShouldHandleNullOptions` in [ContentstackClientTests.cs](../../contentstack.model.generator.tests/ContentstackClientTests.cs)).

**Async:** use `public async Task MethodName_...()` and `await` inside; xUnit supports async test methods.

#### 5. Arrange / Act / Assert

Keep the same structure as existing files:

1. **Arrange** — build `ContentstackOptions`, `ModelGenerator`, mocks, or test data (use obvious **placeholder** strings like `test_api_key`, not real credentials).
2. **Act** — call the method under test (or construct the object).
3. **Assert** — `Assert.Equal`, `Assert.NotNull`, `Assert.True`, etc.

Leave a blank line between phases if it helps readability (many tests in this repo separate blocks with spacing).

#### 6. Moq and HTTP

- Use **Moq** when you need substitutes for interfaces or virtual members; follow imports and style in [OAuthServiceTests.cs](../../contentstack.model.generator.tests/OAuthServiceTests.cs) and other OAuth/HTTP tests.
- For **HTTP**, prefer mocking at the boundary you own (e.g. handlers) rather than hitting real network URLs in unit tests.

#### 7. CLI / `ModelGenerator` options

- For option metadata and validation, tests often use **reflection** (`typeof(ModelGenerator).GetProperty`, `GetCustomAttribute<RequiredAttribute>()`, `OptionAttribute`) — see [ModelGeneratorTests.cs](../../contentstack.model.generator.tests/ModelGeneratorTests.cs).
- When simulating CLI behavior, you can set properties on `new ModelGenerator()` directly; full end-to-end CLI execution is heavier — keep tests focused and fast unless you add a dedicated integration test.

#### 8. Before you open a PR

- Run the full suite: `dotnet test contentstack.model.generator/contentstack.model.generator.sln`.
- **CI does not run** `dotnet test` in GitHub Actions today — local green tests are the gate (see [dev-workflow/SKILL.md](../dev-workflow/SKILL.md)).
- Re-read [Credentials and secrets](#credentials-and-secrets) below — no real tokens or keys in source.

### Test file map

| File | Typical focus |
| --- | --- |
| [ContentstackOptionsTests.cs](../../contentstack.model.generator.tests/ContentstackOptionsTests.cs) | `ContentstackOptions` construction and options. |
| [ContentstackClientTests.cs](../../contentstack.model.generator.tests/ContentstackClientTests.cs) | `ContentstackClient` behavior (headers, config). |
| [HTTPRequestHandlerTests.cs](../../contentstack.model.generator.tests/HTTPRequestHandlerTests.cs) | HTTP request handling. |
| [OAuthServiceTests.cs](../../contentstack.model.generator.tests/OAuthServiceTests.cs) | OAuth helpers (PKCE URL generation, etc.). |
| [OAuthTokenExchangeTests.cs](../../contentstack.model.generator.tests/OAuthTokenExchangeTests.cs) | Token exchange flows. |
| [OAuthErrorHandlingTests.cs](../../contentstack.model.generator.tests/OAuthErrorHandlingTests.cs) | OAuth error paths. |
| [OAuthIntegrationTests.cs](../../contentstack.model.generator.tests/OAuthIntegrationTests.cs) | Broader OAuth integration scenarios. |
| [ModelGeneratorOAuthTests.cs](../../contentstack.model.generator.tests/ModelGeneratorOAuthTests.cs) | CLI / `ModelGenerator` + OAuth interactions. |
| [ModelGeneratorTests.cs](../../contentstack.model.generator.tests/ModelGeneratorTests.cs) | CLI option metadata / `ModelGenerator` property behavior. |
| [ModelGeneratorTestHelpers.cs](../../contentstack.model.generator.tests/ModelGeneratorTestHelpers.cs) | Not a test class - shared `Field`/`MetaData`/`Contenttype` fixture builders and `CreateGenerator(...)` factory for the codegen test files below. |
| [ModelGeneratorFieldTypeResolutionTests.cs](../../contentstack.model.generator.tests/ModelGeneratorFieldTypeResolutionTests.cs) | `GetDatatypeForField` - field-type → C# type mapping, reference/global_field resolution, multiplicity wrapping. |
| [ModelGeneratorAddParamsTests.cs](../../contentstack.model.generator.tests/ModelGeneratorAddParamsTests.cs) | `AddParams` - attribute + property emission per field type, markdown special case. |
| [ModelGeneratorNullableVariantTests.cs](../../contentstack.model.generator.tests/ModelGeneratorNullableVariantTests.cs) | `IsNullable` (`--is-nullable`/`-N`) toggle across `AddParams`/`CreateLinkClass`/`CreateFile`/`CreateModularBlockConverter`, plus the `CreateEmbeddedObjectClass` no-suffix asymmetry. |
| [ModelGeneratorCreateFileTests.cs](../../contentstack.model.generator.tests/ModelGeneratorCreateFileTests.cs) | `CreateFile` end-to-end - class shape, nested blocks/group generation, using directives, embedded-items interpolation regression. |
| [ModelGeneratorSupportingClassGenerationTests.cs](../../contentstack.model.generator.tests/ModelGeneratorSupportingClassGenerationTests.cs) | `CreateLinkClass`, `CreateEmbeddedObjectClass`, `CreateModularBlockConverter` called directly. |

### Credentials and secrets

- **Do not** commit real stack API keys, authtokens, client secrets, or production OAuth codes.
- Prefer **mocks** (Moq) and **fake/staging** endpoints where integration tests need HTTP; keep secrets in CI **encrypted secrets**, not in source.
- Vulnerability reporting: [SECURITY.md](../../SECURITY.md) (do not file security issues as public GitHub issues).
