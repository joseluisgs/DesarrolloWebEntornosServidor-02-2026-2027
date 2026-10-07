using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProductosConfigLogging.Middleware;

/// <summary>
/// Manejador global de excepciones.
/// Safety net: captura excepciones no controladas y las convierte en ProblemDetails.
/// Con Result Pattern, las excepciones de dominio NO deberían llegar aquí,
/// pero es bueno tenerlo por si acaso (bugs, errores de BD, etc.).
/// </summary>
public class GlobalExceptionHandler(
    RequestDelegate next,
    ILogger<GlobalExceptionHandler> logger
)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            var errorId = Guid.NewGuid().ToString()[..8];
            logger.LogError(ex, "Excepción no manejada. ErrorId: {ErrorId}", errorId);
            await HandleExceptionAsync(context, ex, errorId);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception, string errorId)
    {
        context.Response.ContentType = "application/json";

        var statusCode = exception switch
        {
            ArgumentException => StatusCodes.Status400BadRequest,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            TimeoutException => StatusCodes.Status408RequestTimeout,
            _ => StatusCodes.Status500InternalServerError
        };

        var message = exception switch
        {
            ArgumentException arg => arg.Message,
            UnauthorizedAccessException => "No autorizado",
            TimeoutException => "Tiempo de espera agotado",
            _ => "Ha ocurrido un error interno"
        };

        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            errorId,
            status = (int)statusCode,
            message,
            timestamp = DateTime.UtcNow.ToString("o"),
            path = context.Request.Path,
            method = context.Request.Method
        };

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
    }
}

/// <summary>
/// Extensión para registrar el middleware de excepciones.
/// </summary>
public static class GlobalExceptionHandlerExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionHandler>();
    }
}
