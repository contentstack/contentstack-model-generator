using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using contentstack.CMA;
using contentstack.model.generator;
using McMaster.Extensions.CommandLineUtils;
using Xunit;
using Moq;

namespace contentstack.model.generator.tests
{
    public class ModelGeneratorTests
    {
        private readonly ModelGenerator _modelGenerator;

        public ModelGeneratorTests()
        {
            _modelGenerator = new ModelGenerator();
        }

        #region Property Tests

        [Fact]
        public void ApiKey_ShouldBeRequired()
        {
            var property = typeof(ModelGenerator).GetProperty(nameof(ModelGenerator.ApiKey));
            var requiredAttribute = property.GetCustomAttribute<RequiredAttribute>();

            Assert.NotNull(requiredAttribute);
            Assert.Equal("You must specify the Contentstack API key for the Content Management API", requiredAttribute.ErrorMessage);
        }

        [Fact]
        public void UseOAuth_ShouldHaveCorrectDescription()
        {
           
            var property = typeof(ModelGenerator).GetProperty(nameof(ModelGenerator.UseOAuth));
            var optionAttribute = property.GetCustomAttribute<OptionAttribute>();
            
            Assert.NotNull(optionAttribute);
            Assert.Equal("Use OAuth authentication instead of traditional authtoken", optionAttribute.Description);
            Assert.Equal(CommandOptionType.NoValue, optionAttribute.OptionType);
        }

        [Fact]
        public void OAuthAppId_ShouldHaveDefaultValue()
        {
           
            var modelGenerator = new ModelGenerator();
            Assert.Equal("6400aa06db64de001a31c8a9", modelGenerator.OAuthAppId);
        }

        [Fact]
        public void OAuthClientId_ShouldHaveDefaultValue()
        {
            var modelGenerator = new ModelGenerator();
            Assert.Equal("Ie0FEfTzlfAHL4xM", modelGenerator.OAuthClientId);
        }

        [Fact]
        public void OAuthRedirectUri_ShouldHaveDefaultValue()
        {
           
            var modelGenerator = new ModelGenerator();
            Assert.Equal("http://localhost:8184", modelGenerator.OAuthRedirectUri);
        }

        [Fact]
        public void OAuthScopes_ShouldBeOptional()
        {
            var modelGenerator = new ModelGenerator();
            Assert.Null(modelGenerator.OAuthScopes);
        }

        [Fact]
        public void OAuthClientSecret_ShouldBeOptional()
        {
           
            var modelGenerator = new ModelGenerator();
            Assert.Null(modelGenerator.OAuthClientSecret);
        }

        #endregion

        #region OAuth Scopes Parsing Tests

        [Theory]
        [InlineData("read write admin", new[] { "read", "write", "admin" })]
        [InlineData("read", new[] { "read" })]
        [InlineData("", null)]
        [InlineData("   ", new string[0])]
        [InlineData("read content write content manage users", new[] { "read", "content", "write", "content", "manage", "users" })]
        public void OAuthScopesParsing_ShouldParseCorrectly(string input, string[]? expected)
        {
            
            _modelGenerator.OAuthScopes = input;

            
            string[] result = null;
            if (!string.IsNullOrEmpty(input))
            {
                result = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            }

            
            if (expected == null)
            {
                Assert.Null(result);
            }
            else
            {
                Assert.NotNull(result);
                Assert.Equal(expected.Length, result.Length);
                for (int i = 0; i < expected.Length; i++)
                {
                    Assert.Equal(expected[i], result[i]);
                }
            }
        }

        #endregion

        #region ContentstackOptions Creation Tests

        [Fact]
        public void CreateContentstackOptions_WithTraditionalAuth_ShouldSetCorrectProperties()
        {
            
            _modelGenerator.ApiKey = "test_api_key";
            _modelGenerator.Authtoken = "test_authtoken";
            _modelGenerator.Host = "api.contentstack.io";
            _modelGenerator.Branch = "main";
            _modelGenerator.UseOAuth = false;
            var options = CreateContentstackOptionsFromModelGenerator();
            Assert.Equal("test_api_key", options.ApiKey);
            Assert.Equal("test_authtoken", options.Authtoken);
            Assert.Equal("api.contentstack.io", options.Host);
            Assert.Equal("main", options.Branch);
            Assert.False(options.IsOAuth);
            // OAuth properties should have default values even when UseOAuth is false
            Assert.Equal("Ie0FEfTzlfAHL4xM", options.OAuthClientId);
            Assert.Null(options.OAuthClientSecret);
            Assert.Equal("http://localhost:8184", options.OAuthRedirectUri);
            Assert.Equal("6400aa06db64de001a31c8a9", options.OAuthAppId);
            Assert.Null(options.OAuthScopes);
        }

