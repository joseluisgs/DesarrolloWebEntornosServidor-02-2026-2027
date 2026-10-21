using CSharpFunctionalExtensions;
using ProductosEFCore.Errors;
using ProductosEFCore.Models;

namespace ProductosEFCore.Services;

/// <summary>
/// Servicio que demostra las tres políticas de concurrencia.
/// </summary>
public interface IConcurrencyService
{
    Task<Result<Producto, DomainError>> UpdateOptimisticAsync(long id, int newStock);
    Task<Result<Producto, DomainError>> UpdatePessimisticAsync(long id, int newStock);
    Task<Result<Producto, DomainError>> UpdateMixedAsync(long id, int cantidad);
}
