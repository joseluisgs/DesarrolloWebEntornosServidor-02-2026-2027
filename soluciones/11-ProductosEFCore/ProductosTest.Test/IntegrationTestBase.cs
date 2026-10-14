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

        // Limpiar la BD antes de cada test para evitar duplicados
        await using var connection = new Npgsql.NpgsqlConnection(_postgres.GetConnectionString());
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = """
            TRUNCATE TABLE "Productos", "Categorias" RESTART IDENTITY CASCADE;
            """;
        await command.ExecuteNonQueryAsync();
    }

    [TearDown]
    public async Task TearDown()
    {
        await Db.DisposeAsync();
    }

    /// <summary>
    /// Inserta un producto directamente en la BD para tests.
    /// </summary>
    protected async Task<Producto> CreateTestProducto(
        string nombre = "Test Product",
        decimal precio = 10.00m,
        long categoriaId = 1)
    {
        var producto = new Producto
        {
            Nombre = nombre,
            Precio = precio,
            CategoriaId = categoriaId,
            Imagen = "https://picsum.photos/seed/test/300",
            CreatedAt = DateTime.UtcNow
        };

        Db.Productos.Add(producto);
        await Db.SaveChangesAsync();
        return producto;
    }

    /// <summary>
    /// Inserta una categoría directamente en la BD para tests.
    /// </summary>
    protected async Task<Categoria> CreateTestCategoria(
        string nombre = "Test Category",
        string descripcion = "Test description")
    {
        var categoria = new Categoria
        {
            Nombre = nombre,
            Descripcion = descripcion
        };

        Db.Categorias.Add(categoria);
        await Db.SaveChangesAsync();
        return categoria;
    }
}
