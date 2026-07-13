using System;
using System.Collections.Generic;
using System.IO;
using contentstack.model.generator.Model;
using Xunit;
using static contentstack.model.generator.tests.ModelGeneratorTestHelpers;

namespace contentstack.model.generator.tests
{
    /// <summary>
    /// CreateFile() end-to-end: class shape, RTE-reference extends-clause toggle, nested
    /// blocks/group generation, using-directive emission, and the embedded-items
    /// interpolation-bug regression.
    ///
    /// Not covered (intentionally): the "file already exists" overwrite-prompt branch in
    /// shouldCreateFile() - with Force=false (the default) that calls Prompt.GetYesNo(),
    /// which blocks on console input and would hang a headless test run. Every test here
    /// uses a fresh unique temp directory so that branch is never reached.
    /// </summary>
    public class ModelGeneratorCreateFileTests : IDisposable
    {
        private readonly DirectoryInfo _tempDir;

        public ModelGeneratorCreateFileTests()
        {
            _tempDir = Directory.CreateTempSubdirectory("cmg_createfile_tests_");
        }

        private string ReadGenerated(string fileName) =>
            File.ReadAllText(Path.Combine(_tempDir.FullName, fileName));

        #region Class shape

        [Fact]
        public void CreateFile_SimpleContentType_WritesFileWithConstUidAndContentTypeUid()
        {
            var generator = CreateGenerator();
            var contentType = MakeContentType("blog", "Blog");

            generator.CreateFile("Blog", "ContentstackModels", contentType, _tempDir);
            var content = ReadGenerated("Blog.cs");

            Assert.Contains("public const string ContentType = \"blog\";", content);
            Assert.Contains("[JsonPropertyName(\"uid\")]", content);
            Assert.Contains("public string Uid { get; set; }", content);
            Assert.Contains("[JsonPropertyName(\"_content_type_uid\")]", content);
            Assert.Contains("public string ContentTypeUid { get; set; }", content);
        }

        [Fact]
        public void CreateFile_SchemaWithoutRteReference_ExtendsIEmbeddedObjectOnly()
        {
            var generator = CreateGenerator();
            var contentType = MakeContentType("blog", "Blog", new List<Field> { TextField("title") });

            generator.CreateFile("Blog", "ContentstackModels", contentType, _tempDir);
            var content = ReadGenerated("Blog.cs");

            Assert.Contains("public partial class Blog : IEmbeddedObject", content);
            Assert.DoesNotContain("IEntryEmbedable", content);
        }

        [Fact]
        public void CreateFile_SchemaWithRteReference_ExtendsIEntryEmbedableAndIEmbeddedObject()
        {
            var generator = CreateGenerator();
            var contentType = MakeContentType("blog", "Blog", new List<Field> { TextField("body", referenceTo: "some-rte-reference") });

            generator.CreateFile("Blog", "ContentstackModels", contentType, _tempDir);
            var content = ReadGenerated("Blog.cs");

            Assert.Contains("public partial class Blog : IEntryEmbedable, IEmbeddedObject", content);
        }

        [Fact]
        public void CreateFile_SchemaWithoutRteReference_OmitsEmbeddedItemsPropertyBlock()
        {
            var generator = CreateGenerator();
            var contentType = MakeContentType("blog", "Blog", new List<Field> { TextField("title") });

            generator.CreateFile("Blog", "ContentstackModels", contentType, _tempDir);
            var content = ReadGenerated("Blog.cs");

            Assert.DoesNotContain("embeddedItems", content);
        }

        #endregion

        #region Nested modular blocks

        [Fact]
        public void CreateFile_SchemaWithBlocksField_CreatesNestedModularBlockDirectoryAndFiles()
        {
            var generator = CreateGenerator();
            var blocks = new List<Contenttype> { MakeContentType("heading", "Heading") };
            var contentType = MakeContentType("blog", "Blog", new List<Field> { BlocksField("sections", "Sections", blocks) });

            generator.CreateFile("Blog", "ContentstackModels", contentType, _tempDir);
            var blocksDir = Path.Combine(_tempDir.FullName, "BlogBlocks");

            Assert.True(Directory.Exists(blocksDir));
            Assert.True(File.Exists(Path.Combine(blocksDir, "MBBlogSectionsHeading.cs")));
            Assert.True(File.Exists(Path.Combine(blocksDir, "MBBlogSectionsEnum.cs")));
            Assert.True(File.Exists(Path.Combine(blocksDir, "MBBlogSections.cs")));
            Assert.True(File.Exists(Path.Combine(blocksDir, "MBBlogSectionsConverter.cs")));
        }

