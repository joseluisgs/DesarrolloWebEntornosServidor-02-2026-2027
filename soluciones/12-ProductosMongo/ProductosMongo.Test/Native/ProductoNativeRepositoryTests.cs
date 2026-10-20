using FluentAssertions;
using MongoDB.Bson;
using MongoDB.Driver;
using NUnit.Framework;
using ProductosMongo.Models;
using ProductosMongo.Repositories.Native;

namespace ProductosMongo.Test.Native;

/// <summary>
/// Tests de integracion para el repositorio con Driver Nativo de MongoDB.
/// </summary>
[TestFixture]
public class ProductoNativeRepositoryTests : IntegrationTestBase
{
    private ProductoNativeRepository _repository = null!;

    [SetUp]
    public void SetUpRepository()
    {
        _repository = new ProductoNativeRepository(Collection);
    }

    [TestFixture]
    public class GetAll : ProductoNativeRepositoryTests
    {
        [Test]
        public void GetAll_ExistenProductos_RetornaTodos()
        {
            // Arrange
            CreateTestProducto("Teclado", 89.99m, "Electronica");
            CreateTestProducto("Raton", 35.50m, "Electronica");

            // Act
            var productos = _repository.GetAll().ToList();

            // Assert
            productos.Should().HaveCount(2);
        }

        [Test]
        public void GetAll_NoHayProductos_RetornaColeccionVacia()
        {
            // Act
            var productos = _repository.GetAll().ToList();

            // Assert
            productos.Should().BeEmpty();
        }
    }

    [TestFixture]
    public class GetById : ProductoNativeRepositoryTests
    {
        [Test]
        public void GetById_ProductoExiste_RetornaProducto()
        {
            // Arrange
            var creado = CreateTestProducto("Monitor", 299.00m, "Electronica");

            // Act
            var producto = _repository.GetById(creado.Id.ToString());

            // Assert
            producto.Should().NotBeNull();
            producto!.Nombre.Should().Be("Monitor");
            producto.Precio.Should().Be(299.00m);
        }

        [Test]
        public void GetById_ProductoNoExiste_RetornaNull()
        {
            // Arrange
            var idInexistente = ObjectId.GenerateNewId().ToString();

            // Act
            var producto = _repository.GetById(idInexistente);

            // Assert
            producto.Should().BeNull();
        }
    }

    [TestFixture]
    public class Create : ProductoNativeRepositoryTests
    {
        [Test]
        public void Create_ProductoValido_RetornaProductoConId()
        {
            // Arrange
            var producto = new Producto
            {
                Nombre = "Silla Ergonomica",
                Precio = 249.99m,
                Imagen = "https://picsum.photos/seed/silla/300",
                Categoria = new Categoria { Nombre = "Mobiliario", Descripcion = "Muebles" },
                ProveedorId = ProveedorTestId
            };

            // Act
            var creado = _repository.Add(producto);

            // Assert
            creado.Id.Should().NotBe(default(ObjectId));
            creado.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            creado.IsDeleted.Should().BeFalse();
        }
    }

    [TestFixture]
    public class Referencia : ProductoNativeRepositoryTests
    {
        [Test]
        public void GetById_ProveedorReferenciado_PersisteSoloElId()
        {
            // Arrange
            var creado = CreateTestProducto("Teclado", 89.99m, "Electronica");

            // Act
            var guardado = Collection.Find(p => p.Id == creado.Id).First();
            var bson = guardado.ToBsonDocument();

            // Assert: la referencia viaja como _id, sin documento embebido
            guardado.ProveedorId.Should().Be(ProveedorTestId);
            bson.Contains("proveedor_id").Should().BeTrue();
            bson.Contains("proveedor").Should().BeFalse();

            // Assert: la composición sí viaja completa dentro del documento
            bson.Contains("categoria").Should().BeTrue();
            guardado.Categoria.Nombre.Should().Be("Electronica");
        }

        [Test]
        public void GetById_ProveedorExisteEnOtraColeccion_ReferenciaNoColgante()
        {
            // Arrange
            var creado = CreateTestProducto("Raton", 35.50m, "Electronica");

            // Act
            var proveedor = ProveedoresCollection
                .Find(p => p.Id == creado.ProveedorId)
                .FirstOrDefault();

            // Assert
            proveedor.Should().NotBeNull();
            proveedor!.Nombre.Should().Be("Logitech");
        }
    }

    [TestFixture]
    public class Delete : ProductoNativeRepositoryTests
    {
        [Test]
        public void Delete_ProductoExiste_MarcaComoBorrado()
        {
            // Arrange
            var producto = CreateTestProducto("Para Borrar", 10.00m, "Test");

            // Act
            var resultado = _repository.Delete(producto.Id.ToString());

            // Assert
            resultado.Should().BeTrue();
            var borrado = _repository.GetById(producto.Id.ToString());
            borrado.Should().BeNull();
        }
    }

    [TestFixture]
    public class Duplicated : ProductoNativeRepositoryTests
    {
        [Test]
        public void IsDuplicated_NombreExistente_RetornaTrue()
        {
            // Arrange
            CreateTestProducto("Teclado", 89.99m, "Electronica");

            // Act
            var existe = _repository.IsDuplicated("Teclado");

            // Assert
            existe.Should().BeTrue();
        }

        [Test]
        public void IsDuplicated_NombreNoExiste_RetornaFalse()
        {
            // Act
            var existe = _repository.IsDuplicated("Inexistente");

            // Assert
            existe.Should().BeFalse();
        }
    }
}
