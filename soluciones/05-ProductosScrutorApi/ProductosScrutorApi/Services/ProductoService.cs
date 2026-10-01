using ProductosScrutorApi.Models;
using ProductosScrutorApi.Repositories;

namespace ProductosScrutorApi.Services;

/// <summary>
/// Servicio de productos con lógica de negocio.
/// </summary>
public class ProductoService(IProductoRepository repository) : IProductoService
{
    public IEnumerable<Producto> GetAll() => repository.GetAll();

    public Producto? GetById(long id) => repository.GetById(id);

    public Producto Create(Producto producto)
    {
        if (string.IsNullOrWhiteSpace(producto.Nombre))
            throw new ArgumentException("El nombre es obligatorio");

        if (producto.Precio <= 0)
            throw new ArgumentException("El precio debe ser mayor que cero");

        return repository.Add(producto);
    }

    public Producto? Update(long id, Producto producto)
    {
        if (string.IsNullOrWhiteSpace(producto.Nombre))
            throw new ArgumentException("El nombre es obligatorio");

        if (producto.Precio <= 0)
            throw new ArgumentException("El precio debe ser mayor que cero");

        return repository.Update(id, producto);
    }

    public Producto? PatchPrice(long id, decimal precio)
    {
        if (precio <= 0)
            throw new ArgumentException("El precio debe ser mayor que cero");

        return repository.PatchPrice(id, precio);
    }

    public bool Delete(long id) => repository.Delete(id);

    public IEnumerable<Producto> Search(string? nombre) => repository.Search(nombre);

    public IEnumerable<Producto> FilterByCategoria(string categoria) =>
        repository.FilterByCategoria(categoria);

    public IEnumerable<Producto> FilterByPrecio(decimal? min, decimal? max) =>
        repository.FilterByPrecio(min, max);

    public IEnumerable<Producto> OrderByPrecio(bool asc) => repository.OrderByPrecio(asc);

    public Dictionary<string, List<Producto>> GroupByCategoria() =>
        repository.GroupByCategoria();

    public object GetEstadisticas() => repository.GetEstadisticas();
}
