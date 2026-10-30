using Cassandra;
using Cassandra.Data.Linq;
using ProductosCassandra.Models;
using ISession = Cassandra.ISession;

namespace ProductosCassandra.Repositories.Cassandra;

/// <inheritdoc />
public class CategoriaCassandraRepository(ISession session) : ICategoriaRepository
{
    // El mapper de DataStax usa los atributos [Table]/[Column]/[PartitionKey] del modelo.
    private readonly Table<Categoria> _categorias = new(session);

    /// <inheritdoc />
    public async Task<IReadOnlyList<Categoria>> GetCategoriasAsync()
    {
        // SELECT * FROM categorias
        var categorias = await _categorias.Select(c => c).ExecuteAsync();
        return categorias.ToList();
    }

    /// <inheritdoc />
    public async Task<Categoria?> GetCategoriaAsync(long id)
    {
        // SELECT * FROM categorias WHERE id = ?
        var categorias = await _categorias.Where(c => c.Id == id).Take(1).ExecuteAsync();
        return categorias.FirstOrDefault();
    }
}
