using CSharpFunctionalExtensions;
using ProductosEFCore.Dtos;
using ProductosEFCore.Errors;
using ProductosEFCore.Models;

namespace ProductosEFCore.Services;

public interface IProductoService
{
    IEnumerable<Producto> GetAll();
    IEnumerable<Producto> GetDeleted();
    Result<Producto, DomainError> GetById(long id);
    Result<Producto, DomainError> Create(CreateProductoDto dto);
    Result<Producto, DomainError> Update(long id, UpdateProductoDto dto);
    UnitResult<DomainError> Delete(long id);
    UnitResult<DomainError> Restore(long id);
    IEnumerable<Producto> Search(string termino);
    IEnumerable<Producto> Filter(string? nombre, long? categoriaId, decimal? precioMin, decimal? precioMax);
    (IEnumerable<Producto> Items, int TotalPages, int TotalItems) GetPaged(int page, int pageSize);
    IEnumerable<Categoria> GetAllCategorias();
    Categoria? GetCategoriaById(long id);
}
