using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using Serilog;
using ProveedoresMicroService.Entity;
using ProveedoresMicroService.Models;

namespace ProveedoresMicroService.Infrastructures;

/// <summary>
/// Configuracion de la base de datos MongoDB.
/// </summary>
public static class DatabaseConfig
{
    /// <summary>
    /// Configura MongoDB con Entity Framework Core.
    /// </summary>
    public static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        Log.Information("Configurando MongoDB...");

        var connectionString = configuration.GetConnectionString("MongoConnection")
            ?? "mongodb://localhost:27020/proveedores_db";

        services.AddDbContextFactory<MongoDbContext>(options =>
            options.UseMongoDB(connectionString), ServiceLifetime.Scoped);

        Log.Information("MongoDB configurado correctamente.");
        return services;
    }

    /// <summary>
    /// Inicializa la base de datos y crea datos de prueba.
    /// </summary>
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<MongoDbContext>>();
        using var mongoCtx = factory.CreateDbContext();

        if (!mongoCtx.Proveedores.Any())
        {
            Log.Information("Seed: Creando proveedores de prueba...");
            // Id explícito por documento: evita conflicto de tracking con claves vacías
            mongoCtx.Proveedores.AddRange(
                new Proveedor { Id = ObjectId.GenerateNewId().ToString(), Nombre = "Logitech", Email = "contacto@logitech.com", Telefono = "+34 912 345 678", Direccion = "Calle Mayor 1, Madrid" },
                new Proveedor { Id = ObjectId.GenerateNewId().ToString(), Nombre = "Samsung", Email = "ventas@samsung.com", Telefono = "+34 913 456 789", Direccion = "Av. Industria 50, Barcelona" },
                new Proveedor { Id = ObjectId.GenerateNewId().ToString(), Nombre = "Apple", Email = "enterprise@apple.com", Telefono = "+34 900 123 456", Direccion = "Paseo de la Castellana 100, Madrid" },
                new Proveedor { Id = ObjectId.GenerateNewId().ToString(), Nombre = "Xiaomi", Email = "info@xiaomi.es", Telefono = "+34 915 678 901", Direccion = "Calle Gran Via 28, Madrid" }
            );
            await mongoCtx.SaveChangesAsync();
            Log.Information("Seed: 4 proveedores creados");
        }
    }
}
