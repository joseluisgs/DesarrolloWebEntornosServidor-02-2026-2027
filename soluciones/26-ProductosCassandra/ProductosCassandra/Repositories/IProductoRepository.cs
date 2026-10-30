using ProductosCassandra.Models;

namespace ProductosCassandra.Repositories;

/// <summary>
/// Acceso a datos de productos en Cassandra.
/// Cada método responde a UNA consulta planificada (query-first):
/// no hay un GetAll genérico porque no existe una tabla que devuelva todos los productos.
/// </summary>
public interface IProductoRepository
{
    /// <summary>
    /// Consulta Q1: lee la partición de una categoría entera, ordenada por la clustering key.
    /// </summary>
    /// <param name="categoriaId">Id de la categoría (partition key).</param>
    /// <returns>Productos de esa categoría.</returns>
    Task<IReadOnlyList<ProductoPorCategoria>> GetProductosPorCategoriaAsync(long categoriaId);

    /// <summary>Consulta Q2: el producto suelto por su id.</summary>
    /// <param name="id">Id del producto.</param>
    /// <returns>El producto o null.</returns>
    Task<ProductoPorId?> GetProductoPorIdAsync(long id);

    /// <summary>
    /// Escribe el producto en las DOS tablas, copiando el nombre de la categoría en cada fila.
    /// </summary>
    /// <param name="producto">Datos del producto nuevo.</param>
    /// <param name="categoria">Categoría resuelta: de aquí sale el nombre que se copia.</param>
    /// <returns>Tarea asíncrona.</returns>
    Task CrearProductoAsync(CreateProducto producto, Categoria categoria);

    /// <summary>Borra el producto de las dos tablas donde vive copiado.</summary>
    /// <param name="id">Id del producto.</param>
    /// <param name="categoriaId">Id de la categoría.</param>
    /// <returns>Tarea asíncrona.</returns>
    Task BorrarProductoAsync(long id, long categoriaId);

    /// <summary>Consulta Q4: el histórico de precios de un producto, del más reciente al más antiguo.</summary>
    /// <param name="productoId">Id del producto (partition key).</param>
    /// <returns>Los cambios de precio ordenados por fecha descendente.</returns>
    Task<IReadOnlyList<HistorialPrecioPorProducto>> GetHistorialPreciosAsync(long productoId);

    /// <summary>Consulta Q5: los precios que cambiaron en un día dado (el informe).</summary>
    /// <param name="fecha">Día del informe (partition key).</param>
    /// <returns>Los cambios de ese día.</returns>
    Task<IReadOnlyList<CambioPrecioPorFecha>> GetCambiosPrecioAsync(DateTime fecha);

    /// <summary>
    /// Registra un cambio de precio en todas las copias: historial, informe del día
    /// y precio actual en las tablas de lectura.
    /// </summary>
    /// <param name="productoId">Id del producto.</param>
    /// <param name="precioNuevo">Precio nuevo.</param>
    /// <returns>false si el producto no existe.</returns>
    Task<bool> RegistrarCambioPrecioAsync(long productoId, double precioNuevo);
}
