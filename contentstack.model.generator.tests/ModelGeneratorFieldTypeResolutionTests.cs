using System;
using contentstack.model.generator.Model;
using Xunit;
using static contentstack.model.generator.tests.ModelGeneratorTestHelpers;

namespace contentstack.model.generator.tests
{
    public class ModelGeneratorFieldTypeResolutionTests
    {
        #region Scalar field types

        [Fact]
        public void GetDatatypeForField_TextField_ReturnsString()
        {
            var generator = CreateGenerator();
            var field = TextField("title");

            var result = generator.GetDatatypeForField(field, "Blog");

            Assert.Equal("string", result);
        }

        [Fact]
        public void GetDatatypeForField_NumberField_ReturnsDouble()
        {
            var generator = CreateGenerator();
            var field = NumberField("views");

            var result = generator.GetDatatypeForField(field, "Blog");

            Assert.Equal("double", result);
        }

        [Fact]
        public void GetDatatypeForField_BooleanField_ReturnsBool()
        {
            var generator = CreateGenerator();
            var field = BooleanField("published");

            var result = generator.GetDatatypeForField(field, "Blog");

            Assert.Equal("bool", result);
        }

        [Fact]
        public void GetDatatypeForField_DateField_ReturnsDateTime()
        {
            var generator = CreateGenerator();
            var field = DateField("published_on");

            var result = generator.GetDatatypeForField(field, "Blog");

            Assert.Equal("DateTime", result);
        }

        [Fact]
        public void GetDatatypeForField_FileField_ReturnsAsset()
        {
            var generator = CreateGenerator();
            var field = FileField("cover_image");

            var result = generator.GetDatatypeForField(field, "Blog");

            Assert.Equal("Asset", result);
        }

        [Fact]
        public void GetDatatypeForField_LinkField_ReturnsContentstackLink()
        {
            var generator = CreateGenerator();
            var field = LinkTypeField("cta");

            var result = generator.GetDatatypeForField(field, "Blog");

            Assert.Equal("ContentstackLink", result);
        }

        [Fact]
        public void GetDatatypeForField_JsonFieldWithJsonRte_ReturnsNode()
        {
            var generator = CreateGenerator();
            var field = JsonField("rich_text", isJsonRTE: true);

            var result = generator.GetDatatypeForField(field, "Blog");

            Assert.Equal("Node", result);
        }

        [Fact]
        public void GetDatatypeForField_JsonFieldWithoutJsonRte_ReturnsDynamic()
        {
            var generator = CreateGenerator();
            var field = JsonField("raw_json", isJsonRTE: false);

            var result = generator.GetDatatypeForField(field, "Blog");

            Assert.Equal("dynamic", result);
        }

        [Fact]
        public void GetDatatypeForField_UnknownDataType_ReturnsObject()
        {
            var generator = CreateGenerator();
            var field = UnknownTypeField("mystery");

            var result = generator.GetDatatypeForField(field, "Blog");

            Assert.Equal("object", result);
        }

        #endregion

        #region Blocks / group naming

        [Fact]
        public void GetDatatypeForField_BlocksField_ReturnsModularBlockPrefixedName()
        {
            var generator = CreateGenerator();
            var field = BlocksField("sections", "Sections", new System.Collections.Generic.List<Contenttype>());

            var result = generator.GetDatatypeForField(field, "Blog");

            Assert.Equal("MBBlogSections", result);
        }

        [Fact]
        public void GetDatatypeForField_GroupField_ReturnsGroupPrefixedName()
        {
            var generator = CreateGenerator();
            var field = GroupField("meta", "Meta", new System.Collections.Generic.List<Field>());

            var result = generator.GetDatatypeForField(field, "Blog");

            Assert.Equal("GroupBlogMeta", result);
        }

        #endregion

        #region Reference / global_field resolution

        [Fact]
        public void GetDatatypeForField_ReferenceSingle_Resolved_ReturnsClassName()
        {
            var generator = CreateGenerator(seedContentTypes: new[] { MakeContentType("blog_post", "Blog Post") });
            var field = ReferenceField("related", SingleRef("blog_post"));

            var result = generator.GetDatatypeForField(field, "Blog");

            Assert.Equal("BlogPost", result);
        }

