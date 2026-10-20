using FluentAssertions;
using MongoDB.Bson;
using NUnit.Framework;
using ProductosMongo.Models;
using ProductosMongo.Repositories.EfCore;

namespace ProductosMongo.Test.EfCore;

/// <summary>
/// Tests de integracion para el repositorio con Entity Framework Core + MongoDB.
/// </summary>
[TestFixture]
public class ProductoEfCoreRepositoryTests : IntegrationTestBase
{
    private ProductoEfCoreRepository _repository = null!;

    [SetUp]
    public void SetUpRepository()
    {
        _repository = new ProductoEfCoreRepository(DbContext);
    }

    [TestFixture]
    public class GetAll : ProductoEfCoreRepositoryTests
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
    public class GetById : ProductoEfCoreRepositoryTests
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
    public class Create : ProductoEfCoreRepositoryTests
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

            // Assert: la composición debe persistir DENTRO del documento
            var guardado = _repository.GetById(creado.Id.ToString());
            guardado.Should().NotBeNull();
            guardado!.Categoria.Should().NotBeNull();
            guardado.Categoria.Nombre.Should().Be("Mobiliario");
        }
    }

    [TestFixture]
    public class Referencia : ProductoEfCoreRepositoryTests
    {
        [Test]
        public void GetById_ProveedorReferenciado_LeeElIdMapeado()
        {
            // Arrange: el producto lo escribe el Driver Nativo...
            var creado = CreateTestProducto("Teclado", 89.99m, "Electronica");

            // Act: ...y EF Core lo lee con la misma semántica de campos
            var guardado = _repository.GetById(creado.Id.ToString());

            // Assert
            guardado.Should().NotBeNull();
            guardado!.ProveedorId.Should().Be(ProveedorTestId);
            guardado.Categoria.Nombre.Should().Be("Electronica");
        }

        [Test]
        public void GetById_ProveedorEnOtraColeccion_SeResuelveEnDosConsultas()
        {
            // Arrange
            var creado = CreateTestProducto("Raton", 35.50m, "Electronica");

            // Act: no hay Include() en MongoDB: dos consultas a mano
            var producto = _repository.GetById(creado.Id.ToString());
            var proveedor = DbContext.Proveedores.FirstOrDefault(p => p.Id == producto!.ProveedorId);

            // Assert
            proveedor.Should().NotBeNull();
            proveedor!.Nombre.Should().Be("Logitech");
        }
    }

    [TestFixture]
    public class Delete : ProductoEfCoreRepositoryTests
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
    public class Duplicated : ProductoEfCoreRepositoryTests
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
