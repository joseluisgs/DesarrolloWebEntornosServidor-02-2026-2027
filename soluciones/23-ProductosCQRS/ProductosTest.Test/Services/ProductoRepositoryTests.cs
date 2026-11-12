using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ProductosCQRS.Entity;
using ProductosCQRS.Models;
using ProductosCQRS.Repositories;

namespace ProductosCQRS.Test.Services;

[TestFixture]
public class ProductoRepositoryTests : IntegrationTestBase
{
    private IProductoRepository _repository = null!;

    [SetUp]
    public void SetUp()
    {
        _repository = new ProductoRepository(Db);
    }

    [TestFixture]
    public class GetAll : ProductoRepositoryTests
    {
        [Test]
        public async Task GetAll_ExistenProductos_RetornaTodos()
        {
            // Arrange
            await CreateTestProducto("Teclado", 89.99m, 1);
            await CreateTestProducto("Raton", 35.50m, 1);

            // Act
            var productos = _repository.GetAll().ToList();

            // Assert
            productos.Should().HaveCount(2);
        }
    }

    [TestFixture]
    public class Create : ProductoRepositoryTests
    {
        [Test]
        public void Create_ProductoValido_RetornaProductoConId()
        {
            // Arrange
            var producto = new Producto
            {
                Nombre = "Silla",
                Precio = 249.99m,
                CategoriaId = 1,
                CreatedAt = DateTime.UtcNow
            };

            // Act
            var creado = _repository.Add(producto);

            // Assert
            creado.Id.Should().BeGreaterThan(0);
            creado.Nombre.Should().Be("Silla");
        }
    }

    [TestFixture]
    public class Duplicate : ProductoRepositoryTests
    {
        [Test]
        public async Task IsDuplicated_NombreExistente_RetornaTrue()
        {
            // Arrange
            await CreateTestProducto("Teclado", 89.99m, 1);

            // Act
            var existe = _repository.IsDuplicated("Teclado");

            // Assert
            existe.Should().BeTrue();
        }
    }
}
