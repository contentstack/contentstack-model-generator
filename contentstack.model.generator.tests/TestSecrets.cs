using System;

namespace contentstack.model.generator.tests
{
    /// <summary>
    /// Per-run generated values for tests that need credential-shaped inputs but must not
    /// commit literal secrets. None of these are real credentials.
    /// </summary>
    internal static class TestSecrets
    {
        /// <summary>Stack API key-shaped value shared by every test in this run.</summary>
        public static readonly string ApiKey = "blt" + Guid.NewGuid().ToString("N").Substring(0, 16);

        /// <summary>OAuth client secret-shaped value shared by every test in this run.</summary>
        public static readonly string ClientSecret = Guid.NewGuid().ToString("N");
    }
}
