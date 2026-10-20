using CSharpFunctionalExtensions;
using MongoDB.Bson;
using ProductosMongo.Errors;
using ProductosMongo.Models;
using ProductosMongo.Repositories;

namespace ProductosMongo.Services;

/// <summary>
/// Servicio de productos con patrón Result.
/// Implementa la lógica de negocio y validaciones.
/// </summary>
public class ProductoService(IProductoRepository repository) : IProductoService
{
    /// <inheritdoc />
    public IEnumerable<Producto> GetAll() => repository.GetAll();

    /// <inheritdoc />
    public Result<Producto, DomainError> GetById(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return Result.Failure<Producto, DomainError>(ProductoError.NotFound(id));

        var producto = repository.GetById(id);
        return producto is not null
            ? Result.Success<Producto, DomainError>(producto)
            : Result.Failure<Producto, DomainError>(ProductoError.NotFound(id));
    }

    /// <inheritdoc />
    public Result<Producto, DomainError> Create(Producto producto)
    {
        if (string.IsNullOrWhiteSpace(producto.Nombre))
            return Result.Failure<Producto, DomainError>(ProductoError.NombreVacio());

        if (producto.Precio <= 0)
            return Result.Failure<Producto, DomainError>(ProductoError.PrecioInvalido(producto.Precio));

        if (producto.ProveedorId == ObjectId.Empty)
            return Result.Failure<Producto, DomainError>(ProductoError.ProveedorInvalido());

        if (repository.IsDuplicated(producto.Nombre))
            return Result.Failure<Producto, DomainError>(ProductoError.NombreDuplicado(producto.Nombre));

        var created = repository.Add(producto);
        return Result.Success<Producto, DomainError>(created);
    }

    /// <inheritdoc />
    public Result<Producto, DomainError> Update(Producto producto)
    {
        if (string.IsNullOrWhiteSpace(producto.Nombre))
            return Result.Failure<Producto, DomainError>(ProductoError.NombreVacio());

        if (producto.Precio <= 0)
            return Result.Failure<Producto, DomainError>(ProductoError.PrecioInvalido(producto.Precio));

        if (producto.ProveedorId == ObjectId.Empty)
            return Result.Failure<Producto, DomainError>(ProductoError.ProveedorInvalido());

        var id = producto.Id.ToString();
        var existing = repository.GetById(id);
        if (existing is null)
            return Result.Failure<Producto, DomainError>(ProductoError.NotFound(id));

        if (repository.IsDuplicated(producto.Nombre, id))
            return Result.Failure<Producto, DomainError>(ProductoError.NombreDuplicado(producto.Nombre));

        var updated = repository.Update(producto);
        return updated is not null
            ? Result.Success<Producto, DomainError>(updated)
            : Result.Failure<Producto, DomainError>(ProductoError.NotFound(id));
    }

    /// <inheritdoc />
    public UnitResult<DomainError> Delete(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return UnitResult.Failure<DomainError>(ProductoError.NotFound(id));

        var deleted = repository.Delete(id);
        return deleted
            ? UnitResult.Success<DomainError>()
            : UnitResult.Failure<DomainError>(ProductoError.NotFound(id));
    }

    /// <inheritdoc />
    public IEnumerable<Producto> Search(string query) => repository.Search(query);

    /// <inheritdoc />
    public IEnumerable<Producto> Filter(string? nombre, decimal? precioMin, decimal? precioMax) =>
        repository.Filter(nombre, precioMin, precioMax);

    /// <inheritdoc />
    public IEnumerable<Producto> GetDeleted() => repository.GetDeleted();
}
