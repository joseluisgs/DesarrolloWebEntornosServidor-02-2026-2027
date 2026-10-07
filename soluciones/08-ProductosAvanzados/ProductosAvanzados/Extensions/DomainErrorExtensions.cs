using Microsoft.AspNetCore.Mvc;
using ProductosAvanzados.Errors;

namespace ProductosAvanzados.Extensions;

/// <summary>
/// Opcion C (7.8.3): centraliza el mapeo DomainError -> HTTP en una unica extension,
/// en lugar de repetir un <c>error switch</c> en cada controlador.
/// </summary>
/// <remarks>
/// Devuelve <see cref="ActionResult{T}"/> (y no <c>IActionResult</c>) para que los
/// controladores usen el tipo tipado y el OpenAPI/Swagger documente la respuesta real.
/// </remarks>
public static class DomainErrorExtensions
{
    public static ActionResult<T> ToHttpResult<T>(this DomainError error) => error switch
    {
        NotFoundError => new NotFoundObjectResult(new { message = error.Message }),
        ValidationError ve => new BadRequestObjectResult(new { message = ve.Message, errors = ve.Errors }),
        ConflictError => new ConflictObjectResult(new { message = error.Message }),
        BusinessRuleError => new UnprocessableEntityObjectResult(new { message = error.Message }),
        _ => new ObjectResult(new { message = "Error interno del servidor" }) { StatusCode = 500 }
    };
}
