using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductosMicroService.Dtos;
using ProductosMicroService.Services;

namespace ProductosMicroService.Controllers;

/// <summary>
/// API de Productos: CRUD con cache y validacion de Proveedores.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductosController(IProductoService service) : ControllerBase
{
    /// <summary>
    /// Obtiene todos los productos (cache → PostgreSQL).
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var result = await service.GetAllReadAsync();
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error.Message });
    }

    /// <summary>
    /// Obtiene un producto por ID (cache → PostgreSQL).
    /// </summary>
    [HttpGet("{id:long}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await service.GetByIdReadAsync(id);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error.Message });
    }

    /// <summary>
    /// Crea un nuevo producto (requiere JWT + rol Admin).
    /// Responde con el mismo DTO de lectura que GET, para que la respuesta sea
    /// idéntica en Swagger, en Bruno y en la suite de automatización.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Create([FromBody] CreateProductoDto dto)
    {
        var result = service.Create(dto);
        if (result.IsFailure)
            return BadRequest(new { error = result.Error.Message });

        var lectura = await service.GetByIdReadAsync(result.Value!.Id);
        return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, lectura.Value);
    }

    /// <summary>
    /// Actualiza un producto (requiere JWT + rol Admin).
    /// Igual que Create: responde con el DTO de lectura.
    /// </summary>
    [HttpPut("{id:long}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateProductoDto dto)
    {
        var result = service.Update(id, dto);
        if (result.IsFailure)
            return BadRequest(new { error = result.Error.Message });

        var lectura = await service.GetByIdReadAsync(id);
        return Ok(lectura.Value);
    }

    /// <summary>
    /// Elimina un producto (requiere JWT + rol Admin).
    /// </summary>
    [HttpDelete("{id:long}")]
    [Authorize(Roles = "ADMIN")]
    public IActionResult Delete(long id)
    {
        var result = service.Delete(id);
        return result.IsSuccess ? NoContent() : NotFound(new { error = result.Error.Message });
    }
}
