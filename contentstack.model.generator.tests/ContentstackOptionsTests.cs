using System;
using Xunit;
using contentstack.CMA;

namespace contentstack.model.generator.tests
{
    public class ContentstackOptionsTests
    {
        [Fact]
        public void Constructor_ShouldInitializeWithDefaultValues()
        {
            
            var options = new ContentstackOptions();
            Assert.Null(options.ApiKey);
            Assert.Null(options.Authtoken);
            Assert.Null(options.Host);
            Assert.Null(options.Branch);
            Assert.Null(options.Version);
            Assert.False(options.IsOAuth);
            Assert.Null(options.Authorization);
            Assert.Null(options.OAuthClientId);
            Assert.Null(options.OAuthClientSecret);
            Assert.Null(options.OAuthRedirectUri);
            Assert.Null(options.OAuthAppId);
            Assert.Null(options.OAuthScopes);
            Assert.Null(options.AccessToken);
            Assert.Null(options.RefreshToken);
            Assert.Null(options.TokenExpiresAt);
        }

        [Fact]
        public void Properties_ShouldBeSettableAndGettable()
        {
            
            var options = new ContentstackOptions();
            var testDate = DateTime.UtcNow.AddHours(1);
            var testScopes = new[] { "read", "write" };
            options.ApiKey = "test_api_key";
            options.Authtoken = "test_authtoken";
            options.Host = "api.contentstack.io";
            options.Branch = "main";
            options.Version = "v3";
            options.IsOAuth = true;
            options.Authorization = "Bearer test_token";
            options.OAuthClientId = "test_client_id";
            options.OAuthClientSecret = "test_client_secret";
            options.OAuthRedirectUri = "http://localhost:8080";
            options.OAuthAppId = "test_app_id";
            options.OAuthScopes = testScopes;
            options.AccessToken = "test_access_token";
            options.RefreshToken = "test_refresh_token";
            options.TokenExpiresAt = testDate;
            Assert.Equal("test_api_key", options.ApiKey);
            Assert.Equal("test_authtoken", options.Authtoken);
            Assert.Equal("api.contentstack.io", options.Host);
            Assert.Equal("main", options.Branch);
            Assert.Equal("v3", options.Version);
            Assert.True(options.IsOAuth);
            Assert.Equal("Bearer test_token", options.Authorization);
            Assert.Equal("test_client_id", options.OAuthClientId);
            Assert.Equal("test_client_secret", options.OAuthClientSecret);
            Assert.Equal("http://localhost:8080", options.OAuthRedirectUri);
            Assert.Equal("test_app_id", options.OAuthAppId);
            Assert.Equal(testScopes, options.OAuthScopes);
            Assert.Equal("test_access_token", options.AccessToken);
            Assert.Equal("test_refresh_token", options.RefreshToken);
            Assert.Equal(testDate, options.TokenExpiresAt);
        }

        [Fact]
        public void OAuthScopes_ShouldHandleNullValue()
        {
            var options = new ContentstackOptions();
            options.OAuthScopes = null; 
            Assert.Null(options.OAuthScopes);
        }

        [Fact]
        public void OAuthScopes_ShouldHandleEmptyArray()
        {
            
            var options = new ContentstackOptions();
            var emptyScopes = new string[0];
            options.OAuthScopes = emptyScopes;
            Assert.NotNull(options.OAuthScopes);
            Assert.Empty(options.OAuthScopes);
        }

        [Fact]
        public void OAuthScopes_ShouldHandleSingleScope()
        {
            
            var options = new ContentstackOptions();
            var singleScope = new[] { "read" };
            options.OAuthScopes = singleScope;
            Assert.NotNull(options.OAuthScopes);
            Assert.Single(options.OAuthScopes);
            Assert.Equal("read", options.OAuthScopes[0]);
        }

        [Fact]
        public void OAuthScopes_ShouldHandleMultipleScopes()
        {
            
            var options = new ContentstackOptions();
            var multipleScopes = new[] { "read", "write", "admin" };
            options.OAuthScopes = multipleScopes;
            Assert.NotNull(options.OAuthScopes);
            Assert.Equal(3, options.OAuthScopes.Length);
            Assert.Equal("read", options.OAuthScopes[0]);
            Assert.Equal("write", options.OAuthScopes[1]);
            Assert.Equal("admin", options.OAuthScopes[2]);
        }

