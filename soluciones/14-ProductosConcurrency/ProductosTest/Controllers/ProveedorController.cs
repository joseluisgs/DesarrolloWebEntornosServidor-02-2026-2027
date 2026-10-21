using ProductosEFCore.Dtos;
using ProductosEFCore.Extensions;
using ProductosEFCore.Models;
using ProductosEFCore.Services;
using Microsoft.AspNetCore.Mvc;
using ProductosEFCore.Errors;

namespace ProductosEFCore.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProveedorController(IProveedorService service) : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<Proveedor>> GetAll()
    {
        var proveedores = service.GetAll();
        return Ok(proveedores);
    }

    [HttpGet("{id}")]
    public ActionResult<Proveedor> GetById(string id)
    {
        var result = service.GetById(id);
        return result.IsSuccess
            ? Ok(result.Value)
            : result.Error.ToHttpResult<Proveedor>();
    }

    [HttpPost]
    public ActionResult<Proveedor> Create([FromBody] CreateProveedorDto dto)
    {
        var result = service.Create(dto);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value.Id.ToString() }, result.Value)
            : result.Error.ToHttpResult<Proveedor>();
    }

    [HttpPut("{id}")]
    public ActionResult<Proveedor> Update(string id, [FromBody] UpdateProveedorDto dto)
    {
        var result = service.Update(id, dto);
        return result.IsSuccess
            ? Ok(result.Value)
            : result.Error.ToHttpResult<Proveedor>();
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
