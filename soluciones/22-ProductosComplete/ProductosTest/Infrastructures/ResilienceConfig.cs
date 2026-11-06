using Polly;
using Serilog;

namespace ProductosEFCore.Infrastructures;

/// <summary>
/// Configuracion de resiliencia con Polly para llamadas a servicios externos.
/// </summary>
public static class ResilienceConfig
{
    /// <summary>
    /// Registra politicas de resiliencia para llamadas HTTP.
    /// </summary>
    public static IServiceCollection AddResilienceConfig(this IServiceCollection services)
    {
        Log.Information("Configurando resiliencia con Polly...");

        // Retry: 3 reintentos con backoff exponencial
        var retryPolicy = Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                onRetry: (result, delay, retryCount, context) =>
                {
                    Log.Warning("Reintento {RetryCount} tras {Delay}s",
                        retryCount, delay.TotalSeconds);
                });

        // Registrar como IHttpClientFactory con politica de retry
        services.AddHttpClient("ProductosExterno")
            .AddPolicyHandler(retryPolicy);

        Log.Information("Resiliencia configurada: Retry(3) con backoff exponencial");
        return services;
    }
}
