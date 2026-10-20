using ProductosMongo.Extensions;
using ProductosMongo.Models;
using ProductosMongo.Services;
using Microsoft.AspNetCore.Mvc;
using ProductosMongo.Errors;

namespace ProductosMongo.Controllers;

/// <summary>
/// API Controller para CRUD de productos.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductosController(IProductoService service) : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<Producto>> GetAll()
    {
        var productos = service.GetAll();
        return Ok(productos);
    }

    [HttpGet("search")]
    public ActionResult<IEnumerable<Producto>> Search([FromQuery] string q)
    {
        var productos = service.Search(q);
        return Ok(productos);
    }

    [HttpGet("filter")]
    public ActionResult<IEnumerable<Producto>> Filter([FromQuery] string? nombre, [FromQuery] decimal? precioMin, [FromQuery] decimal? precioMax)
    {
        var productos = service.Filter(nombre, precioMin, precioMax);
        return Ok(productos);
    }

    [HttpGet("deleted")]
    public ActionResult<IEnumerable<Producto>> GetDeleted()
    {
        var productos = service.GetDeleted();
        return Ok(productos);
    }

    [HttpGet("{id}")]
    public ActionResult<Producto> GetById(string id)
    {
        var result = service.GetById(id);
        return result.IsSuccess
            ? Ok(result.Value)
            : result.Error.ToHttpResult<Producto>();
    }

    [HttpPost]
    public ActionResult<Producto> Create([FromBody] Producto producto)
    {
        var result = service.Create(producto);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value.Id.ToString() }, result.Value)
            : result.Error.ToHttpResult<Producto>();
    }

    [HttpPut("{id}")]
    public ActionResult<Producto> Update(string id, [FromBody] Producto producto)
    {
        if (id != producto.Id.ToString())
            return BadRequest(new { message = "El ID de la URL no coincide con el del cuerpo" });

        var result = service.Update(producto);
        return result.IsSuccess
            ? Ok(result.Value)
            : result.Error.ToHttpResult<Producto>();
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(string id)
    {
        var result = service.Delete(id);
        if (result.IsSuccess) return NoContent();

        var error = result.Error;
        return error switch
        {
            NotFoundError => NotFound(new { message = error.Message }),
            _ => StatusCode(StatusCodes.Status500InternalServerError, new { message = error.Message })
        };
    }
}
