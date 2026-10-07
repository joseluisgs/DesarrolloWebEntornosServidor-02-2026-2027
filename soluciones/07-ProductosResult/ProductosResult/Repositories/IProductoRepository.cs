using ProductosResult.Models;

namespace ProductosResult.Repositories;

public interface IProductoRepository
{
    IEnumerable<Producto> GetAll();
    Producto? GetById(long id);
    Producto Add(Producto producto);
    Producto? Update(long id, Producto producto);
    Producto? PatchPrice(long id, decimal precio);
    bool Delete(long id);
    IEnumerable<Producto> Search(string termino);
}
