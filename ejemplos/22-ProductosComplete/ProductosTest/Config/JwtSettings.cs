namespace ProductosEFCore.Config;

/// <summary>
/// Configuracion JWT. Se lee de appsettings.json en la seccion Jwt.
/// </summary>
public class JwtSettings
{
    public const string SectionName = "Jwt";
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpirationMinutes { get; set; } = 30;
}
