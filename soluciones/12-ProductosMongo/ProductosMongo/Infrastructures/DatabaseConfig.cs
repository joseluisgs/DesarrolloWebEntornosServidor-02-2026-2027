using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using ProductosMongo.Config;
using ProductosMongo.Entity;
using ProductosMongo.Models;
using Serilog;

namespace ProductosMongo.Infrastructures;

/// <summary>
/// Configuración de bases de datos.
/// Soporta Driver Nativo y EF Core según la clave de configuración Productos:RepositoryType.
/// </summary>
public static class DatabaseConfig
{
    /// <summary>
    /// Registra los servicios de base de datos según el tipo de repositorio configurado.
    /// </summary>
    /// <param name="services">Colección de servicios.</param>
    /// <param name="configuration">Configuración de la app.</param>
    /// <returns>IServiceCollection para encadenar.</returns>
    public static IServiceCollection AddDatabases(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var repositoryType = configuration["Productos:RepositoryType"] ?? "MongoDbNative";
        Log.Information("Configurando base de datos: {RepositoryType}", repositoryType);

        var settings = configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>()!;

        if (repositoryType == "MongoDbNative")
        {
            Log.Information("Usando Driver Nativo de MongoDB");
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            services.AddSingleton<IMongoClient>(client);
            services.AddSingleton<IMongoDatabase>(database);
            services.AddSingleton<IMongoCollection<Producto>>(database.GetCollection<Producto>("productos"));
        }
        else if (repositoryType == "MongoDbEfCore")
        {
            Log.Information("Usando Entity Framework Core con MongoDB");
            services.AddDbContext<ProductosMongoDbContext>(options =>
                options.UseMongoDB(settings.ConnectionString, settings.DatabaseName));
        }

        Log.Information("Base de datos configurada correctamente.");
        return services;
    }
}
