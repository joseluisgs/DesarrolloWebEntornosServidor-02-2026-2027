using Microsoft.Extensions.DependencyInjection;
using Serilog;
using ProductosEFCore.Repositories;

namespace ProductosEFCore.Infrastructures;

/// <summary>
/// Configuración de repositorios.
/// </summary>
public static class RepositoriesConfig
{
    /// <summary>
    /// Registra todos los repositorios en el contenedor de dependencias.
    /// </summary>
    /// <param name="services">Colección de servicios.</param>
    /// <returns>IServiceCollection para encadenar.</returns>
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        Log.Information("Registrando repositorios...");

        services.AddScoped<IProductoRepository, ProductoRepository>();
        services.AddScoped<ICategoriaRepository, CategoriaRepository>();
        services.AddScoped<IProveedorRepository, ProveedorRepository>();

        Log.Information("Repositorios registrados correctamente.");
        return services;
    }
}
