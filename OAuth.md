## OAuth 2.0 Setup

### Prerequisites
1. **Contentstack Account**: You need a Contentstack account with appropriate permissions
2. **OAuth App**: Create an OAuth application in your Contentstack dashboard
3. **Redirect URI**: Configure a valid redirect URI (e.g., `http://localhost:8184`)

### OAuth Flow
1. **Authorization**: The tool displays the Contentstack OAuth authorization URL for you to open manually
2. **Authentication**: Open the URL in your browser, log in to your Contentstack account and authorize the application
3. **Callback**: You'll be redirected to your specified redirect URI with an authorization code
4. **Code Entry**: Copy the authorization code from the redirect URL and paste it into the tool
5. **Token Exchange**: The tool automatically exchanges the code for an access token
6. **Model Generation**: The tool fetches your content types and generates models
7. **Logout**: The tool automatically logs out and clears tokens

### Security Features
- **PKCE Support**: Uses Proof Key for Code Exchange for enhanced security
- **Client Secret Optional**: Supports both confidential and public clients
- **Automatic Token Management**: Handles token refresh and expiration
- **Secure Logout**: Automatically clears tokens after model generation

### Troubleshooting OAuth
- **Invalid Redirect URI**: Ensure the redirect URI matches exactly what's configured in your OAuth app
- **Client ID/Secret Issues**: Verify your OAuth app credentials
- **Network Issues**: Check your internet connection and Contentstack service status
- **Permission Issues**: Ensure your account has the necessary permissions for the stack
