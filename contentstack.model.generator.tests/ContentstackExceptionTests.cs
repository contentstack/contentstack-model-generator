using System;
using System.Collections.Generic;
using System.Net;
using contentstack.CMA;
using Xunit;

namespace contentstack.model.generator.tests
{
    public class ContentstackExceptionTests
    {
        [Fact]
        public void DefaultConstructor_CreatesInstanceWithEmptyErrorMessage()
        {
            var exception = new ContentstackException();

            Assert.Equal(string.Empty, exception.ErrorMessage);
        }

        [Fact]
        public void Constructor_WithMessage_SetsBaseMessageAndErrorMessage()
        {
            var exception = new ContentstackException("something failed");

            Assert.Equal("something failed", exception.Message);
            Assert.Equal("something failed", exception.ErrorMessage);
        }

        [Fact]
        public void Constructor_WithInnerException_SetsErrorMessageFromInnerExceptionMessage()
        {
            var inner = new InvalidOperationException("inner failure");

            var exception = new ContentstackException(inner);

            Assert.Equal("inner failure", exception.ErrorMessage);
            Assert.Equal("inner failure", exception.Message);
        }

        [Fact]
        public void Constructor_WithMessageAndInnerException_SetsBaseMessageButNotShadowedMessageProperty()
        {
            // ContentstackException declares `public new string Message`, which shadows
            // Exception.Message. This constructor forwards to base(message, innerException)
            // (so the base Exception's Message is set) but - unlike the other constructors -
            // never sets ErrorMessage, so the shadowing `Message` property itself stays null.
            var inner = new InvalidOperationException("root cause");

            var exception = new ContentstackException("wrapped failure", inner);

            Assert.Null(exception.Message);
            Assert.Equal("wrapped failure", ((Exception)exception).Message);
            Assert.Same(inner, exception.InnerException);
        }

        [Fact]
        public void ErrorMessage_Setter_AlsoUpdatesMessageProperty()
        {
            var exception = new ContentstackException();

            exception.ErrorMessage = "updated message";

            Assert.Equal("updated message", exception.Message);
        }

        [Fact]
        public void StatusCode_CanBeSetAndRetrieved()
        {
            var exception = new ContentstackException { StatusCode = HttpStatusCode.NotFound };

            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        }

        [Fact]
        public void ErrorCode_CanBeSetAndRetrieved()
        {
            var exception = new ContentstackException { ErrorCode = 141 };

            Assert.Equal(141, exception.ErrorCode);
        }

        [Fact]
        public void Errors_CanBeSetAndRetrieved()
        {
            var errors = new Dictionary<string, object> { { "title", "is required" } };

            var exception = new ContentstackException { Errors = errors };

            Assert.Same(errors, exception.Errors);
        }
    }
}
