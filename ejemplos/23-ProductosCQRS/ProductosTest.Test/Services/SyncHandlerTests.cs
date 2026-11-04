using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ProductosCQRS.Entity;
using ProductosCQRS.Entity.Mongo;
using ProductosCQRS.Features.Productos.Commands;
using ProductosCQRS.Features.Productos.Sync;
using ProductosCQRS.Models;
using ProductosCQRS.Repositories;
using Testcontainers.MongoDb;

namespace ProductosCQRS.Test.Services;

[TestFixture]
public class SyncHandlerTests
{
    private MongoDbContainer _mongo = null!;
    private MongoDbContext _mongoContext = null!;
    private IProductoReadRepository _readRepository = null!;
    private SyncProductoHandler _handler = null!;

    [OneTimeSetUp]
    public async Task ClassSetUp()
    {
        _mongo = new MongoDbBuilder().WithImage("mongo:7.0").Build();
        await _mongo.StartAsync();

        _mongoContext = new MongoDbContext(
            new DbContextOptionsBuilder<MongoDbContext>()
                .UseMongoDB(_mongo.GetConnectionString(), "test_read")
                .Options);

        _readRepository = new ProductoReadRepository(_mongoContext);
        _handler = new SyncProductoHandler(_readRepository);
    }

    [OneTimeTearDown]
    public async Task ClassTearDown()
    {
        await _mongoContext.DisposeAsync();
        await _mongo.DisposeAsync();
    }

    [SetUp]
    public void SetUp()
    {
        _mongoContext.Productos.RemoveRange(_mongoContext.Productos);
        _mongoContext.SaveChanges();
    }

    [TestFixture]
    public class CreateSync : SyncHandlerTests
    {
        [Test]
        public async Task Handle_ProductoCreado_SeSincronizaMongoDB()
        {
            // Arrange
            var producto = new Producto
            {
                Id = 1,
                Nombre = "Teclado",
                Precio = 89.99m,
                Categoria = new Categoria { Id = 1, Nombre = "Electronica" },
                Proveedor = new Proveedor { Id = 1, Nombre = "Logitech" }
            };

            var notification = new ProductoCreadoNotification(producto);

            // Act
            await _handler.Handle(notification, CancellationToken.None);

            // Assert
            var read = await _readRepository.GetByIdAsync(1);
            read.Should().NotBeNull();
            read!.Nombre.Should().Be("Teclado");
            read.Categoria.Should().NotBeNull();
            read.Categoria!.Nombre.Should().Be("Electronica");
        }
    }

    [TestFixture]
    public class DeleteSync : SyncHandlerTests
    {
        [Test]
        public async Task Handle_ProductoEliminado_SeEliminaMongoDB()
        {
            // Arrange
            var producto = new ProductoRead
            {
                Id = 1,
                Nombre = "Para Borrar",
                Precio = 10m
            };
            await _readRepository.UpsertAsync(producto);

            var notification = new ProductoEliminadoNotification(1);

            // Act
            await _handler.Handle(notification, CancellationToken.None);

            // Assert
            var read = await _readRepository.GetByIdAsync(1);
            read.Should().BeNull();
        }
    }
}
