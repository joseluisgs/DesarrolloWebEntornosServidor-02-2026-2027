using ProductosMinimalApiDI.Models;

namespace ProductosMinimalApiDI.Repositories;

/// <summary>
/// Implementación del repositorio de productos con almacenamiento en memoria.
/// </summary>
public class ProductoRepository : IProductoRepository
{
    private readonly Dictionary<long, Producto> _productos = new();
    private long _nextId = 1;

    /// <inheritdoc />
    public IEnumerable<Producto> GetAll()
        => _productos.Values.Where(p => p.IsActivo);

    /// <inheritdoc />
    public Producto? GetById(long id)
        => _productos.TryGetValue(id, out var p) && p.IsActivo ? p : null;

    /// <inheritdoc />
    public Producto Add(Producto producto)
    {
        producto.Id = _nextId++;
        producto.CreatedAt = DateTime.UtcNow;
        _productos[producto.Id] = producto;
        return producto;
    }

    /// <inheritdoc />
    public Producto? Update(long id, Producto producto)
    {
        if (!_productos.TryGetValue(id, out var existente) || !existente.IsActivo)
            return null;

        existente.Nombre = producto.Nombre;
        existente.Precio = producto.Precio;
        existente.Categoria = producto.Categoria;
        existente.Imagen = producto.Imagen;
        existente.UpdatedAt = DateTime.UtcNow;
        return existente;
    }

    /// <inheritdoc />
    public Producto? PatchPrice(long id, decimal precio)
    {
        if (!_productos.TryGetValue(id, out var existente) || !existente.IsActivo)
            return null;

        existente.Precio = precio;
        existente.UpdatedAt = DateTime.UtcNow;
        return existente;
    }

    /// <inheritdoc />
    public bool Delete(long id)
    {
        if (!_productos.TryGetValue(id, out var existente) || !existente.IsActivo)
            return false;

        existente.DeletedAt = DateTime.UtcNow;
        return true;
    }

    /// <inheritdoc />
    public IEnumerable<Producto> Search(string? nombre)
        => _productos.Values
            .Where(p => p.IsActivo)
            .Where(p => p.Nombre.Contains(nombre ?? "", StringComparison.OrdinalIgnoreCase))
            .OrderBy(p => p.Nombre);

    /// <inheritdoc />
    public IEnumerable<Producto> FilterByCategoria(string categoria)
        => _productos.Values
            .Where(p => p.IsActivo)
            .Where(p => p.Categoria.Equals(categoria, StringComparison.OrdinalIgnoreCase));

    /// <inheritdoc />
    public IEnumerable<Producto> FilterByPrecio(decimal? min, decimal? max)
        => _productos.Values
            .Where(p => p.IsActivo)
            .Where(p => p.Precio >= (min ?? 0) && p.Precio <= (max ?? decimal.MaxValue))
            .OrderBy(p => p.Precio);

    /// <inheritdoc />
    public IEnumerable<Producto> OrderByPrecio(bool asc)
        => asc
            ? _productos.Values.Where(p => p.IsActivo).OrderBy(p => p.Precio)
            : _productos.Values.Where(p => p.IsActivo).OrderByDescending(p => p.Precio);

    /// <inheritdoc />
    public Dictionary<string, List<Producto>> GroupByCategoria()
        => _productos.Values
            .Where(p => p.IsActivo)
            .GroupBy(p => p.Categoria)
            .ToDictionary(g => g.Key, g => g.ToList());

    /// <inheritdoc />
    public object GetEstadisticas()
    {
        var activos = _productos.Values.Where(p => p.IsActivo).ToList();
        return new
        {
            Total = activos.Count,
            PrecioMedio = activos.Any() ? activos.Average(p => p.Precio) : 0,
            PorCategoria = activos.GroupBy(p => p.Categoria)
                .ToDictionary(g => g.Key, g => g.Count())
        };
    }
}
