using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using ProductosMicroService.Entity;
using ProductosMicroService.Models;
using ProductosMicroService.Repositories;

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
            ?? "Host=localhost;Port=5440;Database=productos_db;Username=admin;Password=admin123";

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

        if (!db.Productos.Any())
        {
            // Un producto referencia a un proveedor de MongoDB, asi que el id se pide
            // al microservicio de Proveedores. En el arranque en paralelo puede que todavia
            // no este listo, por eso se reintenta antes de rendirse.
            var proveedorId = await PrimerProveedorAsync(app);

            var electronica = db.Categorias.First(c => c.Nombre == "Electronica").Id;
            var mobiliario = db.Categorias.First(c => c.Nombre == "Mobiliario").Id;
            var perifericos = db.Categorias.First(c => c.Nombre == "Perifericos").Id;

            Log.Information("Seed: Creando productos de prueba...");
            db.Productos.AddRange(
                new Producto { Nombre = "Logitech MX Master 3S", Precio = 89.99m, CategoriaId = perifericos, ProveedorId = proveedorId, CreatedAt = DateTime.UtcNow },
                new Producto { Nombre = "Teclado mecanico RGB", Precio = 59.99m, CategoriaId = perifericos, ProveedorId = proveedorId, CreatedAt = DateTime.UtcNow },
                new Producto { Nombre = "Samsung Galaxy S24", Precio = 749.00m, CategoriaId = electronica, ProveedorId = proveedorId, CreatedAt = DateTime.UtcNow },
                new Producto { Nombre = "Apple MacBook Air M3", Precio = 1199.00m, CategoriaId = electronica, ProveedorId = proveedorId, CreatedAt = DateTime.UtcNow },
                new Producto { Nombre = "Xiaomi Redmi Note 13", Precio = 229.00m, CategoriaId = electronica, ProveedorId = proveedorId, CreatedAt = DateTime.UtcNow },
                new Producto { Nombre = "Mesa de trabajo ergonomica", Precio = 179.50m, CategoriaId = mobiliario, ProveedorId = proveedorId, CreatedAt = DateTime.UtcNow }
            );
            await db.SaveChangesAsync();
            Log.Information("Seed: 6 productos creados");
        }
    }

    /// <summary>
    /// Devuelve el id del primer proveedor que responda el microservicio de Proveedores.
    /// Si nadie responde, usa un ObjectId fijo para que la semilla no dependa del arranque.
    /// </summary>
    private static async Task<string> PrimerProveedorAsync(WebApplication app)
    {
        const string idPorDefecto = "65a100000000000000000001";

        for (var intento = 1; intento <= 10; intento++)
        {
            try
            {
                using var scope = app.Services.CreateScope();
                var api = scope.ServiceProvider.GetRequiredService<IProveedorApi>();
                var proveedores = await api.GetAll();
                if (proveedores is { Count: > 0 })
                {
                    var id = proveedores[0].Id;
                    if (!string.IsNullOrWhiteSpace(id))
                    {
                        Log.Information("Seed: Proveedor de referencia {Id} ({Nombre})", id, proveedores[0].Nombre);
                        return id;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Debug("Seed: Proveedores aun no responde (intento {Intento}/10): {Motivo}", intento, ex.Message);
            }

            await Task.Delay(1000);
        }

        Log.Warning("Seed: Proveedores no responde; se usara el id fijo {Id}. Los nombres de proveedor saldran como Desconocido.", idPorDefecto);
        return idPorDefecto;
    }
}
