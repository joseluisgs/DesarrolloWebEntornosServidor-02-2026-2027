using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using ProductosEFCore.Cache;
using Serilog;

namespace ProductosEFCore.Infrastructures;

/// <summary>
/// Configuración de caché.
/// En desarrollo usa MemoryCache, en producción usa Redis.
/// El TTL se configura desde appsettings.json en la sección "Cache".
/// </summary>
public static class CacheConfig
{
    /// <summary>
    /// Registra el servicio de caché según el entorno.
    /// Development → MemoryCache (en memoria, sin persistencia).
    /// Producción → Redis (distribuido, persistente, compartido entre instancias).
    /// </summary>
    /// <param name="services">Colección de servicios.</param>
    /// <param name="configuration">Configuración de la app.</param>
    /// <param name="environment">Entorno de la aplicación.</param>
    /// <returns>IServiceCollection para encadenar.</returns>
    public static IServiceCollection AddCache(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        // Registrar opciones de caché desde appsettings.json
        services.Configure<CacheOptions>(
            configuration.GetSection(CacheOptions.SectionName));

        var cacheOptions = configuration
            .GetSection(CacheOptions.SectionName)
            .Get<CacheOptions>() ?? new CacheOptions();

        if (environment.IsDevelopment())
        {
            services.AddMemoryCache();
            services.AddSingleton<ICacheService, MemoryCacheService>();
            Log.Information("Caché: MemoryCache (Desarrollo) — TTL: {Minutes} min",
                cacheOptions.DefaultExpirationMinutes);
        }
        else
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration["Redis:ConnectionString"]
                    ?? "localhost:6379";
            });
            services.AddSingleton<ICacheService, RedisCacheService>();
            Log.Information("Caché: Redis (Producción) — TTL: {Minutes} min",
                cacheOptions.DefaultExpirationMinutes);
        }

        return services;
    }
}
