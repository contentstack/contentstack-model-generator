using System;
using Contentstack.Model.Generator.Model;
using Xunit;

namespace contentstack.model.generator.tests
{
    public class StackResponseModelTests
    {
        [Fact]
        public void Properties_CanBeSetAndRetrieved()
        {
            var stack = new StackResponse
            {
                // deepcode ignore NoHardcodedCredentials: test fixture value, not a real secret
                APIKey = "test_api_key",
                Name = "My Stack",
                MasterLocale = "en-us",
                Uid = "stack_uid_123",
                Description = "A test stack"
            };

            Assert.Equal("test_api_key", stack.APIKey);
            Assert.Equal("My Stack", stack.Name);
            Assert.Equal("en-us", stack.MasterLocale);
            Assert.Equal("stack_uid_123", stack.Uid);
            Assert.Equal("A test stack", stack.Description);
        }

        [Fact]
        public void Settings_Version_CanBeSetAndRetrieved()
        {
            var version = new DateTime(2019, 4, 4);

            var settings = new StackSettings { version = version };

            Assert.Equal(version, settings.version);
        }
    }
}
