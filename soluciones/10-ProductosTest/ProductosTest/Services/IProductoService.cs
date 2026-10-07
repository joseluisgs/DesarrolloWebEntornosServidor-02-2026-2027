using CSharpFunctionalExtensions;
using ProductosTest.Dtos;
using ProductosTest.Errors;
using ProductosTest.Models;

namespace ProductosTest.Services;

public interface IProductoService
{
    Result<IEnumerable<Producto>, DomainError> GetAll();
    Result<Producto, DomainError> GetById(long id);
    Result<Producto, DomainError> Create(CreateProductoDto dto);
    Result<Producto, DomainError> Update(long id, UpdateProductoDto dto);
    Result<Producto, DomainError> PatchPrice(long id, decimal precio);
    UnitResult<DomainError> Delete(long id);
    Result<IEnumerable<Producto>, DomainError> Search(string termino);
    Result<IEnumerable<Producto>, DomainError> Filter(string? nombre, string? categoria, decimal? precioMin, decimal? precioMax);
    Result<(IEnumerable<Producto> Items, int TotalPages, int TotalItems), DomainError> GetPaged(int page, int pageSize);
}
