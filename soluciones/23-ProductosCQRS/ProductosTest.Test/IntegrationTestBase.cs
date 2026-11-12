using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ProductosCQRS.Entity;
using ProductosCQRS.Entity.Mongo;
using ProductosCQRS.Models;
using Testcontainers.MongoDb;
using Testcontainers.PostgreSql;

namespace ProductosCQRS.Test;

/// <summary>
/// Base para tests de integracion con PostgreSQL y MongoDB reales usando TestContainers.
/// </summary>
public abstract class IntegrationTestBase
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:17-alpine")
        .WithDatabase("test_db")
        .WithUsername("test")
        .WithPassword("test")
        .Build();

    private readonly MongoDbContainer _mongo = new MongoDbBuilder()
        .WithImage("mongo:7.0")
        .Build();

    protected AppDbContext Db { get; private set; } = null!;
    protected string ConnectionString { get; private set; } = string.Empty;
    protected string MongoConnectionString { get; private set; } = string.Empty;

    [OneTimeSetUp]
    public async Task ClassSetUp()
    {
        await _postgres.StartAsync();
        await _mongo.StartAsync();
        ConnectionString = _postgres.GetConnectionString();
        MongoConnectionString = _mongo.GetConnectionString();
    }

    [OneTimeTearDown]
    public async Task ClassTearDown()
    {
        await _postgres.DisposeAsync();
        await _mongo.DisposeAsync();
    }

    [SetUp]
    public async Task SetUp()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        Db = new AppDbContext(options);
        await Db.Database.EnsureCreatedAsync();

        // Limpiar la BD antes de cada test
        await using var connection = new Npgsql.NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = """
            TRUNCATE TABLE "Productos", "Categorias",
            "AspNetUsers", "AspNetRoles", "AspNetUserRoles",
            "AspNetUserClaims", "AspNetUserLogins",
            "AspNetUserTokens", "AspNetRoleClaims" RESTART IDENTITY CASCADE;
            """;
        await command.ExecuteNonQueryAsync();

        // Sembrar datos base
        await CreateTestCategoria("Electronica", "Dispositivos");
        await CreateTestCategoria("Mobiliario", "Muebles");
    }

    [TearDown]
    public async Task TearDown()
    {
        await Db.DisposeAsync();
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
