using CSharpFunctionalExtensions;
using Microsoft.Extensions.Options;
using ProductosCQRS.Cache;
using ProductosCQRS.Dtos;
using ProductosCQRS.Errors;
using ProductosCQRS.Mappers;
using ProductosCQRS.Models;
using ProductosCQRS.Repositories;
using Serilog;

namespace ProductosCQRS.Services;

public class ProductoService(
    IProductoRepository repository,
    IProductoReadRepository readRepository,
    ICategoriaRepository categoriaRepository,
    ICacheService cache,
    IOptions<CacheOptions> cacheOptions) : IProductoService
{
    private readonly TimeSpan _cacheExpiration = cacheOptions.Value.DefaultExpiration;

    // ── Claves de caché consistentes ──────────────────────────────────────────
    private const string ReadAllKey = "productos:read:all";
    private const string ReadByIdPrefix = "producto:read:";
    private const string AllCategoriasKey = "categorias:all";

    private static string ReadByIdKey(long id) => $"producto:read:{id}";

    // ════════════════════════════════════════════════════════════════════════════
    // COMMANDS (escritura → PostgreSQL)
    // ════════════════════════════════════════════════════════════════════════════

    public Result<Producto, DomainError> Create(CreateProductoDto dto)
    {
        if (repository.IsDuplicated(dto.Nombre))
            return Result.Failure<Producto, DomainError>(ProductoError.NombreDuplicado(dto.Nombre));

        var producto = dto.ToModel();
        var creado = repository.Add(producto);

        // Invalidar caché de lectura
        cache.RemoveAsync(ReadAllKey).GetAwaiter().GetResult();
        Log.Information("Cache: Invalidada lista productos (read) tras Create");

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

        if (actualizado is not null)
        {
            // Invalidar caché de lectura
            cache.RemoveAsync(ReadByIdKey(id)).GetAwaiter().GetResult();
            cache.RemoveAsync(ReadAllKey).GetAwaiter().GetResult();
            Log.Information("Cache: Invalidada caché read del producto {Id} y lista tras Update", id);
        }

        return actualizado is not null
            ? Result.Success<Producto, DomainError>(actualizado)
            : Result.Failure<Producto, DomainError>(ProductoError.NotFound(id));
    }

    public UnitResult<DomainError> Delete(long id)
    {
        var eliminado = repository.Delete(id);

        if (eliminado)
        {
            // Invalidar caché de lectura
            cache.RemoveAsync(ReadByIdKey(id)).GetAwaiter().GetResult();
            cache.RemoveAsync(ReadAllKey).GetAwaiter().GetResult();
            Log.Information("Cache: Invalidada caché read del producto {Id} y lista tras Delete", id);
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
            cache.RemoveAsync(ReadByIdKey(id)).GetAwaiter().GetResult();
            cache.RemoveAsync(ReadAllKey).GetAwaiter().GetResult();
            Log.Information("Cache: Invalidada caché read del producto {Id} y lista tras Restore", id);
        }

        return restaurado
            ? UnitResult.Success<DomainError>()
            : UnitResult.Failure<DomainError>(ProductoError.NotFound(id));
    }

    // ════════════════════════════════════════════════════════════════════════════
    // QUERIES (lectura → MongoDB + caché)
    // ════════════════════════════════════════════════════════════════════════════

    public async Task<Result<List<ProductoReadDto>, DomainError>> GetAllReadAsync()
    {
        // 1. Buscar en caché
        var cached = await cache.GetAsync<List<ProductoReadDto>>(ReadAllKey);
        if (cached is not null)
        {
            Log.Debug("Cache HIT: GetAll productos (read)");
            return Result.Success<List<ProductoReadDto>, DomainError>(cached);
        }

        // 2. Caché miss → consultar MongoDB
        Log.Debug("Cache MISS: GetAll productos (read) → MongoDB");
        var productos = await readRepository.GetAllAsync();

        var dtos = productos.Select(p => new ProductoReadDto
        {
            Id = p.Id,
            Nombre = p.Nombre,
            Precio = p.Precio,
            CategoriaNombre = p.Categoria?.Nombre ?? "",
            ProveedorNombre = p.Proveedor?.Nombre ?? "",
            SyncAt = p.SyncAt
        }).ToList();

        // 3. Guardar en caché
        await cache.SetAsync(ReadAllKey, dtos, _cacheExpiration);

        return Result.Success<List<ProductoReadDto>, DomainError>(dtos);
    }

    public async Task<Result<ProductoReadDto, DomainError>> GetByIdReadAsync(long id)
    {
        // 1. Buscar en caché
        var cacheKey = ReadByIdKey(id);
        var cached = await cache.GetAsync<ProductoReadDto>(cacheKey);
        if (cached is not null)
        {
            Log.Debug("Cache HIT: GetById producto {Id} (read)", id);
            return Result.Success<ProductoReadDto, DomainError>(cached);
        }

        // 2. Caché miss → consultar MongoDB
        Log.Debug("Cache MISS: GetById producto {Id} (read) → MongoDB", id);
        var producto = await readRepository.GetByIdAsync(id);

        if (producto is null)
            return Result.Failure<ProductoReadDto, DomainError>(ProductoError.NotFound(id));

        var dto = new ProductoReadDto
        {
            Id = producto.Id,
            Nombre = producto.Nombre,
            Precio = producto.Precio,
            CategoriaNombre = producto.Categoria?.Nombre ?? "",
            ProveedorNombre = producto.Proveedor?.Nombre ?? "",
            SyncAt = producto.SyncAt
        };

        // 3. Guardar en caché
        await cache.SetAsync(cacheKey, dto, _cacheExpiration);

        return Result.Success<ProductoReadDto, DomainError>(dto);
    }

    // ════════════════════════════════════════════════════════════════════════════
    // UTILIDADES (escritura directa)
    // ════════════════════════════════════════════════════════════════════════════

    public IEnumerable<Producto> GetAll() => repository.GetAll();

    public IEnumerable<Producto> GetDeleted() => repository.GetDeleted();

    public Result<Producto, DomainError> GetById(long id)
    {
        var producto = repository.GetById(id);
        return producto is not null
            ? Result.Success<Producto, DomainError>(producto)
            : Result.Failure<Producto, DomainError>(ProductoError.NotFound(id));
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
