using Microsoft.Extensions.DependencyInjection;
using ProductosControllersApiDI.Repositories;

namespace ProductosControllersApiDI.Infrastructure;

/// <summary>
/// Configuración de repositorios.
/// </summary>
public static class RepositoriesConfig
{
    /// <summary>
    /// Registra los repositorios en el contenedor de dependencias.
    /// </summary>
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        // Singleton porque el repositorio usa Dictionary en memoria.
        // Si fuera Scoped, cada request perdería los datos.
        // En producción con BD real, sería AddScoped.
        services.AddSingleton<IProductoRepository, ProductoRepository>();
        return services;
    }
}
