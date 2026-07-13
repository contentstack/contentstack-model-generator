using System.Collections.Generic;
using Contentstack.Model.Generator.Model;
using contentstack.model.generator.Model;
using Xunit;

namespace contentstack.model.generator.tests
{
    /// <summary>
    /// Tests for Contentstack.Model.Generator.Model.ContentstackResponse - the DTO
    /// ContentstackClient.GetContentTypes()/GetGlobalFields() deserialize into. Tested
    /// directly against the type rather than through ContentstackClient, since the
    /// latter requires a real network call.
    /// </summary>
    public class ContentstackResponseModelTests
    {
        [Fact]
        public void DefaultConstructor_CreatesInstance()
        {
            var response = new ContentstackResponse();

            Assert.NotNull(response);
        }

        [Fact]
        public void ListContentTypes_CanBeSetAndRetrieved()
        {
            var list = new List<Contenttype> { new Contenttype { Uid = "blog" } };

            var response = new ContentstackResponse { listContentTypes = list };

            Assert.Same(list, response.listContentTypes);
        }

        [Fact]
        public void Count_CanBeSetAndRetrieved()
        {
            var response = new ContentstackResponse { Count = 42 };

            Assert.Equal(42, response.Count);
        }
    }
}
