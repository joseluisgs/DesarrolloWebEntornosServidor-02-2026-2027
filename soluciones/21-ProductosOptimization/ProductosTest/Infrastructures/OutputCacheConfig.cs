using Microsoft.AspNetCore.OutputCaching;

namespace ProductosEFCore.Infrastructures;

/// <summary>
/// Configuración de la caché de respuestas (OutputCache + ETag + 304).
/// Solo se cachean los endpoints que lo declaran con [OutputCache(...)]:
/// los GET anónimos de productos y categorías. Las respuestas se vacían
/// por tag desde el servicio tras cada escritura (IOutputCacheStore.EvictByTagAsync).
/// </summary>
public static class OutputCacheConfig
{
    /// <summary>
    /// Registra el middleware de caché de salidas. Las políticas concretas
    /// (60 s + tag) se declaran por endpoint con [OutputCache(...)].
    /// </summary>
    public static IServiceCollection AddOutputCacheConfig(this IServiceCollection services)
    {
        services.AddOutputCache();
        return services;
    }

    /// <summary>
    /// Habilita la caché de salidas en el pipeline. Va DESPUÉS de la
    /// autenticación y la autorización: si no, podría servir respuestas
    /// cacheadas de un usuario a otro.
    /// </summary>
    public static WebApplication UseOutputCacheConfig(this WebApplication app)
    {
        app.UseOutputCache();
        return app;
    }
}
