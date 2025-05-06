namespace Ezra.Scribe.Core.CloudStorage
{
    public interface IUserTokenStore
    {
        Task<string?> GetRefreshTokenAsync(string userId, CancellationToken cancellationToken);
        Task StoreRefreshTokenAsync(string userId, string refreshToken, CancellationToken cancellationToken);
    }
}
