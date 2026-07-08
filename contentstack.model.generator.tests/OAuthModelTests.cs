using System;
using System.Text.Json;
using Contentstack.Model.Generator.Model;
using Xunit;

namespace contentstack.model.generator.tests
{
    public class OAuthModelTests
    {
        #region OAuthUser / OAuthAppAuthorizationData / OAuthAppAuthorizationResponse

        [Fact]
        public void OAuthUser_Uid_CanBeSetAndRetrieved()
        {
            var user = new OAuthUser { Uid = "user_123" };

            Assert.Equal("user_123", user.Uid);
        }

        [Fact]
        public void OAuthAppAuthorizationData_PropertiesCanBeSetAndRetrieved()
        {
            var data = new OAuthAppAuthorizationData
            {
                AuthorizationUid = "auth_uid_123",
                User = new OAuthUser { Uid = "user_123" }
            };

            Assert.Equal("auth_uid_123", data.AuthorizationUid);
            Assert.Equal("user_123", data.User.Uid);
        }

        [Fact]
        public void OAuthAppAuthorizationResponse_DeserializesFromJson()
        {
            var json = "{\"data\":[{\"authorization_uid\":\"auth_1\",\"user\":{\"uid\":\"user_1\"}}]}";

            var response = JsonSerializer.Deserialize<OAuthAppAuthorizationResponse>(json);

            Assert.NotNull(response);
            Assert.Single(response.Data);
            Assert.Equal("auth_1", response.Data[0].AuthorizationUid);
            Assert.Equal("user_1", response.Data[0].User.Uid);
        }

        #endregion

        #region OAuthOptions

        [Fact]
        public void OAuthOptions_Defaults_MatchContentstackAppDefaults()
        {
            var options = new OAuthOptions();

            Assert.Equal("6400aa06db64de001a31c8a9", options.AppId);
            Assert.Equal("Ie0FEfTzlfAHL4xM", options.ClientId);
            Assert.Equal("http://localhost:8184", options.RedirectUri);
            Assert.Equal("code", options.ResponseType);
            Assert.Null(options.ClientSecret);
            Assert.Null(options.Scope);
        }

        [Fact]
        public void UsePkce_WithNoClientSecret_IsTrue()
        {
            var options = new OAuthOptions();

            Assert.True(options.UsePkce);
        }

        [Fact]
        public void UsePkce_WithClientSecret_IsFalse()
        {
            var options = new OAuthOptions { ClientSecret = "secret" };

            Assert.False(options.UsePkce);
        }

        [Fact]
        public void IsValid_WithDefaultOptions_ReturnsTrue()
        {
            var options = new OAuthOptions();

            Assert.True(options.IsValid());
        }

        [Fact]
        public void IsValid_WithMissingAppId_ReturnsFalseWithMessage()
        {
            var options = new OAuthOptions { AppId = "" };

            var isValid = options.IsValid(out var errorMessage);

            Assert.False(isValid);
            Assert.Equal("AppId is required for OAuth configuration.", errorMessage);
        }

        [Fact]
        public void IsValid_WithMissingClientId_ReturnsFalseWithMessage()
        {
            var options = new OAuthOptions { ClientId = "" };

            var isValid = options.IsValid(out var errorMessage);

            Assert.False(isValid);
            Assert.Equal("ClientId is required for OAuth configuration.", errorMessage);
        }

        [Fact]
        public void IsValid_WithMissingRedirectUri_ReturnsFalseWithMessage()
        {
            var options = new OAuthOptions { RedirectUri = "" };

            var isValid = options.IsValid(out var errorMessage);

            Assert.False(isValid);
            Assert.Equal("RedirectUri is required for OAuth configuration.", errorMessage);
        }

        [Fact]
        public void IsValid_WithRelativeRedirectUri_ReturnsFalseWithMessage()
        {
            var options = new OAuthOptions { RedirectUri = "not-a-valid-uri" };

            var isValid = options.IsValid(out var errorMessage);

            Assert.False(isValid);
            Assert.Equal("RedirectUri must be a valid absolute URI.", errorMessage);
        }

        [Fact]
        public void IsValid_WithNonHttpRedirectUriScheme_ReturnsFalseWithMessage()
        {
            var options = new OAuthOptions { RedirectUri = "ftp://localhost:8184" };

            var isValid = options.IsValid(out var errorMessage);

            Assert.False(isValid);
            Assert.Equal("RedirectUri must use http or https scheme.", errorMessage);
        }

        [Fact]
        public void IsValid_WithMissingResponseType_ReturnsFalseWithMessage()
        {
            var options = new OAuthOptions { ResponseType = "" };

            var isValid = options.IsValid(out var errorMessage);

            Assert.False(isValid);
            Assert.Equal("ResponseType is required for OAuth configuration.", errorMessage);
        }

        [Fact]
        public void IsValid_WithNonCodeResponseType_ReturnsFalseWithMessage()
        {
            var options = new OAuthOptions { ResponseType = "token" };

            var isValid = options.IsValid(out var errorMessage);

            Assert.False(isValid);
            Assert.Equal("ResponseType must be 'code' for authorization code flow.", errorMessage);
        }

        [Fact]
        public void IsValid_NonPkceWithWhitespaceOnlyClientSecret_ReturnsFalseWithMessage()
        {
            // ClientSecret = " " is not IsNullOrEmpty (so UsePkce is false / non-PKCE flow),
            // but IsNullOrWhiteSpace is true, so this reaches the traditional-flow guard.
            var options = new OAuthOptions { ClientSecret = " " };

            var isValid = options.IsValid(out var errorMessage);

            Assert.False(isValid);
            Assert.Equal(
                "ClientSecret is required for traditional OAuth flow. Use PKCE flow (leave ClientSecret empty) for public clients.",
                errorMessage);
        }

