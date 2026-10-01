using Microsoft.AspNetCore.Mvc;
using ProductosControllersApiDI.Models;
using ProductosControllersApiDI.Services;

namespace ProductosControllersApiDI.Controllers;

/// <summary>
/// API Controller para gestionar productos.
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
    /// </summary>
    [HttpGet("{id:long}")]
    public ActionResult<Producto> GetById(long id)
    {
        var producto = service.GetById(id);
        if (producto is null) return NotFound();
        return Ok(producto);
    }

    /// <summary>
    /// Crea un nuevo producto.
    /// </summary>
    [HttpPost]
    public ActionResult<Producto> Create([FromBody] Producto producto)
    {
        try
        {
            var nuevo = service.Add(producto);
            return CreatedAtAction(nameof(GetById), new { id = nuevo.Id }, nuevo);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Actualiza un producto existente.
    /// </summary>
    [HttpPut("{id:long}")]
    public ActionResult<Producto> Update(long id, [FromBody] Producto producto)
    {
        try
        {
            var actualizado = service.Update(id, producto);
            if (actualizado is null) return NotFound();
            return Ok(actualizado);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Actualiza solo el precio de un producto.
    /// </summary>
    [HttpPatch("{id:long}/precio")]
    public ActionResult<Producto> PatchPrice(long id, [FromBody] decimal precio)
    {
        try
        {
            var actualizado = service.PatchPrice(id, precio);
            if (actualizado is null) return NotFound();
            return Ok(actualizado);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Elimina un producto (soft delete).
    /// </summary>
    [HttpDelete("{id:long}")]
    public IActionResult Delete(long id)
    {
        var eliminado = service.Delete(id);
        if (!eliminado) return NotFound();
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
