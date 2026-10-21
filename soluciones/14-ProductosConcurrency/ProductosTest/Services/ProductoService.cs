using CSharpFunctionalExtensions;
using Microsoft.Extensions.Options;
using ProductosEFCore.Cache;
using ProductosEFCore.Dtos;
using ProductosEFCore.Errors;
using ProductosEFCore.Mappers;
using ProductosEFCore.Models;
using ProductosEFCore.Repositories;
using Serilog;

namespace ProductosEFCore.Services;

public class ProductoService(
    IProductoRepository repository,
    ICategoriaRepository categoriaRepository,
    ICacheService cache,
    IOptions<CacheOptions> cacheOptions) : IProductoService
{
    private readonly TimeSpan _cacheExpiration = cacheOptions.Value.DefaultExpiration;

    private const string AllProductosKey = "productos:all";
    private const string AllCategoriasKey = "categorias:all";

    private static string ProductoKey(long id) => $"producto:{id}";

    public IEnumerable<ProductoDto> GetAll()
    {
        Log.Debug("Cache: GetAll productos");
        IEnumerable<Producto> productos = cache.GetOrSetAsync(
            AllProductosKey,
            () => Task.FromResult(repository.GetAll()),
            _cacheExpiration
        ).GetAwaiter().GetResult() ?? [];

        // El servicio devuelve DTOs: el mapeo Modelo -> DTO vive SIEMPRE aqui.
        return productos.Select(p => p.ToDto());
    }

    public IEnumerable<ProductoDto> GetDeleted()
    {
        Log.Debug("Cache: GetDeleted productos");
        IEnumerable<Producto> productos = cache.GetOrSetAsync(
            "productos:deleted",
            () => Task.FromResult(repository.GetDeleted()),
            _cacheExpiration
        ).GetAwaiter().GetResult() ?? [];

        return productos.Select(p => p.ToDto());
    }

    public Result<ProductoDto, DomainError> GetById(long id)
    {
        Log.Debug("Cache: GetById producto {Id}", id);
        var producto = cache.GetOrSetAsync(
            ProductoKey(id),
            () => Task.FromResult(repository.GetById(id)),
            _cacheExpiration
        ).GetAwaiter().GetResult();

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

        cache.RemoveAsync(AllProductosKey).GetAwaiter().GetResult();
        Log.Information("Cache: Invalidada lista productos tras Create");

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

        if (actualizado is not null)
        {
            cache.RemoveAsync(ProductoKey(id)).GetAwaiter().GetResult();
            cache.RemoveAsync(AllProductosKey).GetAwaiter().GetResult();
            Log.Information("Cache: Invalidada caché del producto {Id} y lista tras Update", id);
        }

        return actualizado is not null
            ? Result.Success<ProductoDto, DomainError>(actualizado.ToDto())
            : Result.Failure<ProductoDto, DomainError>(ProductoError.NotFound(id));
    }

    public UnitResult<DomainError> Delete(long id)
    {
        var eliminado = repository.Delete(id);

        if (eliminado)
        {
            cache.RemoveAsync(ProductoKey(id)).GetAwaiter().GetResult();
            cache.RemoveAsync(AllProductosKey).GetAwaiter().GetResult();
            Log.Information("Cache: Invalidada caché del producto {Id} y lista tras Delete", id);
        }

        return eliminado
            ? UnitResult.Success<DomainError>()
            : UnitResult.Failure<DomainError>(ProductoError.NotFound(id));
    }

    public UnitResult<DomainError> Restore(long id)
    {
        var restaurado = repository.Restore(id);

        if (restaurado)
        {
            cache.RemoveAsync(ProductoKey(id)).GetAwaiter().GetResult();
            cache.RemoveAsync(AllProductosKey).GetAwaiter().GetResult();
            Log.Information("Cache: Invalidada caché del producto {Id} y lista tras Restore", id);
        }

        return restaurado
            ? UnitResult.Success<DomainError>()
            : UnitResult.Failure<DomainError>(ProductoError.NotFound(id));
    }

    public IEnumerable<ProductoDto> Search(string termino) =>
        repository.GetAll()
            .Where(p =>
                p.Nombre.Contains(termino, StringComparison.OrdinalIgnoreCase) ||
                p.Categoria.Nombre.Contains(termino, StringComparison.OrdinalIgnoreCase))
            .Select(p => p.ToDto());

    public IEnumerable<ProductoDto> Filter(string? nombre, Guid? categoriaId, decimal? precioMin, decimal? precioMax)
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

    public IEnumerable<CategoriaDto> GetAllCategorias()
    {
        Log.Debug("Cache: GetAll categorías");
        IEnumerable<Categoria> categorias = cache.GetOrSetAsync(
            AllCategoriasKey,
            () => Task.FromResult(categoriaRepository.GetAll()),
            _cacheExpiration
        ).GetAwaiter().GetResult() ?? [];

        return categorias.Select(c => c.ToDto());
    }

    public CategoriaDto? GetCategoriaById(Guid id)
    {
        Log.Debug("Cache: GetCategoriaById {Id}", id);
        var categoria = cache.GetOrSetAsync(
            $"categoria:{id}",
            () => Task.FromResult(categoriaRepository.GetById(id)),
            _cacheExpiration
        ).GetAwaiter().GetResult();

        return categoria?.ToDto();
    }
}
