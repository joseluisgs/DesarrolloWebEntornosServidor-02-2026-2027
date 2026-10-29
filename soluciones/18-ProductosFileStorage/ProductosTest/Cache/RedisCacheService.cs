using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace ProductosEFCore.Cache;

/// <summary>
/// Implementación de caché distribuida usando IDistributedCache (Redis).
/// Ideal para producción y entornos multi-server.
/// El TTL se configura desde appsettings.json.
/// </summary>
public class RedisCacheService(
    IDistributedCache cache,
    IOptions<CacheOptions> options) : ICacheService
{
    private readonly CacheOptions _options = options.Value;

    /// <inheritdoc />
    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        var bytes = await cache.GetAsync(key, ct);
        if (bytes is null) return default;

        return JsonSerializer.Deserialize<T>(bytes);
    }

    /// <inheritdoc />
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken ct = default)
    {
        var bytes = JsonSerializer.SerializeToUtf8Bytes(value);
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? _options.DefaultExpiration,
            SlidingExpiration = _options.SlidingExpiration
        };
        await cache.SetAsync(key, bytes, cacheOptions, ct);
    }

    /// <inheritdoc />
    public Task RemoveAsync(string key, CancellationToken ct = default)
    {
        return cache.RemoveAsync(key, ct);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsAsync(string key, CancellationToken ct = default)
    {
        var bytes = await cache.GetAsync(key, ct);
        return bytes is not null;
    }

    /// <inheritdoc />
    public async Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> factory,
        TimeSpan? expiration = null, CancellationToken ct = default)
    {
        var bytes = await cache.GetAsync(key, ct);
        if (bytes is not null)
            return JsonSerializer.Deserialize<T>(bytes);

        var value = await factory();

        if (value is not null)
        {
            var serialized = JsonSerializer.SerializeToUtf8Bytes(value);
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? _options.DefaultExpiration,
                SlidingExpiration = _options.SlidingExpiration
            };
            await cache.SetAsync(key, serialized, cacheOptions, ct);
        }

        return value;
    }
}
