using ProductosCassandra.Dtos;

namespace ProductosCassandra.Services;

/// <summary>
/// Lógica de negocio sobre productos.
/// Un cambio de precio aquí es UNA operación de negocio; por debajo se convierte en
/// cuatro escrituras en Cassandra, y esa cuenta es asunto de esta capa, no del controlador.
/// </summary>
public interface IProductoService
{
    /// <summary>El producto por su id.</summary>
    /// <param name="id">Id del producto.</param>
    /// <returns>El DTO o null si no existe.</returns>
    Task<ProductoDto?> GetByIdAsync(long id);

    /// <summary>Alta de producto: escribe en las dos tablas con el nombre de la categoría copiado.</summary>
    /// <param name="dto">Datos del producto nuevo (incluye la categoría).</param>
    /// <returns>El DTO creado, o null si la categoría no existe.</returns>
    Task<ProductoDto?> CreateAsync(CreateProductoDto dto);

    /// <summary>Borrado compensatorio en las dos tablas donde vive el producto.</summary>
    /// <param name="id">Id del producto.</param>
    /// <param name="categoriaId">Id de la categoría (lo necesita la tabla por categoría).</param>
    /// <returns>Tarea asíncrona.</returns>
    Task DeleteAsync(long id, long categoriaId);

    /// <summary>El histórico de precios de un producto, del más reciente al más antiguo.</summary>
    /// <param name="id">Id del producto (partition key).</param>
    /// <returns>DTOs del histórico ya ordenados.</returns>
    Task<IReadOnlyList<HistorialPrecioDto>> GetHistorialPreciosAsync(long id);

    /// <summary>Los precios que cambiaron en un día dado (el informe).</summary>
    /// <param name="fecha">Día del informe (partition key).</param>
    /// <returns>DTOs de los cambios de ese día.</returns>
    Task<IReadOnlyList<CambioPrecioDiaDto>> GetCambiosPrecioAsync(DateTime fecha);

    /// <summary>Registra un cambio de precio en todas las copias.</summary>
    /// <param name="id">Id del producto.</param>
    /// <param name="precioNuevo">Precio nuevo.</param>
    /// <returns>false si el producto no existe.</returns>
    Task<bool> RegistrarCambioPrecioAsync(long id, double precioNuevo);
}
