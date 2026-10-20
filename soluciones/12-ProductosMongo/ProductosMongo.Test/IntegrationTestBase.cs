using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using NUnit.Framework;
using ProductosMongo.Entity;
using ProductosMongo.Models;
using Testcontainers.MongoDb;

namespace ProductosMongo.Test;

/// <summary>
/// Base para tests de integracion con MongoDB real usando TestContainers.
/// Cada fixture crea su propio contenedor efimero.
/// </summary>
public abstract class IntegrationTestBase
{
    /// <summary>
    /// ID fijo del proveedor sembrado para los tests.
    /// Los tests no dependen de IDs aleatorios.
    /// </summary>
    protected static readonly ObjectId ProveedorTestId = ObjectId.Parse("64b0000000000000000000ff");

    private readonly MongoDbContainer MongoDb = new MongoDbBuilder()
        .WithImage("mongo:7.0")
        .Build();

#pragma warning disable NUnit1032
    protected IMongoClient Client { get; private set; } = null!;
#pragma warning restore NUnit1032
    protected IMongoDatabase Database { get; private set; } = null!;
    protected IMongoCollection<Producto> Collection { get; private set; } = null!;
    protected IMongoCollection<Proveedor> ProveedoresCollection { get; private set; } = null!;
    protected ProductosMongoDbContext DbContext { get; private set; } = null!;

    [OneTimeSetUp]
    public async Task ClassSetUp()
    {
        await MongoDb.StartAsync();
        Client = new MongoClient(MongoDb.GetConnectionString());
        Database = Client.GetDatabase("test_database");
        Collection = Database.GetCollection<Producto>("productos");
        ProveedoresCollection = Database.GetCollection<Proveedor>("proveedores");

        var options = new DbContextOptionsBuilder<ProductosMongoDbContext>()
            .UseMongoDB(MongoDb.GetConnectionString(), "test_database")
            .Options;

        DbContext = new ProductosMongoDbContext(options);
    }

    [OneTimeTearDown]
    public async Task ClassTearDown()
    {
        await DbContext.DisposeAsync();
        await MongoDb.DisposeAsync();
    }

    [SetUp]
    public void SetUp()
    {
        // Limpiar ambas colecciones antes de cada test
        Collection.DeleteMany(FilterDefinition<Producto>.Empty);
        ProveedoresCollection.DeleteMany(FilterDefinition<Proveedor>.Empty);

        // El proveedor se siembra SIEMPRE: los productos lo referencian por ID.
        // Si no existiera, tendríamos una referencia colgante.
        ProveedoresCollection.InsertOne(new Proveedor
        {
            Id = ProveedorTestId,
            Nombre = "Logitech",
            Contacto = "exportaciones@logitech.com",
            Pais = "Suiza"
        });
    }

    /// <summary>
    /// Inserta un producto directamente en la coleccion para tests.
    /// La categoría va embebida (composición) y el proveedor solo por ID (referencia).
    /// </summary>
    protected Producto CreateTestProducto(
        string nombre = "Test Product",
        decimal precio = 10.00m,
        string categoriaNombre = "Test Category")
    {
        var producto = new Producto
        {
            Nombre = nombre,
            Precio = precio,
            Imagen = "https://picsum.photos/seed/test/300",
            Categoria = new Categoria
            {
                Nombre = categoriaNombre,
                Descripcion = "Test description"
            },
            ProveedorId = ProveedorTestId,
            CreatedAt = DateTime.UtcNow
        };

        Collection.InsertOne(producto);
        return producto;
    }
}
