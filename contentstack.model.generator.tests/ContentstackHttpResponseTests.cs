using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using contentstack.CMA.Http;
using Xunit;

namespace contentstack.model.generator.tests
{
    /// <summary>
    /// Tests for contentstack.CMA.Http.ContentstackResponse - part of an HTTP
    /// abstraction layer (CMA/Http/*) that is not currently wired into
    /// ContentstackClient (which still uses CMA/HTTPRequestHandler.cs). Its
    /// constructor takes a plain HttpResponseMessage, so it's fully testable in
    /// isolation without any real network call.
    /// </summary>
    public class ContentstackHttpResponseTests
    {
        private class SamplePayload
        {
            public string Name { get; set; }
        }

        private static HttpResponseMessage CreateResponse(
            HttpStatusCode statusCode = HttpStatusCode.OK,
            string body = "{\"name\":\"test\"}",
            string mediaType = "application/json")
        {
            var response = new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(body, Encoding.UTF8, mediaType)
            };
            response.Headers.Add("X-Custom-Header", "custom-value");
            return response;
        }

        [Fact]
        public void Constructor_SetsStatusCodeAndSuccessFlag()
        {
            var response = new ContentstackResponse(CreateResponse(HttpStatusCode.OK), new JsonSerializerOptions());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public void Constructor_WithErrorStatusCode_SetsSuccessFlagFalse()
        {
            var response = new ContentstackResponse(CreateResponse(HttpStatusCode.NotFound), new JsonSerializerOptions());

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.False(response.IsSuccessStatusCode);
        }

        [Fact]
        public void Constructor_SetsContentTypeAndContentLength()
        {
            var response = new ContentstackResponse(CreateResponse(body: "{\"a\":1}"), new JsonSerializerOptions());

            Assert.Equal("application/json", response.ContentType);
            Assert.Equal(Encoding.UTF8.GetByteCount("{\"a\":1}"), response.ContentLength);
        }

        [Fact]
        public void ResponseBody_ReturnsUnderlyingHttpResponseMessage()
        {
            var httpResponse = CreateResponse();

            var response = new ContentstackResponse(httpResponse, new JsonSerializerOptions());

            Assert.Same(httpResponse, response.ResponseBody);
        }

        [Fact]
        public void GetHeaderNames_IncludesResponseAndContentHeaders()
        {
            var response = new ContentstackResponse(CreateResponse(), new JsonSerializerOptions());

            var headerNames = response.GetHeaderNames();

            Assert.Contains("X-Custom-Header", headerNames);
            Assert.Contains("Content-Type", headerNames);
        }

        [Fact]
        public void GetHeaderValue_ReturnsMatchingHeaderValue()
        {
            var response = new ContentstackResponse(CreateResponse(), new JsonSerializerOptions());

            Assert.Equal("custom-value", response.GetHeaderValue("X-Custom-Header"));
        }

        [Fact]
        public void GetHeaderValue_MissingHeader_ReturnsEmptyString()
        {
            var response = new ContentstackResponse(CreateResponse(), new JsonSerializerOptions());

            Assert.Equal(string.Empty, response.GetHeaderValue("Does-Not-Exist"));
        }

        [Fact]
        public void IsHeaderPresent_ForExistingHeader_ReturnsTrue()
        {
            var response = new ContentstackResponse(CreateResponse(), new JsonSerializerOptions());

            Assert.True(response.IsHeaderPresent("X-Custom-Header"));
        }

        [Fact]
        public void IsHeaderPresent_ForMissingHeader_ReturnsFalse()
        {
            var response = new ContentstackResponse(CreateResponse(), new JsonSerializerOptions());

            Assert.False(response.IsHeaderPresent("Does-Not-Exist"));
        }

        [Fact]
        public void OpenResponse_ReturnsRawBody()
        {
            var response = new ContentstackResponse(CreateResponse(body: "{\"name\":\"test\"}"), new JsonSerializerOptions());

            Assert.Equal("{\"name\":\"test\"}", response.OpenResponse());
        }

        [Fact]
        public void OpenJsonObjectResponse_ParsesBodyIntoJsonObject()
        {
            var response = new ContentstackResponse(CreateResponse(body: "{\"name\":\"test\"}"), new JsonSerializerOptions());

            var jObject = response.OpenJsonObjectResponse();

            Assert.Equal("test", jObject["name"].GetValue<string>());
        }

        [Fact]
        public void OpenTResponse_DeserializesBodyIntoRequestedType()
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var response = new ContentstackResponse(CreateResponse(body: "{\"name\":\"test\"}"), options);

            var result = response.OpenTResponse<SamplePayload>();

            Assert.Equal("test", result.Name);
        }

        [Fact]
        public void Dispose_IsIdempotent()
        {
            var response = new ContentstackResponse(CreateResponse(), new JsonSerializerOptions());

            response.Dispose();
            var exception = Record.Exception(() => response.Dispose());

            Assert.Null(exception);
        }

        [Fact]
        public void OpenResponse_AfterDispose_ThrowsObjectDisposedException()
        {
            var response = new ContentstackResponse(CreateResponse(), new JsonSerializerOptions());
            response.Dispose();

            Assert.Throws<ObjectDisposedException>(() => response.OpenResponse());
        }
    }
}
