using Microsoft.Extensions.DependencyInjection;
using Serilog;
using AuthMicroService.Services;

namespace AuthMicroService.Infrastructures;

/// <summary>
/// Configuracion de servicios de negocio.
/// </summary>
public static class ServicesConfig
{
    /// <summary>
    /// Registra los servicios de autenticacion.
    /// </summary>
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        Log.Information("Registrando servicios...");
        services.AddScoped<IAuthService, AuthService>();
        Log.Information("Servicios registrados correctamente.");
        return services;
    }
}
