using contentstack.CMA;
using Xunit;

namespace contentstack.model.generator.tests
{
    /// <summary>
    /// ContentstackConstants has a private constructor - the only way to obtain an
    /// instance (even from this InternalsVisibleTo test assembly) is via the internal
    /// static Instance property.
    /// </summary>
    public class ContentstackConstantsTests
    {
        [Fact]
        public void Instance_ReturnsNonNullInstance()
        {
            var instance = ContentstackConstants.Instance;

            Assert.NotNull(instance);
        }

        [Fact]
        public void Instance_EachAccess_ReturnsNewInstance()
        {
            var first = ContentstackConstants.Instance;
            var second = ContentstackConstants.Instance;

            Assert.NotSame(first, second);
        }

        [Fact]
        public void ContentTypeUid_CanBeSetAndRetrieved()
        {
            var instance = ContentstackConstants.Instance;

            instance.ContentTypeUid = "blog_post";

            Assert.Equal("blog_post", instance.ContentTypeUid);
        }

        [Fact]
        public void EntryUid_CanBeSetAndRetrieved()
        {
            var instance = ContentstackConstants.Instance;

            instance.EntryUid = "entry_123";

            Assert.Equal("entry_123", instance.EntryUid);
        }

        [Fact]
        public void Content_Types_DefaultValue_ReturnsContentTypes()
        {
            var instance = ContentstackConstants.Instance;

            Assert.Equal("content_types", instance.Content_Types);
        }

        [Fact]
        public void Content_Types_Setter_OverridesValue()
        {
            var instance = ContentstackConstants.Instance;

            instance.Content_Types = "custom_content_types";

            Assert.Equal("custom_content_types", instance.Content_Types);
        }

        [Fact]
        public void Entries_DefaultValue_ReturnsContentTypesDueToSharedBackingField()
        {
            // Content_Types and Entries read/write the SAME private backing field
            // (initialized to "content_types"), so Entries' "?? \"entries\"" fallback
            // is unreachable on a fresh instance - this pins current behavior.
            var instance = ContentstackConstants.Instance;

            Assert.Equal("content_types", instance.Entries);
        }

        [Fact]
        public void Entries_Setter_AlsoChangesContentTypesValue()
        {
            var instance = ContentstackConstants.Instance;

            instance.Entries = "entries";

            Assert.Equal("entries", instance.Entries);
            Assert.Equal("entries", instance.Content_Types);
        }
    }
}
