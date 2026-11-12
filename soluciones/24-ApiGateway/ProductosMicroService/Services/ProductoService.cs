using CSharpFunctionalExtensions;
using Microsoft.Extensions.Options;
using ProductosMicroService.Cache;
using ProductosMicroService.Dtos;
using ProductosMicroService.Errors;
using ProductosMicroService.Models;
using ProductosMicroService.Repositories;
using Serilog;

namespace ProductosMicroService.Services;

/// <summary>
/// Implementacion del servicio de Productos con cache Redis y validacion de Proveedores remotos.
/// </summary>
public class ProductoService(
    IProductoRepository repository,
    ICategoriaRepository categoriaRepository,
    IProveedorRemoteRepository proveedorRemote,
    ICacheService cache,
    IOptions<CacheOptions> cacheOptions
) : IProductoService
{
    private readonly TimeSpan _cacheExpiration = cacheOptions.Value.DefaultExpiration;

    // ── Claves de cache consistentes ──────────────────────────────────────
    private const string ReadAllKey = "productos:read:all";
    private static string ReadByIdKey(long id) => $"producto:read:{id}";

    // ════════════════════════════════════════════════════════════════════════════
    // QUERIES (lectura → cache + PostgreSQL)
    // ════════════════════════════════════════════════════════════════════════════

    public async Task<Result<List<ProductoReadDto>, DomainError>> GetAllReadAsync()
    {
        var cached = await cache.GetAsync<List<ProductoReadDto>>(ReadAllKey);
        if (cached is not null)
        {
            Log.Debug("Cache HIT: GetAll productos");
            return Result.Success<List<ProductoReadDto>, DomainError>(cached);
        }

        Log.Debug("Cache MISS: GetAll productos → PostgreSQL");
        var productos = repository.GetAll().ToList();

        var dtos = new List<ProductoReadDto>();
        foreach (var p in productos)
        {
            // Obtener nombre del proveedor (via Refit + cache)
            var proveedorNombre = await GetProveedorNombreAsync(p.ProveedorId);

            dtos.Add(new ProductoReadDto(
                p.Id, p.Nombre, p.Precio, p.Imagen,
                p.Categoria?.Nombre ?? "", proveedorNombre));
        }

        await cache.SetAsync(ReadAllKey, dtos, _cacheExpiration);
        return Result.Success<List<ProductoReadDto>, DomainError>(dtos);
    }

    public async Task<Result<ProductoReadDto, DomainError>> GetByIdReadAsync(long id)
    {
        var cacheKey = ReadByIdKey(id);
        var cached = await cache.GetAsync<ProductoReadDto>(cacheKey);
        if (cached is not null)
        {
            Log.Debug("Cache HIT: GetById producto {Id}", id);
            return Result.Success<ProductoReadDto, DomainError>(cached);
        }

        Log.Debug("Cache MISS: GetById producto {Id} → PostgreSQL", id);
        var producto = repository.GetById(id);
        if (producto is null)
            return Result.Failure<ProductoReadDto, DomainError>(
                NotFoundError.FromId(id.ToString(), "Producto"));

        var proveedorNombre = await GetProveedorNombreAsync(producto.ProveedorId);

        var dto = new ProductoReadDto(
            producto.Id, producto.Nombre, producto.Precio, producto.Imagen,
            producto.Categoria?.Nombre ?? "", proveedorNombre);

        await cache.SetAsync(cacheKey, dto, _cacheExpiration);
        return Result.Success<ProductoReadDto, DomainError>(dto);
    }

    // ════════════════════════════════════════════════════════════════════════════
    // COMMANDS (escritura → PostgreSQL + invalidacion cache)
    // ════════════════════════════════════════════════════════════════════════════

    public Result<Producto, DomainError> Create(CreateProductoDto dto)
    {
        // Validar categoria local
        var categoria = categoriaRepository.GetById(dto.CategoriaId);
        if (categoria is null)
            return Result.Failure<Producto, DomainError>(
                NotFoundError.FromId(dto.CategoriaId.ToString(), "Categoria"));

        // Validar nombre duplicado
        if (repository.IsDuplicated(dto.Nombre))
            return Result.Failure<Producto, DomainError>(
                new BusinessRuleError($"Ya existe un producto con el nombre '{dto.Nombre}'"));

        // Validar proveedor remoto
        var proveedor = proveedorRemote.GetByIdAsync(dto.ProveedorId).GetAwaiter().GetResult();
        if (proveedor is null)
            return Result.Failure<Producto, DomainError>(
                NotFoundError.FromId(dto.ProveedorId, "Proveedor"));

        var producto = new Producto
        {
            Nombre = dto.Nombre,
            Precio = dto.Precio,
            CategoriaId = dto.CategoriaId,
            ProveedorId = dto.ProveedorId,
            Imagen = dto.Imagen,
            CreatedAt = DateTime.UtcNow
        };

        repository.Add(producto);

        // Invalidar cache
        cache.RemoveAsync(ReadAllKey).GetAwaiter().GetResult();
        Log.Information("Cache: Invalidada lista productos tras Create");

        return Result.Success<Producto, DomainError>(producto);
    }

    public Result<Producto, DomainError> Update(long id, UpdateProductoDto dto)
    {
        var existente = repository.GetById(id);
        if (existente is null)
            return Result.Failure<Producto, DomainError>(
                NotFoundError.FromId(id.ToString(), "Producto"));

        // Validar categoria local
        var categoria = categoriaRepository.GetById(dto.CategoriaId);
        if (categoria is null)
            return Result.Failure<Producto, DomainError>(
                NotFoundError.FromId(dto.CategoriaId.ToString(), "Categoria"));

        // Validar nombre duplicado
        if (repository.IsDuplicated(dto.Nombre, id))
            return Result.Failure<Producto, DomainError>(
                new BusinessRuleError($"Ya existe un producto con el nombre '{dto.Nombre}'"));

        // Validar proveedor remoto
        var proveedor = proveedorRemote.GetByIdAsync(dto.ProveedorId).GetAwaiter().GetResult();
        if (proveedor is null)
            return Result.Failure<Producto, DomainError>(
                NotFoundError.FromId(dto.ProveedorId, "Proveedor"));

        var producto = new Producto
        {
            Id = id,
            Nombre = dto.Nombre,
            Precio = dto.Precio,
            CategoriaId = dto.CategoriaId,
            ProveedorId = dto.ProveedorId,
            Imagen = dto.Imagen
        };

        var actualizado = repository.Update(producto);
        if (actualizado is null)
            return Result.Failure<Producto, DomainError>(
                NotFoundError.FromId(id.ToString(), "Producto"));

        // Invalidar cache
        cache.RemoveAsync(ReadByIdKey(id)).GetAwaiter().GetResult();
        cache.RemoveAsync(ReadAllKey).GetAwaiter().GetResult();
        Log.Information("Cache: Invalidada cache del producto {Id} y lista tras Update", id);

        return Result.Success<Producto, DomainError>(actualizado);
    }

    public UnitResult<DomainError> Delete(long id)
    {
        var eliminado = repository.Delete(id);
        if (!eliminado)
            return UnitResult.Failure<DomainError>(
                NotFoundError.FromId(id.ToString(), "Producto"));

        // Invalidar cache
        cache.RemoveAsync(ReadByIdKey(id)).GetAwaiter().GetResult();
        cache.RemoveAsync(ReadAllKey).GetAwaiter().GetResult();
        Log.Information("Cache: Invalidada cache del producto {Id} y lista tras Delete", id);

        return UnitResult.Success<DomainError>();
    }

    // ════════════════════════════════════════════════════════════════════════════
    // UTILIDADES PRIVADAS
    // ════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Obtiene el nombre del proveedor via Refit, usando cache.
    /// </summary>
    private async Task<string> GetProveedorNombreAsync(string proveedorId)
    {
        if (string.IsNullOrWhiteSpace(proveedorId))
            return "Desconocido";

        var cacheKey = $"proveedor:{proveedorId}";
        var cached = await cache.GetAsync<ProveedorReadDto>(cacheKey);
        if (cached is not null)
            return cached.Nombre;

        var proveedor = await proveedorRemote.GetByIdAsync(proveedorId);
        if (proveedor is not null)
        {
            await cache.SetAsync(cacheKey, proveedor, _cacheExpiration);
            return proveedor.Nombre;
        }

        return "Desconocido";
    }
}
