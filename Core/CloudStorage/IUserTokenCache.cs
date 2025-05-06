namespace Ezra.Scribe.Core.CloudStorage
{
    public interface IUserTokenCache
    {
        Task<string?> GetAccessTokenAsync(string userId, CancellationToken cancellationToken);
        Task SetAccessTokenAsync(string userId, string accessToken, DateTimeOffset expiresAt, CancellationToken cancellationToken);
    }
}
