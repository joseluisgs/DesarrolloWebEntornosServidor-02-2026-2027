using ProductosTest.Models;

namespace ProductosTest.Repositories;

public interface IProductoRepository
{
    IEnumerable<Producto> GetAll();
    Producto? GetById(long id);
    Producto Add(Producto producto);
    Producto? Update(long id, Producto producto);
    Producto? PatchPrice(long id, decimal precio);
    bool Delete(long id);
    IEnumerable<Producto> Search(string termino);

    /// <summary>
    /// Devuelve los productos activos que cumplen los criterios indicados; los nulos no filtran.
    /// </summary>
    IEnumerable<Producto> Filter(string? nombre, string? categoria, decimal? precioMin, decimal? precioMax);

    /// <summary>
    /// Devuelve una página de productos activos, con el techo de paginación aplicado en la capa de datos.
    /// </summary>
    /// <param name="page">Página solicitada (base 1).</param>
    /// <param name="pageSize">Tamaño de página solicitado; el repositorio lo limita a 1..100.</param>
    /// <returns>Elementos de la página, total de páginas y total de elementos.</returns>
    (IEnumerable<Producto> Items, int TotalPages, int TotalItems) GetPaged(int page, int pageSize);
}
