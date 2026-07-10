[![Contentstack](https://www.contentstack.com/docs/static/images/contentstack.png)](https://www.contentstack.com/)

# Contentstack model generator
This utility is use to generate models based on ContentTypes in Stack.

## Requirements
**As of v1.0.0**, this tool targets **.NET 10.0** and requires the **.NET 10.0 SDK or later**. Versions prior to v1.0.0 targeted .NET 7.0.

### Compatibility
Since v1.0.0, generated model files use `System.Text.Json` instead of `Newtonsoft.Json`. Because generated code references `Contentstack.Core.Models` and `Contentstack.Utils.Interfaces` / `ContentstackHelper` directly, your consuming project must reference System.Text.Json-based SDK versions for the generated code to compile:

| Package | Minimum Version |
|---|---|
| `contentstack.csharp` (Delivery SDK) | `3.0.0` |
| `contentstack.management.csharp` (Management SDK) | `1.0.0` |
| `contentstack.utils` | `2.0.0` |

Generating models into a project still referencing pre-3.0.0 / pre-1.0.0 / pre-2.0.0 (Newtonsoft-based) SDK versions will produce code that fails to compile.

## Installation
To install Contenstack model generator run following command:
```
dotnet tool install  -g contentstack.model.generator
```

## How to use
Once you install ```contentstack.model.generator```  run ```--help``` to view available commands.

### Authentication Methods

The Contentstack Model Generator supports two authentication methods:

1. **Traditional Authtoken Authentication** (default)
2. **OAuth 2.0 Authentication**

### Command Line Options

| Short key | Long Key | Description |
| -- | -- | -- |
| `-a` | `--api-key` | The Stack API key for the Content Management API |
| `-A` | `--authtoken` | The Authtoken for the Content Management API (required for traditional auth) |
| `-b` | `--branch` | The branch header in the API request to fetch or manage modules located within specific branches. |
| `-e` | `--endpoint` | The Contentstack Host for the Content Management API |
| `-n` | `--namespace` | The namespace the classes should be created in (Default Value: ContentstackModels) |
| `-N` | `--is-nullable` | The features that protect against throwing a System.NullReferenceException can be disruptive when turned on. |
| `-f` | `--force` | Automatically overwrite files that already exist |
| `-m` | `--modular-block-prefix` | The Modular block Class Prefix. (Default Value: MB) |
| `-g` | `--group-prefix` | The Group Class Prefix. (Default Value: Group) |
| `-p` | `--path` | Path to the file or directory to create files in. |

### OAuth 2.0 Options

| Long Key | Description |
| -- | -- |
| `--oauth` | Enable OAuth 2.0 authentication (mutually exclusive with traditional auth) |
| `--client-id` | OAuth Client ID (required for OAuth) (Default Value: Ie0FEfTzlfAHL4xM ) |
| `--client-secret` | OAuth Client Secret (optional for public clients using PKCE) |
| `--redirect-uri` | OAuth Redirect URI (required for OAuth) (Default Value: http://localhost:8184 ) |
| `--app-id` | OAuth App ID (optional) ( Default Value: 6400aa06db64de001a31c8a9 ) |
| `--scopes` | OAuth Scopes (optional, space-separated) |

## Examples

### Traditional API Key Authentication

#### Example 1: Basic Usage
To create classes in current directory run following command:
```
contentstack.model.generator -a <stack_api_key> -A <authtoken>
```

#### Example 2: Specific Path
To create classes in specific path run following command:
```
contentstack.model.generator -a <stack_api_key> -A <authtoken> -p /User/xxx/Desktop
```

#### Example 3: With Namespace
To create classes with namespace run following command:
```
contentstack.model.generator -a <stack_api_key> -A <authtoken> -n YourProject.Models
```

#### Example 4: With Nullable Annotations
To allow `Nullable` annotation context in model creation run following command:
```
contentstack.model.generator -a <stack_api_key> -A <authtoken> -N
```

### OAuth 2.0 Authentication

#### Example 5: OAuth with PKCE (Recommended)
For public clients or enhanced security, use OAuth with PKCE:
```
contentstack.model.generator --oauth -a <stack_api_key> --client-id <client_id> --redirect-uri http://localhost:8184
```

#### Example 6: OAuth with Client Secret
For confidential clients with client secret:
```
contentstack.model.generator --oauth -a <stack_api_key> --client-id <client_id> --client-secret <client_secret> --redirect-uri http://localhost:8184
```

#### Example 7: OAuth with App ID
For OAuth with specific app:
```
contentstack.model.generator --oauth -a <stack_api_key> --client-id <client_id> --redirect-uri http://localhost:8184 --app-id <app_id>
```

#### Example 8: OAuth with Custom Path and Namespace
```
contentstack.model.generator --oauth -a <stack_api_key> --client-id <client_id> --redirect-uri http://localhost:8184 -p /path/to/models -n YourProject.Models
```

## OAuth Command Example

Here's what you'll see when running an OAuth command:

```bash

$ contentstack.model.generator --oauth -a <api_key> --client-id myclient123 --redirect-uri http://localhost:8184

OAuth Authentication Required
=============================

Please open the following URL in your browser to authorize the application:

https://app.contentstack.com/#!/apps/6400aa06db64de001a31c8a9/authorize?response_type=code&client_id=myclient123&redirect_uri=http%3A%2F%2Flocalhost%3A8184&code_challenge=...&code_challenge_method=S256

After authorization, you will be redirected to a local URL.
Please copy the 'code' parameter from the redirect URL and paste it here:

Authorization code: [paste the code here]

Exchanging authorization code for access token...
OAuth authentication successful!
Access token expires at: 2026-01-15 14:30:00 UTC
Fetching stack details for the provided API key.
No path specified. Generating files in the current working directory: /Users/you/project.
Fetching content types from My Contentstack Stack stack.
Found 5 content types.
Total content types fetched: 5.
Fetching global fields from stack: My Contentstack Stack.
Found 2 global fields.
Total global fields fetched: 2.
Generating files from content types.
Files created successfully.
Opening output directory: /Users/you/project.

Logging out from OAuth...
OAuth logout successful!
```

## Generated Code

**As of v1.0.0**, generated model files use **System.Text.Json** instead of Newtonsoft.Json:

- Properties are decorated with `[JsonPropertyName("...")]` instead of `[JsonProperty(propertyName: "...")]`.
- Generated files reference `System.Text.Json`, `System.Text.Json.Nodes`, and `System.Text.Json.Serialization` instead of `Newtonsoft.Json`.
- Generated converters (for embedded objects and modular blocks) inherit `System.Text.Json.Serialization.JsonConverter<T>` and implement `Read`/`Write` instead of Newtonsoft's `JsonConverter<T>` with `ReadJson`/`WriteJson`.

A generated model file looks like this:

```csharp
using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Contentstack.Core.Models;
using Contentstack.Utils.Interfaces;

namespace ContentstackModels.Models
{
    public partial class BlogPost : IEmbeddedObject
    {
        public const string ContentType = "blog_post";
        [JsonPropertyName("uid")]
        public string Uid { get; set; }
        [JsonPropertyName("_content_type_uid")]
        public string ContentTypeUid { get; set; }
        [JsonPropertyName("title")]
        public string Title { get; set; }
    }
}
```

**Breaking change:** if you regenerate models for an existing project using v1.0.0 or later, the attributes and converter base classes in the output will change from the Newtonsoft-based shape to the System.Text.Json-based shape shown above. Update your consuming code and Contentstack .NET SDK versions accordingly before regenerating.

### MIT License

Copyright (c) 2012-2026 Contentstack

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

