using CSharpFunctionalExtensions;
using ProductosEFCore.Dtos;
using ProductosEFCore.Errors;
using ProductosEFCore.Models;

namespace ProductosEFCore.Services;

public interface IProveedorService
{
    IEnumerable<Proveedor> GetAll();
    Result<Proveedor, DomainError> GetById(string id);
    Result<Proveedor, DomainError> Create(CreateProveedorDto dto);
    Result<Proveedor, DomainError> Update(string id, UpdateProveedorDto dto);
    UnitResult<DomainError> Delete(string id);
}
