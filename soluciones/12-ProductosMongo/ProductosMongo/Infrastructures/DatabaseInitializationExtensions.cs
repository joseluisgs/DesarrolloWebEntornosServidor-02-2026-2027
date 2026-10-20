using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using ProductosMongo.Config;
using ProductosMongo.Entity;
using ProductosMongo.Seed;
using Serilog;

namespace ProductosMongo.Infrastructures;

/// <summary>
/// Extension methods para inicialización de base de datos y seed de datos.
/// En Development: borra y recrea datos limpios SIEMPRE.
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
            Log.Information("Producción: sin seed de datos. La base de datos se gestiona manualmente.");
            return;
        }

        using var scope = app.Services.CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var repositoryType = configuration["Productos:RepositoryType"] ?? "MongoDbNative";

        Log.Information("[DESARROLLO] Entorno de desarrollo detectado: se borraran y recrearan los datos.");

        if (repositoryType == "MongoDbNative")
        {
            // Driver Nativo: borrar colección y sembrar desde cero
            var mongoClient = scope.ServiceProvider.GetRequiredService<IMongoClient>();
            var settings = configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>()!;
            var database = mongoClient.GetDatabase(settings.DatabaseName);

            Log.Information("[Driver Nativo] Borrando colecciones 'proveedores' y 'productos', y sembrando datos frescos...");
            MongoDbSeeder.Seed(database);
            Log.Information("[Driver Nativo] 5 proveedores referenciados y 10 productos sembrados.");
        }
        else
        {
            // EF Core: borrar y recrear la base de datos, luego sembrar con el mismo modelo
            var context = scope.ServiceProvider.GetRequiredService<ProductosMongoDbContext>();

            Log.Information("[EF Core] Eliminando y recreando la base de datos...");
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            Log.Information("[EF Core] Sembrando proveedores y productos...");
            MongoDbSeeder.Seed(context);
            Log.Information("[EF Core] Base de datos recreada con 5 proveedores y 10 productos.");
        }

        Log.Information("Base de datos inicializada correctamente.");
    }
}
