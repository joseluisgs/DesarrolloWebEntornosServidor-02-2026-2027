using ProductosEFCore.Models;

namespace ProductosEFCore.Repositories;

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
}

public interface ICategoriaRepository
{
    IEnumerable<Categoria> GetAll();
    Categoria? GetById(long id);
    Categoria? GetByNombre(string nombre);
}
