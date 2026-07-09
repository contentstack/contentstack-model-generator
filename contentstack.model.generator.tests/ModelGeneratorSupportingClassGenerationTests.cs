using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using static contentstack.model.generator.tests.ModelGeneratorTestHelpers;

namespace contentstack.model.generator.tests
{
    /// <summary>
    /// CreateLinkClass(), CreateEmbeddedObjectClass(), CreateModularBlockConverter(),
    /// CreateHelperClass(), CreateStringHelperClass(), and CreateDisplayAttributeClass()
    /// called directly (bypassing CreateFile()) - these are only invoked from OnExecute()
    /// in production, so there is no other way to reach them in a unit test.
    /// </summary>
    public class ModelGeneratorSupportingClassGenerationTests : IDisposable
    {
        private readonly DirectoryInfo _tempDir;

        public ModelGeneratorSupportingClassGenerationTests()
        {
            _tempDir = Directory.CreateTempSubdirectory("cmg_supporting_tests_");
        }

        private string ReadGenerated(string fileName) =>
            File.ReadAllText(Path.Combine(_tempDir.FullName, fileName));

        #region CreateLinkClass

        [Fact]
        public void CreateLinkClass_WritesFileWithTitleAndHrefProperties()
        {
            var generator = CreateGenerator();

            generator.CreateLinkClass("ContentstackModels", _tempDir);
            var content = ReadGenerated("ContentstackLink.cs");

            Assert.Contains("[JsonPropertyName(\"title\")]", content);
            Assert.Contains("public string Title { get; set; }", content);
            Assert.Contains("[JsonPropertyName(\"href\")]", content);
            Assert.Contains("public string Href { get; set; }", content);
        }

        [Fact]
        public void CreateLinkClass_UsingDirectives_IncludeTemplateStartBlock()
        {
            var generator = CreateGenerator();

            generator.CreateLinkClass("ContentstackModels", _tempDir);
            var content = ReadGenerated("ContentstackLink.cs");

            Assert.Contains("using Contentstack.Utils.Interfaces;", content);
            Assert.Contains("using System.Text.Json.Serialization;", content);
        }

        [Fact]
        public void CreateLinkClass_ExistingFile_PromptYes_OverwritesFile()
        {
            var original = Console.In;
            try
            {
                CreateGenerator(isNullable: false).CreateLinkClass("ContentstackModels", _tempDir);

                Console.SetIn(new StringReader("y\n"));
                CreateGenerator(isNullable: true).CreateLinkClass("ContentstackModels", _tempDir);

                var content = ReadGenerated("ContentstackLink.cs");
                Assert.Contains("public string? Title { get; set; }", content);
            }
            finally
            {
                Console.SetIn(original);
            }
        }

        [Fact]
        public void CreateLinkClass_ExistingFile_PromptNo_SkipsOverwrite()
        {
            var original = Console.In;
            try
            {
                CreateGenerator(isNullable: false).CreateLinkClass("ContentstackModels", _tempDir);

                Console.SetIn(new StringReader("n\n"));
                CreateGenerator(isNullable: true).CreateLinkClass("ContentstackModels", _tempDir);

                var content = ReadGenerated("ContentstackLink.cs");
                Assert.Contains("public string Title { get; set; }", content);
                Assert.DoesNotContain("public string? Title { get; set; }", content);
            }
            finally
            {
                Console.SetIn(original);
            }
        }

        #endregion

        #region CreateHelperClass

        [Fact]
        public void CreateHelperClass_WritesFileWithGetDescriptionAndFieldExists()
        {
            var generator = CreateGenerator();

            generator.CreateHelperClass("ContentstackModels", _tempDir);
            var content = ReadGenerated("ContentstackHelper.cs");

            Assert.Contains("public static string GetDescription(Enum en)", content);
            Assert.Contains("public static bool FieldExists(string fieldName, JsonObject jObject)", content);
            Assert.Contains("return jObject[fieldName] != null;", content);
        }

        #endregion

        #region CreateStringHelperClass

        [Fact]
        public void CreateStringHelperClass_WritesFileWithToHtmlAndToListHtmlExtensions()
        {
            var generator = CreateGenerator();

            generator.CreateStringHelperClass("ContentstackModels", _tempDir);
            var content = ReadGenerated("ContentstackStringExtension.cs");

            Assert.Contains("using Markdig;", content);
            Assert.Contains("public static string ToHtml(this String str)", content);
            Assert.Contains("public static List<string> ToListHtml(this List<string> str)", content);
        }

        #endregion

        #region CreateDisplayAttributeClass

        [Fact]
        public void CreateDisplayAttributeClass_WritesAttributeClassWithDisplayNameConstructor()
        {
            var generator = CreateGenerator();

            generator.CreateDisplayAttributeClass("ContentstackModels", _tempDir);
            var content = ReadGenerated("DisplayNameAttribute.cs");

            Assert.Contains("[AttributeUsage(AttributeTargets.Field)]", content);
            Assert.Contains("public partial class DisplayNameAttribute: Attribute", content);
            Assert.Contains("public DisplayNameAttribute(string displayName)", content);
        }

        #endregion

        #region CreateEmbeddedObjectClass

        [Fact]
        public void CreateEmbeddedObjectClass_WritesConverterWithJsonConverterAttributeAndClassDeclaration()
        {
            var generator = CreateGenerator();

            generator.CreateEmbeddedObjectClass("ContentstackModels", _tempDir);
            var content = ReadGenerated("IEmbeddedObjectConverter.cs");

            Assert.Contains("[CSJsonConverter(\"IEmbeddedObjectConverter\")]", content);
            Assert.Contains("public partial class IEmbeddedObjectConverter : JsonConverter<List<IEmbeddedObject>>", content);
        }

