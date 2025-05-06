using Ezra.Scribe.Core.CloudStorage.GoogleDrive;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Auth.OAuth2.Flows;
using Microsoft.Extensions.Options;
using Ezra.Scribe.Core.CloudStorage;

namespace Ezra.Scribe.Infrastructure.GoogleDrive
{
    public class GoogleDriveAuthService : IGoogleDriveAuthService
    {
        private readonly IUserTokenCache _tokenCache;
        private readonly IUserTokenStore _tokenStore;
        private readonly GoogleDriveSettings _settings;

        public GoogleDriveAuthService(
            IUserTokenCache tokenCache,
            IUserTokenStore tokenStore,
            IOptions<GoogleDriveSettings> options)
        {
            _tokenCache = tokenCache;
            _tokenStore = tokenStore;
            _settings = options.Value;

            if (string.IsNullOrWhiteSpace(_settings.ClientId))
                throw new ArgumentException("GoogleDriveSettings.ClientId must be provided.", nameof(_settings.ClientId));
            if (string.IsNullOrWhiteSpace(_settings.ClientSecret))
                throw new ArgumentException("GoogleDriveSettings.ClientSecret must be provided.", nameof(_settings.ClientSecret));
        }

        public async Task<GoogleCredential> GetGoogleCredentialsAsync(string userId, CancellationToken cancellationToken)
        {
            var accessToken = await _tokenCache.GetAccessTokenAsync(userId, cancellationToken).ConfigureAwait(false);
            if (!string.IsNullOrEmpty(accessToken))
                return GoogleCredential.FromAccessToken(accessToken);

            var refreshToken = await _tokenStore.GetRefreshTokenAsync(userId, cancellationToken).ConfigureAwait(false);
            if (string.IsNullOrEmpty(refreshToken))
                throw new InvalidOperationException("No refresh token found for user.");

            var token = await RefreshAccessTokenAsync(userId, refreshToken, cancellationToken).ConfigureAwait(false);
            if (string.IsNullOrEmpty(token.AccessToken))
                throw new InvalidOperationException("Failed to refresh access token.");

            await CacheAccessTokenAsync(userId, token.AccessToken, token.ExpiresInSeconds, cancellationToken).ConfigureAwait(false);

            return GoogleCredential.FromAccessToken(token.AccessToken);
        }

        private async Task<TokenResponse> RefreshAccessTokenAsync(string userId, string refreshToken, CancellationToken cancellationToken)
        {
            var initializer = new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = _settings.ClientId,
                    ClientSecret = _settings.ClientSecret
                },                
            };
            var flow = new GoogleAuthorizationCodeFlow(initializer);
            return await flow.RefreshTokenAsync(userId, refreshToken, cancellationToken).ConfigureAwait(false);
        }

        private async Task CacheAccessTokenAsync(string userId, string accessToken, long? expiresInSeconds, CancellationToken cancellationToken)
        {
            var expiresAt = DateTimeOffset.UtcNow.AddSeconds(expiresInSeconds ?? 3600);
            await _tokenCache.SetAccessTokenAsync(userId, accessToken, expiresAt, cancellationToken).ConfigureAwait(false);
        }

        public Task RevokeRefreshTokenAsync(string userId, CancellationToken cancellationToken)
        {
            // TODO: Implement token revocation logic if needed
            throw new NotImplementedException();
        }
    }
}
