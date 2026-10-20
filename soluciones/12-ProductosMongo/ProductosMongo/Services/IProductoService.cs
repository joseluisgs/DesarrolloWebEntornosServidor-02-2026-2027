using CSharpFunctionalExtensions;
using ProductosMongo.Errors;
using ProductosMongo.Models;

namespace ProductosMongo.Services;

/// <summary>
/// Interfaz del servicio de productos.
/// Patrón Result con CSharpFunctionalExtensions.
/// </summary>
public interface IProductoService
{
    IEnumerable<Producto> GetAll();
    Result<Producto, DomainError> GetById(string id);
    Result<Producto, DomainError> Create(Producto producto);
    Result<Producto, DomainError> Update(Producto producto);
    UnitResult<DomainError> Delete(string id);
    IEnumerable<Producto> Search(string query);
    IEnumerable<Producto> Filter(string? nombre, decimal? precioMin, decimal? precioMax);
    IEnumerable<Producto> GetDeleted();
}
