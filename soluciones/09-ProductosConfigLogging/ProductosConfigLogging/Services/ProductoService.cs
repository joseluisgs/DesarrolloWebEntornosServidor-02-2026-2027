using CSharpFunctionalExtensions;
using ProductosConfigLogging.Dtos;
using ProductosConfigLogging.Errors;
using ProductosConfigLogging.Mappers;
using ProductosConfigLogging.Repositories;

namespace ProductosConfigLogging.Services;

public class ProductoService(IProductoRepository repository) : IProductoService
{
    // El servicio trabaja con modelos por dentro y devuelve DTOs al exterior:
    // el mapeo Modelo -> DTO vive SIEMPRE aquí, nunca en el controlador.
    public Result<IEnumerable<ProductoDto>, DomainError> GetAll() =>
        Result.Success<IEnumerable<ProductoDto>, DomainError>(repository.GetAll().Select(p => p.ToDto()));

    public Result<ProductoDto, DomainError> GetById(long id)
    {
        var producto = repository.GetById(id);
        return producto is not null
            ? Result.Success<ProductoDto, DomainError>(producto.ToDto())
            : Result.Failure<ProductoDto, DomainError>(ProductoError.NotFound(id));
    }

    public Result<ProductoDto, DomainError> Create(CreateProductoDto dto)
    {
        var producto = dto.ToModel();
        var creado = repository.Add(producto);
        return Result.Success<ProductoDto, DomainError>(creado.ToDto());
    }

    public Result<ProductoDto, DomainError> Update(long id, UpdateProductoDto dto)
    {
        var producto = dto.ToModel();
        var actualizado = repository.Update(id, producto);
        return actualizado is not null
            ? Result.Success<ProductoDto, DomainError>(actualizado.ToDto())
            : Result.Failure<ProductoDto, DomainError>(ProductoError.NotFound(id));
    }

    public Result<ProductoDto, DomainError> PatchPrice(long id, decimal precio)
    {
        if (precio < 0)
            return Result.Failure<ProductoDto, DomainError>(ProductoError.PrecioInvalido(precio));

        var actualizado = repository.PatchPrice(id, precio);
        return actualizado is not null
            ? Result.Success<ProductoDto, DomainError>(actualizado.ToDto())
            : Result.Failure<ProductoDto, DomainError>(ProductoError.NotFound(id));
    }

    public UnitResult<DomainError> Delete(long id)
    {
        var eliminado = repository.Delete(id);
        return eliminado
            ? UnitResult.Success<DomainError>()
            : UnitResult.Failure<DomainError>(ProductoError.NotFound(id));
    }

    public Result<IEnumerable<ProductoDto>, DomainError> Search(string termino) =>
        Result.Success<IEnumerable<ProductoDto>, DomainError>(repository.Search(termino).Select(p => p.ToDto()));

    public Result<IEnumerable<ProductoDto>, DomainError> Filter(string? nombre, string? categoria, decimal? precioMin, decimal? precioMax) =>
        Result.Success<IEnumerable<ProductoDto>, DomainError>(repository.Filter(nombre, categoria, precioMin, precioMax).Select(p => p.ToDto()));

    public Result<(IEnumerable<ProductoDto> Items, int TotalPages, int TotalItems), DomainError> GetPaged(int page, int pageSize)
    {
        // El recorte y el conteo son trabajo de datos: aquí solo se envuelve en Result.
        var (items, totalPages, totalItems) = repository.GetPaged(page, pageSize);
        return Result.Success<(IEnumerable<ProductoDto>, int, int), DomainError>((items.Select(p => p.ToDto()), totalPages, totalItems));
    }
}
