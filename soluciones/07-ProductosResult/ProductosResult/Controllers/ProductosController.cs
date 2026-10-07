using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using ProductosResult.Errors;
using ProductosResult.Extensions;
using ProductosResult.Models;
using ProductosResult.Services;

namespace ProductosResult.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductosController(IProductoService service) : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<Producto>> GetAll()
    {
        var result = service.GetAll();
        return result.IsSuccess ? Ok(result.Value) : result.Error.ToHttpResult<IEnumerable<Producto>>();
    }

    [HttpGet("{id:long}")]
    public ActionResult<Producto> GetById(long id)
    {
        var result = service.GetById(id);
        return result.IsSuccess ? Ok(result.Value) : result.Error.ToHttpResult<Producto>();
    }

    [HttpPost]
    public ActionResult<Producto> Create([FromBody] Producto producto)
    {
        var result = service.Create(producto);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value)
            : result.Error.ToHttpResult<Producto>();
    }

    [HttpPut("{id:long}")]
    public ActionResult<Producto> Update(long id, [FromBody] Producto producto)
    {
        var result = service.Update(id, producto);
        return result.IsSuccess ? Ok(result.Value) : result.Error.ToHttpResult<Producto>();
    }

    [HttpPatch("{id:long}/precio")]
    public ActionResult<Producto> PatchPrice(long id, [FromBody] decimal precio)
    {
        var result = service.PatchPrice(id, precio);
        return result.IsSuccess ? Ok(result.Value) : result.Error.ToHttpResult<Producto>();
    }

    // 204 sin payload tipado: ActionResult (sin generico) + switch inline,
    // igual que hace Tienda en sus endpoints Delete.
    [HttpDelete("{id:long}")]
    public ActionResult Delete(long id)
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

    [HttpGet("search")]
    public ActionResult<IEnumerable<Producto>> Search([FromQuery] string? termino)
    {
        var result = service.Search(termino);
        return result.IsSuccess ? Ok(result.Value) : result.Error.ToHttpResult<IEnumerable<Producto>>();
    }
}
