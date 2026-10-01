using ProductosMinimalApiDI.Services;

namespace ProductosMinimalApiDI.Infrastructure;

/// <summary>
/// Configuración de servicios para Inyección de Dependencias.
/// </summary>
public static class ServicesConfig
{
    /// <summary>
    /// Registra los servicios en el contenedor de DI.
    /// </summary>
    /// <param name="services">Colección de servicios.</param>
    /// <returns>Colección de servicios para encadenamiento.</returns>
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IProductoService, ProductoService>();
        return services;
    }
}
