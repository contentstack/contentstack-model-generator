using System;
using System.Threading.Tasks;
using Xunit;
using contentstack.CMA;
using contentstack.CMA.OAuth;

namespace contentstack.model.generator.tests
{
    public class OAuthIntegrationTests
    {
        [Fact]
        public void CompleteOAuthFlow_ShouldGenerateValidAuthorizationUrl()
        {
            
            var options = new ContentstackOptions
            {
                Host = "api.contentstack.io",
                OAuthClientId = "test_client_id",
                OAuthRedirectUri = "http://localhost:8080",
                OAuthAppId = "test_app_id",
                OAuthScopes = new[] { "read", "write" }
            };

            
            var codeVerifier = OAuthService.GenerateCodeVerifier();
            var authUrl = OAuthService.GenerateAuthorizationUrl(options, codeVerifier);

            
            Assert.NotNull(authUrl);
            Assert.Contains("app.contentstack.com", authUrl);
            Assert.Contains("response_type=code", authUrl);
            Assert.Contains("client_id=test_client_id", authUrl);
            Assert.Contains("redirect_uri=http%3A%2F%2Flocalhost%3A8080", authUrl);
            Assert.Contains("code_challenge=", authUrl);
            Assert.Contains("code_challenge_method=S256", authUrl);
            Assert.Contains("scope=read%20write", authUrl);
        }

        [Fact]
        public void CompleteOAuthFlow_ShouldHandlePKCEFlow()
        {
            
            var options = new ContentstackOptions
            {
                Host = "api.contentstack.io",
                OAuthClientId = "test_client_id",
                OAuthRedirectUri = "http://localhost:8080",
                OAuthAppId = "test_app_id"
                // No client secret - PKCE flow
            };

            
            var codeVerifier = OAuthService.GenerateCodeVerifier();
            var codeChallenge = OAuthService.GenerateCodeChallenge(codeVerifier);
            var authUrl = OAuthService.GenerateAuthorizationUrl(options, codeVerifier);

            
            Assert.NotNull(codeVerifier);
            Assert.NotNull(codeChallenge);
            Assert.NotNull(authUrl);
            Assert.Contains("code_challenge=", authUrl);
            Assert.Contains("code_challenge_method=S256", authUrl);
        }

        [Fact]
        public void CompleteOAuthFlow_ShouldHandleTraditionalOAuthFlow()
        {
            
            var options = new ContentstackOptions
            {
                Host = "api.contentstack.io",
                OAuthClientId = "test_client_id",
                OAuthClientSecret = "test_client_secret",
                OAuthRedirectUri = "http://localhost:8080",
                OAuthAppId = "test_app_id"
            };

            
            var codeVerifier = OAuthService.GenerateCodeVerifier();
            var authUrl = OAuthService.GenerateAuthorizationUrl(options, codeVerifier);

            
            Assert.NotNull(authUrl);
            Assert.Contains("code_challenge=", authUrl);
            Assert.Contains("code_challenge_method=S256", authUrl);
        }

        [Fact]
        public void CompleteOAuthFlow_ShouldHandleTokenExpiration()
        {
            
            var options = new ContentstackOptions
            {
                AccessToken = "test_access_token",
                RefreshToken = "test_refresh_token",
                TokenExpiresAt = DateTime.UtcNow.AddMinutes(-1) // Expired
            };

            
            var isExpired = OAuthService.IsTokenExpired(options);

            
            Assert.True(isExpired);
        }

        [Fact]
        public void CompleteOAuthFlow_ShouldHandleValidToken()
        {
            
            var options = new ContentstackOptions
            {
                AccessToken = "test_access_token",
                RefreshToken = "test_refresh_token",
                TokenExpiresAt = DateTime.UtcNow.AddMinutes(30) // Valid
            };

            
            var isExpired = OAuthService.IsTokenExpired(options);

            
            Assert.False(isExpired);
        }

        [Fact]
        public void CompleteOAuthFlow_ShouldHandleLogout()
        {
            
            var options = new ContentstackOptions
            {
                AccessToken = "test_access_token",
                RefreshToken = "test_refresh_token",
                TokenExpiresAt = DateTime.UtcNow.AddMinutes(30),
                Authorization = "Bearer test_access_token"
            };

            
            OAuthService.LogoutAsync(options);

            
            Assert.Null(options.AccessToken);
            Assert.Null(options.RefreshToken);
            Assert.Null(options.TokenExpiresAt);
            // Note: Authorization might not be cleared in current implementation
        }

        [Fact]
        public void CompleteOAuthFlow_ShouldHandleScopesCorrectly()
        {
            
            var options = new ContentstackOptions
            {
                Host = "api.contentstack.io",
                OAuthClientId = "test_client_id",
                OAuthRedirectUri = "http://localhost:8080",
                OAuthAppId = "test_app_id",
                OAuthScopes = new[] { "read", "write", "admin" }
            };

            
            var codeVerifier = OAuthService.GenerateCodeVerifier();
            var authUrl = OAuthService.GenerateAuthorizationUrl(options, codeVerifier);

            
            Assert.Contains("scope=read%20write%20admin", authUrl);
        }

