using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using contentstack.model.generator.Model;
using Xunit;
using static contentstack.model.generator.tests.ModelGeneratorTestHelpers;

namespace contentstack.model.generator.tests
{
    /// <summary>
    /// Toggles IsNullable (the --is-nullable / -N CLI flag) across the code-generation
    /// methods that append a nullable-reference suffix, and pins the one known asymmetry:
    /// CreateEmbeddedObjectClass()'s Read/Write signatures never take a nullable suffix.
    /// </summary>
    public class ModelGeneratorNullableVariantTests : IDisposable
    {
        private readonly DirectoryInfo _tempDir;

        public ModelGeneratorNullableVariantTests()
        {
            _tempDir = Directory.CreateTempSubdirectory("cmg_nullable_tests_");
        }

        [Theory]
        [InlineData(true, "?")]
        [InlineData(false, "")]
        public void AddParams_ScalarField_NullableSuffix_MatchesIsNullableSetting(bool isNullable, string suffix)
        {
            var generator = CreateGenerator(isNullable: isNullable);
            var schema = new List<Field> { TextField("title") };
            var sb = new StringBuilder();

            generator.AddParams("Blog", schema, sb);
            var content = sb.ToString();

            Assert.Contains($"public string{suffix} Title {{ get; set; }}", content);
        }

        [Theory]
        [InlineData(true, "?")]
        [InlineData(false, "")]
        public void CreateLinkClass_NullableSuffix_MatchesIsNullableSetting(bool isNullable, string suffix)
        {
            var generator = CreateGenerator(isNullable: isNullable);

            generator.CreateLinkClass("ContentstackModels", _tempDir);
            var content = File.ReadAllText(Path.Combine(_tempDir.FullName, "ContentstackLink.cs"));

            Assert.Contains($"public string{suffix} Title {{ get; set; }}", content);
            Assert.Contains($"public string{suffix} Href {{ get; set; }}", content);
        }

        [Theory]
        [InlineData(true, "?")]
        [InlineData(false, "")]
        public void CreateFile_UidAndContentTypeUid_NullableSuffix_MatchesIsNullableSetting(bool isNullable, string suffix)
        {
            var generator = CreateGenerator(isNullable: isNullable);
            var contentType = MakeContentType("blog", "Blog", new List<Field>());

            generator.CreateFile("Blog", "ContentstackModels", contentType, _tempDir);
            var content = File.ReadAllText(Path.Combine(_tempDir.FullName, "Blog.cs"));

            Assert.Contains($"public string{suffix} Uid {{ get; set; }}", content);
            Assert.Contains($"public string{suffix} ContentTypeUid {{ get; set; }}", content);
        }

        [Theory]
        [InlineData(true, "?")]
        [InlineData(false, "")]
        public void CreateFile_EmbeddedItemsProperty_NullableSuffix_MatchesIsNullableSetting(bool isNullable, string suffix)
        {
            var generator = CreateGenerator(isNullable: isNullable);
            var schema = new List<Field> { TextField("body", referenceTo: "some-rte-reference") };
            var contentType = MakeContentType("blog", "Blog", schema);

            generator.CreateFile("Blog", "ContentstackModels", contentType, _tempDir);
            var content = File.ReadAllText(Path.Combine(_tempDir.FullName, "Blog.cs"));

            Assert.Contains($"public Dictionary<string, List<IEmbeddedObject>>{suffix} embeddedItems {{ get; set; }}", content);
        }

        [Theory]
        [InlineData(true, "?")]
        [InlineData(false, "")]
        public void CreateModularBlockConverter_ReadWriteSignatures_NullableSuffix_MatchesIsNullableSetting(bool isNullable, string suffix)
        {
            var generator = CreateGenerator(isNullable: isNullable);
            var blockTypes = new Dictionary<string, string> { { "heading", "MBTestHeading" } };

            generator.CreateModularBlockConverter("ContentstackModels", "MBTest", blockTypes, _tempDir);
            var content = File.ReadAllText(Path.Combine(_tempDir.FullName, "MBTestConverter.cs"));

            Assert.Contains($"public override MBTest{suffix} Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)", content);
            Assert.Contains($"public override void Write(Utf8JsonWriter writer, MBTest{suffix} value, JsonSerializerOptions options)", content);
        }

        [Fact]
        public void CreateEmbeddedObjectClass_ReadWriteSignatures_NeverHaveNullableSuffix()
        {
            var generator = CreateGenerator(isNullable: true);

            generator.CreateEmbeddedObjectClass("ContentstackModels", _tempDir);
            var content = File.ReadAllText(Path.Combine(_tempDir.FullName, "IEmbeddedObjectConverter.cs"));

            Assert.Contains("public override List<IEmbeddedObject> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)", content);
            Assert.Contains("public override void Write(Utf8JsonWriter writer, List<IEmbeddedObject> value, JsonSerializerOptions options)", content);
            Assert.DoesNotContain("List<IEmbeddedObject>?", content);
        }

        public void Dispose()
        {
            try { Directory.Delete(_tempDir.FullName, recursive: true); } catch { /* best-effort cleanup */ }
        }
    }
}
