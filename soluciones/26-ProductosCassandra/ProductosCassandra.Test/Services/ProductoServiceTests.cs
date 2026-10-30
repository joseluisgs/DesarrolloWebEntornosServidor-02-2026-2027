using FluentAssertions;
using NUnit.Framework;
using ProductosCassandra.Dtos;

namespace ProductosCassandra.Test.Services;

/// <summary>
/// Tests de la capa de servicios sobre un Cassandra real.
/// El servicio no añade magia: orquesta los repositorios y devuelve DTOs,
/// pero sus decisiones (qué tabla escribe, cuándo devuelve null) son las que
/// el controlador da por buenas.
/// </summary>
[TestFixture]
public class ProductoServiceTests : IntegrationTestBase
{
    [Test]
    public async Task GetByIdAsync_Existente_DevuelveElDtoConLaCategoriaCopiada()
    {
        // Act
        var producto = await ProductoServicio.GetByIdAsync(201);

        // Assert: el nombre de la categoria llega copiado en la fila, sin cruce
        producto.Should().NotBeNull();
        producto!.Nombre.Should().Be("Monitor 27\" QHD");
        producto.CategoriaNombre.Should().Be("Monitores");
    }

    [Test]
    public async Task GetByIdAsync_Inexistente_DevuelveNull()
    {
        // Act
        var producto = await ProductoServicio.GetByIdAsync(999);

        // Assert
        producto.Should().BeNull();
    }

    [Test]
    public async Task CreateAsync_CategoriaInexistente_DevuelveNull()
    {
        // Arrange
        var dto = new CreateProductoDto { Id = 105, Nombre = "Silla Gamer", Precio = 189.99, CategoriaId = 999 };

        // Act: la categoria 999 no existe
        var creado = await ProductoServicio.CreateAsync(dto);

        // Assert: el servicio devuelve null y el controlador lo traduce en 404
        creado.Should().BeNull();
    }

    [Test]
    public async Task CreateAsync_CategoriaValida_DevuelveElDtoCreado()
    {
        // Arrange
        var dto = new CreateProductoDto { Id = 106, Nombre = "Webcam 4K", Precio = 79.99, CategoriaId = 2 };

        // Act
        var creado = await ProductoServicio.CreateAsync(dto);

        // Assert: el DTO de respuesta trae la categoria resuelta, ya copiada
        creado.Should().NotBeNull();
        creado!.CategoriaNombre.Should().Be("Monitores");
        (await ProductoRepositorio.GetProductoPorIdAsync(106)).Should().NotBeNull();

        // Limpieza: borrado compensatorio en las dos tablas
        await ProductoServicio.DeleteAsync(106, 2);
        (await ProductoRepositorio.GetProductoPorIdAsync(106)).Should().BeNull();
    }

    [Test]
    public async Task GetHistorialPreciosAsync_ProductoConCambios_DevuelveLosDtoOrdenados()
    {
        // Act
        var historial = await ProductoServicio.GetHistorialPreciosAsync(101);

        // Assert: del mas reciente al mas antiguo, tal y como lo ordena la tabla
        historial.Should().HaveCount(2);
        historial.Select(h => h.Precio).Should().ContainInOrder(89.99, 99.99);
        historial[0].PrecioAnterior.Should().Be(99.99);
    }

    [Test]
    public async Task GetCambiosPrecioAsync_DiaConCambios_DevuelveElInforme()
    {
        // Act
        var cambios = await ProductoServicio.GetCambiosPrecioAsync(
            new DateTime(2026, 10, 15, 0, 0, 0, DateTimeKind.Utc));

        // Assert
        cambios.Should().ContainSingle(c => c.ProductoId == 101 && c.Precio == 89.99);
    }

    [Test]
    public async Task RegistrarCambioPrecioAsync_ProductoInexistente_DevuelveFalse()
    {
        // Act
        var encontrado = await ProductoServicio.RegistrarCambioPrecioAsync(999, 10.0);

        // Assert
        encontrado.Should().BeFalse();
    }
}
