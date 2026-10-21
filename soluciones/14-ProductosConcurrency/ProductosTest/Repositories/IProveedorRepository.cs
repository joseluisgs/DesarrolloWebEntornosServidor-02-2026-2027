using ProductosEFCore.Models;

namespace ProductosEFCore.Repositories;

public interface IProveedorRepository
{
    IEnumerable<Proveedor> GetAll();
    Proveedor? GetById(string id);
    Proveedor Add(Proveedor proveedor);
    Proveedor? Update(Proveedor proveedor);
    bool Delete(string id);
    bool IsDuplicated(string nombre, string excludeId = "");
}
