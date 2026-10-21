using CSharpFunctionalExtensions;
using Microsoft.Extensions.Options;
using ProductosEFCore.Cache;
using ProductosEFCore.Dtos;
using ProductosEFCore.Errors;
using ProductosEFCore.Models;
using ProductosEFCore.Repositories;
using Serilog;

namespace ProductosEFCore.Services;

public class ProveedorService(
    IProveedorRepository repository,
    ICacheService cache,
    IOptions<CacheOptions> cacheOptions) : IProveedorService
{
    private readonly TimeSpan _cacheExpiration = cacheOptions.Value.DefaultExpiration;
    private const string AllProveedoresKey = "proveedores:all";

    private static string ProveedorKey(string id) => $"proveedor:{id}";

    public IEnumerable<Proveedor> GetAll()
    {
        Log.Debug("Cache: GetAll proveedores");
        return cache.GetOrSetAsync(
            AllProveedoresKey,
            () => Task.FromResult(repository.GetAll()),
            _cacheExpiration
        ).GetAwaiter().GetResult() ?? [];
    }

    public Result<Proveedor, DomainError> GetById(string id)
    {
        Log.Debug("Cache: GetById proveedor {Id}", id);
        var proveedor = cache.GetOrSetAsync(
            ProveedorKey(id),
            () => Task.FromResult(repository.GetById(id)),
            _cacheExpiration
        ).GetAwaiter().GetResult();

        return proveedor is not null
            ? Result.Success<Proveedor, DomainError>(proveedor)
            : Result.Failure<Proveedor, DomainError>(ProveedorError.NotFound(id));
    }

    public Result<Proveedor, DomainError> Create(CreateProveedorDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nombre))
            return Result.Failure<Proveedor, DomainError>(ProveedorError.NombreVacio());

        if (repository.IsDuplicated(dto.Nombre))
            return Result.Failure<Proveedor, DomainError>(ProveedorError.CodigoDuplicado(dto.Nombre));

        var proveedor = new Proveedor
        {
            Nombre = dto.Nombre,
            Contacto = dto.Contacto
        };

        var creado = repository.Add(proveedor);

        cache.RemoveAsync(AllProveedoresKey).GetAwaiter().GetResult();
        Log.Information("Cache: Invalidada lista proveedores tras Create");

        return Result.Success<Proveedor, DomainError>(creado);
    }

    public Result<Proveedor, DomainError> Update(string id, UpdateProveedorDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nombre))
            return Result.Failure<Proveedor, DomainError>(ProveedorError.NombreVacio());

        var existente = repository.GetById(id);
        if (existente is null)
            return Result.Failure<Proveedor, DomainError>(ProveedorError.NotFound(id));

        if (repository.IsDuplicated(dto.Nombre, id))
            return Result.Failure<Proveedor, DomainError>(ProveedorError.CodigoDuplicado(dto.Nombre));

        var proveedor = new Proveedor
        {
            Id = id,
            Nombre = dto.Nombre,
            Contacto = dto.Contacto
        };

        var actualizado = repository.Update(proveedor);

        if (actualizado is not null)
        {
            cache.RemoveAsync(ProveedorKey(id)).GetAwaiter().GetResult();
            cache.RemoveAsync(AllProveedoresKey).GetAwaiter().GetResult();
            Log.Information("Cache: Invalidada caché del proveedor {Id} y lista tras Update", id);
        }

        return actualizado is not null
            ? Result.Success<Proveedor, DomainError>(actualizado)
            : Result.Failure<Proveedor, DomainError>(ProveedorError.NotFound(id));
    }

    public UnitResult<DomainError> Delete(string id)
    {
        var eliminado = repository.Delete(id);

        if (eliminado)
        {
            cache.RemoveAsync(ProveedorKey(id)).GetAwaiter().GetResult();
            cache.RemoveAsync(AllProveedoresKey).GetAwaiter().GetResult();
            Log.Information("Cache: Invalidada caché del proveedor {Id} y lista tras Delete", id);
        }

        return eliminado
            ? UnitResult.Success<DomainError>()
            : UnitResult.Failure<DomainError>(ProveedorError.NotFound(id));
    }
}
