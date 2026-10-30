using ProductosCassandra.Dtos;

namespace ProductosCassandra.Services;

/// <summary>
/// Lógica de negocio sobre categorías.
/// </summary>
public interface ICategoriaService
{
    /// <summary>Todas las categorías del catálogo.</summary>
    /// <returns>Lista de DTOs de categoría.</returns>
    Task<IReadOnlyList<CategoriaDto>> GetAllAsync();

    /// <summary>Una categoría por su id.</summary>
    /// <param name="id">Id de la categoría.</param>
    /// <returns>El DTO o null si no existe.</returns>
    Task<CategoriaDto?> GetByIdAsync(long id);

    /// <summary>Los productos de una categoría (la consulta estrella de Cassandra).</summary>
    /// <param name="categoriaId">Id de la categoría (partition key).</param>
    /// <returns>DTOs de los productos de esa categoría.</returns>
    Task<IReadOnlyList<ProductoDto>> GetProductosAsync(long categoriaId);
}
