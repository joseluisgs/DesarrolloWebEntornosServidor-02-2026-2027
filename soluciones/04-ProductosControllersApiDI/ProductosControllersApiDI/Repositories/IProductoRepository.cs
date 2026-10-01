using ProductosControllersApiDI.Models;

namespace ProductosControllersApiDI.Repositories;

/// <summary>
/// Interfaz del repositorio de productos.
/// Define las operaciones de acceso a datos.
/// </summary>
public interface IProductoRepository
{
    IEnumerable<Producto> GetAll();
    Producto? GetById(long id);
    Producto Add(Producto producto);
    Producto? Update(long id, Producto producto);
    Producto? PatchPrice(long id, decimal precio);
    bool Delete(long id);
    IEnumerable<Producto> Search(string termino);
    IEnumerable<Producto> FilterByCategoria(string categoria);
    IEnumerable<Producto> FilterByPrecio(decimal min, decimal max);
    IEnumerable<Producto> OrderByPrecio(bool descendente = false);
    IEnumerable<IGrouping<string, Producto>> GroupByCategoria();
    object GetEstadisticas();
}
