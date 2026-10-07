using Microsoft.AspNetCore.Mvc;
using ProductosExcepciones.Models;
using ProductosExcepciones.Services;

namespace ProductosExcepciones.Controllers;

/// <summary>
/// API Controller para gestionar productos.
/// Las excepciones de dominio se propagan al middleware global.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductosController(IProductoService service) : ControllerBase
{
    /// <summary>
    /// Obtiene todos los productos activos.
    /// </summary>
    [HttpGet]
    public ActionResult<IEnumerable<Producto>> GetAll() =>
        Ok(service.GetAll());

    /// <summary>
    /// Obtiene un producto por su ID.
    /// Lanza NotFoundException si no existe.
    /// </summary>
    [HttpGet("{id:long}")]
    public ActionResult<Producto> GetById(long id) =>
        Ok(service.GetById(id));

    /// <summary>
    /// Crea un nuevo producto.
    /// Lanza ValidationException si los datos son inválidos.
    /// Lanza ConflictException si el nombre ya existe.
    /// </summary>
    [HttpPost]
    public ActionResult<Producto> Create([FromBody] Producto producto)
    {
        var nuevo = service.Add(producto);
        return CreatedAtAction(nameof(GetById), new { id = nuevo.Id }, nuevo);
    }

    /// <summary>
    /// Actualiza un producto existente.
    /// Lanza NotFoundException si no existe.
    /// </summary>
    [HttpPut("{id:long}")]
    public ActionResult<Producto> Update(long id, [FromBody] Producto producto) =>
        Ok(service.Update(id, producto));

    /// <summary>
    /// Actualiza solo el precio de un producto.
    /// Lanza NotFoundException si no existe.
    /// </summary>
    [HttpPatch("{id:long}/precio")]
    public ActionResult<Producto> PatchPrice(long id, [FromBody] decimal precio) =>
        Ok(service.PatchPrice(id, precio));

    /// <summary>
    /// Elimina un producto (soft delete).
    /// Lanza NotFoundException si no existe.
    /// </summary>
    [HttpDelete("{id:long}")]
    public IActionResult Delete(long id)
    {
        service.Delete(id);
        return NoContent();
    }

    /// <summary>
    /// Busca productos por término de búsqueda.
    /// </summary>
    [HttpGet("search")]
    public ActionResult<IEnumerable<Producto>> Search([FromQuery] string termino) =>
        Ok(service.Search(termino));

    /// <summary>
    /// Filtra productos por categoría.
    /// </summary>
    [HttpGet("filter/categoria")]
    public ActionResult<IEnumerable<Producto>> FilterByCategoria([FromQuery] string categoria) =>
        Ok(service.FilterByCategoria(categoria));

    /// <summary>
    /// Filtra productos por rango de precios.
    /// </summary>
    [HttpGet("filter/precio")]
    public ActionResult<IEnumerable<Producto>> FilterByPrecio(
        [FromQuery] decimal min = 0, [FromQuery] decimal max = decimal.MaxValue) =>
        Ok(service.FilterByPrecio(min, max));

    /// <summary>
    /// Ordena productos por precio.
    /// </summary>
    [HttpGet("order/precio")]
    public ActionResult<IEnumerable<Producto>> OrderByPrecio(
        [FromQuery] bool descendente = false) =>
        Ok(service.OrderByPrecio(descendente));

    /// <summary>
    /// Agrupa productos por categoría.
    /// </summary>
    [HttpGet("group/categoria")]
    public ActionResult GetByCategoria()
    {
        var agrupados = service.GroupByCategoria()
            .Select(g => new { Categoria = g.Key, Productos = g.ToList() });
        return Ok(agrupados);
    }

    /// <summary>
    /// Obtiene estadísticas de los productos.
    /// </summary>
    [HttpGet("stats")]
    public ActionResult GetEstadisticas() =>
        Ok(service.GetEstadisticas());
}
