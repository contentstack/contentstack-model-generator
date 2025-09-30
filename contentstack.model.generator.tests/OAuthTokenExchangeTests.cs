using System;
using contentstack.CMA.OAuth;
using Xunit;

namespace contentstack.model.generator.tests
{
    public class OAuthTokenExchangeTests
    {
        #region OAuthTokenResponse Tests

        [Fact]
        public void OAuthTokenResponse_ShouldHaveCorrectProperties()
        {
            
            var response = new OAuthTokenResponse
            {
                AccessToken = "test_access_token",
                RefreshToken = "test_refresh_token",
                TokenType = "Bearer",
                ExpiresIn = 3600,
                OrganizationUid = "test_org_uid",
                UserUid = "test_user_uid"
            };

            
            Assert.Equal("test_access_token", response.AccessToken);
            Assert.Equal("test_refresh_token", response.RefreshToken);
            Assert.Equal("Bearer", response.TokenType);
            Assert.Equal(3600, response.ExpiresIn);
            Assert.Equal("test_org_uid", response.OrganizationUid);
            Assert.Equal("test_user_uid", response.UserUid);
        }

        [Fact]
        public void OAuthTokenResponse_ShouldHandleNullValues()
        {
            
            var response = new OAuthTokenResponse();

            
            Assert.Null(response.AccessToken);
            Assert.Null(response.RefreshToken);
            Assert.Null(response.TokenType);
            Assert.Equal(0, response.ExpiresIn);
            Assert.Null(response.OrganizationUid);
            Assert.Null(response.UserUid);
        }

        [Fact]
        public void OAuthTokenResponse_ShouldBeSerializable()
        {
            
            var response = new OAuthTokenResponse
            {
                AccessToken = "test_access_token",
                RefreshToken = "test_refresh_token",
                TokenType = "Bearer",
                ExpiresIn = 3600,
                OrganizationUid = "test_org_uid",
                UserUid = "test_user_uid"
            };

            
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(response);
            var deserializedResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<OAuthTokenResponse>(json);

            
            Assert.NotNull(deserializedResponse);
            Assert.Equal(response.AccessToken, deserializedResponse.AccessToken);
            Assert.Equal(response.RefreshToken, deserializedResponse.RefreshToken);
            Assert.Equal(response.TokenType, deserializedResponse.TokenType);
            Assert.Equal(response.ExpiresIn, deserializedResponse.ExpiresIn);
            Assert.Equal(response.OrganizationUid, deserializedResponse.OrganizationUid);
            Assert.Equal(response.UserUid, deserializedResponse.UserUid);
        }

        #endregion

        #region OAuth Flow Integration Notes
        #endregion
    }
}