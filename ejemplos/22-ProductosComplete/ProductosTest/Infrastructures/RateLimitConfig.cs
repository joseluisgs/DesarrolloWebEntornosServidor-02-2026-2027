using System.Globalization;
using System.Threading.RateLimiting;
using Serilog;

namespace ProductosEFCore.Infrastructures;

/// <summary>
/// Configuracion de Rate Limiting con la API nativa de .NET
/// (<c>System.Threading.RateLimiting</c>), sin dependencias de terceros.
/// </summary>
/// <remarks>
/// Antes se usaba el paquete <c>AspNetCoreRateLimit</c>, pero lleva mas de 4
/// anos sin actualizarse. La API nativa (desde .NET 7) cubre el mismo caso de
/// uso sin anadir dependencias al proyecto.
/// Reglas aplicadas por cliente (IP) y ambito:
/// <list type="bullet">
/// <item>Todas las peticiones: 60 por minuto.</item>
/// <item>Escritura (<c>POST /api/productos</c>): 10 por minuto.</item>
/// <item>Cliente local (loopback): 100 por minuto.</item>
/// </list>
/// Si dos reglas comparten periodo se aplica la mas restrictiva.
/// </remarks>
public static class RateLimitConfig
{
    /// <summary>Limite por minuto para peticiones de escritura.</summary>
    private const int EscrituraLimit = 10;

    /// <summary>Limite por minuto para el resto de peticiones.</summary>
    private const int GeneralLimit = 60;

    /// <summary>Limite por minuto para clientes en local (loopback).</summary>
    private const int LocalLimit = 100;

    /// <summary>Periodo comun a todas las reglas.</summary>
    private static readonly TimeSpan Window = TimeSpan.FromMinutes(1);

    /// <summary>
    /// Configura rate limiting global basado en IP, verbo y ruta.
    /// </summary>
    public static IServiceCollection AddRateLimitingConfig(this IServiceCollection services)
    {
        Log.Information("Configurando rate limiting...");

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            // Limite global: se aplica a todas las peticiones sin necesidad de
            // atributos [EnableRateLimiting] en los controllers.
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            {
                var ip = ResolveClientIp(context);
                var esEscritura = HttpMethods.IsPost(context.Request.Method)
                    && context.Request.Path.StartsWithSegments("/api/productos");

                // Particion: IP + ambito. Lectura y escritura llevan contadores
                // independientes y la escritura es siempre la mas restrictiva.
                var (ambito, limite) = esEscritura
                    ? ("escritura", EscrituraLimit)
                    : IsLoopback(ip)
                        ? ("local", LocalLimit)
                        : ("general", GeneralLimit);

                var partitionKey = string.Create(
                    CultureInfo.InvariantCulture,
                    $"{ip}_{ambito}");

                return RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ =>
                    new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = limite,
                        Window = Window,
                        QueueLimit = 0,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        AutoReplenishment = true
                    });
            });

            // Respuesta 429 con cuerpo JSON y cabecera Retry-After.
            // OnRejected devuelve ValueTask, por eso el lambda es async.
            options.OnRejected = async (context, cancellationToken) =>
            {
                var retryAfter = 1;
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var remaining))
                {
                    retryAfter = Math.Max(1, (int)Math.Ceiling(remaining.TotalSeconds));
                }

                context.HttpContext.Response.Headers.RetryAfter =
                    retryAfter.ToString(CultureInfo.InvariantCulture);

                await context.HttpContext.Response.WriteAsJsonAsync(new
                {
                    message = "Demasiadas solicitudes. Por favor, intente mas tarde.",
                    errorType = "RateLimitError",
                    retryAfter
                }, cancellationToken);
            };
        });

        Log.Information(
            "Rate limiting configurado: 60 req/min generales, 100 req/min en local, " +
            "10 req/min en POST /api/productos");
        return services;
    }

    /// <summary>
    /// Resuelve la IP del cliente: cabeceras del proxy inverso
    /// (<c>X-Forwarded-For</c> primer salto, luego <c>X-Real-IP</c>) con
    /// fallback a la IP de conexion directa.
    /// </summary>
    private static string ResolveClientIp(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwarded)
            && forwarded.Count > 0)
        {
            var first = forwarded.ToString().Split(',')[0].Trim();
            if (first.Length > 0)
            {
                return first;
            }
        }

        if (context.Request.Headers.TryGetValue("X-Real-IP", out var realIp)
            && realIp.Count > 0)
        {
            var value = realIp.ToString().Trim();
            if (value.Length > 0)
            {
                return value;
            }
        }

        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }

    /// <summary>Indica si la IP corresponde a la propia maquina (desarrollo local).</summary>
    private static bool IsLoopback(string ip) =>
        ip is "127.0.0.1" or "::1" or "::ffff:127.0.0.1";
}
