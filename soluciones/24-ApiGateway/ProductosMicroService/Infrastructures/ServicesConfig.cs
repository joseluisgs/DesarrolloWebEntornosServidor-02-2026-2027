using Microsoft.Extensions.DependencyInjection;
using Serilog;
using ProductosMicroService.Services;

namespace ProductosMicroService.Infrastructures;

/// <summary>
/// Configuracion de servicios de negocio.
/// </summary>
public static class ServicesConfig
{
    /// <summary>
    /// Registra los servicios de productos.
    /// </summary>
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        Log.Information("Registrando servicios...");
        services.AddScoped<IProductoService, ProductoService>();
        Log.Information("Servicios registrados correctamente.");
        return services;
    }
}
