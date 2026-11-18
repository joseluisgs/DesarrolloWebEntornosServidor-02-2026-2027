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
            Log.Information("[Productos] caché Redis HIT: la lista sale de Redis sin tocar PostgreSQL");
            return Result.Success<List<ProductoReadDto>, DomainError>(cached);
        }

        Log.Information("[Productos] caché Redis MISS: leyendo la lista completa de PostgreSQL");
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
            Log.Information("[Productos] caché Redis HIT: producto {Id} servido desde Redis", id);
            return Result.Success<ProductoReadDto, DomainError>(cached);
        }

        Log.Information("[Productos] caché Redis MISS: producto {Id} leído de PostgreSQL", id);
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
        {
            Log.Information("[Productos] alta de '{Nombre}' RECHAZADA: la categoría {Categoria} no existe",
                dto.Nombre, dto.CategoriaId);
            return Result.Failure<Producto, DomainError>(
                NotFoundError.FromId(dto.CategoriaId.ToString(), "Categoria"));
        }

        // Validar nombre duplicado
        if (repository.IsDuplicated(dto.Nombre))
        {
            Log.Information("[Productos] alta de '{Nombre}' RECHAZADA: ya hay un producto con ese nombre",
                dto.Nombre);
            return Result.Failure<Producto, DomainError>(
                new BusinessRuleError($"Ya existe un producto con el nombre '{dto.Nombre}'"));
        }

        // Validar proveedor remoto
        var proveedor = proveedorRemote.GetByIdAsync(dto.ProveedorId).GetAwaiter().GetResult();
        if (proveedor is null)
        {
            Log.Information("[Productos] alta de '{Nombre}' RECHAZADA: el proveedor {Proveedor} no existe o no responde",
                dto.Nombre, dto.ProveedorId);
            return Result.Failure<Producto, DomainError>(
                NotFoundError.FromId(dto.ProveedorId, "Proveedor"));
        }

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
        Log.Information("[Productos] alta OK: '{Nombre}' ({Precio} €) en {Categoria} · proveedor {Proveedor}",
            producto.Nombre, producto.Precio, categoria.Nombre, proveedor.Nombre);

        // Invalidar cache
        cache.RemoveAsync(ReadAllKey).GetAwaiter().GetResult();
        Log.Information("[Productos] caché invalidada: la lista deje de servirse desde Redis tras el alta");

        return Result.Success<Producto, DomainError>(producto);
    }

    public Result<Producto, DomainError> Update(long id, UpdateProductoDto dto)
    {
        var existente = repository.GetById(id);
        if (existente is null)
        {
            Log.Information("[Productos] modificación de {Id} RECHAZADA: no existe", id);
            return Result.Failure<Producto, DomainError>(
                NotFoundError.FromId(id.ToString(), "Producto"));
        }

        // Validar categoria local
        var categoria = categoriaRepository.GetById(dto.CategoriaId);
        if (categoria is null)
        {
            Log.Information("[Productos] modificación de {Id} RECHAZADA: la categoría {Categoria} no existe",
                id, dto.CategoriaId);
            return Result.Failure<Producto, DomainError>(
                NotFoundError.FromId(dto.CategoriaId.ToString(), "Categoria"));
        }

        // Validar nombre duplicado
        if (repository.IsDuplicated(dto.Nombre, id))
        {
            Log.Information("[Productos] modificación de {Id} RECHAZADA: '{Nombre}' ya es de otro producto",
                id, dto.Nombre);
            return Result.Failure<Producto, DomainError>(
                new BusinessRuleError($"Ya existe un producto con el nombre '{dto.Nombre}'"));
        }

        // Validar proveedor remoto
        var proveedor = proveedorRemote.GetByIdAsync(dto.ProveedorId).GetAwaiter().GetResult();
        if (proveedor is null)
        {
            Log.Information("[Productos] modificación de {Id} RECHAZADA: el proveedor {Proveedor} no existe o no responde",
                id, dto.ProveedorId);
            return Result.Failure<Producto, DomainError>(
                NotFoundError.FromId(dto.ProveedorId, "Proveedor"));
        }

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
        Log.Information("[Productos] caché invalidada: producto {Id} y lista", id);
        Log.Information("[Productos] producto {Id} actualizado → '{Nombre}' ({Precio} €) · proveedor {Proveedor}",
            id, dto.Nombre, dto.Precio, proveedor.Nombre);

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
        Log.Information("[Productos] caché invalidada: producto {Id} y lista", id);
        Log.Information("[Productos] producto {Id} eliminado de PostgreSQL", id);

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
        {
            // Redis ya tiene al proveedor: no hace falta volver a llamar a Proveedores.
            // En Debug para que una lista de 6 productos no repita la misma línea 6 veces.
            Log.Debug("[Productos] proveedor {Id}: '{Nombre}' servido desde Redis, sin llamar a Proveedores",
                proveedorId, cached.Nombre);
            return cached.Nombre;
        }

        var proveedor = await proveedorRemote.GetByIdAsync(proveedorId);
        if (proveedor is not null)
        {
            await cache.SetAsync(cacheKey, proveedor, _cacheExpiration);
            Log.Information("[Productos] proveedor {Id}: '{Nombre}' guardado en Redis durante {Minutos} min",
                proveedorId, proveedor.Nombre, _cacheExpiration.TotalMinutes);
            return proveedor.Nombre;
        }

        Log.Information("[Productos] proveedor {Id}: sin dato, se mostrará 'Desconocido'", proveedorId);
        return "Desconocido";
    }
}
