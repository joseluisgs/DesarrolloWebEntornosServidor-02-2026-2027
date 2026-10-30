using FluentAssertions;
using NUnit.Framework;
using ProductosCassandra.Models;

namespace ProductosCassandra.Test.Repositories;

/// <summary>
/// Tests del repositorio de productos contra un Cassandra real (TestContainers).
/// </summary>
[TestFixture]
public class ProductoCassandraRepositoryTests : IntegrationTestBase
{
    [Test]
    public async Task GetProductosPorCategoria_ParticionPerifericos_DevuelveLosTresOrdenadosPorClusteringKey()
    {
        // Act: leer la particion 1 entera
        var productos = await ProductoRepositorio.GetProductosPorCategoriaAsync(1);

        // Assert: salen ordenados por producto_id de serie, sin ORDER BY
        productos.Should().HaveCount(3);
        productos.Select(p => p.ProductoId).Should().ContainInOrder(101, 102, 103);

        // Denormalizacion: cada fila trae el nombre de la categoria copiado
        productos.Should().OnlyContain(p => p.CategoriaNombre == "Periféricos");
    }

    [Test]
    public async Task GetProductosPorCategoria_ParticionVacia_DevuelveVacio()
    {
        // Act: una categoria que no existe es una particion vacia, no un error
        var productos = await ProductoRepositorio.GetProductosPorCategoriaAsync(999);

        // Assert
        productos.Should().BeEmpty();
    }

    [Test]
    public async Task GetProductoPorId_Existia_DevuelveElProducto()
    {
        // Act
        var producto = await ProductoRepositorio.GetProductoPorIdAsync(201);

        // Assert
        producto.Should().NotBeNull();
        producto!.Nombre.Should().Be("Monitor 27\" QHD");
        producto.CategoriaId.Should().Be(2);
    }

    [Test]
    public async Task CrearProducto_EscribeEnAmbasTablas_ConElNombreDeLaCategoriaCopiado()
    {
        // Arrange
        var categoria = await CategoriaRepositorio.GetCategoriaAsync(1);
        var nuevo = new CreateProducto(104, "Webcam Full HD", 45.50);

        // Act
        await ProductoRepositorio.CrearProductoAsync(nuevo, categoria!);

        // Assert: la tabla por id tiene el producto...
        var porId = await ProductoRepositorio.GetProductoPorIdAsync(104);
        porId.Should().NotBeNull();
        porId!.CategoriaNombre.Should().Be("Periféricos");

        // ...y la particion de la categoria crecio una fila mas
        var enParticion = await ProductoRepositorio.GetProductosPorCategoriaAsync(1);
        enParticion.Should().ContainSingle(p => p.ProductoId == 104);

        // Limpieza: el borrado compensatorio borra en las DOS tablas
        await ProductoRepositorio.BorrarProductoAsync(104, 1);
        (await ProductoRepositorio.GetProductoPorIdAsync(104)).Should().BeNull();
        (await ProductoRepositorio.GetProductosPorCategoriaAsync(1)).Should().HaveCount(3);
    }

    [Test]
    public async Task GetHistorialPrecios_ProductoConCambios_DevuelveLosPreciosOrdenados()
    {
        // Act: el Teclado (101) tuvo dos cambios en el sembrado
        var historial = await ProductoRepositorio.GetHistorialPreciosAsync(101);

        // Assert: el orden temporal DESC esta guardado en la tabla, sin ORDER BY
        historial.Should().HaveCount(2);
        historial.Select(h => h.Precio).Should().ContainInOrder(89.99, 99.99);

        // La fila mas reciente sabe cuanto cambio
        historial[0].PrecioAnterior.Should().Be(99.99);
    }

    [Test]
    public async Task GetCambiosPrecio_DiaConCambios_DevuelveLosDelDia()
    {
        // Act: el informe del 1 de octubre
        var cambios = await ProductoRepositorio.GetCambiosPrecioAsync(
            new DateTime(2026, 10, 1, 0, 0, 0, DateTimeKind.Utc));

        // Assert: la particion ES el dia
        cambios.Should().ContainSingle(c => c.ProductoId == 101 && c.Precio == 99.99);
    }

    [Test]
    public async Task RegistrarCambioPrecio_EscribeElCambioEnTodasLasCopias()
    {
        // Arrange: el Raton (102) vale 29.99 en el sembrado
        var precioAntes = (await ProductoRepositorio.GetProductoPorIdAsync(102))!.Precio;

        // Act
        var encontrado = await ProductoRepositorio.RegistrarCambioPrecioAsync(102, 24.99);

        // Assert: la fila nueva del historial sabe cuanto cambio
        encontrado.Should().BeTrue();
        var historial = await ProductoRepositorio.GetHistorialPreciosAsync(102);
        historial.Should().ContainSingle();
        historial[0].Precio.Should().Be(24.99);
        historial[0].PrecioAnterior.Should().Be(precioAntes);

        // El precio actual se actualizo en la copia de lectura por id
        (await ProductoRepositorio.GetProductoPorIdAsync(102))!.Precio.Should().Be(24.99);

        // Y el informe del dia de hoy tiene el cambio
        var hoy = DateTime.UtcNow.Date;
        (await ProductoRepositorio.GetCambiosPrecioAsync(hoy)).Should().ContainSingle(c => c.ProductoId == 102);

        // Limpieza: se compensa en CQL directo, como haria la aplicacion
        await EjecutarCqlAsync(
            "DELETE FROM historial_precios_por_producto WHERE producto_id = ? AND fecha = ?",
            102L, historial[0].Fecha);
        await EjecutarCqlAsync(
            "DELETE FROM cambios_precio_por_fecha WHERE fecha = ? AND producto_id = ?",
            hoy, 102L);
        await EjecutarCqlAsync(
            "UPDATE productos_por_id SET precio = ? WHERE id = ?", precioAntes, 102L);
        await EjecutarCqlAsync(
            "UPDATE productos_por_categoria SET precio = ? WHERE categoria_id = ? AND producto_id = ?",
            precioAntes, 1L, 102L);
    }

    [Test]
    public async Task RegistrarCambioPrecio_ProductoInexistente_DevuelveFalse()
    {
        // Act
        var encontrado = await ProductoRepositorio.RegistrarCambioPrecioAsync(999, 10.0);

        // Assert
        encontrado.Should().BeFalse();
    }
}