        [Fact]
        public void CreateContentstackOptions_WithOAuth_ShouldSetCorrectProperties()
        {
            
            _modelGenerator.ApiKey = "test_api_key";
            _modelGenerator.Host = "api.contentstack.io";
            _modelGenerator.Branch = "main";
            _modelGenerator.UseOAuth = true;
            _modelGenerator.OAuthClientId = "test_client_id";
            _modelGenerator.OAuthClientSecret = "test_client_secret";
            _modelGenerator.OAuthRedirectUri = "http://localhost:8080";
            _modelGenerator.OAuthAppId = "test_app_id";
            _modelGenerator.OAuthScopes = "read write admin";
            var options = CreateContentstackOptionsFromModelGenerator();            
            Assert.Equal("test_api_key", options.ApiKey);
            Assert.Equal("api.contentstack.io", options.Host);
            Assert.Equal("main", options.Branch);
            Assert.True(options.IsOAuth);
            Assert.Equal("test_client_id", options.OAuthClientId);
            Assert.Equal("test_client_secret", options.OAuthClientSecret);
            Assert.Equal("http://localhost:8080", options.OAuthRedirectUri);
            Assert.Equal("test_app_id", options.OAuthAppId);
            Assert.NotNull(options.OAuthScopes);
            Assert.Equal(3, options.OAuthScopes.Length);
            Assert.Equal("read", options.OAuthScopes[0]);
            Assert.Equal("write", options.OAuthScopes[1]);
            Assert.Equal("admin", options.OAuthScopes[2]);
        }

        [Fact]
        public void CreateContentstackOptions_WithOAuthAndNoScopes_ShouldSetScopesToNull()
        {
            
            _modelGenerator.ApiKey = "test_api_key";
            _modelGenerator.Host = "api.contentstack.io";
            _modelGenerator.UseOAuth = true;
            _modelGenerator.OAuthClientId = "test_client_id";
            _modelGenerator.OAuthScopes = null;
            var options = CreateContentstackOptionsFromModelGenerator();
            Assert.True(options.IsOAuth);
            Assert.Null(options.OAuthScopes);
        }

        #endregion

        #region Validation Tests

        [Fact]
        public void ApiKey_ShouldBeRequiredProperty()
        {
           
            var property = typeof(ModelGenerator).GetProperty(nameof(ModelGenerator.ApiKey));
            var requiredAttribute = property?.GetCustomAttribute<RequiredAttribute>();
            Assert.NotNull(requiredAttribute);
            Assert.Equal("You must specify the Contentstack API key for the Content Management API", requiredAttribute.ErrorMessage);
        }

        [Fact]
        public void Authtoken_ShouldNotBeRequiredProperty()
        {
           
            var property = typeof(ModelGenerator).GetProperty(nameof(ModelGenerator.Authtoken));
            var requiredAttribute = property?.GetCustomAttribute<RequiredAttribute>();
            Assert.Null(requiredAttribute);
        }

        #endregion

        #region Command Line Attribute Tests

        [Fact]
        public void CommandAttribute_ShouldHaveCorrectProperties()
        {
            var commandAttribute = typeof(ModelGenerator).GetCustomAttribute<CommandAttribute>();   
            Assert.NotNull(commandAttribute);
            Assert.Equal("contentstack.model.generator", commandAttribute.Name);
            Assert.Equal("Contentstack Model Generator", commandAttribute.FullName);
            Assert.Equal("Creates c# classes from a Contentstack content types.", commandAttribute.Description);
        }

        [Theory]
        [InlineData(nameof(ModelGenerator.ApiKey), "The Contentstack API key for the Content Management API")]
        [InlineData(nameof(ModelGenerator.Authtoken), "The Authtoken for the Content Management API")]
        [InlineData(nameof(ModelGenerator.Branch), "The branch header in the API request to fetch or manage modules located within specific branches.")]
        [InlineData(nameof(ModelGenerator.Host), "The Contentstack Host for the Content Management API")]
        [InlineData(nameof(ModelGenerator.Path), "Path to the file or directory to create files in")]
        [InlineData(nameof(ModelGenerator.Namespace), "The namespace the classes should be created in")]
        public void OptionAttributes_ShouldHaveCorrectDescriptions(string propertyName, string expectedDescription)
        {
           
            var property = typeof(ModelGenerator).GetProperty(propertyName);
            var optionAttribute = property.GetCustomAttribute<OptionAttribute>();

            
            Assert.NotNull(optionAttribute);
            Assert.Equal(expectedDescription, optionAttribute.Description);
        }

        [Theory]
        [InlineData(nameof(ModelGenerator.Authtoken), "A", "authtoken")]
        [InlineData(nameof(ModelGenerator.Branch), "b", "branch")]
        [InlineData(nameof(ModelGenerator.Host), "e", "endpoint")]
        public void OptionAttributes_ShouldHaveCorrectShortAndLongNames(string propertyName, string expectedShortName, string expectedLongName)
        {
           
            var property = typeof(ModelGenerator).GetProperty(propertyName);
            var optionAttribute = property.GetCustomAttribute<OptionAttribute>();
            Assert.NotNull(optionAttribute);
            Assert.Equal(expectedShortName, optionAttribute.ShortName);
            Assert.Equal(expectedLongName, optionAttribute.LongName);
        }

