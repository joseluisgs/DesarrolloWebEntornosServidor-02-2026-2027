using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using ProductosEFCore.Entity;

namespace ProductosEFCore.Infrastructures;

/// <summary>
/// Configuración de la base de datos PostgreSQL.
/// </summary>
public static class DatabaseConfig
{
    /// <summary>
    /// Configura PostgreSQL con Entity Framework Core.
    /// </summary>
    /// <param name="services">Colección de servicios.</param>
    /// <param name="configuration">Configuración de la app.</param>
    /// <returns>IServiceCollection para encadenar.</returns>
    public static IServiceCollection AddDatabases(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        Log.Information("Configurando PostgreSQL...");

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Host=localhost;Port=5432;Database=productos_efcore;Username=postgres;Password=postgres";

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        Log.Information("PostgreSQL configurado correctamente.");
        return services;
    }
}
