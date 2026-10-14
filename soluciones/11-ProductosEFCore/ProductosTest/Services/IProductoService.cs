using CSharpFunctionalExtensions;
using ProductosEFCore.Dtos;
using ProductosEFCore.Errors;

namespace ProductosEFCore.Services;

public interface IProductoService
{
    IEnumerable<ProductoDto> GetAll();
    IEnumerable<ProductoDto> GetDeleted();
    Result<ProductoDto, DomainError> GetById(long id);
    Result<ProductoDto, DomainError> Create(CreateProductoDto dto);
    Result<ProductoDto, DomainError> Update(long id, UpdateProductoDto dto);
    UnitResult<DomainError> Delete(long id);
    UnitResult<DomainError> Restore(long id);
    IEnumerable<ProductoDto> Search(string termino);
    IEnumerable<ProductoDto> Filter(string? nombre, long? categoriaId, decimal? precioMin, decimal? precioMax);
    (IEnumerable<ProductoDto> Items, int TotalPages, int TotalItems) GetPaged(int page, int pageSize);
    IEnumerable<CategoriaDto> GetAllCategorias();
    CategoriaDto? GetCategoriaById(long id);
}