        [Fact]
        public void CreateFile_SchemaWithBlocksField_AddsCorrectUsingDirectiveToTopLevelFile()
        {
            var generator = CreateGenerator();
            var blocks = new List<Contenttype> { MakeContentType("heading", "Heading") };
            var contentType = MakeContentType("blog", "Blog", new List<Field> { BlocksField("sections", "Sections", blocks) });

            generator.CreateFile("Blog", "ContentstackModels", contentType, _tempDir);
            var content = ReadGenerated("Blog.cs");

            Assert.Contains($"using ContentstackModels.{_tempDir.Name}.BlogBlocks;", content);
        }

        [Fact]
        public void CreateFile_SchemaWithEmptyBlocksList_StillCreatesConverterFile()
        {
            var generator = CreateGenerator();
            var contentType = MakeContentType("blog", "Blog", new List<Field> { BlocksField("sections", "Sections", new List<Contenttype>()) });

            generator.CreateFile("Blog", "ContentstackModels", contentType, _tempDir);
            var blocksDir = Path.Combine(_tempDir.FullName, "BlogBlocks");
            var converterContent = File.ReadAllText(Path.Combine(blocksDir, "MBBlogSectionsConverter.cs"));

            Assert.Contains("return new MBBlogSections();", converterContent);
            Assert.DoesNotContain("ContentstackHelper.FieldExists", converterContent);
            Assert.False(File.Exists(Path.Combine(blocksDir, "MBBlogSectionsEnum.cs")));
            Assert.False(File.Exists(Path.Combine(blocksDir, "MBBlogSections.cs")));
        }

        [Fact]
        public void CreateFile_SchemaWithBlocksFieldReferencingGlobalField_CreatesGlobalFieldModularFile()
        {
            var globalFieldSchema = new List<Field>
            {
                TextField("note"),
                BlocksField("sub_blocks", "SubBlocks", new List<Contenttype>()),
                GroupField("sub_group", "SubGroup", new List<Field>())
            };
            var generator = CreateGenerator(seedContentTypes: new[] { MakeContentType("global_heading", "Global Heading", globalFieldSchema) });
            var blocks = new List<Contenttype> { MakeContentType("heading", "Heading", referenceTo: "global_heading") };
            var contentType = MakeContentType("blog", "Blog", new List<Field> { BlocksField("sections", "Sections", blocks) });

            generator.CreateFile("Blog", "ContentstackModels", contentType, _tempDir);
            var blockFile = Path.Combine(_tempDir.FullName, "BlogBlocks", "MBBlogSectionsHeading.cs");

            Assert.True(File.Exists(blockFile));
            var content = File.ReadAllText(blockFile);
            Assert.Contains("[JsonPropertyName(\"note\")]", content);
            Assert.Contains("public string Note { get; set; }", content);
            Assert.Contains("using ContentstackModels.Models.GlobalHeadingBlocks;", content);
            Assert.Contains("using ContentstackModels.Models.GlobalHeadingGroup;", content);
        }

        [Fact]
        public void CreateFile_NestedBlocksWithinBlock_AddsUsingDirectiveForNestedModularBlocks()
        {
            var nestedBlocks = new List<Contenttype> { MakeContentType("nested_heading", "Nested Heading") };
            var innerSchema = new List<Field> { BlocksField("nested_sections", "NestedSections", nestedBlocks) };
            var blocks = new List<Contenttype> { MakeContentType("heading", "Heading", innerSchema) };
            var contentType = MakeContentType("blog", "Blog", new List<Field> { BlocksField("sections", "Sections", blocks) });
            var generator = CreateGenerator();

            generator.CreateFile("Blog", "ContentstackModels", contentType, _tempDir);
            var blocksDir = Path.Combine(_tempDir.FullName, "BlogBlocks");
            var blockFile = Path.Combine(blocksDir, "MBBlogSectionsHeading.cs");

            Assert.True(Directory.Exists(Path.Combine(blocksDir, "MBBlogSectionsHeadingBlocks")));
            Assert.True(File.Exists(blockFile));
            var content = File.ReadAllText(blockFile);
            Assert.Contains($"using ContentstackModels.{_tempDir.Name}.BlogBlocks.MBBlogSectionsHeadingBlocks;", content);
        }

        #endregion

