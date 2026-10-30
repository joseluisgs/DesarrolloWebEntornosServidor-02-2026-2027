using ProductosCassandra.Models;

namespace ProductosCassandra.Repositories;

/// <summary>
/// Acceso a datos de categorías en Cassandra.
/// </summary>
public interface ICategoriaRepository
{
    /// <summary>Consulta Q3: todas las categorías.</summary>
    /// <returns>Lista de categorías.</returns>
    Task<IReadOnlyList<Categoria>> GetCategoriasAsync();

    /// <summary>Devuelve una categoría por su id, o null si no existe.</summary>
    /// <param name="id">Id de la categoría.</param>
    /// <returns>La categoría o null.</returns>
    Task<Categoria?> GetCategoriaAsync(long id);
}
