using System.Text.Json;
using ProductosEFCore.Dtos;
using ProductosEFCore.Extensions;
using ProductosEFCore.Services;
using ProductosEFCore.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductosEFCore.Errors;

namespace ProductosEFCore.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductosController(IProductoService service, IStorageService storage) : ControllerBase
{
    [HttpGet]
    public ActionResult<List<ProductoDto>> GetAll()
    {
        var productos = service.GetAll();
        return Ok(productos.ToList());
    }

    [HttpGet("{id:long}")]
    public ActionResult<ProductoDto> GetById(long id)
    {
        var result = service.GetById(id);
        return result.IsSuccess
            ? Ok(result.Value)
            : result.Error.ToHttpResult<ProductoDto>();
    }

    [Authorize(Roles = "ADMIN")]
    [HttpPost]
    public ActionResult<ProductoDto> Create([FromBody] CreateProductoDto dto)
    {
        var result = service.Create(dto);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value)
            : result.Error.ToHttpResult<ProductoDto>();
    }

    [Authorize(Roles = "ADMIN,USER")]
    [HttpPut("{id:long}")]
    public ActionResult<ProductoDto> Update(long id, [FromBody] UpdateProductoDto dto)
    {
        var result = service.Update(id, dto);
        return result.IsSuccess
            ? Ok(result.Value)
            : result.Error.ToHttpResult<ProductoDto>();
    }

    [Authorize(Roles = "ADMIN")]
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

    [Authorize(Roles = "ADMIN")]
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

    /// <summary>
    /// Crea un nuevo producto con imagen (multipart/form-data).
    /// </summary>
    [Authorize(Roles = "ADMIN")]
    [HttpPost("imagen")]
    public async Task<ActionResult<ProductoDto>> CreateConImagen(
        [FromForm] string nombre,
        [FromForm] decimal precio,
        [FromForm] long categoriaId,
        [FromForm] IFormFile archivo)
    {
        // Guardar imagen
        var saveResult = await storage.SaveFileAsync(archivo, "productos");
        if (saveResult.IsFailure)
            return saveResult.Error.ToHttpResult<ProductoDto>();

        // Crear producto con la imagen
        var dto = new CreateProductoDto
        {
            Nombre = nombre,
            Precio = precio,
            CategoriaId = categoriaId,
            Imagen = saveResult.Value
        };
        var result = service.Create(dto);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value)
            : result.Error.ToHttpResult<ProductoDto>();
    }

    /// <summary>
    /// Actualiza la imagen de un producto existente (PATCH parcial).
    /// </summary>
    [Authorize(Roles = "ADMIN")]
    [HttpPatch("{id:long}/imagen")]
    public async Task<ActionResult<ProductoDto>> UpdateImagen(long id, IFormFile archivo)
    {
        var existente = service.GetById(id);
        if (existente.IsFailure)
            return existente.Error.ToHttpResult<ProductoDto>();

        // Eliminar imagen anterior si existe
        if (!string.IsNullOrEmpty(existente.Value.Imagen))
            await storage.DeleteFileAsync(existente.Value.Imagen);

        // Guardar nueva imagen
        var saveResult = await storage.SaveFileAsync(archivo, "productos");
        if (saveResult.IsFailure)
            return saveResult.Error.ToHttpResult<ProductoDto>();

        // Actualizar solo la imagen
        var dto = new UpdateProductoDto
        {
            Nombre = existente.Value.Nombre,
            Precio = existente.Value.Precio,
            CategoriaId = existente.Value.CategoriaId,
            Imagen = saveResult.Value
        };
        var updateResult = service.Update(id, dto);
        return updateResult.IsSuccess
            ? Ok(updateResult.Value)
            : updateResult.Error.ToHttpResult<ProductoDto>();
    }

    [HttpGet("deleted")]
    public ActionResult<List<ProductoDto>> GetDeleted()
    {
        var productos = service.GetDeleted();
        return Ok(productos.ToList());
    }

    [HttpGet("search")]
    public ActionResult<List<ProductoDto>> Search([FromQuery] string q)
    {
        var productos = service.Search(q);
        return Ok(productos.ToList());
    }

    [HttpGet("filter")]
    public ActionResult<List<ProductoDto>> Filter(
        [FromQuery] string? nombre,
        [FromQuery] long? categoriaId,
        [FromQuery] decimal? precioMin,
        [FromQuery] decimal? precioMax)
    {
        var productos = service.Filter(nombre, categoriaId, precioMin, precioMax);
        return Ok(productos.ToList());
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
            Data = items.ToList(),
            Pagination = new PageMetadata(page, pageSize, totalPages, totalItems)
        });
    }

    [HttpGet("categorias")]
    public ActionResult<List<CategoriaDto>> GetCategorias()
    {
        var categorias = service.GetAllCategorias();
        return Ok(categorias.ToList());
    }

    [HttpGet("categorias/{id:long}")]
    public ActionResult<CategoriaDto> GetCategoriaById(long id)
    {
        var categoria = service.GetCategoriaById(id);
        return categoria is not null
            ? Ok(categoria)
            : NotFound(new { message = $"Categoria con ID {id} no encontrada" });
    }
}
