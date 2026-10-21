using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ProductosEFCore.Entity;
using ProductosEFCore.Models;
using Testcontainers.PostgreSql;

namespace ProductosEFCore.Test;

/// <summary>
/// Base para tests de integración con PostgreSQL real usando TestContainers.
/// Cada fixture crea su propio contenedor efímero.
/// </summary>
public abstract class IntegrationTestBase
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:17-alpine")
        .WithDatabase("test_db")
        .WithUsername("test")
        .WithPassword("test")
        .Build();

    protected AppDbContext Db { get; private set; } = null!;

    [OneTimeSetUp]
    public async Task ClassSetUp()
    {
        await _postgres.StartAsync();
    }

    [OneTimeTearDown]
    public async Task ClassTearDown()
    {
        await _postgres.DisposeAsync();
    }

    [SetUp]
    public async Task SetUp()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(_postgres.GetConnectionString())
            .Options;

        Db = new AppDbContext(options);
        await Db.Database.EnsureCreatedAsync();

        await using var connection = new Npgsql.NpgsqlConnection(_postgres.GetConnectionString());
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = """
            TRUNCATE TABLE "Productos", "Categorias", "Proveedores" RESTART IDENTITY CASCADE;
            """;
        await command.ExecuteNonQueryAsync();
    }

    [TearDown]
    public async Task TearDown()
    {
        await Db.DisposeAsync();
    }

    protected async Task<Producto> CreateTestProducto(
        string nombre = "Test Product",
        decimal precio = 10.00m,
        Guid? categoriaId = null)
    {
        var producto = new Producto
        {
            Nombre = nombre,
            Precio = precio,
            CategoriaId = categoriaId ?? Guid.NewGuid(),
            Imagen = "https://picsum.photos/seed/test/300",
            CreatedAt = DateTime.UtcNow
        };

        Db.Productos.Add(producto);
        await Db.SaveChangesAsync();
        return producto;
    }

    protected async Task<Categoria> CreateTestCategoria(
        Guid? id = null,
        string nombre = "Test Category",
        string descripcion = "Test description")
    {
        var categoria = new Categoria
        {
            Id = id ?? Guid.NewGuid(),
            Nombre = nombre,
            Descripcion = descripcion
        };

        Db.Categorias.Add(categoria);
        await Db.SaveChangesAsync();
        return categoria;
    }
}
