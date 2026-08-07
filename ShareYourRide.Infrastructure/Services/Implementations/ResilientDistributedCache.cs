using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace ShareYourRide.Infrastructure.Services.Implementations
{
    /// <summary>
    /// Uses a primary distributed cache (Redis) and transparently falls back to a local
    /// in-memory cache when the Redis server is unreachable.
    /// </summary>
    public class ResilientDistributedCache : IDistributedCache
    {
        private readonly IDistributedCache _primary;
        private readonly IDistributedCache _fallback;
        private readonly ILogger<ResilientDistributedCache> _logger;

        public ResilientDistributedCache(
            IDistributedCache primary,
            IDistributedCache fallback,
            ILogger<ResilientDistributedCache> logger)
        {
            _primary = primary;
            _fallback = fallback;
            _logger = logger;
        }

        private static bool IsConnectivityError(Exception ex) =>
            ex is RedisConnectionException || ex is RedisTimeoutException || ex is TimeoutException;

        private T Execute<T>(Func<IDistributedCache, T> action)
        {
            try
            {
                return action(_primary);
            }
            catch (Exception ex) when (IsConnectivityError(ex))
            {
                _logger.LogWarning(ex, "Redis unavailable, using in-memory cache fallback.");
                return action(_fallback);
            }
        }

        private async Task<T> ExecuteAsync<T>(Func<IDistributedCache, Task<T>> action)
        {
            try
            {
                return await action(_primary);
            }
            catch (Exception ex) when (IsConnectivityError(ex))
            {
                _logger.LogWarning(ex, "Redis unavailable, using in-memory cache fallback.");
                return await action(_fallback);
            }
        }

        public byte[]? Get(string key) => Execute(c => c.Get(key));

        public Task<byte[]?> GetAsync(string key, CancellationToken token = default) =>
            ExecuteAsync(c => c.GetAsync(key, token));

        public void Refresh(string key) => Execute<object?>(c => { c.Refresh(key); return null; });

        public Task RefreshAsync(string key, CancellationToken token = default) =>
            ExecuteAsync<object?>(async c => { await c.RefreshAsync(key, token); return null; });

        public void Remove(string key) => Execute<object?>(c => { c.Remove(key); return null; });

        public Task RemoveAsync(string key, CancellationToken token = default) =>
            ExecuteAsync<object?>(async c => { await c.RemoveAsync(key, token); return null; });

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options) =>
            Execute<object?>(c => { c.Set(key, value, options); return null; });

        public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default) =>
            ExecuteAsync<object?>(async c => { await c.SetAsync(key, value, options, token); return null; });
    }
}
