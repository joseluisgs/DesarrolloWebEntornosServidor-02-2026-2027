using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Serilog;

namespace ProductosMicroService.Cache;

/// <summary>
/// Implementacion de cache distribuida con Redis (IDistributedCache).
/// </summary>
public class DistributedCacheService(IDistributedCache cache, IOptions<CacheOptions> options) : ICacheService
{
    private readonly TimeSpan _expiration = options.Value.DefaultExpiration;

    /// <inheritdoc />
    public async Task<T?> GetAsync<T>(string key)
    {
        try
        {
            var data = await cache.GetStringAsync(key);
            if (data is null) return default;

            Log.Debug("Cache HIT: {Key}", key);
            return JsonSerializer.Deserialize<T>(data);
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Error leyendo cache {Key}", key);
            return default;
        }
    }

    /// <inheritdoc />
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        try
        {
            var data = JsonSerializer.Serialize(value);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? _expiration
            };
            await cache.SetStringAsync(key, data, options);
            Log.Debug("Cache SET: {Key}", key);
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Error guardando cache {Key}", key);
        }
    }

    /// <inheritdoc />
    public async Task RemoveAsync(string key)
    {
        try
        {
            await cache.RemoveAsync(key);
            Log.Debug("Cache REMOVE: {Key}", key);
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Error eliminando cache {Key}", key);
        }
    }
}
