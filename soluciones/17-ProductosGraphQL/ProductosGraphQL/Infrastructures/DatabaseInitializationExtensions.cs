using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProductosGraphQL.Entity;
using ProductosGraphQL.Seed;
using Serilog;

namespace ProductosGraphQL.Infrastructures;

/// <summary>
/// Extension methods para inicialización de base de datos.
/// En Development: borra y recrea la base de datos con datos semilla (HasData)
/// y siembra los roles y usuarios de Identity.
/// En Producción: no realiza ninguna operación.
/// </summary>
public static class DatabaseInitializationExtensions
{
    /// <summary>
    /// Inicializa la base de datos según el entorno.
    /// </summary>
    /// <param name="app">WebApplication.</param>
    /// <param name="isDevelopment">Indica si el entorno es de desarrollo.</param>
    /// <returns>Task.</returns>
    public static async Task InitializeDatabaseAsync(this WebApplication app, bool isDevelopment)
    {
        Log.Information("Inicializando base de datos...");

        using var scope = app.Services.CreateScope();

        if (!isDevelopment)
        {
            Log.Information("Producción: sin migraciones automáticas.");
            return;
        }

        // Desarrollo: recrear BD completa
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Log.Information("[DESARROLLO] Eliminando y recreando la base de datos...");
        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();
        Log.Information("[DESARROLLO] Base de datos recreada con datos semilla (HasData).");

        // Roles y usuarios de Identity: sin ellos el registro no puede asignar
        // rol y las mutaciones AdminOnly no tendrían ningún usuario autorizado.
        await IdentitySeeder.SeedAsync(scope.ServiceProvider);
    }
}
