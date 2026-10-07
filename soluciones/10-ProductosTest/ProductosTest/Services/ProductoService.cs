using CSharpFunctionalExtensions;
using ProductosTest.Dtos;
using ProductosTest.Errors;
using ProductosTest.Mappers;
using ProductosTest.Models;
using ProductosTest.Repositories;

namespace ProductosTest.Services;

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

    public Result<Producto, DomainError> Create(CreateProductoDto dto)
    {
        var producto = dto.ToModel();
        var creado = repository.Add(producto);
        return Result.Success<Producto, DomainError>(creado);
    }

    public Result<Producto, DomainError> Update(long id, UpdateProductoDto dto)
    {
        var producto = dto.ToModel();
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

    public Result<IEnumerable<Producto>, DomainError> Search(string termino) =>
        Result.Success<IEnumerable<Producto>, DomainError>(repository.Search(termino));

    public Result<IEnumerable<Producto>, DomainError> Filter(string? nombre, string? categoria, decimal? precioMin, decimal? precioMax) =>
        Result.Success<IEnumerable<Producto>, DomainError>(repository.Filter(nombre, categoria, precioMin, precioMax));

    public Result<(IEnumerable<Producto> Items, int TotalPages, int TotalItems), DomainError> GetPaged(int page, int pageSize)
    {
        // El recorte y el conteo son trabajo de datos: aquí solo se envuelve en Result.
        var (items, totalPages, totalItems) = repository.GetPaged(page, pageSize);
        return Result.Success<(IEnumerable<Producto>, int, int), DomainError>((items, totalPages, totalItems));
    }
}
