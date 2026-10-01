using ProductosControllersApiDI.Models;

namespace ProductosControllersApiDI.Repositories;

/// <summary>
/// Implementación del repositorio de productos con almacenamiento en diccionario.
/// </summary>
public class ProductoRepository : IProductoRepository
{
    private readonly Dictionary<long, Producto> _productos = [];
    private long _nextId = 1;

    public ProductoRepository()
    {
        // Datos iniciales de ejemplo
        var now = DateTime.UtcNow;
        Add(new Producto { Nombre = "Portátil", Precio = 899.99m, Categoria = "Electrónica", CreatedAt = now });
        Add(new Producto { Nombre = "Ratón", Precio = 29.99m, Categoria = "Electrónica", CreatedAt = now });
        Add(new Producto { Nombre = "Teclado", Precio = 79.99m, Categoria = "Electrónica", CreatedAt = now });
        Add(new Producto { Nombre = "Silla", Precio = 199.99m, Categoria = "Mobiliario", CreatedAt = now });
        Add(new Producto { Nombre = "Escritorio", Precio = 349.99m, Categoria = "Mobiliario", CreatedAt = now });
    }

    /// <inheritdoc />
    public IEnumerable<Producto> GetAll() =>
        _productos.Values.Where(p => p.IsActivo);

    /// <inheritdoc />
    public Producto? GetById(long id) =>
        _productos.TryGetValue(id, out var producto) && producto.IsActivo ? producto : null;

    /// <inheritdoc />
    public Producto Add(Producto producto)
    {
        var nuevo = new Producto
        {
            Id = _nextId++,
            Nombre = producto.Nombre,
            Precio = producto.Precio,
            Categoria = producto.Categoria,
            Imagen = producto.Imagen,
            CreatedAt = DateTime.UtcNow
        };
        _productos[nuevo.Id] = nuevo;
        return nuevo;
    }

    /// <inheritdoc />
    public Producto? Update(long id, Producto producto)
    {
        if (!_productos.TryGetValue(id, out var existente) || !existente.IsActivo)
            return null;

        var actualizado = new Producto
        {
            Id = id,
            Nombre = producto.Nombre,
            Precio = producto.Precio,
            Categoria = producto.Categoria,
            Imagen = producto.Imagen,
            CreatedAt = existente.CreatedAt,
            UpdatedAt = DateTime.UtcNow
        };
        _productos[id] = actualizado;
        return actualizado;
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
        if (!_productos.TryGetValue(id, out var producto) || !producto.IsActivo)
            return false;

        producto.DeletedAt = DateTime.UtcNow;
        return true;
    }

    /// <inheritdoc />
    public IEnumerable<Producto> Search(string termino) =>
        _productos.Values.Where(p => p.IsActivo &&
            (p.Nombre.Contains(termino, StringComparison.OrdinalIgnoreCase) ||
             p.Categoria.Contains(termino, StringComparison.OrdinalIgnoreCase)));

    /// <inheritdoc />
    public IEnumerable<Producto> FilterByCategoria(string categoria) =>
        _productos.Values.Where(p => p.IsActivo &&
            p.Categoria.Equals(categoria, StringComparison.OrdinalIgnoreCase));

    /// <inheritdoc />
    public IEnumerable<Producto> FilterByPrecio(decimal min, decimal max) =>
        _productos.Values.Where(p => p.IsActivo && p.Precio >= min && p.Precio <= max);

    /// <inheritdoc />
    public IEnumerable<Producto> OrderByPrecio(bool descendente = false) =>
        descendente
            ? _productos.Values.Where(p => p.IsActivo).OrderByDescending(p => p.Precio)
            : _productos.Values.Where(p => p.IsActivo).OrderBy(p => p.Precio);

    /// <inheritdoc />
    public IEnumerable<IGrouping<string, Producto>> GroupByCategoria() =>
        _productos.Values.Where(p => p.IsActivo).GroupBy(p => p.Categoria);

    /// <inheritdoc />
    public object GetEstadisticas()
    {
        var activos = _productos.Values.Where(p => p.IsActivo).ToList();
        return new
        {
            TotalProductos = activos.Count,
            PrecioMedio = activos.Any() ? activos.Average(p => p.Precio) : 0,
            PrecioMinimo = activos.Any() ? activos.Min(p => p.Precio) : 0,
            PrecioMaximo = activos.Any() ? activos.Max(p => p.Precio) : 0,
            Categorias = activos.Select(p => p.Categoria).Distinct().Count()
        };
    }
}
