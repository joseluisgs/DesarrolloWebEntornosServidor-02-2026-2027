using Microsoft.Extensions.DependencyInjection;
using Serilog;
using ProductosEFCore.Services;

namespace ProductosEFCore.Infrastructures;

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
        services.AddScoped<IAuthService, AuthService>();

        Log.Information("Servicios registrados correctamente.");
        return services;
    }
}
