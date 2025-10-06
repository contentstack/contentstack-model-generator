using System;
using System.Threading.Tasks;
using Xunit;
using contentstack.CMA;
using contentstack.CMA.OAuth;

namespace contentstack.model.generator.tests
{
    public class OAuthErrorHandlingTests
    {
        [Fact]
        public void GenerateCodeChallenge_ShouldThrowOnNullInput()
        {
            
            string codeVerifier = null;

            
            Assert.Throws<ArgumentNullException>(() => OAuthService.GenerateCodeChallenge(codeVerifier));
        }

        [Fact]
        public void GenerateCodeChallenge_ShouldThrowOnEmptyInput()
        {
            
            string codeVerifier = "";

            
            Assert.Throws<ArgumentException>(() => OAuthService.GenerateCodeChallenge(codeVerifier));
        }

        [Fact]
        public void GenerateCodeChallenge_ShouldThrowOnWhitespaceInput()
        {
            
            string codeVerifier = "   ";

            
            Assert.Throws<ArgumentException>(() => OAuthService.GenerateCodeChallenge(codeVerifier));
        }

        [Fact]
        public void GenerateAuthorizationUrl_ShouldThrowOnNullOptions()
        {
            
            ContentstackOptions options = null;
            string codeVerifier = "test_verifier";

            
            Assert.Throws<ArgumentNullException>(() => OAuthService.GenerateAuthorizationUrl(options, codeVerifier));
        }

        [Fact]
        public void GenerateAuthorizationUrl_ShouldThrowOnNullCodeVerifier()
        {
            
            var options = new ContentstackOptions
            {
                Host = "api.contentstack.io",
                OAuthClientId = "test_client_id",
                OAuthRedirectUri = "http://localhost:8080",
                OAuthAppId = "test_app_id"
            };
            string codeVerifier = null;

            
            Assert.Throws<ArgumentNullException>(() => OAuthService.GenerateAuthorizationUrl(options, codeVerifier));
        }

        [Fact]
        public void GenerateAuthorizationUrl_ShouldThrowOnEmptyCodeVerifier()
        {
            
            var options = new ContentstackOptions
            {
                Host = "api.contentstack.io",
                OAuthClientId = "test_client_id",
                OAuthRedirectUri = "http://localhost:8080",
                OAuthAppId = "test_app_id"
            };
            string codeVerifier = "";

            
            Assert.Throws<ArgumentException>(() => OAuthService.GenerateAuthorizationUrl(options, codeVerifier));
        }

        [Fact]
        public void GenerateAuthorizationUrl_ShouldThrowOnNullClientId()
        {
            
            var options = new ContentstackOptions
            {
                Host = "api.contentstack.io",
                OAuthClientId = null,
                OAuthRedirectUri = "http://localhost:8080",
                OAuthAppId = "test_app_id"
            };
            string codeVerifier = "test_verifier";
            Assert.Throws<ArgumentNullException>(() => OAuthService.GenerateAuthorizationUrl(options, codeVerifier));
        }

        [Fact]
        public void GenerateAuthorizationUrl_ShouldThrowOnEmptyClientId()
        {
            var options = new ContentstackOptions
            {
                Host = "api.contentstack.io",
                OAuthClientId = "",
                OAuthRedirectUri = "http://localhost:8080",
                OAuthAppId = "test_app_id"
            };
            string codeVerifier = "test_verifier";

            
            Assert.Throws<ArgumentException>(() => OAuthService.GenerateAuthorizationUrl(options, codeVerifier));
        }

        [Fact]
        public void GenerateAuthorizationUrl_ShouldThrowOnNullRedirectUri()
        {
            
            var options = new ContentstackOptions
            {
                Host = "api.contentstack.io",
                OAuthClientId = "test_client_id",
                OAuthRedirectUri = null,
                OAuthAppId = "test_app_id"
            };
            string codeVerifier = "test_verifier";

            
            Assert.Throws<ArgumentNullException>(() => OAuthService.GenerateAuthorizationUrl(options, codeVerifier));
        }

        [Fact]
        public void GenerateAuthorizationUrl_ShouldThrowOnEmptyRedirectUri()
        {
            
            var options = new ContentstackOptions
            {
                Host = "api.contentstack.io",
                OAuthClientId = "test_client_id",
                OAuthRedirectUri = "",
                OAuthAppId = "test_app_id"
            };
            string codeVerifier = "test_verifier";

            
            Assert.Throws<ArgumentException>(() => OAuthService.GenerateAuthorizationUrl(options, codeVerifier));
        }

        [Fact]
        public void GenerateAuthorizationUrl_ShouldThrowOnNullHost()
        {
            
            var options = new ContentstackOptions
            {
                Host = null,
                OAuthClientId = "test_client_id",
                OAuthRedirectUri = "http://localhost:8080",
                OAuthAppId = "test_app_id"
            };
            string codeVerifier = "test_verifier";

            
            Assert.Throws<ArgumentNullException>(() => OAuthService.GenerateAuthorizationUrl(options, codeVerifier));
        }

        [Fact]
        public void GenerateAuthorizationUrl_ShouldThrowOnEmptyHost()
        {
            
            var options = new ContentstackOptions
            {
                Host = "",
                OAuthClientId = "test_client_id",
                OAuthRedirectUri = "http://localhost:8080",
                OAuthAppId = "test_app_id"
            };
            string codeVerifier = "test_verifier";

            
            Assert.Throws<ArgumentException>(() => OAuthService.GenerateAuthorizationUrl(options, codeVerifier));
        }

