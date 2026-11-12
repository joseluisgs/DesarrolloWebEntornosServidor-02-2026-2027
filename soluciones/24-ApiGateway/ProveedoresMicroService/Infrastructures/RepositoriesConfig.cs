using Microsoft.Extensions.DependencyInjection;
using Serilog;
using ProveedoresMicroService.Repositories;

namespace ProveedoresMicroService.Infrastructures;

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
        Log.Information("Registrando repositorios...");
        services.AddScoped<IProveedorRepository, ProveedorRepository>();
        Log.Information("Repositorios registrados correctamente.");
        return services;
    }
}
