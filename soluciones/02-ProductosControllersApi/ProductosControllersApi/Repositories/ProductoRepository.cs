using ProductosControllersApi.Models;

namespace ProductosControllersApi.Repositories;

/// <summary>
/// Repositorio en memoria basado en <see cref="Dictionary{TKey,TValue}"/>. Sin bloqueos: la
/// colección es privada y los productos son registros inmutables que se reemplazan con <c>with</c>.
/// </summary>
public sealed class ProductoRepository : IProductoRepository
{
    private readonly Dictionary<long, Producto> _productos = [];
    private long _nextId = 1;

    public ProductoRepository() {
        _productos[1] = new Producto
        {
            Id = 1,
            Nombre = "Camiseta",
            Precio = 19.99m,
            Categoria = "Ropa",
            Imagen = "https://example.com/camiseta.jpg",
            CreatedAt = DateTime.UtcNow
        };
        _productos[2] = new Producto
        {
            Id = 2,
            Nombre = "Pantalón",
            Precio = 39.99m,
            Categoria = "Ropa",
            Imagen = "https://example.com/pantalon.jpg",
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <inheritdoc />
    public IReadOnlyList<Producto> GetAll() => Activos();

    /// <inheritdoc />
    public Producto? GetById(long id) => TryGet(id);

    /// <inheritdoc />
    public Producto Add(Producto producto)
    {
        var nuevo = producto with
        {
            Id = _nextId++,
            CreatedAt = DateTime.UtcNow
        };

        _productos[nuevo.Id] = nuevo;
        return nuevo;
    }

    /// <inheritdoc />
    public Producto? Update(long id, Producto producto)
    {
        if (TryGet(id) is not { } existente)
        {
            return null;
        }

        var actualizado = existente with
        {
            Nombre = producto.Nombre,
            Precio = producto.Precio,
            Categoria = producto.Categoria,
            Imagen = producto.Imagen,
            UpdatedAt = DateTime.UtcNow
        };

        _productos[id] = actualizado;
        return actualizado;
    }

    /// <inheritdoc />
    public Producto? UpdatePrecio(long id, decimal precio)
    {
        if (TryGet(id) is not { } existente)
        {
            return null;
        }

        var actualizado = existente with { Precio = precio, UpdatedAt = DateTime.UtcNow };

        _productos[id] = actualizado;
        return actualizado;
    }

    /// <inheritdoc />
    public bool SoftDelete(long id)
    {
        if (TryGet(id) is not { } existente)
        {
            return false;
        }

        _productos[id] = existente with { DeletedAt = DateTime.UtcNow };
        return true;
    }

    /// <inheritdoc />
    public IReadOnlyList<Producto> Search(string? nombre)
    {
        var texto = nombre ?? string.Empty;

        return Activos()
            .Where(p => p.Nombre.Contains(texto, StringComparison.OrdinalIgnoreCase))
            .OrderBy(p => p.Nombre)
            .ToList();
    }

    /// <inheritdoc />
    public IReadOnlyList<Producto> GetByCategoria(string categoria) =>
        Activos()
            .Where(p => p.Categoria.Equals(categoria, StringComparison.OrdinalIgnoreCase))
            .ToList();

    /// <inheritdoc />
    public IReadOnlyList<Producto> GetByPrecio(decimal? min, decimal? max) =>
        Activos()
            .Where(p => p.Precio >= (min ?? 0) && p.Precio <= (max ?? decimal.MaxValue))
            .OrderBy(p => p.Precio)
            .ToList();

    /// <inheritdoc />
    public IReadOnlyList<Producto> GetOrdered(bool asc)
    {
        var activos = Activos();

        return asc
            ? activos.OrderBy(p => p.Precio).ToList()
            : activos.OrderByDescending(p => p.Precio).ToList();
    }

    /// <inheritdoc />
    public Dictionary<string, List<Producto>> GroupByCategoria() =>
        Activos()
            .GroupBy(p => p.Categoria)
            .ToDictionary(g => g.Key, g => g.ToList());

    /// <inheritdoc />
    public ProductoEstadisticas GetEstadisticas()
    {
        var activos = Activos();

        return new ProductoEstadisticas(
            Total: activos.Count,
            PrecioMedio: activos is [] ? 0 : activos.Average(p => p.Precio),
            PorCategoria: activos
                .GroupBy(p => p.Categoria)
                .ToDictionary(g => g.Key, g => g.Count()));
    }

    /// <summary>
    /// Devuelve una instantánea de los productos activos (los eliminados lógicamente quedan fuera).
    /// </summary>
    private List<Producto> Activos() => [.. _productos.Values.Where(p => p.IsActivo)];

    /// <summary>
    /// Devuelve un producto activo por id, o <c>null</c> si no existe o está eliminado.
    /// </summary>
    private Producto? TryGet(long id) =>
        _productos.TryGetValue(id, out var producto) && producto.IsActivo ? producto : null;
}
