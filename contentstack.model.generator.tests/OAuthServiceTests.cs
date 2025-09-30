using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using contentstack.CMA;
using contentstack.CMA.OAuth;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace contentstack.model.generator.tests
{
    public class OAuthServiceTests
    {
        [Fact]
        public void GenerateCodeVerifier_ShouldReturnValidBase64UrlEncodedString()
        {
            
            var codeVerifier = OAuthService.GenerateCodeVerifier();

            
            Assert.NotNull(codeVerifier);
            Assert.NotEmpty(codeVerifier);
            Assert.True(codeVerifier.Length >= 43); // Minimum length for PKCE
            Assert.True(codeVerifier.Length <= 128); // Maximum length for PKCE
            
            // Should be Base64URL encoded (no padding, no + or / characters)
            Assert.DoesNotContain("+", codeVerifier);
            Assert.DoesNotContain("/", codeVerifier);
            Assert.DoesNotContain("=", codeVerifier);
        }

        [Fact]
        public void GenerateCodeVerifier_ShouldReturnDifferentValues()
        {
            
            var codeVerifier1 = OAuthService.GenerateCodeVerifier();
            var codeVerifier2 = OAuthService.GenerateCodeVerifier();

            
            Assert.NotEqual(codeVerifier1, codeVerifier2);
        }

        [Theory]
        [InlineData("test_code_verifier_123")]
        [InlineData("another_test_verifier")]
        [InlineData("a")]
        public void GenerateCodeChallenge_ShouldReturnValidBase64UrlEncodedString(string codeVerifier)
        {
            
            var codeChallenge = OAuthService.GenerateCodeChallenge(codeVerifier);

            
            Assert.NotNull(codeChallenge);
            Assert.NotEmpty(codeChallenge);
            
            // Should be Base64URL encoded
            Assert.DoesNotContain("+", codeChallenge);
            Assert.DoesNotContain("/", codeChallenge);
            Assert.DoesNotContain("=", codeChallenge);
        }

        [Fact]
        public void GenerateCodeChallenge_ShouldReturnConsistentResults()
        {
            
            var codeVerifier = "test_code_verifier_123";

            
            var challenge1 = OAuthService.GenerateCodeChallenge(codeVerifier);
            var challenge2 = OAuthService.GenerateCodeChallenge(codeVerifier);

            
            Assert.Equal(challenge1, challenge2);
        }

        [Fact]
        public void GenerateAuthorizationUrl_ShouldIncludeRequiredParameters()
        {
            
            var options = new ContentstackOptions
            {
                Host = "api.contentstack.io",
                OAuthClientId = "test_client_id",
                OAuthRedirectUri = "http://localhost:8080",
                OAuthAppId = "test_app_id"
            };
            var codeVerifier = "test_code_verifier";

            
            var authUrl = OAuthService.GenerateAuthorizationUrl(options, codeVerifier);

            
            Assert.NotNull(authUrl);
            Assert.Contains("app.contentstack.com", authUrl);
            Assert.Contains("response_type=code", authUrl);
            Assert.Contains("client_id=test_client_id", authUrl);
            Assert.Contains("redirect_uri=http%3A%2F%2Flocalhost%3A8080", authUrl);
            Assert.Contains("code_challenge=", authUrl);
            Assert.Contains("code_challenge_method=S256", authUrl);
        }

        [Fact]
        public void GenerateAuthorizationUrl_ShouldIncludeScopesWhenProvided()
        {
            
            var options = new ContentstackOptions
            {
                Host = "api.contentstack.io",
                OAuthClientId = "test_client_id",
                OAuthRedirectUri = "http://localhost:8080",
                OAuthAppId = "test_app_id",
                OAuthScopes = new[] { "read", "write" }
            };
            var codeVerifier = "test_code_verifier";

            
            var authUrl = OAuthService.GenerateAuthorizationUrl(options, codeVerifier);

            
            Assert.Contains("scope=read%20write", authUrl);
        }

        [Fact]
        public void GenerateAuthorizationUrl_ShouldNotIncludeScopesWhenNotProvided()
        {
            
            var options = new ContentstackOptions
            {
                Host = "api.contentstack.io",
                OAuthClientId = "test_client_id",
                OAuthRedirectUri = "http://localhost:8080",
                OAuthAppId = "test_app_id",
                OAuthScopes = null
            };
            var codeVerifier = "test_code_verifier";

            
            var authUrl = OAuthService.GenerateAuthorizationUrl(options, codeVerifier);

            
            Assert.DoesNotContain("scope=", authUrl);
        }

        [Fact]
        public void GenerateAuthorizationUrl_ShouldNotIncludeScopesWhenEmptyArray()
        {
            
            var options = new ContentstackOptions
            {
                Host = "api.contentstack.io",
                OAuthClientId = "test_client_id",
                OAuthRedirectUri = "http://localhost:8080",
                OAuthAppId = "test_app_id",
                OAuthScopes = new string[0]
            };
            var codeVerifier = "test_code_verifier";

            
            var authUrl = OAuthService.GenerateAuthorizationUrl(options, codeVerifier);

            
            Assert.DoesNotContain("scope=", authUrl);
        }

        [Theory]
        [InlineData("api.contentstack.io", "app.contentstack.com")]
        [InlineData("https://api.contentstack.io", "app.contentstack.com")]
        [InlineData("http://api.contentstack.io", "app.contentstack.com")]
        public void GetOAuthHost_ShouldTransformHostnameCorrectly(string inputHost, string expectedHost)
        {
            
            var result = OAuthService.GetOAuthHost(inputHost);

            
            Assert.Equal(expectedHost, result);
        }

        [Theory]
        [InlineData("api.contentstack.io", "https://developerhub-api.contentstack.com")]
        [InlineData("https://api.contentstack.io", "https://developerhub-api.contentstack.com")]
        [InlineData("http://api.contentstack.io", "https://developerhub-api.contentstack.com")]
        public void GetDeveloperHubHostname_ShouldTransformHostnameCorrectly(string inputHost, string expectedHost)
        {
            
            var result = OAuthService.GetDeveloperHubHostname(inputHost);

            
            Assert.Equal(expectedHost, result);
        }

        [Fact]
        public void IsTokenExpired_ShouldReturnTrueForExpiredToken()
        {
            
            var options = new ContentstackOptions
            {
                TokenExpiresAt = DateTime.UtcNow.AddMinutes(-1) // Expired 1 minute ago
            };

            
            var isExpired = OAuthService.IsTokenExpired(options);

            
            Assert.True(isExpired);
        }

        [Fact]
        public void IsTokenExpired_ShouldReturnFalseForValidToken()
        {
            
            var options = new ContentstackOptions
            {
                TokenExpiresAt = DateTime.UtcNow.AddMinutes(30) // Valid for 30 more minutes
            };

            
            var isExpired = OAuthService.IsTokenExpired(options);

            
            Assert.False(isExpired);
        }

        [Fact]
        public void IsTokenExpired_ShouldReturnTrueForNullExpiration()
        {
            
            var options = new ContentstackOptions
            {
                TokenExpiresAt = null
            };

            
            var isExpired = OAuthService.IsTokenExpired(options);

            
            Assert.True(isExpired);
        }

        [Fact]
        public void IsTokenExpired_ShouldReturnTrueForNearExpiration()
        {
            
            var options = new ContentstackOptions
            {
                TokenExpiresAt = DateTime.UtcNow.AddSeconds(30) // Expires in 30 seconds (near expiration)
            };

            
            var isExpired = OAuthService.IsTokenExpired(options);

            
            Assert.True(isExpired);
        }

        [Fact]
        public void LogoutAsync_ShouldClearTokens()
        {
            
            var options = new ContentstackOptions
            {
                AccessToken = "test_access_token",
                RefreshToken = "test_refresh_token",
                TokenExpiresAt = DateTime.UtcNow.AddMinutes(30)
            };

            
            OAuthService.LogoutAsync(options);

            
            Assert.Null(options.AccessToken);
            Assert.Null(options.RefreshToken);
            Assert.Null(options.TokenExpiresAt);
        }

        [Fact]
        public async Task LogoutAsync_ShouldNotThrowWhenTokensAreNull()
        {
            
            var options = new ContentstackOptions
            {
                AccessToken = null,
                RefreshToken = null,
                TokenExpiresAt = null
            };
            var exception = await Record.ExceptionAsync(() => OAuthService.LogoutAsync(options));
            Assert.Null(exception);
        }
    }
}
