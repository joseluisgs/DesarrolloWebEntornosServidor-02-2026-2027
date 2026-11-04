using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using ProductosMicroService.Entity;
using ProductosMicroService.Models;

namespace ProductosMicroService.Infrastructures;

/// <summary>
/// Configuracion de la base de datos PostgreSQL.
/// </summary>
public static class DatabaseConfig
{
    /// <summary>
    /// Configura PostgreSQL con Entity Framework Core.
    /// </summary>
    public static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        Log.Information("Configurando PostgreSQL...");

        var connectionString = configuration.GetConnectionString("PostgresConnection")
            ?? "Host=localhost;Database=productos_db;Username=admin;Password=admin123";

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        Log.Information("PostgreSQL configurado correctamente.");
        return services;
    }

    /// <summary>
    /// Inicializa la base de datos y crea datos de prueba.
    /// </summary>
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await db.Database.EnsureCreatedAsync();

        if (!db.Categorias.Any())
        {
            Log.Information("Seed: Creando categorias de prueba...");
            db.Categorias.AddRange(
                new Categoria { Nombre = "Electronica", Descripcion = "Dispositivos electronicos" },
                new Categoria { Nombre = "Mobiliario", Descripcion = "Muebles y accesorios" },
                new Categoria { Nombre = "Perifericos", Descripcion = "Teclados, ratones, etc." }
            );
            await db.SaveChangesAsync();
            Log.Information("Seed: 3 categorias creadas");
        }
    }
}
