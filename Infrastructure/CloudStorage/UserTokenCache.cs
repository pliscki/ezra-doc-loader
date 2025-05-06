using Microsoft.Extensions.Caching.Memory;
using Ezra.Scribe.Core.CloudStorage;

namespace Ezra.Scribe.Infrastructure.CloudStorage
{
    public class UserTokenCache : IUserTokenCache
    {
        private readonly IMemoryCache _memoryCache;

        public UserTokenCache(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public Task<string?> GetAccessTokenAsync(string userId, CancellationToken cancellationToken)
        {
            if (!_memoryCache.TryGetValue<(string AccessToken, DateTimeOffset ExpiresAt)>(userId, out var entry))
                return Task.FromResult<string?>(null);

            if (entry.ExpiresAt <= DateTimeOffset.UtcNow)
            {
                _memoryCache.Remove(userId);
                return Task.FromResult<string?>(null);
            }

            return Task.FromResult<string?>(entry.AccessToken);
        }

        public Task SetAccessTokenAsync(string userId, string accessToken, DateTimeOffset expiresAt, CancellationToken cancellationToken)
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = expiresAt
            };
            _memoryCache.Set(userId, (accessToken, expiresAt), options);
            return Task.CompletedTask;
        }
    }
}