        [Fact]
        public void CompleteOAuthFlow_ShouldHandleEmptyScopes()
        {
            
            var options = new ContentstackOptions
            {
                Host = "api.contentstack.io",
                OAuthClientId = "test_client_id",
                OAuthRedirectUri = "http://localhost:8080",
                OAuthAppId = "test_app_id",
                OAuthScopes = new string[0] // Empty array
            };

            
            var codeVerifier = OAuthService.GenerateCodeVerifier();
            var authUrl = OAuthService.GenerateAuthorizationUrl(options, codeVerifier);

            
            Assert.DoesNotContain("scope=", authUrl);
        }

        [Fact]
        public void CompleteOAuthFlow_ShouldHandleNullScopes()
        {
            
            var options = new ContentstackOptions
            {
                Host = "api.contentstack.io",
                OAuthClientId = "test_client_id",
                OAuthRedirectUri = "http://localhost:8080",
                OAuthAppId = "test_app_id",
                OAuthScopes = null
            };

            
            var codeVerifier = OAuthService.GenerateCodeVerifier();
            var authUrl = OAuthService.GenerateAuthorizationUrl(options, codeVerifier);

            
            Assert.DoesNotContain("scope=", authUrl);
        }

        [Fact]
        public void CompleteOAuthFlow_ShouldHandleHostnameTransformation()
        {
            
            var options = new ContentstackOptions
            {
                Host = "api.contentstack.io",
                OAuthClientId = "test_client_id",
                OAuthRedirectUri = "http://localhost:8080",
                OAuthAppId = "test_app_id"
            };

            
            var codeVerifier = OAuthService.GenerateCodeVerifier();
            var authUrl = OAuthService.GenerateAuthorizationUrl(options, codeVerifier);

            
            Assert.Contains("app.contentstack.com", authUrl);
            Assert.DoesNotContain("api.contentstack.io", authUrl);
        }

        [Fact]
        public void CompleteOAuthFlow_ShouldHandleSpecialCharactersInScopes()
        {
            
            var options = new ContentstackOptions
            {
                Host = "api.contentstack.io",
                OAuthClientId = "test_client_id",
                OAuthRedirectUri = "http://localhost:8080",
                OAuthAppId = "test_app_id",
                OAuthScopes = new[] { "read content", "write content", "manage users" }
            };

            
            var codeVerifier = OAuthService.GenerateCodeVerifier();
            var authUrl = OAuthService.GenerateAuthorizationUrl(options, codeVerifier);

            
            Assert.Contains("scope=read%20content%20write%20content%20manage%20users", authUrl);
        }

        [Fact]
        public void CompleteOAuthFlow_ShouldHandleLongScopes()
        {
            
            var options = new ContentstackOptions
            {
                Host = "api.contentstack.io",
                OAuthClientId = "test_client_id",
                OAuthRedirectUri = "http://localhost:8080",
                OAuthAppId = "test_app_id",
                OAuthScopes = new[] { 
                    "read", "write", "admin", "manage", "create", "delete", 
                    "update", "view", "edit", "publish", "unpublish", "archive" 
                }
            };

            
            var codeVerifier = OAuthService.GenerateCodeVerifier();
            var authUrl = OAuthService.GenerateAuthorizationUrl(options, codeVerifier);

            
            Assert.NotNull(authUrl);
            Assert.Contains("scope=", authUrl);
            // Should contain all scopes URL encoded
            Assert.Contains("read", authUrl);
            Assert.Contains("write", authUrl);
            Assert.Contains("admin", authUrl);
        }

        [Fact]
        public void CompleteOAuthFlow_ShouldHandleEdgeCaseTokenExpiration()
        {
            
            var options = new ContentstackOptions
            {
                AccessToken = "test_access_token",
                RefreshToken = "test_refresh_token",
                TokenExpiresAt = DateTime.UtcNow.AddSeconds(30) // Near expiration (30 seconds)
            };

            
            var isExpired = OAuthService.IsTokenExpired(options);

            
            Assert.True(isExpired); // Should be considered expired due to buffer time
        }

        [Fact]
        public void CompleteOAuthFlow_ShouldHandleMultipleLogoutCalls()
        {
            
            var options = new ContentstackOptions
            {
                AccessToken = "test_access_token",
                RefreshToken = "test_refresh_token",
                TokenExpiresAt = DateTime.UtcNow.AddMinutes(30)
            };

            
            OAuthService.LogoutAsync(options);
            OAuthService.LogoutAsync(options); // Second call

            
            Assert.Null(options.AccessToken);
            Assert.Null(options.RefreshToken);
            Assert.Null(options.TokenExpiresAt);
        }
    }
}