        #region findRTEReference (direct)

        [Fact]
        public void FindRTEReference_NullSchema_ReturnsFalse()
        {
            var generator = CreateGenerator();

            var result = generator.findRTEReference(null);

            Assert.False(result);
        }

        [Fact]
        public void FindRTEReference_BlocksFieldWithNestedRteReference_ReturnsTrue()
        {
            var generator = CreateGenerator();
            var nestedSchema = new List<Field> { TextField("body", referenceTo: "some-rte-reference") };
            var blocks = new List<Contenttype> { MakeContentType("heading", "Heading", nestedSchema) };
            var schema = new List<Field> { BlocksField("sections", "Sections", blocks) };

            var result = generator.findRTEReference(schema);

            Assert.True(result);
        }

        [Fact]
        public void FindRTEReference_GroupFieldWithNullSchema_ReturnsFalse()
        {
            var generator = CreateGenerator();
            var schema = new List<Field> { GroupField("meta", "Meta", null) };

            var result = generator.findRTEReference(schema);

            Assert.False(result);
        }

        #endregion

        #region Nested groups

        [Fact]
        public void CreateFile_SchemaWithGroupField_CreatesNestedGroupDirectoryAndFile()
        {
            var generator = CreateGenerator();
            var contentType = MakeContentType("blog", "Blog", new List<Field> { GroupField("meta", "Meta", new List<Field> { TextField("note") }) });

            generator.CreateFile("Blog", "ContentstackModels", contentType, _tempDir);
            var groupDir = Path.Combine(_tempDir.FullName, "BlogGroup");
            var groupFile = Path.Combine(groupDir, "GroupBlogMeta.cs");

            Assert.True(Directory.Exists(groupDir));
            Assert.True(File.Exists(groupFile));

            var content = File.ReadAllText(groupFile);
            Assert.Contains("[JsonPropertyName(\"note\")]", content);
            Assert.Contains("public string Note { get; set; }", content);
        }

        [Fact]
        public void CreateFile_SchemaWithGroupField_AddsCorrectUsingDirectiveToTopLevelFile()
        {
            var generator = CreateGenerator();
            var contentType = MakeContentType("blog", "Blog", new List<Field> { GroupField("meta", "Meta", new List<Field> { TextField("note") }) });

            generator.CreateFile("Blog", "ContentstackModels", contentType, _tempDir);
            var content = ReadGenerated("Blog.cs");

            Assert.Contains($"using ContentstackModels.{_tempDir.Name}.BlogGroup;", content);
        }

        [Fact]
        public void CreateFile_GroupFieldContainingBlocksField_AddsModularUsingDirectiveInsideGroup()
        {
            var nestedBlocks = new List<Contenttype> { MakeContentType("sub_heading", "Sub Heading") };
            var groupSchema = new List<Field> { BlocksField("sub_sections", "SubSections", nestedBlocks) };
            var contentType = MakeContentType("blog", "Blog", new List<Field> { GroupField("meta", "Meta", groupSchema) });
            var generator = CreateGenerator();

            generator.CreateFile("Blog", "ContentstackModels", contentType, _tempDir);
            var groupFile = Path.Combine(_tempDir.FullName, "BlogGroup", "GroupBlogMeta.cs");

            Assert.True(File.Exists(groupFile));
            var content = File.ReadAllText(groupFile);
            Assert.Contains($"using ContentstackModels.{_tempDir.Name}.BlogGroup.GroupBlogMetaBlocks;", content);
        }

        #endregion

        #region Embedded-items interpolation bug regression

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void CreateFile_RteReferenceField_EmbeddedItemsLine_IsCorrectlyInterpolated_NotLiteralBraces(bool isNullable)
        {
            var generator = CreateGenerator(isNullable: isNullable);
            var contentType = MakeContentType("blog", "Blog", new List<Field> { TextField("body", referenceTo: "some-rte-reference") });

            generator.CreateFile("Blog", "ContentstackModels", contentType, _tempDir);
            var content = ReadGenerated("Blog.cs");

            Assert.DoesNotContain("{nullableString()}", content);
            var suffix = isNullable ? "?" : "";
            Assert.Contains($"public Dictionary<string, List<IEmbeddedObject>>{suffix} embeddedItems {{ get; set; }}", content);
        }

        #endregion

        public void Dispose()
        {
            try { Directory.Delete(_tempDir.FullName, recursive: true); } catch { /* best-effort cleanup */ }
        }
    }
}
