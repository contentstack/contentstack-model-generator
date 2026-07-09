### Version: 1.0.0
#### Date: 

##### Breaking Changes:
- Removed the `Newtonsoft.Json` dependency entirely. Both the tool's internals and the models/converters it generates now use `System.Text.Json`.
- Generated model files now use `[JsonPropertyName]` instead of `[JsonProperty]`, and reference `System.Text.Json` / `System.Text.Json.Nodes` / `System.Text.Json.Serialization` instead of `Newtonsoft.Json`.
- Generated embedded-object and modular-block converters now inherit `System.Text.Json.Serialization.JsonConverter<T>` (`Read`/`Write`) instead of Newtonsoft's `JsonConverter<T>` (`ReadJson`/`WriteJson`). Regenerating models against an existing project will change these attributes and converters - update consuming code accordingly.
- Requires **.NET 10.0** or later (previously .NET 7.0).
- `ContentstackException` no longer supports legacy `BinaryFormatter`-based serialization (removed the obsolete `SerializationInfo`/`StreamingContext` constructor and `GetObjectData` override).

##### Enhancements:
- Upgraded target framework from net7.0 to net10.0.
- Updated CI/CD pipeline to use .NET 10 SDK (actions/setup-dotnet@v4).
- Updated Microsoft.AspNetCore.Mvc.Testing from 9.0.9 to 10.0.0.
- Migrated internal model DTOs (Field, MetaData, Contenttype, StackResponse, ContentStackError, OAuth token response) from `[JsonProperty]` to `[JsonPropertyName]`.
- Migrated OAuthService token exchange/refresh deserialization to `System.Text.Json`.
- Migrated the HTTP layer (IResponse, ContentstackResponse, HTTPRequestHandler, ContentstackHttpRequest) from Newtonsoft `JObject` to `System.Text.Json.Nodes.JsonObject`.
- Migrated ContentstackClient's serializer settings, response deserialization, and error parsing to `System.Text.Json` (`JsonSerializerOptions`, `JsonNode`).
- Migrated the code-generation templates (using statements, field attributes, link class, embedded-object converter, modular-block converter, and helper class) to emit `System.Text.Json`-based generated code.
- Fixed reference-field type resolution (`GetDatatypeForContentType`) to use `System.Text.Json.JsonElement` instead of the removed Newtonsoft `JArray`/`JObject` types.

##### Bug fix:
- Fixed GetHeader method visibility (private → internal) to allow test access.
- Fixed a template bug where the generated `embeddedItems` property's nullable annotation was emitted as literal text (`{nullableString()}`) instead of being applied, for content types with RTE-embedded references.

### Version: 0.5.1
#### Date: Jan-12-2026

- Improved Error messages 

### Version: 0.5.0
#### Date: Oct-05-2025

- Feat: Added OAuth Support in Dotnet Model Generator

### Version: 0.4.6
#### Date: Feb-19-2024

- Fix JsonProperty tag to get added for Uid field.

### Version: 0.4.5
#### Date: Feb-06-2024

- Extended JsonProperty usage all fields, irrespective of underscore presence in the field name.

### Version: 0.4.4
#### Date: Apr-12-2023

- Support for nullable annotation context and nullable warning context to avoid runtime errors.
- Support for branch specific model generation added

### Version: 0.4.3
#### Date: Feb-25-2022

- Upgraded to Net 6.0 compatibility.

### Version: 0.4.2
#### Date: Feb-25-2022

- Modular block with global field class naming issue resolved

### Version: 0.4.1
#### Date: Jul-16-2021

##### New Feature:
- Feature Json RTE support added

### Version: 0.4.0
#### Date: Apr-09-2021

##### New Feature:
- Embedded Items feature support added to generate IEmbeddedObject and IEntryEmbedable

### Version: 0.3.0
#### Date: Mar-12-2021

##### Update API:
- Removed Delivery Token support from plugin use Authtoken instead.

##### Bug fix:
- Reference field List/Object issue for new Stack resolved.

### Version: 0.2.3
#### Date: Aug-12-2020 
- DisplayNameAttribute added for Modular block Enum
- Skip limit functionality added to support Stack with more than 100 Content Type and Global Field
- Contentstack Response Class added
- Modular block converter to implement CSJsonConverterAttribute to autoload converters 

### Version: 0.2.2
#### Date: June-17-2020 
- Modular block with Global field

### Version: 0.2.1
#### Date: May-15-2020 
- Entry Links URL Parsing
- Modular block Enum Parsing


### Version: 0.2.0 
#### Date: May-4-2020 
- Host addition support added

### Version: 0.1.3 
#### Date: Feb-14-2020 

- ContentstackHelper update with using statement
- Update to Class implementation with 'partial class'
- Added const string 'ContentType' to specify content-type of class 
- Support to set reference field class in MultiCT for single reference 

### Version: 0.1.0 
#### Date: Jan-10-2020 

- Introduce ContentStack model generator CLI for DOTNET.
