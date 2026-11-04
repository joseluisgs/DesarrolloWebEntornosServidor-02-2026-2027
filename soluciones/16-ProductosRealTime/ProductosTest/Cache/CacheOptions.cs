namespace ProductosEFCore.Cache;

/// <summary>
/// Opciones de configuración de caché.
/// Se configura desde appsettings.json en la sección "Cache".
/// </summary>
public class CacheOptions
{
    /// <summary>
    /// Sección de configuración.
    /// </summary>
    public const string SectionName = "Cache";

    /// <summary>
    /// Tiempo de vida por defecto de las entradas en caché (en minutos).
    /// </summary>
    public int DefaultExpirationMinutes { get; set; } = 30;

    /// <summary>
    /// Tiempo de expiración deslizante (en minutos). Si el dato no se accede
    /// durante este tiempo, se elimina. Se renueva en cada acceso.
    /// </summary>
    public int SlidingExpirationMinutes { get; set; } = 10;

    /// <summary>
    /// Convierte los minutos a TimeSpan.
    /// </summary>
    public TimeSpan DefaultExpiration => TimeSpan.FromMinutes(DefaultExpirationMinutes);

    /// <summary>
    /// Convierte los minutos a TimeSpan.
    /// </summary>
    public TimeSpan SlidingExpiration => TimeSpan.FromMinutes(SlidingExpirationMinutes);
}
