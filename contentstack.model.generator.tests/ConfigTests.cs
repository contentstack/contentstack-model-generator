using contentstack.CMA;
using Xunit;

namespace contentstack.model.generator.tests
{
    public class ConfigTests
    {
        [Fact]
        public void Port_DefaultValue_Is443()
        {
            var config = new Config();

            Assert.Equal("443", config.Port);
        }

        [Fact]
        public void Protocol_DefaultValue_IsHttps()
        {
            var config = new Config();

            Assert.Equal("https", config.Protocol);
        }

        [Fact]
        public void Host_DefaultValue_IsHostURL()
        {
            var config = new Config();

            Assert.Equal("cdn.contentstack.io", config.Host);
        }

        [Fact]
        public void Version_DefaultValue_IsV3()
        {
            var config = new Config();

            Assert.Equal("v3", config.Version);
        }

        [Fact]
        public void Port_Setter_OverridesDefaultValue()
        {
            var config = new Config { Port = "8080" };

            Assert.Equal("8080", config.Port);
        }

        [Fact]
        public void Host_Setter_OverridesDefaultValue()
        {
            var config = new Config { Host = "api.contentstack.io" };

            Assert.Equal("api.contentstack.io", config.Host);
        }

        [Fact]
        public void Version_Setter_OverridesDefaultValue()
        {
            var config = new Config { Version = "v4" };

            Assert.Equal("v4", config.Version);
        }

        [Fact]
        public void BaseUrl_WithDefaults_CombinesProtocolHostAndVersion()
        {
            var config = new Config();

            Assert.Equal("https://cdn.contentstack.io/v3", config.BaseUrl);
        }

        [Fact]
        public void BaseUrl_WithOverriddenHostAndVersion_ReflectsOverrides()
        {
            var config = new Config { Host = "api.contentstack.io", Version = "v4" };

            Assert.Equal("https://api.contentstack.io/v4", config.BaseUrl);
        }

        [Fact]
        public void BaseUrl_TrimsSlashesFromComponents()
        {
            var config = new Config { Host = "/api.contentstack.io/", Version = "/v3/" };

            Assert.Equal("https://api.contentstack.io/v3", config.BaseUrl);
        }

        [Fact]
        public void ApiKey_AppUid_AuthToken_CanBeSetAndRetrieved()
        {
            var config = new Config
            {
                ApiKey = "test_api_key",
                AppUid = "test_app_uid",
                AuthToken = "test_auth_token"
            };

            Assert.Equal("test_api_key", config.ApiKey);
            Assert.Equal("test_app_uid", config.AppUid);
            Assert.Equal("test_auth_token", config.AuthToken);
        }
    }
}
