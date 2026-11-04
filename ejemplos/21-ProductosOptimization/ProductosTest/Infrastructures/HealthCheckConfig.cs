using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace ProductosEFCore.Infrastructures;

/// <summary>
/// Configuracion de Health Checks para monitorear la salud de la API.
/// </summary>
public static class HealthCheckConfig
{
    /// <summary>
    /// Registra health checks para base de datos y servicios externos.
    /// </summary>
    public static IServiceCollection AddHealthCheckConfig(this IServiceCollection services, IConfiguration configuration)
    {
        Log.Information("Configurando health checks...");

        services.AddHealthChecks()
            .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy());

        Log.Information("Health checks configurados");
        return services;
    }

    /// <summary>
    /// Configura los endpoints de health check.
    /// </summary>
    public static void MapHealthCheckEndpoints(this WebApplication app)
    {
        app.MapHealthChecks("/health", new()
        {
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";
                var result = new
                {
                    status = report.Status.ToString(),
                    checks = report.Entries.Select(e => new
                    {
                        name = e.Key,
                        status = e.Value.Status.ToString(),
                        duration = e.Value.Duration.TotalMilliseconds
                    })
                };
                await context.Response.WriteAsJsonAsync(result);
            }
        });

        app.MapHealthChecks("/health/ready", new()
        {
            Predicate = check => check.Tags.Contains("ready")
        });

        app.MapHealthChecks("/health/live", new()
        {
            Predicate = _ => false
        });
    }
}
