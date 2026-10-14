using System.Text.Json;
using ProductosEFCore.Dtos;
using ProductosEFCore.Extensions;
using ProductosEFCore.Mappers;
using ProductosEFCore.Services;
using Microsoft.AspNetCore.Mvc;
using ProductosEFCore.Errors;

namespace ProductosEFCore.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductosController(IProductoService service) : ControllerBase
{
    [HttpGet]
    public ActionResult<List<ProductoDto>> GetAll()
    {
        var productos = service.GetAll();
        return Ok(productos.Select(p => p.ToDto()).ToList());
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

    [HttpPost("{id:long}/restore")]
    public ActionResult Restore(long id)
    {
        var result = service.Restore(id);
        if (result.IsSuccess) return Ok(new { message = $"Producto {id} restaurado" });

        var error = result.Error;
        return error switch
        {
            NotFoundError => NotFound(new { message = error.Message }),
            _ => StatusCode(StatusCodes.Status500InternalServerError, new { message = error.Message })
        };
    }

    [HttpGet("deleted")]
    public ActionResult<List<ProductoDto>> GetDeleted()
    {
        var productos = service.GetDeleted();
        return Ok(productos.Select(p => p.ToDto()).ToList());
    }

    [HttpGet("search")]
    public ActionResult<List<ProductoDto>> Search([FromQuery] string q)
    {
        var productos = service.Search(q);
        return Ok(productos.Select(p => p.ToDto()).ToList());
    }

    [HttpGet("filter")]
    public ActionResult<List<ProductoDto>> Filter(
        [FromQuery] string? nombre,
        [FromQuery] long? categoriaId,
        [FromQuery] decimal? precioMin,
        [FromQuery] decimal? precioMax)
    {
        var productos = service.Filter(nombre, categoriaId, precioMin, precioMax);
        return Ok(productos.Select(p => p.ToDto()).ToList());
    }

    [HttpGet("paged")]
    public ActionResult<PagedResponse<ProductoDto>> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        // Techo de paginacion: pagina >= 1 y tamano entre 1 y 100.
        // Importan los DOS extremos: page=0 devuelve todo y pageSize=0 vacia la respuesta.
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var (items, totalPages, totalItems) = service.GetPaged(page, pageSize);

return Ok(new PagedResponse<ProductoDto>
        {
            Data = items.Select(p => p.ToDto()).ToList(),
            Pagination = new PageMetadata(page, pageSize, totalPages, totalItems)
        });
    }

    [HttpGet("categorias")]
    public ActionResult<List<CategoriaDto>> GetCategorias()
    {
        var categorias = service.GetAllCategorias();
        return Ok(categorias.Select(c => c.ToDto()).ToList());
    }

    [HttpGet("categorias/{id:long}")]
    public ActionResult<CategoriaDto> GetCategoriaById(long id)
    {
        var categoria = service.GetCategoriaById(id);
        return categoria is not null
            ? Ok(categoria.ToDto())
            : NotFound(new { message = $"Categoría con ID {id} no encontrada" });
    }
}
