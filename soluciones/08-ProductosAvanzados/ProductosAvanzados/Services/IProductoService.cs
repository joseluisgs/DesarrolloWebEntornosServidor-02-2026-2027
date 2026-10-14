using CSharpFunctionalExtensions;
using ProductosAvanzados.Dtos;
using ProductosAvanzados.Errors;

namespace ProductosAvanzados.Services;

public interface IProductoService
{
    Result<IEnumerable<ProductoDto>, DomainError> GetAll();
    Result<ProductoDto, DomainError> GetById(long id);
    Result<ProductoDto, DomainError> Create(CreateProductoDto dto);
    Result<ProductoDto, DomainError> Update(long id, UpdateProductoDto dto);
    Result<ProductoDto, DomainError> PatchPrice(long id, decimal precio);
    UnitResult<DomainError> Delete(long id);
    Result<IEnumerable<ProductoDto>, DomainError> Search(string termino);
    Result<IEnumerable<ProductoDto>, DomainError> Filter(string? nombre, string? categoria, decimal? precioMin, decimal? precioMax);
    Result<(IEnumerable<ProductoDto> Items, int TotalPages, int TotalItems), DomainError> GetPaged(int page, int pageSize);
}
