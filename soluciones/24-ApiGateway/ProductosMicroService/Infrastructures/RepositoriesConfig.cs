using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using Serilog;
using ProductosMicroService.Cache;
using ProductosMicroService.Repositories;

namespace ProductosMicroService.Infrastructures;

/// <summary>
/// Configuracion de repositorios, cache y clientes remotos.
/// </summary>
public static class RepositoriesConfig
{
    /// <summary>
    /// Registra repositorios, cache Redis y clientes Refit.
    /// </summary>
    public static IServiceCollection AddRepositories(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        Log.Information("Registrando repositorios...");

        // Repositorios locales
        services.AddScoped<IProductoRepository, ProductoRepository>();
        services.AddScoped<ICategoriaRepository, CategoriaRepository>();

        // Repositorio remoto (Refit).
        // La URL no se escribe a mano: sale de la configuración, para que en local
        // apunte a localhost:5103 y en Docker al DNS del contenedor (red interna).
        var proveedoresBaseUrl = configuration["Proveedores:BaseUrl"] ?? "http://localhost:5103";
        services.AddScoped<IProveedorRemoteRepository, ProveedorRemoteRepository>();
        services.AddRefitClient<IProveedorApi>()
            .ConfigureHttpClient(c =>
            {
                c.BaseAddress = new Uri(proveedoresBaseUrl);
                c.Timeout = TimeSpan.FromSeconds(10);
            });

        // Log de demostración: dónde acaba la llamada interna a Proveedores.
        // En local es localhost:5103; en Docker, http://proveedores-microservice:5103.
        Log.Information("[Productos] Cliente Refit hacia Proveedores: {Url} · timeout de 10 s", proveedoresBaseUrl);

        // Cache distribuida (Redis)
        var redis = configuration.GetConnectionString("RedisConnection") ?? "localhost:6390";
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redis;
            options.InstanceName = "Productos_";
        });
        services.Configure<CacheOptions>(configuration.GetSection(CacheOptions.SectionName));
        services.AddScoped<ICacheService, DistributedCacheService>();

        // Log de demostración: dónde se guarda la caché de lecturas
        Log.Information("[Productos] Caché Redis: {Redis} (prefijo 'Productos_')", redis);

        Log.Information("Repositorios y cache registrados correctamente.");
        return services;
    }
}
