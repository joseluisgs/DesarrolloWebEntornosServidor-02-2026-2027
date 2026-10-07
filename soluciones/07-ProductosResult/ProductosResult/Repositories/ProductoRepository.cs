using ProductosResult.Models;

namespace ProductosResult.Repositories;

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
}
