using Microsoft.AspNetCore.Mvc;
using ProductosScrutorApi.Models;
using ProductosScrutorApi.Services;

namespace ProductosScrutorApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductosController(IProductoService service) : ControllerBase
{
    [HttpGet]
    public ActionResult<List<Producto>> GetAll() =>
        Ok(service.GetAll());

    [HttpGet("{id:long}")]
    public ActionResult<Producto> GetById(long id)
    {
        var producto = service.GetById(id);
        return producto is not null ? Ok(producto) : NotFound();
    }

    [HttpPost]
    public ActionResult<Producto> Create(Producto producto)
    {
        try
        {
            var creado = service.Create(producto);
            return CreatedAtAction(nameof(GetById), new { id = creado.Id }, creado);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:long}")]
    public ActionResult<Producto> Update(long id, Producto producto)
    {
        try
        {
            var actualizado = service.Update(id, producto);
            return actualizado is not null ? Ok(actualizado) : NotFound();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPatch("{id:long}")]
    public ActionResult<Producto> Patch(long id, Dictionary<string, object> cambios)
    {
        if (cambios.TryGetValue("precio", out var precio) && precio is System.Text.Json.JsonElement jsonVal)
        {
            try
            {
                var actualizado = service.PatchPrice(id, jsonVal.GetDecimal());
                return actualizado is not null ? Ok(actualizado) : NotFound();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        return BadRequest();
    }

    [HttpDelete("{id:long}")]
    public ActionResult Delete(long id) =>
        service.Delete(id) ? NoContent() : NotFound();

    [HttpGet("search")]
    public ActionResult<List<Producto>> Search([FromQuery] string? nombre) =>
        Ok(service.Search(nombre));

    [HttpGet("categoria/{categoria}")]
    public ActionResult<List<Producto>> FilterByCategoria(string categoria) =>
        Ok(service.FilterByCategoria(categoria));

    [HttpGet("precio")]
    public ActionResult<List<Producto>> FilterByPrecio([FromQuery] decimal? min, [FromQuery] decimal? max) =>
        Ok(service.FilterByPrecio(min, max));

    [HttpGet("ordenar")]
    public ActionResult<List<Producto>> OrderByPrecio([FromQuery] bool? asc) =>
        Ok(service.OrderByPrecio(asc ?? true));

    [HttpGet("grupo-categoria")]
    public ActionResult<Dictionary<string, List<Producto>>> GroupByCategoria() =>
        Ok(service.GroupByCategoria());

    [HttpGet("estadisticas")]
    public ActionResult GetEstadisticas() =>
        Ok(service.GetEstadisticas());
}
