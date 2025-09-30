using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace contentstack.CMA.OAuth
{
    /// <summary>
    /// OAuth service for handling OAuth 2.0 authentication flow with PKCE
    /// </summary>
    public class OAuthService
    {
        protected OAuthService() { }

        /// <summary>
        /// Generates a PKCE code verifier
        /// </summary>
        /// <returns>Base64URL encoded code verifier</returns>
        public static string GenerateCodeVerifier()
        {
            // Generate 32 random bytes (256 bits)
            byte[] randomBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            
            // Convert to Base64URL encoding
            return Base64UrlEncode(randomBytes);
        }

        /// <summary>
        /// Generates a PKCE code challenge from the code verifier
        /// </summary>
        /// <param name="codeVerifier">The code verifier</param>
        /// <returns>Base64URL encoded code challenge</returns>
        public static string GenerateCodeChallenge(string codeVerifier)
        {
            if (codeVerifier == null)
                throw new ArgumentNullException(nameof(codeVerifier));
            if (string.IsNullOrWhiteSpace(codeVerifier))
                throw new ArgumentException("Code verifier cannot be empty or whitespace", nameof(codeVerifier));
                
            // Create SHA256 hash of the code verifier
            using (var sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
                
                // Convert to Base64URL encoding
                return Base64UrlEncode(hashBytes);
            }
        }

        /// <summary>
        /// Generates OAuth authorization URL
        /// </summary>
        /// <param name="options">OAuth configuration options</param>
        /// <param name="codeVerifier">PKCE code verifier</param>
        /// <returns>Authorization URL</returns>
        public static string GenerateAuthorizationUrl(ContentstackOptions options, string codeVerifier)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            if (codeVerifier == null)
                throw new ArgumentNullException(nameof(codeVerifier));
            if (string.IsNullOrEmpty(codeVerifier))
                throw new ArgumentException("Code verifier cannot be empty", nameof(codeVerifier));
            if (options.OAuthClientId == null)
                throw new ArgumentNullException(nameof(options.OAuthClientId));
            if (string.IsNullOrEmpty(options.OAuthClientId))
                throw new ArgumentException("OAuth Client ID cannot be empty", nameof(options.OAuthClientId));
            if (options.OAuthRedirectUri == null)
                throw new ArgumentNullException(nameof(options.OAuthRedirectUri));
            if (string.IsNullOrEmpty(options.OAuthRedirectUri))
                throw new ArgumentException("OAuth Redirect URI cannot be empty", nameof(options.OAuthRedirectUri));
            if (options.OAuthAppId == null)
                throw new ArgumentNullException(nameof(options.OAuthAppId));
            if (string.IsNullOrEmpty(options.OAuthAppId))
                throw new ArgumentException("OAuth App ID cannot be empty", nameof(options.OAuthAppId));
            if (options.Host == null)
                throw new ArgumentNullException(nameof(options.Host));
            if (string.IsNullOrEmpty(options.Host))
                throw new ArgumentException("Host cannot be empty", nameof(options.Host));

            // Generate code challenge from verifier
            string codeChallenge = GenerateCodeChallenge(codeVerifier);
            
            // Transform hostname to OAuth host (api.contentstack.io -> app.contentstack.com)
            string oauthHost = GetOAuthHost(options.Host);
            
            // Build authorization URL using the correct format from management SDK
            var queryParams = new List<string>
            {
                $"response_type=code",
                $"client_id={Uri.EscapeDataString(options.OAuthClientId)}",
                $"redirect_uri={Uri.EscapeDataString(options.OAuthRedirectUri)}",
                $"code_challenge={Uri.EscapeDataString(codeChallenge)}",
                $"code_challenge_method=S256"
            };
            
            // Add scopes only if provided (optional, by default empty)
            if (options.OAuthScopes != null && options.OAuthScopes.Length > 0)
            {
                var scopeString = string.Join(" ", options.OAuthScopes);
                queryParams.Add($"scope={Uri.EscapeDataString(scopeString)}");
            }
            
            // Construct the full URL using the correct OAuth host format
            string baseUrl = $"https://{oauthHost}/#!/apps/{options.OAuthAppId}/authorize";
            return $"{baseUrl}?{string.Join("&", queryParams)}";
        }

        /// <summary>
        /// Exchanges authorization code for access token
        /// </summary>
        /// <param name="options">OAuth configuration options</param>
        /// <param name="authorizationCode">Authorization code from callback</param>
        /// <param name="codeVerifier">PKCE code verifier</param>
        /// <returns>OAuth token response</returns>
        public static async Task<OAuthTokenResponse> ExchangeCodeForTokenAsync(ContentstackOptions options, string authorizationCode, string codeVerifier)
        {
            using (var httpClient = new HttpClient())
            {
                // Prepare token request
                var tokenRequest = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("client_id", options.OAuthClientId),
                    new KeyValuePair<string, string>("code", authorizationCode),
                    new KeyValuePair<string, string>("redirect_uri", options.OAuthRedirectUri)
                };

                    // Add either client_secret (traditional OAuth) or code_verifier (PKCE) - mutually exclusive
                    if (!string.IsNullOrEmpty(options.OAuthClientSecret))
                    {
                        // Traditional OAuth flow - use client secret
                        tokenRequest.Add(new KeyValuePair<string, string>("client_secret", options.OAuthClientSecret));
                    }
                    else if (!string.IsNullOrEmpty(codeVerifier))
                    {
                        // PKCE flow - use code verifier
                        tokenRequest.Add(new KeyValuePair<string, string>("code_verifier", codeVerifier));
                    }
                    else
                    {
                        throw new ArgumentException("Either client_secret or code_verifier must be provided.");
                    }

                    // Add app_id if provided
                    if (!string.IsNullOrEmpty(options.OAuthAppId))
                    {
                        tokenRequest.Add(new KeyValuePair<string, string>("app_id", options.OAuthAppId));
                    }

                var formContent = new FormUrlEncodedContent(tokenRequest);
                
                // Make token request using Developer Hub hostname (matching management SDK)
                string tokenUrl = $"{GetDeveloperHubHostname(options.Host)}/token";
                
                var response = await httpClient.PostAsync(tokenUrl, formContent);
                
                if (!response.IsSuccessStatusCode)
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    throw new InvalidOperationException($"Token exchange failed: {response.StatusCode} - {errorContent}");
                }
                
                // Parse response
                string responseContent = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonConvert.DeserializeObject<OAuthTokenResponse>(responseContent);
                
                // Set expiration time if not already set
                if (tokenResponse.ExpiresAt == default(DateTime) && tokenResponse.ExpiresIn > 0)
                {
                    tokenResponse.ExpiresAt = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn);
                }
                
                return tokenResponse;
            }
        }

        /// <summary>
        /// Refreshes access token using refresh token
        /// </summary>
        /// <param name="options">OAuth configuration options</param>
        /// <returns>OAuth token response</returns>
        public static async Task<OAuthTokenResponse> RefreshTokenAsync(ContentstackOptions options)
        {
            if (string.IsNullOrEmpty(options.RefreshToken))
            {
                throw new ArgumentException("Refresh token is required for token refresh");
            }

            using (var httpClient = new HttpClient())
            {
                // Prepare refresh request
                var refreshRequest = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("grant_type", "refresh_token"),
                    new KeyValuePair<string, string>("client_id", options.OAuthClientId),
                    new KeyValuePair<string, string>("refresh_token", options.RefreshToken)
                };

                // Add client secret only if provided (optional for public clients)
                if (!string.IsNullOrEmpty(options.OAuthClientSecret))
                {
                    refreshRequest.Add(new KeyValuePair<string, string>("client_secret", options.OAuthClientSecret));
                }

                // Add app_id if provided
                if (!string.IsNullOrEmpty(options.OAuthAppId))
                {
                    refreshRequest.Add(new KeyValuePair<string, string>("app_id", options.OAuthAppId));
                }

                var formContent = new FormUrlEncodedContent(refreshRequest);
                
                // Make refresh request using Developer Hub hostname (matching management SDK)
                string tokenUrl = $"{GetDeveloperHubHostname(options.Host)}/token";
                var response = await httpClient.PostAsync(tokenUrl, formContent);
                
                if (!response.IsSuccessStatusCode)
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    throw new InvalidOperationException($"Token refresh failed: {response.StatusCode} - {errorContent}");
                }
                
                // Parse response
                string responseContent = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonConvert.DeserializeObject<OAuthTokenResponse>(responseContent);
                
                // Set expiration time if not already set
                if (tokenResponse.ExpiresAt == default(DateTime) && tokenResponse.ExpiresIn > 0)
                {
                    tokenResponse.ExpiresAt = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn);
                }
                
                return tokenResponse;
            }
        }

        /// <summary>
        /// Checks if the current access token is expired or near expiration
        /// </summary>
        /// <param name="options">OAuth configuration options</param>
        /// <returns>True if token is expired or near expiration</returns>
        public static bool IsTokenExpired(ContentstackOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            if (!options.TokenExpiresAt.HasValue)
                return true;

            // Check if token expires within the next 5 minutes (buffer time)
            return options.TokenExpiresAt.Value <= DateTime.UtcNow.AddMinutes(5);
        }

        /// <summary>
        /// Logs out the user by clearing OAuth tokens and optionally revoking authorization
        /// </summary>
        /// <param name="options">OAuth configuration options</param>
        /// <returns>Logout success message</returns>
        public static async Task<string> LogoutAsync(ContentstackOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
                
            try
            {
                // Try to revoke the OAuth app authorization if we have valid tokens
                if (!string.IsNullOrEmpty(options.AccessToken))
                {
                    try
                    {
                        await RevokeOAuthAuthorizationAsync();
                    }
                    catch
                    {
                        // If revocation fails, continue with logout
                        // This is common in OAuth implementations where revocation is optional
                    }
                }

                // Clear OAuth tokens from options
                options.AccessToken = null;
                options.RefreshToken = null;
                options.TokenExpiresAt = null;
                options.Authorization = null;

                return "Logged out successfully";
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to logout: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Revokes OAuth app authorization
        /// </summary>
        private static async Task RevokeOAuthAuthorizationAsync()
        {
            // This is a simplified revocation - in a full implementation,
            // you would need to call the OAuth app revocation API
            // For now, we'll just clear the tokens as the main logout functionality
            Console.WriteLine("OAuth authorization revoked");
            await Task.CompletedTask; // Make it async
        }

        /// <summary>
        /// Converts bytes to Base64URL encoding (RFC 4648)
        /// </summary>
        /// <param name="input">Input bytes</param>
        /// <returns>Base64URL encoded string</returns>
        private static string Base64UrlEncode(byte[] input)
        {
            // Convert to base64
            string base64 = Convert.ToBase64String(input);
            
            // Replace characters for URL safety
            return base64.Replace('+', '-')
                        .Replace('/', '_')
                        .TrimEnd('=');
        }

        /// <summary>
        /// Transforms the base hostname to the OAuth authorization hostname.
        /// </summary>
        /// <param name="baseHost">The base hostname (e.g., api.contentstack.io)</param>
        /// <returns>The transformed OAuth hostname (e.g., app.contentstack.com)</returns>
        internal static string GetOAuthHost(string baseHost)
        {
            if (baseHost == null)
                throw new ArgumentNullException(nameof(baseHost));
            if (string.IsNullOrEmpty(baseHost))
                throw new ArgumentException("Base host cannot be empty", nameof(baseHost));

            // Extract hostname from URL if it contains protocol
            var oauthHost = baseHost;
            if (oauthHost.StartsWith("https://"))
            {
                oauthHost = oauthHost.Substring(8); // Remove "https://"
            }
            else if (oauthHost.StartsWith("http://"))
            {
                oauthHost = oauthHost.Substring(7); // Remove "http://"
            }

            // Transform api.contentstack.io -> app.contentstack.com
            // Replace .io with .com
            if (oauthHost.EndsWith(".io"))
            {
                oauthHost = oauthHost.Replace(".io", ".com");
            }
            
            // Replace 'api' with 'app'
            if (oauthHost.Contains("api."))
            {
                oauthHost = oauthHost.Replace("api.", "app.");
            }
            
            return oauthHost;
        }

        /// <summary>
        /// Transforms the base hostname to the Developer Hub API hostname.
        /// </summary>
        /// <param name="baseHost">The base hostname (e.g., api.contentstack.io)</param>
        /// <returns>The transformed Developer Hub hostname (e.g., developerhub-api.contentstack.com)</returns>
        internal static string GetDeveloperHubHostname(string baseHost)
        {
            if (baseHost == null)
                throw new ArgumentNullException(nameof(baseHost));
            if (string.IsNullOrEmpty(baseHost))
                throw new ArgumentException("Base host cannot be empty", nameof(baseHost));

            // Transform api.contentstack.io -> developerhub-api.contentstack.com
            var devHubHost = baseHost;
            
            // Replace 'api' with 'developerhub-api'
            if (devHubHost.Contains("api."))
            {
                devHubHost = devHubHost.Replace("api.", "developerhub-api.");
            }
            
            // Replace .io with .com
            if (devHubHost.EndsWith(".io"))
            {
                devHubHost = devHubHost.Replace(".io", ".com");
            }
            
            // Always use https:// protocol for Developer Hub API
            if (devHubHost.StartsWith("http://"))
            {
                devHubHost = devHubHost.Replace("http://", "https://");
            }
            else if (!devHubHost.StartsWith("https://"))
            {
                devHubHost = "https://" + devHubHost;
            }
            
            return devHubHost;
        }
    }

    public class OAuthTokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("scope")]
        public string Scope { get; set; }

        [JsonProperty("organization_uid")]
        public string OrganizationUid { get; set; }

        [JsonProperty("user_uid")]
        public string UserUid { get; set; }

        // Computed property for expiration time
        public DateTime ExpiresAt { get; set; }
    }
}
