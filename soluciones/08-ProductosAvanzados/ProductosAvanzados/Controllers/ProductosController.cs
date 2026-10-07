using System.Text.Json;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using ProductosAvanzados.Dtos;
using ProductosAvanzados.Errors;
using ProductosAvanzados.Extensions;
using ProductosAvanzados.Helpers;
using ProductosAvanzados.Mappers;
using ProductosAvanzados.Services;

namespace ProductosAvanzados.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductosController(IProductoService service) : ControllerBase
{
    [HttpGet]
    public ActionResult<List<ProductoDto>> GetAll()
    {
        var result = service.GetAll();
        return result.IsSuccess
            ? Ok(result.Value.Select(p => p.ToDto()).ToList())
            : result.Error.ToHttpResult<List<ProductoDto>>();
    }

    [HttpGet("{id:long}")]
    public ActionResult<ProductoDto> GetById(long id)
    {
        var result = service.GetById(id);
        return result.IsSuccess
            ? Ok(result.Value.ToDto())
            : result.Error.ToHttpResult<ProductoDto>();
    }

    [HttpPost]
    public ActionResult<ProductoDto> Create([FromBody] CreateProductoDto dto)
    {
        var result = service.Create(dto);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value.ToDto())
            : result.Error.ToHttpResult<ProductoDto>();
    }

    [HttpPut("{id:long}")]
    public ActionResult<ProductoDto> Update(long id, [FromBody] UpdateProductoDto dto)
    {
        var result = service.Update(id, dto);
        return result.IsSuccess
            ? Ok(result.Value.ToDto())
            : result.Error.ToHttpResult<ProductoDto>();
    }

    [HttpPatch("{id:long}/precio")]
    public ActionResult<ProductoDto> PatchPrice(long id, [FromBody] decimal precio)
    {
        var result = service.PatchPrice(id, precio);
        return result.IsSuccess
            ? Ok(result.Value.ToDto())
            : result.Error.ToHttpResult<ProductoDto>();
    }

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
    public ActionResult<List<ProductoDto>> Search([FromQuery] string q)
    {
        var result = service.Search(q);
        return result.IsSuccess
            ? Ok(result.Value.Select(p => p.ToDto()).ToList())
            : result.Error.ToHttpResult<List<ProductoDto>>();
    }

    [HttpGet("filter")]
    public ActionResult<List<ProductoDto>> Filter(
        [FromQuery] string? nombre,
        [FromQuery] string? categoria,
        [FromQuery] decimal? precioMin,
        [FromQuery] decimal? precioMax)
    {
        var result = service.Filter(nombre, categoria, precioMin, precioMax);
        return result.IsSuccess
            ? Ok(result.Value.Select(p => p.ToDto()).ToList())
            : result.Error.ToHttpResult<List<ProductoDto>>();
    }

    [HttpGet("paged")]
    public ActionResult<PagedResponse<ProductoDto>> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        // Techo de paginacion: pagina >= 1 y tamano entre 1 y 100.
        // Importan los DOS extremos: page=0 devuelve todo y pageSize=0 vacia la respuesta.
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var result = service.GetPaged(page, pageSize);
        if (result.IsFailure)
            return result.Error.ToHttpResult<PagedResponse<ProductoDto>>();

        var (items, totalPages, totalItems) = result.Value;

        var linkHeader = PaginationHelper.CreateLinkHeader(page, totalPages, pageSize, "/api/productos/paged");
        if (!string.IsNullOrEmpty(linkHeader))
            Response.Headers.Append("Link", linkHeader);

        return Ok(new PagedResponse<ProductoDto>
        {
            Data = items.Select(p => p.ToDto()).ToList(),
            Pagination = new PageMetadata(page, pageSize, totalPages, totalItems)
        });
    }

    [HttpPost("query")]
    public ActionResult<List<ProductoDto>> QueryProductos([FromBody] JsonElement query)
    {
        var nombre = query.TryGetProperty("nombre", out var n) ? n.GetString() : null;
        var categoria = query.TryGetProperty("categoria", out var c) ? c.GetString() : null;
        var precioMin = query.TryGetProperty("precioMin", out var pmin) ? pmin.GetDecimal() : (decimal?)null;
        var precioMax = query.TryGetProperty("precioMax", out var pmax) ? pmax.GetDecimal() : (decimal?)null;

        var result = service.Filter(nombre, categoria, precioMin, precioMax);
        return result.IsSuccess
            ? Ok(result.Value.Select(p => p.ToDto()).ToList())
            : result.Error.ToHttpResult<List<ProductoDto>>();
    }
}
