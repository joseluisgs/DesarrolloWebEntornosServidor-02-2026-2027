using ProductosScrutorApi.Models;

namespace ProductosScrutorApi.Repositories;

/// <summary>
/// Interfaz para el repositorio de productos.
/// </summary>
public interface IProductoRepository
{
    IEnumerable<Producto> GetAll();
    Producto? GetById(long id);
    Producto Add(Producto producto);
    Producto? Update(long id, Producto producto);
    Producto? PatchPrice(long id, decimal precio);
    bool Delete(long id);
    IEnumerable<Producto> Search(string? nombre);
    IEnumerable<Producto> FilterByCategoria(string categoria);
    IEnumerable<Producto> FilterByPrecio(decimal? min, decimal? max);
    IEnumerable<Producto> OrderByPrecio(bool asc);
    Dictionary<string, List<Producto>> GroupByCategoria();
    object GetEstadisticas();
}
