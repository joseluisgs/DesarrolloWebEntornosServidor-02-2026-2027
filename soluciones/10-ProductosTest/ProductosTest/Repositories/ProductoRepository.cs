using ProductosTest.Models;

namespace ProductosTest.Repositories;

public class ProductoRepository : IProductoRepository
{
    private readonly Dictionary<long, Producto> _productos = [];
    private long _nextId = 1;

    public IEnumerable<Producto> GetAll() =>
        _productos.Values.Where(p => p.IsActivo);

    public Producto? GetById(long id) =>
        _productos.TryGetValue(id, out var producto) && producto.IsActivo ? producto : null;

    public Producto Add(Producto producto)
    {
        producto.Id = _nextId++;
        producto.CreatedAt = DateTime.UtcNow;
        _productos[producto.Id] = producto;
        return producto;
    }

    public Producto? Update(long id, Producto producto)
    {
        if (!_productos.TryGetValue(id, out var existente) || !existente.IsActivo)
            return null;

        producto.Id = id;
        producto.CreatedAt = existente.CreatedAt;
        producto.UpdatedAt = DateTime.UtcNow;
        _productos[id] = producto;
        return producto;
    }

    public Producto? PatchPrice(long id, decimal precio)
    {
        if (!_productos.TryGetValue(id, out var producto) || !producto.IsActivo)
            return null;

        producto.Precio = precio;
        producto.UpdatedAt = DateTime.UtcNow;
        return producto;
    }

    public bool Delete(long id)
    {
        if (!_productos.TryGetValue(id, out var producto) || !producto.IsActivo)
            return false;

        producto.IsActivo = false;
        producto.DeletedAt = DateTime.UtcNow;
        return true;
    }

    public IEnumerable<Producto> Search(string termino) =>
        _productos.Values.Where(p =>
            p.IsActivo &&
            (p.Nombre.Contains(termino, StringComparison.OrdinalIgnoreCase) ||
             p.Categoria.Contains(termino, StringComparison.OrdinalIgnoreCase)));

    public IEnumerable<Producto> Filter(string? nombre, string? categoria, decimal? precioMin, decimal? precioMax)
    {
        var filtrados = _productos.Values.Where(p => p.IsActivo);

        if (!string.IsNullOrEmpty(nombre))
            filtrados = filtrados.Where(p => p.Nombre.Contains(nombre, StringComparison.OrdinalIgnoreCase));
        if (!string.IsNullOrEmpty(categoria))
            filtrados = filtrados.Where(p => p.Categoria.Equals(categoria, StringComparison.OrdinalIgnoreCase));
        if (precioMin.HasValue)
            filtrados = filtrados.Where(p => p.Precio >= precioMin.Value);
        if (precioMax.HasValue)
            filtrados = filtrados.Where(p => p.Precio <= precioMax.Value);

        return filtrados;
    }

    public (IEnumerable<Producto> Items, int TotalPages, int TotalItems) GetPaged(int page, int pageSize)
    {
        // El techo de paginación vive en la capa de datos: el filtro puede construirse en código
        // (jobs, tests, GraphQL) sin pasar por la validación REST, así que aquí es donde el
        // Skip/Take no sale de 1..100.
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var all = _productos.Values.Where(p => p.IsActivo).ToList();
        var totalItems = all.Count;
        var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
        var items = all.Skip((page - 1) * pageSize).Take(pageSize);

        return (items, totalPages, totalItems);
    }
}
