using System.Threading.Tasks;
using contentstack.model.generator;
using Xunit;

namespace contentstack.model.generator.tests
{
    /// <summary>
    /// Program.Main() is the real CLI entry point. These tests only exercise
    /// argument-validation paths that return before any network call is made
    /// (missing --api-key / missing --authtoken), so they run safely offline.
    /// </summary>
    public class ProgramTests
    {
        [Fact]
        public async Task Main_WithNoArguments_ReturnsNonZeroExitCode()
        {
            var result = await Program.Main(new string[0]);

            Assert.NotEqual(Program.OK, result);
        }

        [Fact]
        public async Task Main_WithApiKeyButNoAuthTokenOrOAuth_ReturnsErrorCode()
        {
            var result = await Program.Main(new[] { "--api-key", "test_key" });

            Assert.Equal(Program.ERROR, result);
        }

        [Fact]
        public void ExitCodeConstants_HaveExpectedValues()
        {
            Assert.Equal(0, Program.OK);
            Assert.Equal(1, Program.ERROR);
            Assert.Equal(2, Program.EXCEPTION);
        }
    }
}
