using Microsoft.AspNetCore.Mvc;
using ProductosMongo.Errors;

namespace ProductosMongo.Extensions;

/// <summary>
/// Extensiones para convertir DomainError en resultados HTTP.
/// </summary>
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
