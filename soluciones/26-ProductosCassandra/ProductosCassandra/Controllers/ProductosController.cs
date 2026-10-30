using Microsoft.AspNetCore.Mvc;
using ProductosCassandra.Dtos;
using ProductosCassandra.Services;

namespace ProductosCassandra.Controllers;

/// <summary>
/// Endpoints de productos. No hay GET /api/Productos (listado general) a propósito:
/// query-first significa que solo existen las consultas que el modelo prevé, y ninguna
/// tabla devuelve el catálogo entero.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductosController(IProductoService service) : ControllerBase
{
    /// <summary>Consulta Q2: el producto suelto por su id.</summary>
    /// <param name="id">Id del producto.</param>
    /// <returns>El producto, o 404 si no existe.</returns>
    [HttpGet("{id:long}")]
    public async Task<ActionResult<ProductoDto>> GetById(long id)
    {
        var producto = await service.GetByIdAsync(id);
        return producto is not null
            ? Ok(producto)
            : NotFound(new { message = $"Producto con ID {id} no encontrado" });
    }

    /// <summary>
    /// Consulta Q4: el histórico de precios de un producto, del más reciente al más
    /// antiguo. El orden lo da la tabla (CLUSTERING ORDER BY (fecha DESC)), sin ORDER BY.
    /// </summary>
    /// <param name="id">Id del producto (partition key).</param>
    /// <returns>Los cambios de precio del producto.</returns>
    [HttpGet("{id:long}/historial-precios")]
    public async Task<ActionResult<List<HistorialPrecioDto>>> GetHistorialPrecios(long id)
    {
        var historial = await service.GetHistorialPreciosAsync(id);
        return Ok(historial.ToList());
    }

    /// <summary>Consulta Q5: qué precios cambiaron en un día dado (el informe).</summary>
    /// <param name="fecha">Día del informe (partition key).</param>
    /// <returns>Los cambios de precio de ese día.</returns>
    [HttpGet("precios-cambios/{fecha:datetime}")]
    public async Task<ActionResult<List<CambioPrecioDiaDto>>> GetCambiosPrecio(DateTime fecha)
    {
        var cambios = await service.GetCambiosPrecioAsync(fecha);
        return Ok(cambios.ToList());
    }

    /// <summary>
    /// Alta de producto: una operación de negocio que se convierte en DOS escrituras,
    /// una en cada tabla, con el nombre de la categoría copiado en ambas.
    /// El id lo aporta el cliente porque en Cassandra no hay autoincremento.
    /// </summary>
    /// <param name="dto">Datos del producto nuevo.</param>
    /// <returns>201 con el producto creado, o 404 si la categoría no existe.</returns>
    [HttpPost]
    public async Task<ActionResult<ProductoDto>> Create([FromBody] CreateProductoDto dto)
    {
        var creado = await service.CreateAsync(dto);
        return creado is not null
            ? CreatedAtAction(nameof(GetById), new { id = creado.Id }, creado)
            : NotFound(new { message = $"La categoría {dto.CategoriaId} no existe" });
    }

    /// <summary>
    /// Cambio de precio: UNA petición que escribe en TODAS las copias (histórico,
    /// informe del día y precio actual en las dos tablas de lectura). Cuatro escrituras.
    /// </summary>
    /// <param name="id">Id del producto.</param>
    /// <param name="dto">Precio nuevo.</param>
    /// <returns>204, o 404 si el producto no existe.</returns>
    [HttpPut("{id:long}/precio")]
    public async Task<IActionResult> CambiarPrecio(long id, [FromBody] CambioPrecioDto dto)
    {
        var encontrado = await service.RegistrarCambioPrecioAsync(id, dto.Precio);
        return encontrado
            ? NoContent()
            : NotFound(new { message = $"Producto con ID {id} no encontrado" });
    }

    /// <summary>
    /// Borrado compensatorio: el producto vive en dos tablas, así que se borra de las dos.
    /// CQL no tiene transacciones repartidas; la compensación la orquesta la aplicación.
    /// </summary>
    /// <param name="id">Id del producto.</param>
    /// <param name="categoriaId">Id de la categoría, que es parte de la clave de una tabla.</param>
    /// <returns>204 siempre, borrado o no.</returns>
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, [FromQuery] long categoriaId)
    {
        await service.DeleteAsync(id, categoriaId);
        return NoContent();
    }
}
