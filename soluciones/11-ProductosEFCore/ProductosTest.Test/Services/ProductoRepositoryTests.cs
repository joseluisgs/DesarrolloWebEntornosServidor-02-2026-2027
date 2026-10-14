using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ProductosEFCore.Models;

namespace ProductosEFCore.Test.Services;

[TestFixture]
public class ProductoRepositoryTests : IntegrationTestBase
{
    [SetUp]
    public async Task SetUpCategorias()
    {
        // Insertar categorías base para los tests
        await CreateTestCategoria("Electrónica", "Dispositivos electrónicos");
        await CreateTestCategoria("Mobiliario", "Muebles");
        await CreateTestCategoria("Instrumentos", "Instrumentos musicales");
    }

    [TestFixture]
    public class GetAll : ProductoRepositoryTests
    {
        [Test]
        public async Task GetAll_ExistenProductos_RetornaTodos()
        {
            // Arrange
            await CreateTestProducto("Teclado", 89.99m, 1);
            await CreateTestProducto("Ratón", 35.50m, 1);

            // Act
            var productos = Db.Productos.ToList();

            // Assert
            productos.Should().HaveCount(2);
        }

        [Test]
        public async Task GetAll_NoHayProductos_RetornaColeccionVacia()
        {
            // Act
            var productos = Db.Productos.ToList();

            // Assert
            productos.Should().BeEmpty();
        }
    }

    [TestFixture]
    public class GetById : ProductoRepositoryTests
    {
        [Test]
        public async Task GetById_ProductoExiste_RetornaProducto()
        {
            // Arrange
            var creado = await CreateTestProducto("Monitor", 299.00m, 1);

            // Act
            var producto = await Db.Productos.FindAsync(creado.Id);

            // Assert
            producto.Should().NotBeNull();
            producto!.Nombre.Should().Be("Monitor");
            producto.Precio.Should().Be(299.00m);
        }

        [Test]
        public async Task GetById_ProductoNoExiste_RetornaNull()
        {
            // Act
            var producto = await Db.Productos.FindAsync(999L);

            // Assert
            producto.Should().BeNull();
        }
    }

    [TestFixture]
    public class Create : ProductoRepositoryTests
    {
        [Test]
        public async Task Create_ProductoValido_RetornaProductoConId()
        {
            // Arrange
            var producto = new Producto
            {
                Nombre = "Silla Ergonómica",
                Precio = 249.99m,
                CategoriaId = 2,
                Imagen = "https://picsum.photos/seed/silla/300",
                CreatedAt = DateTime.UtcNow
            };

            // Act
            Db.Productos.Add(producto);
            await Db.SaveChangesAsync();

            // Assert
            producto.Id.Should().BeGreaterThan(0);
            var guardado = await Db.Productos.FindAsync(producto.Id);
            guardado.Should().NotBeNull();
            guardado!.Nombre.Should().Be("Silla Ergonómica");
        }
    }

    [TestFixture]
    public class Delete : ProductoRepositoryTests
    {
        [Test]
        public async Task Delete_ProductoExiste_MarcaComoBorrado()
        {
            // Arrange
            var producto = await CreateTestProducto("Para Borrar", 10.00m, 1);

            // Act
            producto.IsDeleted = true;
            await Db.SaveChangesAsync();

            // Assert
            var borrado = await Db.Productos.FindAsync(producto.Id);
            borrado!.IsDeleted.Should().BeTrue();
        }
    }

    [TestFixture]
    public class Restore : ProductoRepositoryTests
    {
        [Test]
        public async Task Restore_ProductoBorrado_Restaura()
        {
            // Arrange
            var producto = await CreateTestProducto("Para Restaurar", 10.00m, 1);
            producto.IsDeleted = true;
            await Db.SaveChangesAsync();

            // Act
            var restaurado = await Db.Productos.IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.Id == producto.Id && p.IsDeleted);
            restaurado!.IsDeleted = false;
            await Db.SaveChangesAsync();

            // Assert
            var verificacion = await Db.Productos.FindAsync(producto.Id);
            verificacion!.IsDeleted.Should().BeFalse();
        }
    }

    [TestFixture]
    public class Duplicated : ProductoRepositoryTests
    {
        [Test]
        public async Task IsDuplicated_NombreExistente_RetornaTrue()
        {
            // Arrange
            await CreateTestProducto("Teclado", 89.99m, 1);

            // Act
            var existe = Db.Productos.Any(p => p.Nombre == "Teclado");

            // Assert
            existe.Should().BeTrue();
        }

        [Test]
        public async Task IsDuplicated_NombreNoExiste_RetornaFalse()
        {
            // Act
            var existe = Db.Productos.Any(p => p.Nombre == "Inexistente");

            // Assert
            existe.Should().BeFalse();
        }
    }

    [TestFixture]
    public class SeedData : ProductoRepositoryTests
    {
        [Test]
        public async Task Seed_CategoriasInsertadas_Retorna3()
        {
            // Act
            var categorias = await Db.Categorias.ToListAsync();

            // Assert
            categorias.Should().HaveCount(3);
        }
    }
}
