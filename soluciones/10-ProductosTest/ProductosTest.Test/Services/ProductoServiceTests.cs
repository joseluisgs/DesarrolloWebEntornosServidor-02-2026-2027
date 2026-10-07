using FluentAssertions;
using Moq;
using NUnit.Framework;
using ProductosTest.Dtos;
using ProductosTest.Errors;
using ProductosTest.Models;
using ProductosTest.Repositories;
using ProductosTest.Services;

namespace ProductosTest.Test.Services;

[TestFixture]
public class ProductoServiceTests
{
    private Mock<IProductoRepository> _repositoryMock = null!;
    private ProductoService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _repositoryMock = new Mock<IProductoRepository>();
        _service = new ProductoService(_repositoryMock.Object);
    }

    [TestFixture]
    public class GetAll : ProductoServiceTests
    {
        [Test]
        public void GetAll_ExistenProductos_RetornaTodos()
        {
            // Arrange
            var productos = new List<Producto>
            {
                new() { Id = 1, Nombre = "Teclado", Precio = 45 },
                new() { Id = 2, Nombre = "Ratón", Precio = 25 }
            };
            _repositoryMock.Setup(r => r.GetAll()).Returns(productos);

            // Act
            var resultado = _service.GetAll();

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Should().HaveCount(2);
        }

        [Test]
        public void GetAll_NoHayProductos_RetornaColeccionVacia()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAll()).Returns(new List<Producto>());

            // Act
            var resultado = _service.GetAll();

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Should().BeEmpty();
        }
    }

    [TestFixture]
    public class GetById : ProductoServiceTests
    {
        [Test]
        public void GetById_ProductoExiste_RetornaProducto()
        {
            // Arrange
            var producto = new Producto { Id = 1, Nombre = "Teclado", Precio = 45 };
            _repositoryMock.Setup(r => r.GetById(1)).Returns(producto);

            // Act
            var resultado = _service.GetById(1);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Nombre.Should().Be("Teclado");
            resultado.Value.Precio.Should().Be(45);
        }

        [Test]
        public void GetById_ProductoNoExiste_RetornaFailure()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetById(99)).Returns((Producto?)null);

            // Act
            var resultado = _service.GetById(99);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<NotFoundError>();
        }
    }

    [TestFixture]
    public class Create : ProductoServiceTests
    {
        [Test]
        public void Create_DtoValido_RetornaProductoCreado()
        {
            // Arrange
            var dto = new CreateProductoDto { Nombre = "Monitor", Precio = 299, Categoria = "Electrónica" };
            _repositoryMock.Setup(r => r.Add(It.IsAny<Producto>()))
                .Returns((Producto p) => { p.Id = 1; p.CreatedAt = DateTime.UtcNow; return p; });

            var service = new ProductoService(_repositoryMock.Object);

            // Act
            var resultado = service.Create(dto);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Nombre.Should().Be("Monitor");
            resultado.Value.Id.Should().Be(1);

            // Verify: se llamó a Add exactamente una vez
            _repositoryMock.Verify(r => r.Add(It.IsAny<Producto>()), Times.Once);
        }
    }

    [TestFixture]
    public class Update : ProductoServiceTests
    {
        [Test]
        public void Update_ProductoExiste_RetornaProductoActualizado()
        {
            // Arrange
            var dto = new UpdateProductoDto { Nombre = "Teclado Pro", Precio = 55, Categoria = "Periféricos" };
            var existente = new Producto { Id = 1, Nombre = "Teclado", Precio = 45 };
            _repositoryMock.Setup(r => r.Update(1, It.IsAny<Producto>()))
                .Returns((long id, Producto p) => { p.Id = id; return p; });

            // Act
            var resultado = _service.Update(1, dto);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Nombre.Should().Be("Teclado Pro");

            // Verify: se llamó a Update con ID 1
            _repositoryMock.Verify(r => r.Update(1, It.IsAny<Producto>()), Times.Once);
        }

        [Test]
        public void Update_ProductoNoExiste_RetornaFailure()
        {
            // Arrange
            var dto = new UpdateProductoDto { Nombre = "No Existe", Precio = 10, Categoria = "X" };
            _repositoryMock.Setup(r => r.Update(99, It.IsAny<Producto>())).Returns((Producto?)null);

            // Act
            var resultado = _service.Update(99, dto);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<NotFoundError>();
        }
    }

    [TestFixture]
    public class PatchPrice : ProductoServiceTests
    {
        [TestCase(100)]
        [TestCase(0)]
        [TestCase(999.99)]
        public void PatchPrice_PrecioValido_RetornaProducto(decimal precio)
        {
            // Arrange
            var producto = new Producto { Id = 1, Nombre = "Teclado", Precio = 45 };
            _repositoryMock.Setup(r => r.PatchPrice(1, precio)).Returns(producto);

            // Act
            var resultado = _service.PatchPrice(1, precio);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
        }

        [Test]
        public void PatchPrice_PrecioNegativo_RetornaFailure()
        {
            // Act
            var resultado = _service.PatchPrice(1, -10);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<ValidationError>();

            // Verify: NUNCA se llamó a PatchPrice con precio negativo
            _repositoryMock.Verify(r => r.PatchPrice(It.IsAny<long>(), It.IsAny<decimal>()), Times.Never);
        }

        [Test]
        public void PatchPrice_ProductoNoExiste_RetornaFailure()
        {
            // Arrange
            _repositoryMock.Setup(r => r.PatchPrice(99, 50)).Returns((Producto?)null);

            // Act
            var resultado = _service.PatchPrice(99, 50);

            // Assert
            resultado.IsFailure.Should().BeTrue();
        }
    }

    [TestFixture]
    public class Delete : ProductoServiceTests
    {
        [Test]
        public void Delete_ProductoExiste_RetornaSuccess()
        {
            // Arrange
            _repositoryMock.Setup(r => r.Delete(1)).Returns(true);

            // Act
            var resultado = _service.Delete(1);

            // Assert
            resultado.IsSuccess.Should().BeTrue();

            // Verify: se llamó a Delete exactamente una vez con ID 1
            _repositoryMock.Verify(r => r.Delete(1), Times.Once);
        }

        [Test]
        public void Delete_ProductoNoExiste_RetornaFailure()
        {
            // Arrange
            _repositoryMock.Setup(r => r.Delete(99)).Returns(false);

            // Act
            var resultado = _service.Delete(99);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<NotFoundError>();

            // Verify: se llamó a Delete una vez
            _repositoryMock.Verify(r => r.Delete(99), Times.Once);
        }
    }

    [TestFixture]
    public class Search : ProductoServiceTests
    {
        [Test]
        public void Search_Termino_RetornaResultados()
        {
            // Arrange
            var productos = new List<Producto>
            {
                new() { Id = 1, Nombre = "Teclado Mecánico", Categoria = "Periféricos" }
            };
            _repositoryMock.Setup(r => r.Search("Teclado")).Returns(productos);

            // Act
            var resultado = _service.Search("Teclado");

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Should().HaveCount(1);
        }
    }
}
