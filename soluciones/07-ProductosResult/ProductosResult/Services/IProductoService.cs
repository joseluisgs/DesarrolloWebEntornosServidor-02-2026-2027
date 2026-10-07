using CSharpFunctionalExtensions;
using ProductosResult.Errors;
using ProductosResult.Models;

namespace ProductosResult.Services;

public interface IProductoService
{
    Result<IEnumerable<Producto>, DomainError> GetAll();
    Result<Producto, DomainError> GetById(long id);
    Result<Producto, DomainError> Create(Producto producto);
    Result<Producto, DomainError> Update(long id, Producto producto);
    Result<Producto, DomainError> PatchPrice(long id, decimal precio);
    UnitResult<DomainError> Delete(long id);
    Result<IEnumerable<Producto>, DomainError> Search(string? termino);
}