        [Fact]
        public void OAuthScopes_ShouldHandleScopesWithSpaces()
        {
            
            var options = new ContentstackOptions();
            var scopesWithSpaces = new[] { "read content", "write content", "manage users" };
            options.OAuthScopes = scopesWithSpaces;
            Assert.NotNull(options.OAuthScopes);
            Assert.Equal(3, options.OAuthScopes.Length);
            Assert.Equal("read content", options.OAuthScopes[0]);
            Assert.Equal("write content", options.OAuthScopes[1]);
            Assert.Equal("manage users", options.OAuthScopes[2]);
        }

        [Fact]
        public void OAuthScopes_ShouldHandleEmptyStringScopes()
        {
            
            var options = new ContentstackOptions();
            var scopesWithEmpty = new[] { "read", "", "write" };
            options.OAuthScopes = scopesWithEmpty;
            Assert.NotNull(options.OAuthScopes);
            Assert.Equal(3, options.OAuthScopes.Length);
            Assert.Equal("read", options.OAuthScopes[0]);
            Assert.Equal("", options.OAuthScopes[1]);
            Assert.Equal("write", options.OAuthScopes[2]);
        }

        [Fact]
        public void OAuthScopes_ShouldHandleNullStringScopes()
        {
            
            var options = new ContentstackOptions();
            var scopesWithNull = new[] { "read", null, "write" };
            options.OAuthScopes = scopesWithNull;
            Assert.NotNull(options.OAuthScopes);
            Assert.Equal(3, options.OAuthScopes.Length);
            Assert.Equal("read", options.OAuthScopes[0]);
            Assert.Null(options.OAuthScopes[1]);
            Assert.Equal("write", options.OAuthScopes[2]);
        }

        [Fact]
        public void TokenExpiresAt_ShouldHandleNullValue()
        {
            
            var options = new ContentstackOptions();
            options.TokenExpiresAt = null;
            Assert.Null(options.TokenExpiresAt);
        }

        [Fact]
        public void TokenExpiresAt_ShouldHandleDateTimeValue()
        {
            
            var options = new ContentstackOptions();
            var testDate = DateTime.UtcNow.AddHours(2);
            options.TokenExpiresAt = testDate;
            Assert.Equal(testDate, options.TokenExpiresAt);
        }

        [Fact]
        public void TokenExpiresAt_ShouldHandleDateTimeKind()
        {
            
            var options = new ContentstackOptions();
            var utcDate = DateTime.UtcNow.AddHours(1);
            var localDate = DateTime.Now.AddHours(1);
            options.TokenExpiresAt = utcDate;
            Assert.Equal(DateTimeKind.Utc, options.TokenExpiresAt.Value.Kind);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("test_value")]
        public void StringProperties_ShouldHandleVariousValues(string testValue)
        {
            
            var options = new ContentstackOptions();
            options.ApiKey = testValue;
            options.Authtoken = testValue;
            options.Host = testValue;
            options.Branch = testValue;
            options.Version = testValue;
            options.Authorization = testValue;
            options.OAuthClientId = testValue;
            options.OAuthClientSecret = testValue;
            options.OAuthRedirectUri = testValue;
            options.OAuthAppId = testValue;
            options.AccessToken = testValue;
            options.RefreshToken = testValue;
            Assert.Equal(testValue, options.ApiKey);
            Assert.Equal(testValue, options.Authtoken);
            Assert.Equal(testValue, options.Host);
            Assert.Equal(testValue, options.Branch);
            Assert.Equal(testValue, options.Version);
            Assert.Equal(testValue, options.Authorization);
            Assert.Equal(testValue, options.OAuthClientId);
            Assert.Equal(testValue, options.OAuthClientSecret);
            Assert.Equal(testValue, options.OAuthRedirectUri);
            Assert.Equal(testValue, options.OAuthAppId);
            Assert.Equal(testValue, options.AccessToken);
            Assert.Equal(testValue, options.RefreshToken);
        }

        [Fact]
        public void IsOAuth_ShouldDefaultToFalse()
        {
            var options = new ContentstackOptions();
            Assert.False(options.IsOAuth);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void IsOAuth_ShouldBeSettable(bool value)
        {
            var options = new ContentstackOptions();
            options.IsOAuth = value;
            Assert.Equal(value, options.IsOAuth);
        }
    }
}


