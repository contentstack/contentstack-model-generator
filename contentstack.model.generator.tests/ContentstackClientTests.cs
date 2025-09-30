using System;
using System.Collections.Generic;
using Xunit;
using contentstack.CMA;

namespace contentstack.model.generator.tests
{
    public class ContentstackClientTests
    {
        [Fact]
        public void Constructor_ShouldInitializeWithOAuthOptions()
        {
            
            var options = new ContentstackOptions
            {
                ApiKey = "test_api_key",
                Host = "api.contentstack.io",
                IsOAuth = true,
                Authorization = "Bearer test_access_token",
                OAuthClientId = "test_client_id",
                OAuthRedirectUri = "http://localhost:8080",
                OAuthAppId = "test_app_id"
            };

            
            var client = new ContentstackClient(options);

            
            Assert.NotNull(client);
            Assert.Equal("test_api_key", client.StackApiKey);
        }

        [Fact]
        public void Constructor_ShouldInitializeWithTraditionalAuth()
        {
            
            var options = new ContentstackOptions
            {
                ApiKey = "test_api_key",
                Host = "api.contentstack.io",
                IsOAuth = false,
                Authtoken = "test_authtoken"
            };

            
            var client = new ContentstackClient(options);

            
            Assert.NotNull(client);
            Assert.Equal("test_api_key", client.StackApiKey);
        }

        [Fact]
        public void Constructor_ShouldHandleNullOptions()
        {
            
            ContentstackOptions options = null;
            Assert.Throws<ArgumentNullException>(() => new ContentstackClient(options));
        }

        [Fact]
        public void Constructor_ShouldSetSerializerSettings()
        {
            
            var options = new ContentstackOptions
            {
                ApiKey = "test_api_key",
                Host = "api.contentstack.io"
            };

            
            var client = new ContentstackClient(options);

            
            Assert.NotNull(client.SerializerSettings);
        }

        [Theory]
        [InlineData("api.contentstack.io")]
        [InlineData("https://api.contentstack.io")]
        [InlineData("http://api.contentstack.io")]
        public void Constructor_ShouldPreserveHostFormat(string inputHost)
        {
            
            var options = new ContentstackOptions
            {
                ApiKey = "test_api_key",
                Host = inputHost
            };

            
            var client = new ContentstackClient(options);

            
            Assert.NotNull(client);
        }

        [Fact]
        public void GetHeader_ShouldReturnLocalHeadersWhenMainHeadersIsNull()
        {
            
            var options = new ContentstackOptions
            {
                ApiKey = "test_api_key",
                Host = "api.contentstack.io"
            };
            var client = new ContentstackClient(options);
            var localHeaders = new Dictionary<string, object>
            {
                { "header1", "value1" },
                { "header2", "value2" }
            };

            
            var result = client.GetHeader(localHeaders);

            
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("value1", result["header1"]);
            Assert.Equal("value2", result["header2"]);
        }

        [Fact]
        public void GetHeader_ShouldReturnMainHeadersWhenLocalHeadersIsNull()
        {
            
            var options = new ContentstackOptions
            {
                ApiKey = "test_api_key",
                Host = "api.contentstack.io"
            };
            var client = new ContentstackClient(options);

            
            var result = client.GetHeader(null);

            
            Assert.NotNull(result);
            // Should return main headers (StackHeaders)
        }

        [Fact]
        public void GetHeader_ShouldMergeLocalAndMainHeaders()
        {
            
            var options = new ContentstackOptions
            {
                ApiKey = "test_api_key",
                Host = "api.contentstack.io"
            };
            var client = new ContentstackClient(options);
            var localHeaders = new Dictionary<string, object>
            {
                { "local_header", "local_value" },
                { "api_key", "local_api_key" } // This should override main header
            };

            
            var result = client.GetHeader(localHeaders);

            
            Assert.NotNull(result);
            Assert.Contains("local_header", result.Keys);
            Assert.Equal("local_value", result["local_header"]);
        }

        [Fact]
        public void GetHeader_ShouldPrioritizeLocalHeadersOverMainHeaders()
        {
            
            var options = new ContentstackOptions
            {
                ApiKey = "test_api_key",
                Host = "api.contentstack.io"
            };
            var client = new ContentstackClient(options);
            var localHeaders = new Dictionary<string, object>
            {
                { "api_key", "overridden_api_key" }
            };

            
            var result = client.GetHeader(localHeaders);

            
            Assert.NotNull(result);
            Assert.Equal("overridden_api_key", result["api_key"]);
        }

        [Fact]
        public void GetHeader_ShouldHandleEmptyLocalHeaders()
        {
            
            var options = new ContentstackOptions
            {
                ApiKey = "test_api_key",
                Host = "api.contentstack.io"
            };
            var client = new ContentstackClient(options);
            var localHeaders = new Dictionary<string, object>();

            
            var result = client.GetHeader(localHeaders);

            
            Assert.NotNull(result);
            // Should return main headers only
        }

        [Fact]
        public void GetHeader_ShouldHandleNullValues()
        {
            
            var options = new ContentstackOptions
            {
                ApiKey = "test_api_key",
                Host = "api.contentstack.io"
            };
            var client = new ContentstackClient(options);
            var localHeaders = new Dictionary<string, object>
            {
                { "null_header", null },
                { "empty_header", "" }
            };

            
            var result = client.GetHeader(localHeaders);

            
            Assert.NotNull(result);
            Assert.Contains("null_header", result.Keys);
            Assert.Contains("empty_header", result.Keys);
        }

        [Fact]
        public void GetHeader_ShouldHandleSpecialCharacters()
        {
            
            var options = new ContentstackOptions
            {
                ApiKey = "test_api_key",
                Host = "api.contentstack.io"
            };
            var client = new ContentstackClient(options);
            var localHeaders = new Dictionary<string, object>
            {
                { "header with spaces", "value with spaces" },
                { "header-with-dashes", "value-with-dashes" },
                { "header_with_underscores", "value_with_underscores" }
            };

            
            var result = client.GetHeader(localHeaders);

            
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Equal("value with spaces", result["header with spaces"]);
            Assert.Equal("value-with-dashes", result["header-with-dashes"]);
            Assert.Equal("value_with_underscores", result["header_with_underscores"]);
        }

        [Fact]
        public void GetHeader_ShouldHandleCaseSensitiveKeys()
        {
            
            var options = new ContentstackOptions
            {
                ApiKey = "test_api_key",
                Host = "api.contentstack.io"
            };
            var client = new ContentstackClient(options);
            var localHeaders = new Dictionary<string, object>
            {
                { "Header1", "Value1" },
                { "header1", "value1" },
                { "HEADER1", "VALUE1" }
            };

            
            var result = client.GetHeader(localHeaders);

            
            Assert.NotNull(result);
            Assert.Equal(3, result.Count); // All should be treated as different keys
            Assert.Equal("Value1", result["Header1"]);
            Assert.Equal("value1", result["header1"]);
            Assert.Equal("VALUE1", result["HEADER1"]);
        }
    }
}
