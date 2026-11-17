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

        // Cache distribuida (Redis)
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("RedisConnection")
                ?? "localhost:6390";
            options.InstanceName = "Productos_";
        });
        services.Configure<CacheOptions>(configuration.GetSection(CacheOptions.SectionName));
        services.AddScoped<ICacheService, DistributedCacheService>();

        Log.Information("Repositorios y cache registrados correctamente.");
        return services;
    }
}
