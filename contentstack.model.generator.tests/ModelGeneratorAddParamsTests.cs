using System.Collections.Generic;
using System.Text;
using contentstack.model.generator.Model;
using Xunit;
using static contentstack.model.generator.tests.ModelGeneratorTestHelpers;

namespace contentstack.model.generator.tests
{
    public class ModelGeneratorAddParamsTests
    {
        #region Attribute + type emission per field type

        [Fact]
        public void AddParams_TextField_EmitsJsonPropertyNameAndStringProperty()
        {
            var generator = CreateGenerator();
            var schema = new List<Field> { TextField("title") };
            var sb = new StringBuilder();

            generator.AddParams("Blog", schema, sb);
            var content = sb.ToString();

            Assert.Contains("[JsonPropertyName(\"title\")]", content);
            Assert.Contains("public string Title { get; set; }", content);
        }

        [Fact]
        public void AddParams_NumberField_EmitsDoubleProperty()
        {
            var generator = CreateGenerator();
            var schema = new List<Field> { NumberField("views") };
            var sb = new StringBuilder();

            generator.AddParams("Blog", schema, sb);
            var content = sb.ToString();

            Assert.Contains("[JsonPropertyName(\"views\")]", content);
            Assert.Contains("public double Views { get; set; }", content);
        }

        [Fact]
        public void AddParams_BooleanField_EmitsBoolProperty()
        {
            var generator = CreateGenerator();
            var schema = new List<Field> { BooleanField("published") };
            var sb = new StringBuilder();

            generator.AddParams("Blog", schema, sb);
            var content = sb.ToString();

            Assert.Contains("public bool Published { get; set; }", content);
        }

        [Fact]
        public void AddParams_DateField_EmitsDateTimeProperty()
        {
            var generator = CreateGenerator();
            var schema = new List<Field> { DateField("published_on") };
            var sb = new StringBuilder();

            generator.AddParams("Blog", schema, sb);
            var content = sb.ToString();

            Assert.Contains("public DateTime PublishedOn { get; set; }", content);
        }

        [Fact]
        public void AddParams_FileField_EmitsAssetProperty()
        {
            var generator = CreateGenerator();
            var schema = new List<Field> { FileField("cover_image") };
            var sb = new StringBuilder();

            generator.AddParams("Blog", schema, sb);
            var content = sb.ToString();

            Assert.Contains("public Asset CoverImage { get; set; }", content);
        }

        [Fact]
        public void AddParams_LinkField_EmitsContentstackLinkProperty()
        {
            var generator = CreateGenerator();
            var schema = new List<Field> { LinkTypeField("cta") };
            var sb = new StringBuilder();

            generator.AddParams("Blog", schema, sb);
            var content = sb.ToString();

            Assert.Contains("public ContentstackLink Cta { get; set; }", content);
        }

        #endregion

        #region Reference / group / blocks

        [Fact]
        public void AddParams_ReferenceFieldSingle_EmitsResolvedClassNameProperty()
        {
            var generator = CreateGenerator(seedContentTypes: new[] { MakeContentType("blog_post", "Blog Post") });
            var schema = new List<Field> { ReferenceField("related", SingleRef("blog_post")) };
            var sb = new StringBuilder();

            generator.AddParams("Blog", schema, sb);
            var content = sb.ToString();

            Assert.Contains("public BlogPost Related { get; set; }", content);
        }

        [Fact]
        public void AddParams_ReferenceFieldMultipleContentTypes_EmitsObjectProperty()
        {
            var generator = CreateGenerator(seedContentTypes: new[] { MakeContentType("blog_post", "Blog Post") });
            var schema = new List<Field> { ReferenceField("related", SingleRef("blog_post"), refMultipleContentType: true) };
            var sb = new StringBuilder();

            generator.AddParams("Blog", schema, sb);
            var content = sb.ToString();

            Assert.Contains("public object Related { get; set; }", content);
        }

        [Fact]
        public void AddParams_GroupField_EmitsGroupPrefixedTypeProperty()
        {
            var generator = CreateGenerator();
            var schema = new List<Field> { GroupField("meta", "Meta", new List<Field>()) };
            var sb = new StringBuilder();

            generator.AddParams("Blog", schema, sb);
            var content = sb.ToString();

            Assert.Contains("public GroupBlogMeta Meta { get; set; }", content);
        }

        [Fact]
        public void AddParams_BlocksField_EmitsModularBlockPrefixedTypeProperty()
        {
            var generator = CreateGenerator();
            var schema = new List<Field> { BlocksField("sections", "Sections", new List<Contenttype>()) };
            var sb = new StringBuilder();

            generator.AddParams("Blog", schema, sb);
            var content = sb.ToString();

            Assert.Contains("public MBBlogSections Sections { get; set; }", content);
        }

        #endregion

        #region Markdown special case

        [Fact]
        public void AddParams_MarkdownTextField_EmitsGetSetHtmlConversionAndBackingStoreField()
        {
            var generator = CreateGenerator();
            var schema = new List<Field> { TextField("body", isMarkdown: true) };
            var sb = new StringBuilder();

            generator.AddParams("Blog", schema, sb);
            var content = sb.ToString();

            Assert.Contains("public string Body {", content);
            Assert.Contains("this.BodyStore = value;", content);
            Assert.Contains("return this.BodyStore.ToHtml();", content);
            Assert.Contains("private string BodyStore = \"\";", content);
        }

        [Fact]
        public void AddParams_MarkdownTextField_WithIsMultiple_UsesToListHtml()
        {
            var generator = CreateGenerator();
            var schema = new List<Field> { TextField("body", isMarkdown: true, isMultiple: true) };
            var sb = new StringBuilder();

            generator.AddParams("Blog", schema, sb);
            var content = sb.ToString();

            Assert.Contains("return this.BodyStore.ToListHtml();", content);
        }

        [Fact]
        public void AddParams_MarkdownTextField_PropertyDeclaration_NeverHasNullableSuffix()
        {
            var generator = CreateGenerator(isNullable: true);
            var schema = new List<Field> { TextField("body", isMarkdown: true) };
            var sb = new StringBuilder();

            generator.AddParams("Blog", schema, sb);
            var content = sb.ToString();

            Assert.Contains("public string Body {", content);
            Assert.DoesNotContain("public string? Body {", content);
        }

        #endregion

        #region Multiplicity + ordering

        [Fact]
        public void AddParams_MultipleValueField_WrapsPropertyTypeInList()
        {
            var generator = CreateGenerator();
            var schema = new List<Field> { NumberField("scores", isMultiple: true) };
            var sb = new StringBuilder();

            generator.AddParams("Blog", schema, sb);
            var content = sb.ToString();

            Assert.Contains("public List<double> Scores { get; set; }", content);
        }

        [Fact]
        public void AddParams_MultipleFields_PreservesSchemaOrderInOutput()
        {
            var generator = CreateGenerator();
            var schema = new List<Field> { TextField("a"), NumberField("b"), BooleanField("c") };
            var sb = new StringBuilder();

            generator.AddParams("Blog", schema, sb);
            var content = sb.ToString();

            var indexA = content.IndexOf("[JsonPropertyName(\"a\")]");
            var indexB = content.IndexOf("[JsonPropertyName(\"b\")]");
            var indexC = content.IndexOf("[JsonPropertyName(\"c\")]");

            Assert.True(indexA >= 0 && indexB >= 0 && indexC >= 0);
            Assert.True(indexA < indexB);
            Assert.True(indexB < indexC);
        }

        #endregion
    }
}
