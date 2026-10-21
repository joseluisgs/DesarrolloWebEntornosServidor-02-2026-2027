using ProductosEFCore.Extensions;
using ProductosEFCore.Models;
using ProductosEFCore.Services;
using Microsoft.AspNetCore.Mvc;

namespace ProductosEFCore.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConcurrencyController(IConcurrencyService concurrencyService) : ControllerBase
{
    /// <summary>
    /// Actualiza el stock de un producto usando concurrencia optimista.
    /// </summary>
    /// <param name="id">ID del producto.</param>
    /// <param name="request">Nuevo stock.</param>
    /// <returns>Producto actualizado o error de conflicto.</returns>
    [HttpPut("optimistic/{id:long}")]
    public async Task<ActionResult<Producto>> UpdateOptimistic(long id, [FromBody] StockUpdateRequest request)
    {
        var result = await concurrencyService.UpdateOptimisticAsync(id, request.NewStock);
        return result.IsSuccess
            ? Ok(result.Value)
            : result.Error.ToHttpResult<Producto>();
    }

    /// <summary>
    /// Actualiza el stock de un producto usando concurrencia pesimista (FOR UPDATE).
    /// </summary>
    /// <param name="id">ID del producto.</param>
    /// <param name="request">Nuevo stock.</param>
    /// <returns>Producto actualizado o error.</returns>
    [HttpPut("pessimistic/{id:long}")]
    public async Task<ActionResult<Producto>> UpdatePessimistic(long id, [FromBody] StockUpdateRequest request)
    {
        var result = await concurrencyService.UpdatePessimisticAsync(id, request.NewStock);
        return result.IsSuccess
            ? Ok(result.Value)
            : result.Error.ToHttpResult<Producto>();
    }

    /// <summary>
    /// Actualiza el stock de un producto usando concurrencia mixta (validación + SQL directo).
    /// </summary>
    /// <param name="id">ID del producto.</param>
    /// <param name="request">Cantidad a decrementar.</param>
    /// <returns>Producto actualizado o error.</returns>
    [HttpPut("mixed/{id:long}")]
    public async Task<ActionResult<Producto>> UpdateMixed(long id, [FromBody] StockUpdateRequest request)
    {
        var result = await concurrencyService.UpdateMixedAsync(id, request.NewStock);
        return result.IsSuccess
            ? Ok(result.Value)
            : result.Error.ToHttpResult<Producto>();
    }
}

public record StockUpdateRequest
{
    public int NewStock { get; init; }
}
