using ProductosExcepciones.Errors;
using ProductosExcepciones.Models;
using ProductosExcepciones.Repositories;

namespace ProductosExcepciones.Services;

/// <summary>
/// Implementación del servicio de productos con validación mediante excepciones de dominio.
/// </summary>
public class ProductoService(IProductoRepository repository) : IProductoService
{
    /// <inheritdoc />
    public IEnumerable<Producto> GetAll() => repository.GetAll();

    /// <inheritdoc />
    public Producto GetById(long id) =>
        repository.GetById(id) ?? throw ProductoException.NotFound(id);

    /// <inheritdoc />
    public Producto Add(Producto producto)
    {
        if (string.IsNullOrWhiteSpace(producto.Nombre))
            throw ProductoException.NombreVacio();

        if (producto.Precio <= 0)
            throw ProductoException.PrecioInvalido(producto.Precio);

        if (repository.GetByNombre(producto.Nombre) is not null)
            throw ProductoException.NombreDuplicado(producto.Nombre);

        return repository.Add(producto);
    }

    /// <inheritdoc />
    public Producto Update(long id, Producto producto)
    {
        if (string.IsNullOrWhiteSpace(producto.Nombre))
            throw ProductoException.NombreVacio();

        if (producto.Precio <= 0)
            throw ProductoException.PrecioInvalido(producto.Precio);

        var existente = repository.GetById(id)
            ?? throw ProductoException.NotFound(id);

        var duplicado = repository.GetByNombre(producto.Nombre);
        if (duplicado is not null && duplicado.Id != id)
            throw ProductoException.NombreDuplicado(producto.Nombre);

        return repository.Update(id, producto)
            ?? throw ProductoException.NotFound(id);
    }

    /// <inheritdoc />
    public Producto PatchPrice(long id, decimal precio)
    {
        if (precio <= 0)
            throw ProductoException.PrecioInvalido(precio);

        return repository.PatchPrice(id, precio)
            ?? throw ProductoException.NotFound(id);
    }

    /// <inheritdoc />
    public bool Delete(long id)
    {
        if (repository.GetById(id) is null)
            throw ProductoException.NotFound(id);

        return repository.Delete(id);
    }

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
