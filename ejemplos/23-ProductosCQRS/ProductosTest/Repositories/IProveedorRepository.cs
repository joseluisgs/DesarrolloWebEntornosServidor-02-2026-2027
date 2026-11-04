using ProductosCQRS.Entity.Mongo;
using ProductosCQRS.Models;

namespace ProductosCQRS.Repositories;

public interface IProveedorRepository
{
    IEnumerable<Proveedor> GetAll();
    Proveedor? GetById(long id);
    Proveedor? GetByNombre(string nombre);
}
