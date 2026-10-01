using ProductosMinimalApiDI.Models;
using ProductosMinimalApiDI.Repositories;

namespace ProductosMinimalApiDI.Services;

/// <summary>
/// Implementación del servicio de productos con validación.
/// </summary>
public class ProductoService : IProductoService
{
    private readonly IProductoRepository _repository;

    /// <summary>
    /// Inicializa una nueva instancia del servicio de productos.
    /// </summary>
    /// <param name="repository">Repositorio de productos.</param>
    public ProductoService(IProductoRepository repository)
    {
        _repository = repository;
    }

    /// <inheritdoc />
    public IEnumerable<Producto> GetAll()
        => _repository.GetAll();

    /// <inheritdoc />
    public Producto? GetById(long id)
        => _repository.GetById(id);

    /// <inheritdoc />
    public Producto Add(Producto producto)
    {
        if (string.IsNullOrWhiteSpace(producto.Nombre))
            throw new ArgumentException("El nombre es obligatorio.");

        if (producto.Precio < 0)
            throw new ArgumentException("El precio no puede ser negativo.");

        if (string.IsNullOrWhiteSpace(producto.Categoria))
            throw new ArgumentException("La categoría es obligatoria.");

        return _repository.Add(producto);
    }

    /// <inheritdoc />
    public Producto? Update(long id, Producto producto)
    {
        if (string.IsNullOrWhiteSpace(producto.Nombre))
            throw new ArgumentException("El nombre es obligatorio.");

        if (producto.Precio < 0)
            throw new ArgumentException("El precio no puede ser negativo.");

        if (string.IsNullOrWhiteSpace(producto.Categoria))
            throw new ArgumentException("La categoría es obligatoria.");

        return _repository.Update(id, producto);
    }

    /// <inheritdoc />
    public Producto? PatchPrice(long id, decimal precio)
    {
        if (precio < 0)
            throw new ArgumentException("El precio no puede ser negativo.");

        return _repository.PatchPrice(id, precio);
    }

    /// <inheritdoc />
    public bool Delete(long id)
        => _repository.Delete(id);

    /// <inheritdoc />
    public IEnumerable<Producto> Search(string? nombre)
        => _repository.Search(nombre);

    /// <inheritdoc />
    public IEnumerable<Producto> FilterByCategoria(string categoria)
        => _repository.FilterByCategoria(categoria);

    /// <inheritdoc />
    public IEnumerable<Producto> FilterByPrecio(decimal? min, decimal? max)
        => _repository.FilterByPrecio(min, max);

    /// <inheritdoc />
    public IEnumerable<Producto> OrderByPrecio(bool asc)
        => _repository.OrderByPrecio(asc);

    /// <inheritdoc />
    public Dictionary<string, List<Producto>> GroupByCategoria()
        => _repository.GroupByCategoria();

    /// <inheritdoc />
    public object GetEstadisticas()
        => _repository.GetEstadisticas();
}
