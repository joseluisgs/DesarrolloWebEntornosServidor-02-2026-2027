namespace ProductosMicroService.Cache;

/// <summary>
/// Interfaz del servicio de cache.
/// </summary>
public interface ICacheService
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
    Task RemoveAsync(string key);
}

/// <summary>
/// Opciones de configuracion de cache.
/// </summary>
public class CacheOptions
{
    public const string SectionName = "Cache";
    public int DefaultExpirationMinutes { get; set; } = 30;
    public TimeSpan DefaultExpiration => TimeSpan.FromMinutes(DefaultExpirationMinutes);
}
