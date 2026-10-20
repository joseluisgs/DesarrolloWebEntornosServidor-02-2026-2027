using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductosMongo.Repositories;
using ProductosMongo.Repositories.EfCore;
using ProductosMongo.Repositories.Native;
using Serilog;

namespace ProductosMongo.Infrastructures;

/// <summary>
/// Configuración de repositorios.
/// Registra la implementación según el tipo de repositorio configurado.
/// </summary>
public static class RepositoriesConfig
{
    /// <summary>
    /// Registra el repositorio de productos en el contenedor de dependencias.
    /// </summary>
    /// <param name="services">Colección de servicios.</param>
    /// <param name="configuration">Configuración de la app.</param>
    /// <returns>IServiceCollection para encadenar.</returns>
    public static IServiceCollection AddRepositories(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var repositoryType = configuration["Productos:RepositoryType"] ?? "MongoDbNative";
        Log.Information("Registrando repositorios: {RepositoryType}", repositoryType);

        if (repositoryType == "MongoDbNative")
        {
            services.AddScoped<IProductoRepository, ProductoNativeRepository>();
        }
        else if (repositoryType == "MongoDbEfCore")
        {
            services.AddScoped<IProductoRepository, ProductoEfCoreRepository>();
        }

        Log.Information("Repositorios registrados correctamente.");
        return services;
    }
}
