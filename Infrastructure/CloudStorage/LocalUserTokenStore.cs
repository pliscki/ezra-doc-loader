using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Ezra.Scribe.Core.CloudStorage;

namespace Ezra.Scribe.Infrastructure.CloudStorage;

public class LocalUserTokenStore : IUserTokenStore
{
    private readonly ConcurrentDictionary<string, string> _refreshTokens = new();

    public Task<string?> GetRefreshTokenAsync(string userId, CancellationToken cancellationToken)
    {
        if (_refreshTokens.TryGetValue(userId, out var token))
            return Task.FromResult<string?>(token);
        return Task.FromResult<string?>(null);
    }

    public Task StoreRefreshTokenAsync(string userId, string refreshToken, CancellationToken cancellationToken)
    {
        _refreshTokens[userId] = refreshToken;
        return Task.CompletedTask;
    }
}
