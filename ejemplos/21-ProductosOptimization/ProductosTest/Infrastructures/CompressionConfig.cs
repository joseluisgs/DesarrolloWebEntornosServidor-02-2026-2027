using System.IO.Compression;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace ProductosEFCore.Infrastructures;

/// <summary>
/// Configuracion de compresion de respuestas HTTP.
/// </summary>
public static class CompressionConfig
{
    /// <summary>
    /// Configura la compresion Brotli y Gzip para respuestas HTTP.
    /// </summary>
    public static IServiceCollection AddResponseCompressionConfig(this IServiceCollection services)
    {
        Log.Information("Configurando compresion de respuestas...");

        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();
            options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                ["application/json", "application/xml", "text/plain", "text/html"]);
        });

        services.Configure<BrotliCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Optimal;
        });

        services.Configure<GzipCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Fastest;
        });

        Log.Information("Compresion de respuestas configurada (Brotli + Gzip)");
        return services;
    }
}
