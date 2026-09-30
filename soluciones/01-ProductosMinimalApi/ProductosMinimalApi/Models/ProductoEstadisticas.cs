namespace ProductosMinimalApi.Models;

/// <summary>
/// Agregado con los datos de los productos activos que devuelve <c>GET /api/productos/estadisticas</c>.
/// </summary>
/// <param name="Total">Número de productos activos.</param>
/// <param name="PrecioMedio">Precio medio de los activos; 0 si no hay productos.</param>
/// <param name="PorCategoria">Cantidad de productos activos agrupados por categoría.</param>
public record ProductoEstadisticas(
    int Total,
    decimal PrecioMedio,
    Dictionary<string, int> PorCategoria);
