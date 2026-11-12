using ProductosMicroService.Models;

namespace ProductosMicroService.Repositories;

/// <summary>
/// Interfaz del repositorio de Productos (PostgreSQL).
/// </summary>
public interface IProductoRepository
{
    IEnumerable<Producto> GetAll();
    Producto? GetById(long id);
    Producto Add(Producto producto);
    Producto? Update(Producto producto);
    bool Delete(long id);
    bool IsDuplicated(string nombre, long excludeId = 0);
    Task LoadRelationsAsync(Producto producto);
}
