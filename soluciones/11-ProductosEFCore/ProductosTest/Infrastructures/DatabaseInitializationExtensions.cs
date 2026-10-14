using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProductosEFCore.Entity;
using Serilog;

namespace ProductosEFCore.Infrastructures;

/// <summary>
/// Extension methods para inicialización de base de datos.
/// En Development: borra y recrea la base de datos con datos semilla (HasData).
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

        if (!isDevelopment)
        {
            Log.Information("Producción: sin migraciones automáticas. La base de datos se gestiona manualmente.");
            return;
        }

        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Log.Information("[DESARROLLO] Eliminando y recreando la base de datos...");
        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();
        Log.Information("[DESARROLLO] Base de datos recreada con datos semilla (HasData).");
    }
}
