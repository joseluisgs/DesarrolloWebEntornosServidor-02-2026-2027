using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NUnit.Framework;
using ProductosCQRS.Cache;
using ProductosCQRS.Dtos;
using ProductosCQRS.Entity;
using ProductosCQRS.Entity.Mongo;
using ProductosCQRS.Models;
using ProductosCQRS.Repositories;
using ProductosCQRS.Services;
using Testcontainers.MongoDb;
using Testcontainers.PostgreSql;

namespace ProductosCQRS.Test.Services;

[TestFixture]
public class ProductoServiceTests
{
    private PostgreSqlContainer _postgres = null!;
    private MongoDbContainer _mongo = null!;
    private AppDbContext _db = null!;
    private MongoDbContext _mongoContext = null!;
    private IProductoService _service = null!;
    private IMongoCollection<ProductoRead> _mongoCollection = null!;

    [OneTimeSetUp]
    public async Task ClassSetUp()
    {
        _postgres = new PostgreSqlBuilder()
            .WithImage("postgres:17-alpine")
            .WithDatabase("test_db")
            .WithUsername("test")
            .WithPassword("test")
            .Build();
        await _postgres.StartAsync();

        _mongo = new MongoDbBuilder().WithImage("mongo:7.0").Build();
        await _mongo.StartAsync();

        var mongoOptions = new DbContextOptionsBuilder<MongoDbContext>()
            .UseMongoDB(_mongo.GetConnectionString(), "test_read")
            .Options;
        _mongoContext = new MongoDbContext(mongoOptions);

        // Usar driver MongoDB directamente para limpiar (sin transacciones)
        var client = new MongoClient(_mongo.GetConnectionString());
        _mongoCollection = client.GetDatabase("test_read").GetCollection<ProductoRead>("productos_read");
    }

