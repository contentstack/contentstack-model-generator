using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using contentstack.model.generator.Model;

namespace Contentstack.Model.Generator.Model
{
    /// <summary>
    /// Represents the response from the OAuth app authorization API.
    /// </summary>
    public class OAuthAppAuthorizationResponse
    {

        [JsonProperty("data")]
        public OAuthAppAuthorizationData[] Data { get; set; }
    }

    /// <summary>
    /// Represents OAuth app authorization data.
    /// </summary>
    public class OAuthAppAuthorizationData
    {

        [JsonProperty("authorization_uid")]
        public string AuthorizationUid { get; set; }


        [JsonProperty("user")]
        public OAuthUser User { get; set; }
    }


    public class OAuthUser
    {

        [JsonProperty("uid")]
        public string Uid { get; set; }
    }

    /// <summary>
    /// Configuration options for OAuth authentication.
    /// </summary>
    public class OAuthOptions
    {
        /// <summary>
        /// The OAuth application ID. Defaults to the Contentstack app ID.
        /// </summary>
        public string AppId { get; set; } = "6400aa06db64de001a31c8a9";

        /// <summary>
        /// The OAuth client ID. Defaults to the Contentstack client ID.
        /// </summary>
        public string ClientId { get; set; } = "Ie0FEfTzlfAHL4xM";

        /// <summary>
        /// The redirect URI for OAuth callbacks. Defaults to localhost:8184.
        /// </summary>
        public string RedirectUri { get; set; } = "http://localhost:8184";

        /// <summary>
        /// The OAuth client secret. If provided, PKCE flow will be skipped.
        /// If null or empty, PKCE flow will be used for enhanced security.
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// The OAuth response type. Defaults to "code" for authorization code flow.
        /// </summary>
        public string ResponseType { get; set; } = "code";

        /// <summary>
        /// The OAuth scopes to request. Optional array of permission scopes.
        /// </summary>
        public string[] Scope { get; set; }

        /// <summary>
        /// Indicates whether PKCE (Proof Key for Code Exchange) flow should be used.
        /// This is automatically determined based on whether ClientSecret is provided.
        /// </summary>
        public bool UsePkce => string.IsNullOrEmpty(ClientSecret);

        /// <summary>
        /// Validates the OAuth options configuration.
        /// </summary>
        /// <returns>True if the configuration is valid, false otherwise.</returns>
        public bool IsValid()
        {
            return IsValid(out _);
        }

        /// <summary>
        /// Validates the OAuth options configuration and provides detailed error information.
        /// </summary>
        /// <param name="errorMessage">The validation error message if validation fails.</param>
        /// <returns>True if the configuration is valid, false otherwise.</returns>
        public bool IsValid(out string errorMessage)
        {
            errorMessage = null;

            if (string.IsNullOrWhiteSpace(AppId))
            {
                errorMessage = "AppId is required for OAuth configuration.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(ClientId))
            {
                errorMessage = "ClientId is required for OAuth configuration.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(RedirectUri))
            {
                errorMessage = "RedirectUri is required for OAuth configuration.";
                return false;
            }

            if (!Uri.TryCreate(RedirectUri, UriKind.Absolute, out var redirectUri))
            {
                errorMessage = "RedirectUri must be a valid absolute URI.";
                return false;
            }

            if (redirectUri.Scheme != "http" && redirectUri.Scheme != "https")
            {
                errorMessage = "RedirectUri must use http or https scheme.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(ResponseType))
            {
                errorMessage = "ResponseType is required for OAuth configuration.";
                return false;
            }

            if (ResponseType != "code")
            {
                errorMessage = "ResponseType must be 'code' for authorization code flow.";
                return false;
            }

            // For traditional OAuth flow (non-PKCE), client secret is required
            if (!UsePkce && string.IsNullOrWhiteSpace(ClientSecret))
            {
                errorMessage = "ClientSecret is required for traditional OAuth flow. Use PKCE flow (leave ClientSecret empty) for public clients.";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validates the OAuth options configuration and throws an exception if invalid.
        /// </summary>
        /// <exception cref="OAuthConfigurationException">Thrown when the configuration is invalid.</exception>
        public void Validate()
        {
            if (!IsValid(out var errorMessage))
            {
                throw new OAuthConfigurationException(errorMessage);
            }
        }

        /// <summary>
        /// Gets a string representation of the OAuth options for debugging.
        /// </summary>
        /// <returns>A string representation of the OAuth options.</returns>
        public override string ToString()
        {
            return $"OAuthOptions: AppId={AppId}, ClientId={ClientId}, RedirectUri={RedirectUri}, " +
                   $"ResponseType={ResponseType}, UsePkce={UsePkce}, HasScope={Scope?.Length > 0}";
        }
    }

    /// <summary>
    /// Represents the response from OAuth token exchange operations.
    /// </summary>
    public class OAuthResponse
    {

        [JsonProperty("access_token")]
        public string AccessToken { get; set; }


        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }


        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }


        [JsonProperty("organization_uid")]
        public string OrganizationUid { get; set; }


        [JsonProperty("user_uid")]
        public string UserUid { get; set; }
    }
    /// <summary>
    /// Represents OAuth tokens stored in memory for cross-SDK access.
    /// This class enables sharing OAuth tokens between the Management SDK and other SDKs
    /// </summary>
    public class OAuthTokens
    {

        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public DateTime ExpiresAt { get; set; }

        public string OrganizationUid { get; set; }

        public string UserUid { get; set; }

        public string ClientId { get; set; }

        public string AppId { get; set; }

        public bool IsExpired => ExpiresAt == DateTime.MinValue || DateTime.UtcNow >= ExpiresAt;

        public bool NeedsRefresh
        {
            get
            {
                // If ExpiresAt is not set or is MinValue, consider it expired
                if (ExpiresAt == DateTime.MinValue)
                    return true;

                try
                {
                    // Check if we need to refresh (5 minutes before expiration)
                    var refreshTime = ExpiresAt.AddMinutes(-5);
                    return DateTime.UtcNow >= refreshTime || IsExpired;
                }
                catch (ArgumentOutOfRangeException)
                {
                    // If the calculation results in an unrepresentable DateTime, consider it expired
                    return true;
                }
            }
        }

        public bool IsValid => !string.IsNullOrEmpty(AccessToken) && !IsExpired;
    }

    /// <summary>
    /// Thrown when OAuth configuration validation fails in <see cref="OAuthOptions.Validate"/>.
    /// </summary>
    public class OAuthConfigurationException : Exception
    {
        public OAuthConfigurationException(string message) : base(message)
        {
        }
    }
}