        [Fact]
        public void IsValid_NonPkceWithClientSecret_ReturnsTrue()
        {
            var options = new OAuthOptions { ClientSecret = "a-real-secret" };

            Assert.True(options.IsValid());
        }

        [Fact]
        public void Validate_WithValidOptions_DoesNotThrow()
        {
            var options = new OAuthOptions();

            var exception = Record.Exception(() => options.Validate());

            Assert.Null(exception);
        }

        [Fact]
        public void Validate_WithInvalidOptions_ThrowsOAuthConfigurationException()
        {
            var options = new OAuthOptions { AppId = "" };

            var exception = Assert.Throws<OAuthConfigurationException>(() => options.Validate());
            Assert.Equal("AppId is required for OAuth configuration.", exception.Message);
        }

        [Fact]
        public void ToString_IncludesKeyConfigurationValues()
        {
            var options = new OAuthOptions { Scope = new[] { "read", "write" } };

            var result = options.ToString();

            Assert.Contains("AppId=6400aa06db64de001a31c8a9", result);
            Assert.Contains("UsePkce=True", result);
            Assert.Contains("HasScope=True", result);
        }

        #endregion

        #region OAuthResponse

        [Fact]
        public void OAuthResponse_DeserializesFromJson()
        {
            var json = "{\"access_token\":\"tok\",\"refresh_token\":\"ref\",\"expires_in\":3600,"
                       + "\"organization_uid\":\"org_1\",\"user_uid\":\"user_1\"}";

            var response = JsonSerializer.Deserialize<OAuthResponse>(json);

            Assert.Equal("tok", response.AccessToken);
            Assert.Equal("ref", response.RefreshToken);
            Assert.Equal(3600, response.ExpiresIn);
            Assert.Equal("org_1", response.OrganizationUid);
            Assert.Equal("user_1", response.UserUid);
        }

        #endregion

        #region OAuthTokens

        [Fact]
        public void IsExpired_WithDefaultExpiresAt_IsTrue()
        {
            var tokens = new OAuthTokens();

            Assert.True(tokens.IsExpired);
        }

        [Fact]
        public void IsExpired_WithPastExpiresAt_IsTrue()
        {
            var tokens = new OAuthTokens { ExpiresAt = DateTime.UtcNow.AddMinutes(-10) };

            Assert.True(tokens.IsExpired);
        }

        [Fact]
        public void IsExpired_WithFutureExpiresAt_IsFalse()
        {
            var tokens = new OAuthTokens { ExpiresAt = DateTime.UtcNow.AddHours(1) };

            Assert.False(tokens.IsExpired);
        }

        [Fact]
        public void NeedsRefresh_WithDefaultExpiresAt_IsTrue()
        {
            var tokens = new OAuthTokens();

            Assert.True(tokens.NeedsRefresh);
        }

        [Fact]
        public void NeedsRefresh_WithinFiveMinutesOfExpiry_IsTrue()
        {
            var tokens = new OAuthTokens { ExpiresAt = DateTime.UtcNow.AddMinutes(2) };

            Assert.True(tokens.NeedsRefresh);
        }

        [Fact]
        public void NeedsRefresh_WellBeforeExpiry_IsFalse()
        {
            var tokens = new OAuthTokens { ExpiresAt = DateTime.UtcNow.AddHours(1) };

            Assert.False(tokens.NeedsRefresh);
        }

        [Fact]
        public void NeedsRefresh_WhenSubtractingFiveMinutesUnderflowsDateTimeRange_IsTrue()
        {
            var tokens = new OAuthTokens { ExpiresAt = DateTime.MinValue.AddSeconds(1) };

            Assert.True(tokens.NeedsRefresh);
        }

        [Fact]
        public void IsValid_WithAccessTokenAndNotExpired_IsTrue()
        {
            var tokens = new OAuthTokens { AccessToken = "tok", ExpiresAt = DateTime.UtcNow.AddHours(1) };

            Assert.True(tokens.IsValid);
        }

        [Fact]
        public void IsValid_WithNoAccessToken_IsFalse()
        {
            var tokens = new OAuthTokens { AccessToken = null, ExpiresAt = DateTime.UtcNow.AddHours(1) };

            Assert.False(tokens.IsValid);
        }

        [Fact]
        public void IsValid_WhenExpired_IsFalse()
        {
            var tokens = new OAuthTokens { AccessToken = "tok", ExpiresAt = DateTime.UtcNow.AddMinutes(-1) };

            Assert.False(tokens.IsValid);
        }

        [Fact]
        public void OtherProperties_CanBeSetAndRetrieved()
        {
            var tokens = new OAuthTokens
            {
                RefreshToken = "refresh",
                OrganizationUid = "org_1",
                UserUid = "user_1",
                ClientId = "client_1",
                AppId = "app_1"
            };

            Assert.Equal("refresh", tokens.RefreshToken);
            Assert.Equal("org_1", tokens.OrganizationUid);
            Assert.Equal("user_1", tokens.UserUid);
            Assert.Equal("client_1", tokens.ClientId);
            Assert.Equal("app_1", tokens.AppId);
        }

        #endregion

        #region OAuthConfigurationException

        [Fact]
        public void OAuthConfigurationException_SetsMessage()
        {
            var exception = new OAuthConfigurationException("bad config");

            Assert.Equal("bad config", exception.Message);
            Assert.IsAssignableFrom<Exception>(exception);
        }

        #endregion
    }
}
