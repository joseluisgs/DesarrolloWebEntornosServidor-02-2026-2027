using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace ProductosEFCore.Cache;

/// <summary>
/// Implementación de caché en memoria usando IMemoryCache.
/// Ideal para desarrollo y entornos single-server.
/// El TTL se configura desde appsettings.json.
/// </summary>
public class MemoryCacheService(
    IMemoryCache cache,
    IOptions<CacheOptions> options) : ICacheService
{
    private readonly CacheOptions _options = options.Value;

    /// <inheritdoc />
    public Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        cache.TryGetValue(key, out T? value);
        return Task.FromResult(value);
    }

    /// <inheritdoc />
    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken ct = default)
    {
        var cacheEntryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? _options.DefaultExpiration,
            SlidingExpiration = _options.SlidingExpiration
        };
        cache.Set(key, value, cacheEntryOptions);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task RemoveAsync(string key, CancellationToken ct = default)
    {
        cache.Remove(key);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<bool> ExistsAsync(string key, CancellationToken ct = default)
    {
        return Task.FromResult(cache.TryGetValue(key, out _));
    }

    /// <inheritdoc />
    public async Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> factory,
        TimeSpan? expiration = null, CancellationToken ct = default)
    {
        if (cache.TryGetValue(key, out T? value))
            return value;

        value = await factory();

        if (value is not null)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? _options.DefaultExpiration,
                SlidingExpiration = _options.SlidingExpiration
            };
            cache.Set(key, value, cacheEntryOptions);
        }

        return value;
    }
}
