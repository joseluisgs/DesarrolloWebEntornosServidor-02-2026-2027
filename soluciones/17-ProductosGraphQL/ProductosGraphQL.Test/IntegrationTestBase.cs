using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ProductosGraphQL.Entity;
using ProductosGraphQL.Models;
using Testcontainers.PostgreSql;

namespace ProductosGraphQL.Test;

/// <summary>
/// Base para tests de integracion con PostgreSQL real usando TestContainers.
/// Cada fixture crea su propio contenedor efimero.
/// </summary>
public abstract class IntegrationTestBase
{
    protected readonly PostgreSqlContainer Postgres = new PostgreSqlBuilder()
        .WithImage("postgres:17-alpine")
        .WithDatabase("test_db")
        .WithUsername("test")
        .WithPassword("test")
        .Build();

    protected AppDbContext Db { get; private set; } = null!;

    [OneTimeSetUp]
    public async Task ClassSetUp()
    {
        await Postgres.StartAsync();
    }

    [OneTimeTearDown]
    public async Task ClassTearDown()
    {
        await Postgres.DisposeAsync();
    }

    [SetUp]
    public async Task SetUp()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(Postgres.GetConnectionString())
            .Options;

        Db = new AppDbContext(options);
        await Db.Database.EnsureCreatedAsync();

        // Limpiar la BD antes de cada test
        await using var connection = new Npgsql.NpgsqlConnection(Postgres.GetConnectionString());
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = """
            TRUNCATE TABLE "Productos", "Categorias",
            "AspNetUsers", "AspNetRoles", "AspNetUserRoles",
            "AspNetUserClaims", "AspNetUserLogins",
            "AspNetUserTokens", "AspNetRoleClaims" RESTART IDENTITY CASCADE;
            """;
        await command.ExecuteNonQueryAsync();
    }

    [TearDown]
    public async Task TearDown()
    {
        await Db.DisposeAsync();
    }

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
}
