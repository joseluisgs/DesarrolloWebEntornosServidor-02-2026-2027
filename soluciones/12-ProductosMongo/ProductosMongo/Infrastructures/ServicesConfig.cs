using Microsoft.Extensions.DependencyInjection;
using ProductosMongo.Services;
using Serilog;

namespace ProductosMongo.Infrastructures;

/// <summary>
/// Configuración de servicios de negocio.
/// </summary>
public static class ServicesConfig
{
    /// <summary>
    /// Registra todos los servicios de negocio en el contenedor de dependencias.
    /// </summary>
    /// <param name="services">Colección de servicios.</param>
    /// <returns>IServiceCollection para encadenar.</returns>
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        Log.Information("Registrando servicios...");

        services.AddScoped<IProductoService, ProductoService>();

        Log.Information("Servicios registrados correctamente.");
        return services;
    }
}
