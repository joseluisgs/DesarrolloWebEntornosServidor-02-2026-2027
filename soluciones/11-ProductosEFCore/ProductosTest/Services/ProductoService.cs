using CSharpFunctionalExtensions;
using ProductosEFCore.Dtos;
using ProductosEFCore.Errors;
using ProductosEFCore.Mappers;
using ProductosEFCore.Repositories;

namespace ProductosEFCore.Services;

public class ProductoService(IProductoRepository repository, ICategoriaRepository categoriaRepository) : IProductoService
{
    // El servicio trabaja con modelos por dentro y devuelve DTOs al exterior:
    // el mapeo Modelo -> DTO vive SIEMPRE aquí, nunca en el controlador.
    public IEnumerable<ProductoDto> GetAll() => repository.GetAll().Select(p => p.ToDto());

    public IEnumerable<ProductoDto> GetDeleted() => repository.GetDeleted().Select(p => p.ToDto());

    public Result<ProductoDto, DomainError> GetById(long id)
    {
        var producto = repository.GetById(id);
        return producto is not null
            ? Result.Success<ProductoDto, DomainError>(producto.ToDto())
            : Result.Failure<ProductoDto, DomainError>(ProductoError.NotFound(id));
    }

    public Result<ProductoDto, DomainError> Create(CreateProductoDto dto)
    {
        if (repository.IsDuplicated(dto.Nombre))
            return Result.Failure<ProductoDto, DomainError>(ProductoError.NombreDuplicado(dto.Nombre));

        var producto = dto.ToModel();
        var creado = repository.Add(producto);
        return Result.Success<ProductoDto, DomainError>(creado.ToDto());
    }

    public Result<ProductoDto, DomainError> Update(long id, UpdateProductoDto dto)
    {
        var existente = repository.GetById(id);
        if (existente is null)
            return Result.Failure<ProductoDto, DomainError>(ProductoError.NotFound(id));

        if (repository.IsDuplicated(dto.Nombre, id))
            return Result.Failure<ProductoDto, DomainError>(ProductoError.NombreDuplicado(dto.Nombre));

        var producto = dto.ToModel();
        producto.Id = id;
        var actualizado = repository.Update(producto);
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

    public UnitResult<DomainError> Restore(long id)
    {
        var restaurado = repository.Restore(id);
        return restaurado
            ? UnitResult.Success<DomainError>()
            : UnitResult.Failure<DomainError>(ProductoError.NotFound(id));
    }

    public IEnumerable<ProductoDto> Search(string termino) =>
        repository.GetAll().Where(p =>
            p.Nombre.Contains(termino, StringComparison.OrdinalIgnoreCase) ||
            p.Categoria.Nombre.Contains(termino, StringComparison.OrdinalIgnoreCase))
            .Select(p => p.ToDto());

    public IEnumerable<ProductoDto> Filter(string? nombre, long? categoriaId, decimal? precioMin, decimal? precioMax)
    {
        var productos = repository.GetAll().AsEnumerable();

        if (!string.IsNullOrEmpty(nombre))
            productos = productos.Where(p => p.Nombre.Contains(nombre, StringComparison.OrdinalIgnoreCase));
        if (categoriaId.HasValue)
            productos = productos.Where(p => p.CategoriaId == categoriaId.Value);
        if (precioMin.HasValue)
            productos = productos.Where(p => p.Precio >= precioMin.Value);
        if (precioMax.HasValue)
            productos = productos.Where(p => p.Precio <= precioMax.Value);

        return productos.Select(p => p.ToDto());
    }

    public (IEnumerable<ProductoDto> Items, int TotalPages, int TotalItems) GetPaged(int page, int pageSize)
    {
        // Techo de paginacion en la CAPA DE DATOS: aunque el filtro llegue
        // construido en codigo (GraphQL, jobs, tests) el Skip/Take no sale de 1..100.
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var all = repository.GetAll().ToList();
        var totalItems = all.Count;
        var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
        var items = all.Skip((page - 1) * pageSize).Take(pageSize);
        return (items.Select(p => p.ToDto()), totalPages, totalItems);
    }

    public IEnumerable<CategoriaDto> GetAllCategorias() => categoriaRepository.GetAll().Select(c => c.ToDto());

    public CategoriaDto? GetCategoriaById(long id) => categoriaRepository.GetById(id)?.ToDto();
}
