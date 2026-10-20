using ProductosMongo.Models;

namespace ProductosMongo.Repositories;

/// <summary>
/// Interfaz del repositorio de productos.
/// Se puede implementar con Driver Nativo o EF Core.
/// </summary>
public interface IProductoRepository
{
    IEnumerable<Producto> GetAll();
    Producto? GetById(string id);
    Producto Add(Producto producto);
    Producto? Update(Producto producto);
    bool Delete(string id);
    bool IsDuplicated(string nombre, string excludeId = "");
    IEnumerable<Producto> Search(string query);
    IEnumerable<Producto> Filter(string? nombre, decimal? precioMin, decimal? precioMax);
    IEnumerable<Producto> GetDeleted();
}
