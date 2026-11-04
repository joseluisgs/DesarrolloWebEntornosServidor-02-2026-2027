using GreenDonut;
using Microsoft.EntityFrameworkCore;
using ProductosGraphQL.Entity;
using ProductosGraphQL.Models;

namespace ProductosGraphQL.DataLoaders;

/// <summary>
/// DataLoader para cargar categorias por ID.
/// Resuelve el problema N+1 agrupando las consultas.
/// </summary>
public class CategoriaByIdDataLoader : BatchDataLoader<long, Categoria>
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    public CategoriaByIdDataLoader(
        IDbContextFactory<AppDbContext> dbContextFactory,
        IBatchScheduler batchScheduler,
        DataLoaderOptions options)
        : base(batchScheduler, options)
    {
        _dbContextFactory = dbContextFactory;
    }

    protected override async Task<IReadOnlyDictionary<long, Categoria>> LoadBatchAsync(
        IReadOnlyList<long> keys,
        CancellationToken cancellationToken)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        return await context.Categorias
            .Where(c => keys.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id, cancellationToken);
    }
}
