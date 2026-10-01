using ProductosMinimalApiDI.Repositories;

namespace ProductosMinimalApiDI.Infrastructure;

/// <summary>
/// Configuración de repositorios para Inyección de Dependencias.
/// </summary>
public static class RepositoriesConfig
{
    /// <summary>
    /// Registra los repositorios en el contenedor de DI.
    /// </summary>
    /// <param name="services">Colección de servicios.</param>
    /// <returns>Colección de servicios para encadenamiento.</returns>
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        // Singleton porque el repositorio usa Dictionary en memoria.
        // Si fuera Scoped, cada request perdería los datos.
        // En producción con BD real, sería AddScoped.
        services.AddSingleton<IProductoRepository, ProductoRepository>();
        return services;
    }
}
