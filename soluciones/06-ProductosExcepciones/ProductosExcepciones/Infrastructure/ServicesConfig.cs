using Microsoft.Extensions.DependencyInjection;
using ProductosExcepciones.Services;

namespace ProductosExcepciones.Infrastructure;

/// <summary>
/// Configuración de servicios.
/// </summary>
public static class ServicesConfig
{
    /// <summary>
    /// Registra los servicios en el contenedor de dependencias.
    /// </summary>
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IProductoService, ProductoService>();
        return services;
    }
}
