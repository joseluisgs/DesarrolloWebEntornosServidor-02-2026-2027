using ProductosCassandra.Dtos;
using ProductosCassandra.Models;

namespace ProductosCassandra.Mappers;

/// <summary>
/// Proyección de los modelos de Cassandra (filas de tablas) a DTOs de la API.
/// Cada modelo tiene su ToDto: la capa de presentación nunca ve los atributos
/// [Table]/[Column] ni los nombres de columnas de CQL.
/// </summary>
public static class ProductoMapper
{
    /// <summary>Convierte el DTO de entrada en el modelo de creación en memoria.</summary>
    /// <param name="dto">DTO que llega del controlador.</param>
    /// <returns>Modelo de creación, sin la categoría todavía.</returns>
    public static CreateProducto ToModel(this CreateProductoDto dto) =>
        new(dto.Id, dto.Nombre, dto.Precio);

    /// <summary>Convierte una fila de <c>productos_por_id</c> en su DTO.</summary>
    /// <param name="producto">Fila de la tabla por id.</param>
    /// <returns>DTO del producto.</returns>
    public static ProductoDto ToDto(this ProductoPorId producto) =>
        new(producto.Id, producto.Nombre, producto.Precio,
            producto.CategoriaId, producto.CategoriaNombre);

    /// <summary>Convierte una fila de <c>productos_por_categoria</c> en su DTO.</summary>
    /// <param name="producto">Fila de la tabla por categoría.</param>
    /// <returns>DTO del producto.</returns>
    public static ProductoDto ToDto(this ProductoPorCategoria producto) =>
        new(producto.ProductoId, producto.Nombre, producto.Precio,
            producto.CategoriaId, producto.CategoriaNombre);

    /// <summary>Convierte una fila de <c>categorias</c> en su DTO.</summary>
    /// <param name="categoria">Fila de la tabla de categorías.</param>
    /// <returns>DTO de la categoría.</returns>
    public static CategoriaDto ToDto(this Categoria categoria) =>
        new(categoria.Id, categoria.Nombre, categoria.Descripcion);

    /// <summary>Convierte una fila del histórico en su DTO.</summary>
    /// <param name="fila">Fila de <c>historial_precios_por_producto</c>.</param>
    /// <returns>DTO del cambio de precio.</returns>
    public static HistorialPrecioDto ToDto(this HistorialPrecioPorProducto fila) =>
        new(fila.Fecha, fila.Precio, fila.PrecioAnterior);

    /// <summary>Convierte una fila del informe diario en su DTO.</summary>
    /// <param name="fila">Fila de <c>cambios_precio_por_fecha</c>.</param>
    /// <returns>DTO del cambio del día.</returns>
    public static CambioPrecioDiaDto ToDto(this CambioPrecioPorFecha fila) =>
        new(fila.ProductoId, fila.Precio);
}
