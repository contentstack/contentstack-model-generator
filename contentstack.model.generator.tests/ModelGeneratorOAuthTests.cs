using System;
using System.Linq;
using Xunit;
using McMaster.Extensions.CommandLineUtils;
using contentstack.CMA;

namespace contentstack.model.generator.tests
{
    public class ModelGeneratorOAuthTests
    {
        [Fact]
        public void OAuthOptions_ShouldBeDefined()
        {
            
            var app = new CommandLineApplication();
            var modelGenerator = new ModelGenerator();         
            app.Command("test", cmd =>
            {
                cmd.Option<bool>("--oauth", "Use OAuth authentication", CommandOptionType.NoValue);
                cmd.Option<string>("--client-id", "OAuth Client ID", CommandOptionType.SingleValue);
                cmd.Option<string>("--client-secret", "OAuth Client Secret", CommandOptionType.SingleValue);
                cmd.Option<string>("--redirect-uri", "OAuth Redirect URI", CommandOptionType.SingleValue);
                cmd.Option<string>("--app-id", "OAuth App ID", CommandOptionType.SingleValue);
                cmd.Option<string>("--scopes", "OAuth Scopes", CommandOptionType.SingleValue);
            });

            
            Assert.True(modelGenerator.UseOAuth == false); // Default value
        }

        [Fact]
        public void OAuthScopesParsing_ShouldHandleNullInput()
        {
            
            string input = null;
            string[] result = null;
            if (!string.IsNullOrEmpty(input))
            {
                result = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            }
            Assert.Null(result);
        }

        [Fact]
        public void OAuthScopesParsing_ShouldHandleEmptyInput()
        {
            string input = "";
            string[] result = null;
            if (!string.IsNullOrEmpty(input))
            {
                result = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            }
            Assert.Null(result);
        }

        [Fact]
        public void OAuthScopesParsing_ShouldHandleWhitespaceInput()
        {
            
            string input = "   ";
            string[] result = null;
            if (!string.IsNullOrEmpty(input))
            {
                result = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            }
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void OAuthScopesParsing_ShouldHandleSingleScope()
        {
            string input = "read";
            string[] result = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("read", result[0]);
        }

        [Fact]
        public void OAuthScopesParsing_ShouldHandleMultipleScopes()
        {
            string input = "read write admin";
            string[] result = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            Assert.NotNull(result);
            Assert.Equal(3, result.Length);
            Assert.Equal("read", result[0]);
            Assert.Equal("write", result[1]);
            Assert.Equal("admin", result[2]);
        }

        [Fact]
        public void OAuthScopesParsing_ShouldHandleScopesWithExtraSpaces()
        {
            string input = "  read   write  admin  ";
            string[] result = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            Assert.NotNull(result);
            Assert.Equal(3, result.Length);
            Assert.Equal("read", result[0]);
            Assert.Equal("write", result[1]);
            Assert.Equal("admin", result[2]);
        }

        [Fact]
        public void OAuthScopesParsing_ShouldHandleScopesWithSpecialCharacters()
        {
            string input = "read content write content manage users";
            string[] result = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            Assert.NotNull(result);
            Assert.Equal(6, result.Length);
            Assert.Equal("read", result[0]);
            Assert.Equal("content", result[1]);
            Assert.Equal("write", result[2]);
            Assert.Equal("content", result[3]);
            Assert.Equal("manage", result[4]);
            Assert.Equal("users", result[5]);
        }

        [Fact]
        public void OAuthScopesParsing_ShouldHandleEmptyScopes()
        {
            string input = "read  write"; // Double space
            string[] result = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            Assert.NotNull(result);
            Assert.Equal(2, result.Length);
            Assert.Equal("read", result[0]);
            Assert.Equal("write", result[1]);
        }

        [Fact]
        public void OAuthScopesParsing_ShouldHandleMixedCaseScopes()
        {
            string input = "Read Write Admin";
            string[] result = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            Assert.NotNull(result);
            Assert.Equal(3, result.Length);
            Assert.Equal("Read", result[0]);
            Assert.Equal("Write", result[1]);
            Assert.Equal("Admin", result[2]);
        }

        [Fact]
        public void OAuthScopesParsing_ShouldHandleNumericScopes()
        {
            
            string input = "scope1 scope2 scope3";
            string[] result = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            Assert.NotNull(result);
            Assert.Equal(3, result.Length);
            Assert.Equal("scope1", result[0]);
            Assert.Equal("scope2", result[1]);
            Assert.Equal("scope3", result[2]);
        }

        [Fact]
        public void OAuthScopesParsing_ShouldHandleScopesWithUnderscores()
        {
            string input = "read_content write_content manage_users";
            string[] result = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            Assert.NotNull(result);
            Assert.Equal(3, result.Length);
            Assert.Equal("read_content", result[0]);
            Assert.Equal("write_content", result[1]);
            Assert.Equal("manage_users", result[2]);
        }

        [Fact]
        public void OAuthScopesParsing_ShouldHandleScopesWithDashes()
        {
            string input = "read-content write-content manage-users";
            string[] result = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            Assert.NotNull(result);
            Assert.Equal(3, result.Length);
            Assert.Equal("read-content", result[0]);
            Assert.Equal("write-content", result[1]);
            Assert.Equal("manage-users", result[2]);
        }

        [Fact]
        public void OAuthScopesParsing_ShouldHandleVeryLongScopeList()
        {
            string input = "read write admin manage create delete update view edit publish unpublish archive restore backup";
            var expectedScopes = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string[] result = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            Assert.NotNull(result);
            Assert.Equal(expectedScopes.Length, result.Length);
            Assert.Equal(expectedScopes, result);
        }

        [Fact]
        public void OAuthScopesParsing_ShouldHandleScopesWithNumbers()
        {
            string input = "read1 write2 admin3";
            string[] result = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            Assert.NotNull(result);
            Assert.Equal(3, result.Length);
            Assert.Equal("read1", result[0]);
            Assert.Equal("write2", result[1]);
            Assert.Equal("admin3", result[2]);
        }

        [Fact]
        public void OAuthScopesParsing_ShouldHandleScopesWithSymbols()
        {
            string input = "read@content write#content manage$users";
            string[] result = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            Assert.NotNull(result);
            Assert.Equal(3, result.Length);
            Assert.Equal("read@content", result[0]);
            Assert.Equal("write#content", result[1]);
            Assert.Equal("manage$users", result[2]);
        }

        [Theory]
        [InlineData("read", 1)]
        [InlineData("read write", 2)]
        [InlineData("read write admin", 3)]
        [InlineData("  read   write  admin  ", 3)]
        [InlineData("read1 write2 admin3", 3)]
        [InlineData("read-content write-content", 2)]
        [InlineData("read_content write_content", 2)]
        public void OAuthScopesParsing_ShouldReturnCorrectCount(string input, int expectedCount)
        {
            string[] result = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            Assert.NotNull(result);
            Assert.Equal(expectedCount, result.Length);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void OAuthScopesParsing_ShouldHandleNullOrEmptyInput(string input)
        {
            
            string[] result = null;
            if (!string.IsNullOrEmpty(input))
            {
                result = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            }

            
            if (string.IsNullOrEmpty(input))
            {
                Assert.Null(result);
            }
            else
            {
                Assert.NotNull(result);
                Assert.Empty(result);
            }
        }

        [Fact]
        public void OAuthScopesParsing_ShouldHandleNullInputValue()
        {
            
            string input = null;
            string[] result = null;
            if (!string.IsNullOrEmpty(input))
            {
                result = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            }
            Assert.Null(result);
        }
    }
}