        [Fact]
        public void CreateEmbeddedObjectClass_PerContentType_EmitsIfBranchKeyedByContentTypeUid()
        {
            var generator = CreateGenerator(seedContentTypes: new[]
            {
                MakeContentType("blog_post", "Blog Post"),
                MakeContentType("author", "Author")
            });

            generator.CreateEmbeddedObjectClass("ContentstackModels", _tempDir);
            var content = ReadGenerated("IEmbeddedObjectConverter.cs");

            Assert.Contains("if (ctUid == \"blog_post\")", content);
            Assert.Contains("else if (ctUid == \"author\")", content);
            Assert.Contains("JsonSerializer.Deserialize<BlogPost>(obj.ToJsonString(), deserializeOptions);", content);
            Assert.Contains("JsonSerializer.Deserialize<Author>(obj.ToJsonString(), deserializeOptions);", content);
        }

        [Fact]
        public void CreateEmbeddedObjectClass_ContentTypeWithEmptyUid_EmitsBranchForEmptyIdentifier()
        {
            var generator = CreateGenerator(seedContentTypes: new[] { MakeContentType("", "Untitled") });

            generator.CreateEmbeddedObjectClass("ContentstackModels", _tempDir);
            var content = ReadGenerated("IEmbeddedObjectConverter.cs");

            Assert.Contains("if (ctUid == \"\")", content);
        }

        [Fact]
        public void CreateEmbeddedObjectClass_NoContentTypes_OmitsElseKeywordOnFirstBranch()
        {
            var generator = CreateGenerator();

            generator.CreateEmbeddedObjectClass("ContentstackModels", _tempDir);
            var content = ReadGenerated("IEmbeddedObjectConverter.cs");

            Assert.Contains("if (ctUid == \"sys_assets\")", content);
            Assert.DoesNotContain("else if (ctUid == \"sys_assets\")", content);
        }

        [Fact]
        public void CreateEmbeddedObjectClass_AlwaysEmitsSysAssetsFallbackBranch()
        {
            var generator = CreateGenerator(seedContentTypes: new[] { MakeContentType("blog_post", "Blog Post") });

            generator.CreateEmbeddedObjectClass("ContentstackModels", _tempDir);
            var content = ReadGenerated("IEmbeddedObjectConverter.cs");

            Assert.Contains("else if (ctUid == \"sys_assets\")", content);
            Assert.Contains("JsonSerializer.Deserialize<Asset>(obj.ToJsonString(), deserializeOptions);", content);
        }

        #endregion

        #region CreateModularBlockConverter

        [Fact]
        public void CreateModularBlockConverter_WritesConverterClassDeclaration()
        {
            var generator = CreateGenerator();
            var blockTypes = new Dictionary<string, string> { { "heading", "MBTestHeading" } };

            generator.CreateModularBlockConverter("ContentstackModels", "MBTest", blockTypes, _tempDir);
            var content = ReadGenerated("MBTestConverter.cs");

            Assert.Contains("[CSJsonConverter(\"MBTestConverter\")]", content);
            Assert.Contains("public partial class MBTestConverter : JsonConverter<MBTest>", content);
        }

        [Fact]
        public void CreateModularBlockConverter_PerBlockType_EmitsFieldExistsCheckAndBlockTypeAssignment()
        {
            var generator = CreateGenerator();
            var blockTypes = new Dictionary<string, string>
            {
                { "heading", "MBTestHeading" },
                { "quote", "MBTestQuote" }
            };

            generator.CreateModularBlockConverter("ContentstackModels", "MBTest", blockTypes, _tempDir);
            var content = ReadGenerated("MBTestConverter.cs");

            Assert.Contains("if (ContentstackHelper.FieldExists(ContentstackHelper.GetDescription(MBTestEnum.Heading), jObject))", content);
            Assert.Contains("block.BlockType = MBTestEnum.Heading;", content);
            Assert.Contains("if (ContentstackHelper.FieldExists(ContentstackHelper.GetDescription(MBTestEnum.Quote), jObject))", content);
            Assert.Contains("block.BlockType = MBTestEnum.Quote;", content);
        }

        [Fact]
        public void CreateModularBlockConverter_EmptyBlockTypes_EmitsOnlyFallbackReturn()
        {
            var generator = CreateGenerator();
            var blockTypes = new Dictionary<string, string>();

            generator.CreateModularBlockConverter("ContentstackModels", "MBTest", blockTypes, _tempDir);
            var content = ReadGenerated("MBTestConverter.cs");

            Assert.Contains("return new MBTest();", content);
            Assert.DoesNotContain("ContentstackHelper.FieldExists", content);
        }

        [Fact]
        public void CreateModularBlockConverter_Write_WrapsSerializeInNullCheck()
        {
            var generator = CreateGenerator();
            var blockTypes = new Dictionary<string, string> { { "heading", "MBTestHeading" } };

            generator.CreateModularBlockConverter("ContentstackModels", "MBTest", blockTypes, _tempDir);
            var content = ReadGenerated("MBTestConverter.cs");

            Assert.Contains("if (value != null)", content);
            Assert.Contains("JsonSerializer.Serialize(writer, value, options);", content);
        }

        #endregion

        public void Dispose()
        {
            try { Directory.Delete(_tempDir.FullName, recursive: true); } catch { /* best-effort cleanup */ }
        }
    }
}
