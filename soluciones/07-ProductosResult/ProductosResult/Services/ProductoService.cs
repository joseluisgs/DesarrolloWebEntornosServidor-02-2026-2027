using CSharpFunctionalExtensions;
using ProductosResult.Errors;
using ProductosResult.Models;
using ProductosResult.Repositories;

namespace ProductosResult.Services;

public class ProductoService(IProductoRepository repository) : IProductoService
{
    public Result<IEnumerable<Producto>, DomainError> GetAll() =>
        Result.Success<IEnumerable<Producto>, DomainError>(repository.GetAll());

    public Result<Producto, DomainError> GetById(long id)
    {
        var producto = repository.GetById(id);
        return producto is not null
            ? Result.Success<Producto, DomainError>(producto)
            : Result.Failure<Producto, DomainError>(ProductoError.NotFound(id));
    }

    public Result<Producto, DomainError> Create(Producto producto)
    {
        if (string.IsNullOrWhiteSpace(producto.Nombre))
            return Result.Failure<Producto, DomainError>(ProductoError.NombreVacio());

        if (producto.Precio < 0)
            return Result.Failure<Producto, DomainError>(ProductoError.PrecioInvalido(producto.Precio));

        var creado = repository.Add(producto);
        return Result.Success<Producto, DomainError>(creado);
    }

    public Result<Producto, DomainError> Update(long id, Producto producto)
    {
        if (string.IsNullOrWhiteSpace(producto.Nombre))
            return Result.Failure<Producto, DomainError>(ProductoError.NombreVacio());

        if (producto.Precio < 0)
            return Result.Failure<Producto, DomainError>(ProductoError.PrecioInvalido(producto.Precio));

        var actualizado = repository.Update(id, producto);
        return actualizado is not null
            ? Result.Success<Producto, DomainError>(actualizado)
            : Result.Failure<Producto, DomainError>(ProductoError.NotFound(id));
    }

    public Result<Producto, DomainError> PatchPrice(long id, decimal precio)
    {
        if (precio < 0)
            return Result.Failure<Producto, DomainError>(ProductoError.PrecioInvalido(precio));

        var actualizado = repository.PatchPrice(id, precio);
        return actualizado is not null
            ? Result.Success<Producto, DomainError>(actualizado)
            : Result.Failure<Producto, DomainError>(ProductoError.NotFound(id));
    }

    public UnitResult<DomainError> Delete(long id)
    {
        var eliminado = repository.Delete(id);
        return eliminado
            ? UnitResult.Success<DomainError>()
            : UnitResult.Failure<DomainError>(ProductoError.NotFound(id));
    }

    public Result<IEnumerable<Producto>, DomainError> Search(string? termino)
    {
        if (string.IsNullOrWhiteSpace(termino))
        {
            return Result.Failure<IEnumerable<Producto>, DomainError>(
                ValidationError.Create("El término de búsqueda es obligatorio"));
        }

        return Result.Success<IEnumerable<Producto>, DomainError>(repository.Search(termino));
    }
}