        [Fact]
        public void GetDatatypeForField_ReferenceSingle_Unresolved_ReturnsObject()
        {
            var generator = CreateGenerator();
            var field = ReferenceField("related", SingleRef("missing_uid"));

            var result = generator.GetDatatypeForField(field, "Blog");

            Assert.Equal("object", result);
        }

        [Fact]
        public void GetDatatypeForField_ReferenceSingle_WithRefMultipleContentTypesTrue_ReturnsObject()
        {
            var generator = CreateGenerator(seedContentTypes: new[] { MakeContentType("blog_post", "Blog Post") });
            var field = ReferenceField("related", SingleRef("blog_post"), refMultipleContentType: true);

            var result = generator.GetDatatypeForField(field, "Blog");

            Assert.Equal("object", result);
        }

        [Fact]
        public void GetDatatypeForField_ReferenceArrayOfOne_Resolved_ReturnsClassName()
        {
            var generator = CreateGenerator(seedContentTypes: new[] { MakeContentType("blog_post", "Blog Post") });
            var field = ReferenceField("related", MultiRef("blog_post"));

            var result = generator.GetDatatypeForField(field, "Blog");

            Assert.Equal("BlogPost", result);
        }

        [Fact]
        public void GetDatatypeForField_ReferenceArrayOfTwo_ReturnsObject()
        {
            var generator = CreateGenerator(seedContentTypes: new[] { MakeContentType("blog_post", "Blog Post") });
            var field = ReferenceField("related", MultiRef("blog_post", "author"));

            var result = generator.GetDatatypeForField(field, "Blog");

            Assert.Equal("object", result);
        }

        [Fact]
        public void GetDatatypeForField_GlobalField_UsesSameResolutionAsReference()
        {
            var generator = CreateGenerator(seedContentTypes: new[] { MakeContentType("blog_post", "Blog Post") });
            var field = GlobalFieldField("related", SingleRef("blog_post"));

            var result = generator.GetDatatypeForField(field, "Blog");

            Assert.Equal("BlogPost", result);
        }

        #endregion

        #region Multiplicity wrapping

        [Fact]
        public void GetDatatypeForField_Reference_StackVersionAtThreshold_AlwaysWrapsInList()
        {
            var generator = CreateGenerator(stackVersion: DateTime.Parse("2019-04-04"));
            var field = ReferenceField("related", SingleRef("missing_uid"));

            var result = generator.GetDatatypeForField(field, "Blog");

            Assert.Equal("List<object>", result);
        }

        [Fact]
        public void GetDatatypeForField_Reference_StackVersionBeforeThreshold_SingleValue_ReturnsUnwrapped()
        {
            var generator = CreateGenerator();
            var field = ReferenceField("related", SingleRef("missing_uid"));

            var result = generator.GetDatatypeForField(field, "Blog");

            Assert.Equal("object", result);
        }

        [Fact]
        public void GetDatatypeForField_GlobalField_StackVersionAfterThreshold_IsMultipleFalse_ReturnsUnwrapped()
        {
            var generator = CreateGenerator(stackVersion: DateTime.Parse("2019-04-04"));
            var field = GlobalFieldField("related", SingleRef("missing_uid"));

            var result = generator.GetDatatypeForField(field, "Blog");

            Assert.Equal("object", result);
        }

        [Fact]
        public void GetDatatypeForField_AnyFieldType_WithRefMultipleTrue_WrapsInList()
        {
            var generator = CreateGenerator();
            var field = TextField("tags");
            field.FieldMetadata.RefMultiple = true;

            var result = generator.GetDatatypeForField(field, "Blog");

            Assert.Equal("List<string>", result);
        }

        [Fact]
        public void GetDatatypeForField_AnyFieldType_WithIsMultipleTrue_WrapsInList()
        {
            var generator = CreateGenerator();
            var field = NumberField("scores", isMultiple: true);

            var result = generator.GetDatatypeForField(field, "Blog");

            Assert.Equal("List<double>", result);
        }

        [Fact]
        public void GetDatatypeForField_ScalarField_NoMultiplicityFlags_ReturnsUnwrapped()
        {
            var generator = CreateGenerator();
            var field = NumberField("score");

            var result = generator.GetDatatypeForField(field, "Blog");

            Assert.Equal("double", result);
        }

        #endregion
    }
}
