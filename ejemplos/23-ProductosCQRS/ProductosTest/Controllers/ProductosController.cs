using MediatR;
using ProductosCQRS.Dtos;
using ProductosCQRS.Extensions;
using ProductosCQRS.Features.Productos.Commands;
using ProductosCQRS.Features.Productos.Queries;
using ProductosCQRS.Mappers;
using ProductosCQRS.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductosCQRS.Errors;

namespace ProductosCQRS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductosController(IMediator mediator, IStorageService storage) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<ProductoReadDto>>> GetAll()
    {
        var result = await mediator.Send(new GetAllProductosQuery());
        return result.IsSuccess
            ? Ok(result.Value)
            : result.Error.ToHttpResult<List<ProductoReadDto>>();
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ProductoReadDto>> GetById(long id)
    {
        var result = await mediator.Send(new GetProductoByIdQuery(id));
        return result.IsSuccess
            ? Ok(result.Value)
            : result.Error.ToHttpResult<ProductoReadDto>();
    }

    [Authorize(Roles = "ADMIN")]
    [HttpPost]
    public async Task<ActionResult<ProductoDto>> Create([FromBody] CreateProductoDto dto)
    {
        var result = await mediator.Send(new CreateProductoCommand(dto));
        return result.IsSuccess
            ? Ok(result.Value)
            : result.Error.ToHttpResult<ProductoDto>();
    }

    [Authorize(Roles = "ADMIN")]
    [HttpPost("imagen")]
    public async Task<ActionResult<ProductoDto>> CreateConImagen(
        [FromForm] string nombre,
        [FromForm] decimal precio,
        [FromForm] long categoriaId,
        [FromForm] IFormFile archivo)
    {
        var saveResult = await storage.SaveFileAsync(archivo, "productos");
        if (saveResult.IsFailure)
            return BadRequest(new { error = saveResult.Error.Message });

        var dto = new CreateProductoDto
        {
            Nombre = nombre,
            Precio = precio,
            CategoriaId = categoriaId,
            Imagen = saveResult.Value
        };

        var result = await mediator.Send(new CreateProductoCommand(dto));
        return result.IsSuccess
            ? Ok(result.Value)
            : result.Error.ToHttpResult<ProductoDto>();
    }

    [Authorize(Roles = "ADMIN")]
    [HttpPatch("{id:long}/imagen")]
    public async Task<ActionResult> UpdateImagen(long id, IFormFile archivo)
    {
        var saveResult = await storage.SaveFileAsync(archivo, "productos");
        if (saveResult.IsFailure)
            return BadRequest(new { error = saveResult.Error.Message });

        var dto = new UpdateProductoDto
        {
            Nombre = "",
            Precio = 0,
            CategoriaId = 0,
            Imagen = saveResult.Value
        };

        var updateResult = await mediator.Send(new UpdateProductoCommand(id, dto));
        if (updateResult.IsSuccess) return Ok(new { imagen = saveResult.Value });

        var error = updateResult.Error;
        return error switch
        {
            NotFoundError => NotFound(new { message = error.Message }),
            _ => StatusCode(StatusCodes.Status500InternalServerError, new { message = error.Message })
        };
    }

    [Authorize(Roles = "ADMIN,USER")]
    [HttpPut("{id:long}")]
    public async Task<ActionResult<ProductoDto>> Update(long id, [FromBody] UpdateProductoDto dto)
    {
        var result = await mediator.Send(new UpdateProductoCommand(id, dto));
        return result.IsSuccess
            ? Ok(result.Value)
            : result.Error.ToHttpResult<ProductoDto>();
    }

    [Authorize(Roles = "ADMIN")]
    [HttpDelete("{id:long}")]
    public async Task<ActionResult> Delete(long id)
    {
        var result = await mediator.Send(new DeleteProductoCommand(id));
        if (result.IsSuccess) return NoContent();

        var error = result.Error;
        return error switch
        {
            NotFoundError => NotFound(new { message = error.Message }),
            _ => StatusCode(StatusCodes.Status500InternalServerError, new { message = error.Message })
        };
    }
}