        [Fact]
        public void GenerateAuthorizationUrl_ShouldThrowOnNullAppId()
        {
            
            var options = new ContentstackOptions
            {
                Host = "api.contentstack.io",
                OAuthClientId = "test_client_id",
                OAuthRedirectUri = "http://localhost:8080",
                OAuthAppId = null
            };
            string codeVerifier = "test_verifier";

            
            Assert.Throws<ArgumentNullException>(() => OAuthService.GenerateAuthorizationUrl(options, codeVerifier));
        }

        [Fact]
        public void GenerateAuthorizationUrl_ShouldThrowOnEmptyAppId()
        {
            
            var options = new ContentstackOptions
            {
                Host = "api.contentstack.io",
                OAuthClientId = "test_client_id",
                OAuthRedirectUri = "http://localhost:8080",
                OAuthAppId = ""
            };
            string codeVerifier = "test_verifier";

            
            Assert.Throws<ArgumentException>(() => OAuthService.GenerateAuthorizationUrl(options, codeVerifier));
        }

        [Fact]
        public void IsTokenExpired_ShouldThrowOnNullOptions()
        {
            
            ContentstackOptions options = null;

            
            Assert.Throws<ArgumentNullException>(() => OAuthService.IsTokenExpired(options));
        }

        [Fact]
        public async Task LogoutAsync_ShouldThrowOnNullOptions()
        {
            
            ContentstackOptions options = null;

            
            await Assert.ThrowsAsync<ArgumentNullException>(() => OAuthService.LogoutAsync(options));
        }

        [Fact]
        public void GetOAuthHost_ShouldThrowOnNullInput()
        {
            
            string host = null;

            
            Assert.Throws<ArgumentNullException>(() => OAuthService.GetOAuthHost(host));
        }

        [Fact]
        public void GetOAuthHost_ShouldThrowOnEmptyInput()
        {
            
            string host = "";

            
            Assert.Throws<ArgumentException>(() => OAuthService.GetOAuthHost(host));
        }

        [Fact]
        public void GetDeveloperHubHostname_ShouldThrowOnNullInput()
        {
            
            string host = null;

            
            Assert.Throws<ArgumentNullException>(() => OAuthService.GetDeveloperHubHostname(host));
        }

        [Fact]
        public void GetDeveloperHubHostname_ShouldThrowOnEmptyInput()
        {
            
            string host = "";

            
            Assert.Throws<ArgumentException>(() => OAuthService.GetDeveloperHubHostname(host));
        }

        [Theory]
        [InlineData("invalid_host")]
        [InlineData("not_contentstack.com")]
        [InlineData("api.other.com")]
        public void GetOAuthHost_ShouldHandleInvalidHostnames(string invalidHost)
        {
            
            // Should not throw but might return unexpected results
            var result = OAuthService.GetOAuthHost(invalidHost);
            Assert.NotNull(result);
        }

        [Theory]
        [InlineData("invalid_host")]
        [InlineData("not_contentstack.com")]
        [InlineData("api.other.com")]
        public void GetDeveloperHubHostname_ShouldHandleInvalidHostnames(string invalidHost)
        {
            
            // Should not throw but might return unexpected results
            var result = OAuthService.GetDeveloperHubHostname(invalidHost);
            Assert.NotNull(result);
        }

        [Fact]
        public void GenerateCodeVerifier_ShouldReturnConsistentLength()
        {
            
            var verifier1 = OAuthService.GenerateCodeVerifier();
            var verifier2 = OAuthService.GenerateCodeVerifier();
            var verifier3 = OAuthService.GenerateCodeVerifier();

            
            Assert.True(verifier1.Length >= 43);
            Assert.True(verifier1.Length <= 128);
            Assert.True(verifier2.Length >= 43);
            Assert.True(verifier2.Length <= 128);
            Assert.True(verifier3.Length >= 43);
            Assert.True(verifier3.Length <= 128);
        }

        [Fact]
        public void GenerateCodeChallenge_ShouldReturnConsistentLength()
        {
            
            var codeVerifier = "test_code_verifier_123";

            
            var challenge1 = OAuthService.GenerateCodeChallenge(codeVerifier);
            var challenge2 = OAuthService.GenerateCodeChallenge(codeVerifier);

            
            Assert.Equal(challenge1.Length, challenge2.Length);
            Assert.True(challenge1.Length > 0);
        }

        [Fact]
        public void GenerateCodeChallenge_ShouldHandleVeryLongCodeVerifier()
        {
            
            var longCodeVerifier = new string('a', 200); // Very long verifier

            
            // Should not throw
            var challenge = OAuthService.GenerateCodeChallenge(longCodeVerifier);
            Assert.NotNull(challenge);
            Assert.NotEmpty(challenge);
        }

        [Fact]
        public void GenerateCodeChallenge_ShouldHandleSpecialCharacters()
        {
            
            var codeVerifier = "test_verifier_with_special_chars_!@#$%^&*()";

            
            // Should not throw
            var challenge = OAuthService.GenerateCodeChallenge(codeVerifier);
            Assert.NotNull(challenge);
            Assert.NotEmpty(challenge);
        }

        [Fact]
        public void GenerateCodeChallenge_ShouldHandleUnicodeCharacters()
        {
            
            var codeVerifier = "test_verifier_with_unicode_测试_🚀";

            
            // Should not throw
            var challenge = OAuthService.GenerateCodeChallenge(codeVerifier);
            Assert.NotNull(challenge);
            Assert.NotEmpty(challenge);
        }
    }
}