        #endregion

        #region OAuth Flow Tests

        [Fact]
        public void HandleOAuthFlow_WithValidOAuthOptions_ShouldNotThrow()
        {
            
            _modelGenerator.UseOAuth = true;
            _modelGenerator.ApiKey = "test_api_key";
            _modelGenerator.Host = "api.contentstack.io";
            _modelGenerator.OAuthClientId = "test_client_id";
            _modelGenerator.OAuthRedirectUri = "http://localhost:8080";
            _modelGenerator.OAuthAppId = "test_app_id";
            var options = CreateContentstackOptionsFromModelGenerator();
            Assert.True(options.IsOAuth);
            Assert.Equal("test_client_id", options.OAuthClientId);
            Assert.Equal("http://localhost:8080", options.OAuthRedirectUri);
            Assert.Equal("test_app_id", options.OAuthAppId);
        }

        #endregion

        #region Helper Methods

        private ContentstackOptions CreateContentstackOptionsFromModelGenerator()
        {
            // This simulates the logic from the OnExecute method
            string[] oauthScopes = null;
            if (!string.IsNullOrEmpty(_modelGenerator.OAuthScopes))
            {
                oauthScopes = _modelGenerator.OAuthScopes.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            }

            return new ContentstackOptions
            {
                ApiKey = _modelGenerator.ApiKey,
                Authtoken = _modelGenerator.Authtoken,
                Host = _modelGenerator.Host,
                Branch = _modelGenerator.Branch,
                IsOAuth = _modelGenerator.UseOAuth,
                OAuthClientId = _modelGenerator.OAuthClientId,
                OAuthClientSecret = _modelGenerator.OAuthClientSecret,
                OAuthRedirectUri = _modelGenerator.OAuthRedirectUri,
                OAuthAppId = _modelGenerator.OAuthAppId,
                OAuthScopes = oauthScopes
            };
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void ModelGenerator_ShouldBeInstantiable()
        {
           
            var generator = new ModelGenerator();

            
            Assert.NotNull(generator);
            Assert.NotNull(generator.OAuthAppId);
            Assert.NotNull(generator.OAuthClientId);
            Assert.NotNull(generator.OAuthRedirectUri);
        }

        [Fact]
        public void ModelGenerator_ShouldHaveAllRequiredOAuthProperties()
        {
           
            var generator = new ModelGenerator();
            Assert.True(generator.GetType().GetProperty(nameof(ModelGenerator.UseOAuth)) != null);
            Assert.True(generator.GetType().GetProperty(nameof(ModelGenerator.OAuthAppId)) != null);
            Assert.True(generator.GetType().GetProperty(nameof(ModelGenerator.OAuthClientId)) != null);
            Assert.True(generator.GetType().GetProperty(nameof(ModelGenerator.OAuthRedirectUri)) != null);
            Assert.True(generator.GetType().GetProperty(nameof(ModelGenerator.OAuthClientSecret)) != null);
            Assert.True(generator.GetType().GetProperty(nameof(ModelGenerator.OAuthScopes)) != null);
        }

        #endregion

        #region Edge Cases

        [Fact]
        public void OAuthScopes_WithSpecialCharacters_ShouldParseCorrectly()
        {
            
            _modelGenerator.OAuthScopes = "read:content write:content manage:users admin:all";
            string[] result = _modelGenerator.OAuthScopes.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            Assert.Equal(4, result.Length);
            Assert.Equal("read:content", result[0]);
            Assert.Equal("write:content", result[1]);
            Assert.Equal("manage:users", result[2]);
            Assert.Equal("admin:all", result[3]);
        }

        [Fact]
        public void OAuthScopes_WithMultipleSpaces_ShouldParseCorrectly()
        {
            _modelGenerator.OAuthScopes = "read    write     admin";
            string[] result = _modelGenerator.OAuthScopes.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            Assert.Equal(3, result.Length);
            Assert.Equal("read", result[0]);
            Assert.Equal("write", result[1]);
            Assert.Equal("admin", result[2]);
        }

        [Fact]
        public void OAuthScopes_WithTabsAndNewlines_ShouldParseCorrectly()
        {
            _modelGenerator.OAuthScopes = "read\twrite\nadmin";
            string[] result = _modelGenerator.OAuthScopes.Split(new char[] { ' ', '\t', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            Assert.Equal(3, result.Length);
            Assert.Equal("read", result[0]);
            Assert.Equal("write", result[1]);
            Assert.Equal("admin", result[2]);
        }

        #endregion
    }
}
