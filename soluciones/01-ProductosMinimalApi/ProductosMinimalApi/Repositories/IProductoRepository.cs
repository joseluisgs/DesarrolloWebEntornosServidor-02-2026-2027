using ProductosMinimalApi.Models;

namespace ProductosMinimalApi.Repositories;

/// <summary>
/// Contrato de acceso a datos de productos (almacenamiento en memoria).
/// </summary>
/// <remarks>
/// Toda la colección de trabajo son solo productos activos
/// (la eliminación lógica con <see cref="Producto.DeletedAt"/> la oculta el repositorio).
/// </remarks>
public interface IProductoRepository
{
    /// <summary>
    /// Devuelve todos los productos activos.
    /// </summary>
    IReadOnlyList<Producto> GetAll();

    /// <summary>
    /// Devuelve un producto activo por su identificador, o <c>null</c> si no existe o está eliminado.
    /// </summary>
    Producto? GetById(long id);

    /// <summary>
    /// Inserta un producto asignándole <see cref="Producto.Id"/> y <see cref="Producto.CreatedAt"/>.
    /// </summary>
    /// <param name="producto">Producto a crear.</param>
    /// <returns>El producto ya almacenado.</returns>
    Producto Add(Producto producto);

    /// <summary>
    /// Actualiza los campos de un producto existente.
    /// </summary>
    /// <returns>El producto actualizado, o <c>null</c> si no existe o está eliminado.</returns>
    Producto? Update(long id, Producto producto);

    /// <summary>
    /// Actualiza solo el precio de un producto (PATCH).
    /// </summary>
    /// <returns>El producto actualizado, o <c>null</c> si no existe o está eliminado.</returns>
    Producto? UpdatePrecio(long id, decimal precio);

    /// <summary>
    /// Marca un producto como eliminado (borrado lógico).
    /// </summary>
    /// <returns><c>true</c> si se ha eliminado; <c>false</c> si no existía o ya estaba eliminado.</returns>
    bool SoftDelete(long id);

    /// <summary>
    /// Busca productos activos cuyo nombre contenga el texto indicado.
    /// </summary>
    IReadOnlyList<Producto> Search(string? nombre);

    /// <summary>
    /// Devuelve los productos activos de una categoría (sin distinguir mayúsculas/minúsculas).
    /// </summary>
    IReadOnlyList<Producto> GetByCategoria(string categoria);

    /// <summary>
    /// Devuelve los productos activos cuyo precio está entre <paramref name="min"/> y <paramref name="max"/>, ordenados por precio.
    /// </summary>
    IReadOnlyList<Producto> GetByPrecio(decimal? min, decimal? max);

    /// <summary>
    /// Devuelve los productos activos ordenados por precio.
    /// </summary>
    /// <param name="asc"><c>true</c> ascendente, <c>false</c> descendente.</param>
    IReadOnlyList<Producto> GetOrdered(bool asc);

    /// <summary>
    /// Agrupa los productos activos por categoría.
    /// </summary>
    Dictionary<string, List<Producto>> GroupByCategoria();

    /// <summary>
    /// Agrega los productos activos: total, precio medio y desglose por categoría.
    /// </summary>
    ProductoEstadisticas GetEstadisticas();
}
