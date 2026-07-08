using contentstack.model.generator.Model;
using Xunit;

namespace contentstack.model.generator.tests
{
    public class MetaDataModelTests
    {
        [Fact]
        public void DefaultValue_CanBeSetAndRetrieved()
        {
            var metaData = new MetaData { DefaultValue = "fallback" };

            Assert.Equal("fallback", metaData.DefaultValue);
        }

        [Fact]
        public void IsRichText_CanBeSetAndRetrieved()
        {
            var metaData = new MetaData { IsRichText = true };

            Assert.True(metaData.IsRichText);
        }

        [Fact]
        public void IsExtension_CanBeSetAndRetrieved()
        {
            var metaData = new MetaData { IsExtension = true };

            Assert.True(metaData.IsExtension);
        }
    }
}
