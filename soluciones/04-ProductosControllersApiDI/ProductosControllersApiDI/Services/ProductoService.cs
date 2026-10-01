using ProductosControllersApiDI.Models;
using ProductosControllersApiDI.Repositories;

namespace ProductosControllersApiDI.Services;

/// <summary>
/// Implementación del servicio de productos con validación.
/// </summary>
public class ProductoService(IProductoRepository repository) : IProductoService
{
    /// <inheritdoc />
    public IEnumerable<Producto> GetAll() => repository.GetAll();

    /// <inheritdoc />
    public Producto? GetById(long id) => repository.GetById(id);

    /// <inheritdoc />
    public Producto Add(Producto producto)
    {
        if (string.IsNullOrWhiteSpace(producto.Nombre))
            throw new ArgumentException("El nombre del producto es obligatorio.");

        if (producto.Precio < 0)
            throw new ArgumentException("El precio no puede ser negativo.");

        return repository.Add(producto);
    }

    /// <inheritdoc />
    public Producto? Update(long id, Producto producto)
    {
        if (string.IsNullOrWhiteSpace(producto.Nombre))
            throw new ArgumentException("El nombre del producto es obligatorio.");

        if (producto.Precio < 0)
            throw new ArgumentException("El precio no puede ser negativo.");

        return repository.Update(id, producto);
    }

    /// <inheritdoc />
    public Producto? PatchPrice(long id, decimal precio)
    {
        if (precio < 0)
            throw new ArgumentException("El precio no puede ser negativo.");

        return repository.PatchPrice(id, precio);
    }

    /// <inheritdoc />
    public bool Delete(long id) => repository.Delete(id);

    /// <inheritdoc />
    public IEnumerable<Producto> Search(string termino) => repository.Search(termino);

    /// <inheritdoc />
    public IEnumerable<Producto> FilterByCategoria(string categoria) =>
        repository.FilterByCategoria(categoria);

    /// <inheritdoc />
    public IEnumerable<Producto> FilterByPrecio(decimal min, decimal max) =>
        repository.FilterByPrecio(min, max);

    /// <inheritdoc />
    public IEnumerable<Producto> OrderByPrecio(bool descendente = false) =>
        repository.OrderByPrecio(descendente);

    /// <inheritdoc />
    public IEnumerable<IGrouping<string, Producto>> GroupByCategoria() =>
        repository.GroupByCategoria();

    /// <inheritdoc />
    public object GetEstadisticas() => repository.GetEstadisticas();
}
