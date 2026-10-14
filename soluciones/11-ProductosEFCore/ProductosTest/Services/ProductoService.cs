using CSharpFunctionalExtensions;
using ProductosEFCore.Dtos;
using ProductosEFCore.Errors;
using ProductosEFCore.Mappers;
using ProductosEFCore.Models;
using ProductosEFCore.Repositories;

namespace ProductosEFCore.Services;

public class ProductoService(IProductoRepository repository, ICategoriaRepository categoriaRepository) : IProductoService
{
    public IEnumerable<Producto> GetAll() => repository.GetAll();

    public IEnumerable<Producto> GetDeleted() => repository.GetDeleted();

    public Result<Producto, DomainError> GetById(long id)
    {
        var producto = repository.GetById(id);
        return producto is not null
            ? Result.Success<Producto, DomainError>(producto)
            : Result.Failure<Producto, DomainError>(ProductoError.NotFound(id));
    }

    public Result<Producto, DomainError> Create(CreateProductoDto dto)
    {
        if (repository.IsDuplicated(dto.Nombre))
            return Result.Failure<Producto, DomainError>(ProductoError.NombreDuplicado(dto.Nombre));

        var producto = dto.ToModel();
        var creado = repository.Add(producto);
        return Result.Success<Producto, DomainError>(creado);
    }

    public Result<Producto, DomainError> Update(long id, UpdateProductoDto dto)
    {
        var existente = repository.GetById(id);
        if (existente is null)
            return Result.Failure<Producto, DomainError>(ProductoError.NotFound(id));

        if (repository.IsDuplicated(dto.Nombre, id))
            return Result.Failure<Producto, DomainError>(ProductoError.NombreDuplicado(dto.Nombre));

        var producto = dto.ToModel();
        producto.Id = id;
        var actualizado = repository.Update(producto);
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

    public UnitResult<DomainError> Restore(long id)
    {
        var restaurado = repository.Restore(id);
        return restaurado
            ? UnitResult.Success<DomainError>()
            : UnitResult.Failure<DomainError>(ProductoError.NotFound(id));
    }

    public IEnumerable<Producto> Search(string termino) =>
        repository.GetAll().Where(p =>
            p.Nombre.Contains(termino, StringComparison.OrdinalIgnoreCase) ||
            p.Categoria.Nombre.Contains(termino, StringComparison.OrdinalIgnoreCase));

    public IEnumerable<Producto> Filter(string? nombre, long? categoriaId, decimal? precioMin, decimal? precioMax)
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

        return productos;
    }

    public (IEnumerable<Producto> Items, int TotalPages, int TotalItems) GetPaged(int page, int pageSize)
    {
        // Techo de paginacion en la CAPA DE DATOS: aunque el filtro llegue
        // construido en codigo (GraphQL, jobs, tests) el Skip/Take no sale de 1..100.
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var all = repository.GetAll().ToList();
        var totalItems = all.Count;
        var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
        var items = all.Skip((page - 1) * pageSize).Take(pageSize);
        return (items, totalPages, totalItems);
    }

    public IEnumerable<Categoria> GetAllCategorias() => categoriaRepository.GetAll();

    public Categoria? GetCategoriaById(long id) => categoriaRepository.GetById(id);
}