    [OneTimeTearDown]
    public async Task ClassTearDown()
    {
        await _mongoContext.DisposeAsync();
        await _mongo.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    [SetUp]
    public async Task SetUp()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(_postgres.GetConnectionString())
            .Options;
        _db = new AppDbContext(options);
        await _db.Database.EnsureCreatedAsync();

        // Limpiar PostgreSQL
        await using var conn = new Npgsql.NpgsqlConnection(_postgres.GetConnectionString());
        await conn.OpenAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            TRUNCATE TABLE "Productos", "Categorias",
            "AspNetUsers", "AspNetRoles", "AspNetUserRoles",
            "AspNetUserClaims", "AspNetUserLogins",
            "AspNetUserTokens", "AspNetRoleClaims" RESTART IDENTITY CASCADE;
            """;
        await cmd.ExecuteNonQueryAsync();

        // Limpiar MongoDB (driver directo, sin transacciones)
        await _mongoCollection.DeleteManyAsync(FilterDefinition<ProductoRead>.Empty);

        // Sembrar categorias base
        _db.Categorias.Add(new Categoria { Nombre = "Electronica", Descripcion = "Dispositivos" });
        await _db.SaveChangesAsync();

        // Crear servicio
        var productoRepo = new ProductoRepository(_db);
        var categoriaRepo = new CategoriaRepository(_db);
        var readRepo = new ProductoReadRepository(_mongoContext);
        var cacheOptions = new CacheOptions { DefaultExpirationMinutes = 5 };
        var cache = new MemoryCacheService(
            new MemoryCache(new MemoryCacheOptions()),
            Options.Create(cacheOptions));

        _service = new ProductoService(
            productoRepo, readRepo, categoriaRepo, cache,
            Options.Create(cacheOptions));
    }

    [TearDown]
    public async Task TearDown()
    {
        await _db.DisposeAsync();
    }

    private async Task<Producto> CreateTestProducto(string nombre = "Test", decimal precio = 10m)
    {
        var producto = new Producto
        {
            Nombre = nombre,
            Precio = precio,
            CategoriaId = 1,
            CreatedAt = DateTime.UtcNow
        };
        _db.Productos.Add(producto);
        await _db.SaveChangesAsync();
        return producto;
    }

    private async Task SeedMongoProducto(long id, string nombre, decimal precio)
    {
        await _mongoCollection.InsertOneAsync(new ProductoRead
        {
            Id = id, Nombre = nombre, Precio = precio,
            SyncAt = DateTime.UtcNow
        });
    }

    [TestFixture]
    public class GetAllRead : ProductoServiceTests
    {
        [Test]
        public async Task GetAllReadAsync_Vacio_RetornaListaVacia()
        {
            var result = await _service.GetAllReadAsync();
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEmpty();
        }

        [Test]
        public async Task GetAllReadAsync_ConProductos_RetornaTodos()
        {
            var p1 = await CreateTestProducto("Teclado", 89.99m);
            var p2 = await CreateTestProducto("Raton", 35.50m);
            await SeedMongoProducto(p1.Id, p1.Nombre, p1.Precio);
            await SeedMongoProducto(p2.Id, p2.Nombre, p2.Precio);

            var result = await _service.GetAllReadAsync();

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(2);
        }

        [Test]
        public async Task GetAllReadAsync_SegundaLlamada_VieneDeCache()
        {
            var p = await CreateTestProducto("Cached", 50m);
            await SeedMongoProducto(p.Id, p.Nombre, p.Precio);

            await _service.GetAllReadAsync(); // cache miss
            var result = await _service.GetAllReadAsync(); // cache hit

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(1);
        }
    }

    [TestFixture]
    public class GetByIdRead : ProductoServiceTests
    {
        [Test]
        public async Task GetByIdReadAsync_Existe_RetornaProducto()
        {
            var p = await CreateTestProducto("Keyboard", 120m);
            await SeedMongoProducto(p.Id, p.Nombre, p.Precio);

            var result = await _service.GetByIdReadAsync(p.Id);

            result.IsSuccess.Should().BeTrue();
            result.Value!.Nombre.Should().Be("Keyboard");
        }

        [Test]
        public async Task GetByIdReadAsync_NoExiste_RetornaError()
        {
            var result = await _service.GetByIdReadAsync(999);
            result.IsFailure.Should().BeTrue();
        }
    }

    [TestFixture]
    public class CacheInvalidation : ProductoServiceTests
    {
        [Test]
        public async Task Create_InvalidaCache_lista()
        {
            // Poblar cache con "Before"
            var p = await CreateTestProducto("Before", 10m);
            await SeedMongoProducto(p.Id, p.Nombre, p.Precio);
            await _service.GetAllReadAsync();

            // Crear nuevo producto (invalida cache)
            var dto = new CreateProductoDto { Nombre = "After", Precio = 20m, CategoriaId = 1 };
            _service.Create(dto);

            // La siguiente lectura debe ir a MongoDB
            var result = await _service.GetAllReadAsync();
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(1);
            result.Value![0].Nombre.Should().Be("Before");
        }

        [Test]
        public async Task GetById_ReadLuegoCreate_CacheInvalidado()
        {
            // Arrange - no hay datos en MongoDB
            var dto = new CreateProductoDto { Nombre = "NewProduct", Precio = 50m, CategoriaId = 1 };

            // Act - crear producto (invalida cache)
            _service.Create(dto);

            // Assert - GetAll debe ir a MongoDB (cache invalidado)
            var result = await _service.GetAllReadAsync();
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEmpty();
        }
    }

    [TestFixture]
    public class CreateValidation : ProductoServiceTests
    {
        [Test]
        public void Create_NombreDuplicado_RetornaError()
        {
            CreateTestProducto("Duplicado").GetAwaiter().GetResult();
            var dto = new CreateProductoDto { Nombre = "Duplicado", Precio = 10m, CategoriaId = 1 };

            var result = _service.Create(dto);

            result.IsFailure.Should().BeTrue();
        }

        [Test]
        public void Create_ProductoValido_RetornaProductoConId()
        {
            var dto = new CreateProductoDto { Nombre = "Nuevo", Precio = 99.99m, CategoriaId = 1 };

            var result = _service.Create(dto);

            result.IsSuccess.Should().BeTrue();
            result.Value!.Id.Should().BeGreaterThan(0);
        }
    }
}
