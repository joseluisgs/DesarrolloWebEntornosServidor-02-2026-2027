using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ProductosEFCore.Models;

namespace ProductosEFCore.Test.Services;

[TestFixture]
public class ProductoRepositoryTests : IntegrationTestBase
{
    private Guid _categoriaId;

    [SetUp]
    public async Task SetUpCategorias()
    {
        // Insertar categorías base para los tests con Guid explícito
        _categoriaId = Guid.NewGuid();
        await CreateTestCategoria(_categoriaId, "Electrónica", "Dispositivos electrónicos");
        await CreateTestCategoria(Guid.NewGuid(), "Mobiliario", "Muebles");
        await CreateTestCategoria(Guid.NewGuid(), "Instrumentos", "Instrumentos musicales");
    }

    [TestFixture]
    public class GetAll : ProductoRepositoryTests
    {
        [Test]
        public async Task GetAll_ExistenProductos_RetornaTodos()
        {
            await CreateTestProducto("Teclado", 89.99m, _categoriaId);
            await CreateTestProducto("Ratón", 35.50m, _categoriaId);

            var productos = Db.Productos.ToList();

            productos.Should().HaveCount(2);
        }

        [Test]
        public async Task GetAll_NoHayProductos_RetornaColeccionVacia()
        {
            var productos = Db.Productos.ToList();

            productos.Should().BeEmpty();
        }
    }

    [TestFixture]
    public class GetById : ProductoRepositoryTests
    {
        [Test]
        public async Task GetById_ProductoExiste_RetornaProducto()
        {
            var creado = await CreateTestProducto("Monitor", 299.00m, _categoriaId);

            var producto = await Db.Productos.FindAsync(creado.Id);

            producto.Should().NotBeNull();
            producto!.Nombre.Should().Be("Monitor");
            producto.Precio.Should().Be(299.00m);
        }

        [Test]
        public async Task GetById_ProductoNoExiste_RetornaNull()
        {
            var producto = await Db.Productos.FindAsync(999L);

            producto.Should().BeNull();
        }
    }

    [TestFixture]
    public class Create : ProductoRepositoryTests
    {
        [Test]
        public async Task Create_ProductoValido_RetornaProductoConId()
        {
            var producto = new Producto
            {
                Nombre = "Silla Ergonómica",
                Precio = 249.99m,
                CategoriaId = _categoriaId,
                Imagen = "https://picsum.photos/seed/silla/300",
                CreatedAt = DateTime.UtcNow
            };

            Db.Productos.Add(producto);
            await Db.SaveChangesAsync();

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
            var producto = await CreateTestProducto("Para Borrar", 10.00m, _categoriaId);

            producto.IsDeleted = true;
            await Db.SaveChangesAsync();

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
            var producto = await CreateTestProducto("Para Restaurar", 10.00m, _categoriaId);
            producto.IsDeleted = true;
            await Db.SaveChangesAsync();

            var restaurado = await Db.Productos.IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.Id == producto.Id && p.IsDeleted);
            restaurado!.IsDeleted = false;
            await Db.SaveChangesAsync();

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
            await CreateTestProducto("Teclado", 89.99m, _categoriaId);

            var existe = Db.Productos.Any(p => p.Nombre == "Teclado");

            existe.Should().BeTrue();
        }

        [Test]
        public async Task IsDuplicated_NombreNoExiste_RetornaFalse()
        {
            var existe = Db.Productos.Any(p => p.Nombre == "Inexistente");

            existe.Should().BeFalse();
        }
    }

    [TestFixture]
    public class SeedData : ProductoRepositoryTests
    {
        [Test]
        public async Task Seed_CategoriasInsertadas_Retorna3()
        {
            var categorias = await Db.Categorias.ToListAsync();

            categorias.Should().HaveCount(3);
        }
    }
}
