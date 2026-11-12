using CSharpFunctionalExtensions;
using ProductosMicroService.Dtos;
using ProductosMicroService.Errors;
using ProductosMicroService.Models;

namespace ProductosMicroService.Services;

/// <summary>
/// Servicio de Productos: CRUD con cache y validacion de Proveedores remotos.
/// </summary>
public interface IProductoService
{
    Task<Result<List<ProductoReadDto>, DomainError>> GetAllReadAsync();
    Task<Result<ProductoReadDto, DomainError>> GetByIdReadAsync(long id);
    Result<Producto, DomainError> Create(CreateProductoDto dto);
    Result<Producto, DomainError> Update(long id, UpdateProductoDto dto);
    UnitResult<DomainError> Delete(long id);
}
