using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Moq;
using contentstack.CMA;

namespace contentstack.model.generator.tests
{
    public class HTTPRequestHandlerTests : IDisposable
    {
        private readonly HttpRequestHandler _handler;

        public HTTPRequestHandlerTests()
        {
            _handler = new HttpRequestHandler();
        }

        [Fact]
        public void Constructor_ShouldInitializeHttpClient()
        {
            var handler = new HttpRequestHandler();
            Assert.NotNull(handler);
        }

        [Fact]
        public async Task ProcessRequest_ShouldHandleOAuthBearerToken()
        {
            
            var url = "https://api.contentstack.io/v3/stacks";
            var headers = new Dictionary<string, object>
            {
                { "Authorization", "Bearer test_access_token_123" },
                { "api_key", "test_api_key" }
            };
            var bodyJson = new Dictionary<string, object>
            {
                { "param1", "value1" },
                { "param2", "value2" }
            };

            await Assert.ThrowsAsync<HttpRequestException>(() => 
                _handler.ProcessRequest(url, headers, bodyJson));
        }

        [Fact]
        public async Task ProcessRequest_ShouldHandleBearerTokenWithoutPrefix()
        {
            
            var url = "https://api.contentstack.io/v3/stacks";
            var headers = new Dictionary<string, object>
            {
                { "Authorization", "test_access_token_123" }, // No "Bearer " prefix
                { "api_key", "test_api_key" }
            };
            var bodyJson = new Dictionary<string, object>
            {
                { "param1", "value1" }
            };

            await Assert.ThrowsAsync<HttpRequestException>(() => 
                _handler.ProcessRequest(url, headers, bodyJson));
        }

        [Fact]
        public async Task ProcessRequest_ShouldHandleEmptyHeaders()
        {
            
            var url = "https://api.contentstack.io/v3/stacks";
            var headers = new Dictionary<string, object>();
            var bodyJson = new Dictionary<string, object>
            {
                { "param1", "value1" }
            };

            await Assert.ThrowsAsync<HttpRequestException>(() => 
                _handler.ProcessRequest(url, headers, bodyJson));
        }

        [Fact]
        public async Task ProcessRequest_ShouldHandleNullHeaders()
        {
            
            var url = "https://api.contentstack.io/v3/stacks";
            Dictionary<string, object> headers = null;
            var bodyJson = new Dictionary<string, object>
            {
                { "param1", "value1" }
            };

            await Assert.ThrowsAsync<HttpRequestException>(() => 
                _handler.ProcessRequest(url, headers, bodyJson));
        }

        [Fact]
        public async Task ProcessRequest_ShouldHandleStringArrayValues()
        {
            
            var url = "https://api.contentstack.io/v3/stacks";
            var headers = new Dictionary<string, object>
            {
                { "api_key", "test_api_key" }
            };
            var bodyJson = new Dictionary<string, object>
            {
                { "array_param", new[] { "value1", "value2", "value3" } }
            };

            await Assert.ThrowsAsync<HttpRequestException>(() => 
                _handler.ProcessRequest(url, headers, bodyJson));
        }

        [Fact]
        public async Task ProcessRequest_ShouldHandleDictionaryValues()
        {
            
            var url = "https://api.contentstack.io/v3/stacks";
            var headers = new Dictionary<string, object>
            {
                { "api_key", "test_api_key" }
            };
            var bodyJson = new Dictionary<string, object>
            {
                { "dict_param", new Dictionary<string, object> { { "key1", "value1" } } }
            };

            await Assert.ThrowsAsync<HttpRequestException>(() => 
                _handler.ProcessRequest(url, headers, bodyJson));
        }

        [Fact]
        public async Task ProcessRequest_ShouldHandleEmptyBodyJson()
        {
            
            var url = "https://api.contentstack.io/v3/stacks";
            var headers = new Dictionary<string, object>
            {
                { "api_key", "test_api_key" }
            };
            var bodyJson = new Dictionary<string, object>();

            await Assert.ThrowsAsync<HttpRequestException>(() => 
                _handler.ProcessRequest(url, headers, bodyJson));
        }

        [Fact]
        public async Task ProcessRequest_ShouldHandleNullBodyJson()
        {
            
            var url = "https://api.contentstack.io/v3/stacks";
            var headers = new Dictionary<string, object>
            {
                { "api_key", "test_api_key" }
            };
            Dictionary<string, object> bodyJson = null;

            await Assert.ThrowsAsync<ArgumentNullException>(() => 
                _handler.ProcessRequest(url, headers, bodyJson));
        }

        [Fact]
        public async Task ProcessRequest_ShouldHandleInvalidUrl()
        {
            
            var url = "invalid-url";
            var headers = new Dictionary<string, object>
            {
                { "api_key", "test_api_key" }
            };
            var bodyJson = new Dictionary<string, object>
            {
                { "param1", "value1" }
            };

            await Assert.ThrowsAsync<UriFormatException>(() => 
                _handler.ProcessRequest(url, headers, bodyJson));
        }

        [Fact]
        public async Task ProcessRequest_ShouldHandleSpecialCharactersInParameters()
        {
            
            var url = "https://api.contentstack.io/v3/stacks";
            var headers = new Dictionary<string, object>
            {
                { "api_key", "test_api_key" }
            };
            var bodyJson = new Dictionary<string, object>
            {
                { "param with spaces", "value with spaces" },
                { "param&with&special", "value&with&special" },
                { "param=with=equals", "value=with=equals" }
            };

            await Assert.ThrowsAsync<HttpRequestException>(() => 
                _handler.ProcessRequest(url, headers, bodyJson));
        }

        [Fact]
        public void Dispose_ShouldNotThrow()
        {
            var exception = Record.Exception(() => _handler.Dispose());
            Assert.Null(exception);
        }

        [Fact]
        public void Dispose_ShouldBeIdempotent()
        {
            _handler.Dispose();
            var exception = Record.Exception(() => _handler.Dispose());
            Assert.Null(exception);
        }

        public void Dispose()
        {
            _handler?.Dispose();
        }
    }
}
