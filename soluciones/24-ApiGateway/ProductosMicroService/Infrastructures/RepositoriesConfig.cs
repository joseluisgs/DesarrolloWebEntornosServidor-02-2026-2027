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

        // Repositorio remoto (Refit)
        services.AddScoped<IProveedorRemoteRepository, ProveedorRemoteRepository>();
        services.AddRefitClient<IProveedorApi>()
            .ConfigureHttpClient(c =>
            {
                c.BaseAddress = new Uri("http://localhost:5103");
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
