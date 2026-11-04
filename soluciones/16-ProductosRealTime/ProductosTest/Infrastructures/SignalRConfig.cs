using Microsoft.Extensions.DependencyInjection;
using ProductosEFCore.Hubs;
using Serilog;

namespace ProductosEFCore.Infrastructures;

/// <summary>
/// Configuracion de SignalR para comunicacion en tiempo real.
/// </summary>
public static class SignalRConfig
{
    /// <summary>
    /// Registra SignalR y los Hubs en el contenedor de dependencias.
    /// </summary>
    /// <param name="services">Coleccion de servicios.</param>
    /// <returns>IServiceCollection para encadenar.</returns>
    public static IServiceCollection AddSignalRConfiguration(this IServiceCollection services)
    {
        Log.Information("Configurando SignalR...");

        services.AddSignalR(options =>
        {
            options.EnableDetailedErrors = true;  // Solo en desarrollo
            options.KeepAliveInterval = TimeSpan.FromSeconds(30);
            options.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
        });

        Log.Information("SignalR configurado correctamente.");
        return services;
    }
}
