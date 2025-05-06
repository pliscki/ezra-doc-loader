using Google.Apis.Auth.OAuth2;

namespace Ezra.Scribe.Core.CloudStorage.GoogleDrive
{
    public interface IGoogleDriveAuthService
    {
        /// <summary>
        /// Gets a valid GoogleCredential for the specified user, refreshing the access token if necessary.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">A required cancellation token.</param>
        /// <returns>A valid GoogleCredential instance.</returns>
        Task<GoogleCredential> GetGoogleCredentialsAsync(string userId, CancellationToken cancellationToken);

        /// <summary>
        /// Revokes the refresh token for the specified user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">A required cancellation token.</param>
        Task RevokeRefreshTokenAsync(string userId, CancellationToken cancellationToken);
    }
}
