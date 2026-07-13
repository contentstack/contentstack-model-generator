using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using contentstack.CMA.Http;
using Xunit;

namespace contentstack.model.generator.tests
{
    /// <summary>
    /// Tests for contentstack.CMA.Http.ContentstackHttpRequest - part of an HTTP
    /// abstraction layer (CMA/Http/*) that is not currently wired into
    /// ContentstackClient. Its constructor takes an HttpClient directly (unlike
    /// CMA/HTTPRequestHandler.cs, which hardcodes `new HttpClient()`), so it can be
    /// fully tested by backing the HttpClient with a fake HttpMessageHandler - no
    /// real network call, no mocking framework needed.
    /// </summary>
    public class ContentstackHttpRequestTests
    {
        private class FakeHttpMessageHandler : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage, HttpResponseMessage> _responder;

            public HttpRequestMessage LastRequest { get; private set; }

            public FakeHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responder)
            {
                _responder = responder;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                LastRequest = request;
                return Task.FromResult(_responder(request));
            }
        }

        private static ContentstackHttpRequest CreateRequest(
            out FakeHttpMessageHandler handler,
            HttpStatusCode statusCode = HttpStatusCode.OK,
            string body = "{\"ok\":true}")
        {
            handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            });
            var httpClient = new HttpClient(handler);
            return new ContentstackHttpRequest(httpClient, new JsonSerializerOptions())
            {
                RequestUri = new Uri("http://localhost/test")
            };
        }

        [Fact]
        public void Constructor_ExposesUnderlyingHttpClient()
        {
            var httpClient = new HttpClient(new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)));

            var request = new ContentstackHttpRequest(httpClient, new JsonSerializerOptions());

            Assert.Same(httpClient, request.HttpClient);
        }

        [Fact]
        public void Method_Getter_ReflectsUnderlyingRequestMessage()
        {
            var httpClient = new HttpClient(new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)));
            var request = new ContentstackHttpRequest(httpClient, new JsonSerializerOptions())
            {
                Method = HttpMethod.Post
            };

            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.Equal(HttpMethod.Post, request.Request.Method);
        }

        [Fact]
        public async Task GetResponseAsync_SuccessStatusCode_ReturnsContentstackResponse()
        {
            var request = CreateRequest(out _, HttpStatusCode.OK, "{\"name\":\"test\"}");

            var response = await request.GetResponseAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("{\"name\":\"test\"}", response.OpenResponse());
        }

        [Fact]
        public async Task GetResponseAsync_SendsConfiguredRequestUri()
        {
            var request = CreateRequest(out var handler, HttpStatusCode.OK, "{}");

            await request.GetResponseAsync();

            Assert.Equal(new Uri("http://localhost/test"), handler.LastRequest.RequestUri);
        }

        [Fact]
        public async Task GetResponseAsync_AmbiguousRangeStatusCode_ReturnsResponseWithoutThrowing()
        {
            var request = CreateRequest(out _, HttpStatusCode.MultipleChoices, "{}");

            var response = await request.GetResponseAsync();

            Assert.Equal(HttpStatusCode.MultipleChoices, response.StatusCode);
        }

        [Fact]
        public async Task GetResponseAsync_ErrorStatusCode_ThrowsHttpRequestException()
        {
            var request = CreateRequest(out _, HttpStatusCode.NotFound, "{}");

            await Assert.ThrowsAsync<HttpRequestException>(() => request.GetResponseAsync());
        }

        [Fact]
        public void GetResponse_SuccessStatusCode_ReturnsContentstackResponse()
        {
            var request = CreateRequest(out _, HttpStatusCode.OK, "{}");

            var response = request.GetResponse();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public void GetResponse_ErrorStatusCode_UnwrapsAggregateExceptionAndThrowsHttpRequestException()
        {
            var request = CreateRequest(out _, HttpStatusCode.NotFound, "{}");

            Assert.Throws<HttpRequestException>(() => request.GetResponse());
        }

        [Fact]
        public void SetRequestHeaders_AddsHeadersToRequest()
        {
            var httpClient = new HttpClient(new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)));
            var request = new ContentstackHttpRequest(httpClient, new JsonSerializerOptions());

            request.SetRequestHeaders(new Dictionary<string, string> { { "api_key", "test_key" } });

            Assert.True(request.Request.Headers.TryGetValues("api_key", out var values));
            Assert.Contains("test_key", values);
        }

        [Fact]
        public void WriteToRequestBody_SetsRequestContent()
        {
            var httpClient = new HttpClient(new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)));
            var request = new ContentstackHttpRequest(httpClient, new JsonSerializerOptions());
            var content = new StringContent("{}");

            request.WriteToRequestBody(content);

            Assert.Same(content, request.GetRequestContent());
        }

        [Fact]
        public void WriteToRequestBody_WithContentHeaders_SetsContentTypeHeader()
        {
            var httpClient = new HttpClient(new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)));
            var request = new ContentstackHttpRequest(httpClient, new JsonSerializerOptions());
            var content = new StringContent("{}");

            request.WriteToRequestBody(content, new Dictionary<string, string> { { "Content-Type", "application/json" } });

            Assert.Equal("application/json", request.Request.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public void Dispose_IsIdempotent()
        {
            var httpClient = new HttpClient(new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)));
            var request = new ContentstackHttpRequest(httpClient, new JsonSerializerOptions());

            request.Dispose();
            var exception = Record.Exception(() => request.Dispose());

            Assert.Null(exception);
        }

        [Fact]
        public void SetRequestHeaders_AfterDispose_ThrowsObjectDisposedException()
        {
            var httpClient = new HttpClient(new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)));
            var request = new ContentstackHttpRequest(httpClient, new JsonSerializerOptions());
            request.Dispose();

            Assert.Throws<ObjectDisposedException>(() => request.SetRequestHeaders(new Dictionary<string, string>()));
        }
    }
}
