using ProductosCQRS.Entity.Mongo;
using ProductosCQRS.Models;

namespace ProductosCQRS.Repositories;

public interface IProductoRepository
{
    IEnumerable<Producto> GetAll();
    IEnumerable<Producto> GetDeleted();
    Producto? GetById(long id);
    Producto Add(Producto producto);
    Producto? Update(Producto producto);
    bool Delete(long id);
    bool Restore(long id);
    bool IsDuplicated(string nombre, long excludeId = 0);
    Task LoadRelationsAsync(Producto producto);
}
