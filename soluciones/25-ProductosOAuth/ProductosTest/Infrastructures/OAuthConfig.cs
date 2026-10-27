using Serilog;

namespace ProductosEFCore.Infrastructures;

/// <summary>
/// Puertas de login externo (OAuth2/OIDC), activables por configuración.
/// Cada puerta solo se registra si Auth:{Proveedor}:Enabled es true: con la
/// puerta cerrada la app ni siquiera conoce al proveedor. Los ClientId y
/// ClientSecret van en user-secrets, nunca en appsettings.json:
///   dotnet user-secrets set "Google:ClientId" "xxxx.apps.googleusercontent.com"
/// </summary>
public static class OAuthConfig
{
    /// <summary>
    /// Registra las puertas externas activadas en la configuración.
    /// Identity + JWT (la puerta propia) ya está registrada antes, en IdentityConfig;
    /// aquí solo se añaden esquemas de proveedores a la misma autenticación.
    /// </summary>
    /// <param name="services">Colección de servicios.</param>
    /// <param name="configuration">Configuración de la app (sección Auth).</param>
    /// <returns>IServiceCollection para encadenar.</returns>
    public static IServiceCollection AddOAuthDoors(this IServiceCollection services, IConfiguration configuration)
    {
        // Un único AddAuthentication() como menú de puertas: los esquemas
        // defaults (JWT Bearer) ya los puso IdentityConfig, aquí solo se
        // añaden esquemas de proveedores encima.
        var auth = services.AddAuthentication();

        if (configuration.GetValue<bool>("Auth:Google:Enabled"))
        {
            auth.AddGoogle(options =>
            {
                options.ClientId = configuration["Google:ClientId"]
                    ?? throw new InvalidOperationException("Google:ClientId no configurado (user-secrets)");
                options.ClientSecret = configuration["Google:ClientSecret"]
                    ?? throw new InvalidOperationException("Google:ClientSecret no configurado (user-secrets)");
                options.UsePkce = true;   // ver 16.5.5: aunque sea app web, con PKCE no sobra
            });
            Log.Information("Puerta de login externo activa: Google");
        }

        if (configuration.GetValue<bool>("Auth:GitHub:Enabled"))
        {
            auth.AddGitHub(options =>
            {
                options.ClientId = configuration["GitHub:ClientId"]
                    ?? throw new InvalidOperationException("GitHub:ClientId no configurado (user-secrets)");
                options.ClientSecret = configuration["GitHub:ClientSecret"]
                    ?? throw new InvalidOperationException("GitHub:ClientSecret no configurado (user-secrets)");
            });
            Log.Information("Puerta de login externo activa: GitHub");
        }

        if (configuration.GetValue<bool>("Auth:Twitter:Enabled"))
        {
            // Paquete de la comunidad; OAuth 2.0 con las mismas opciones que el resto.
            // El esquema se llama "Twitter": la URL de login es /api/Auth/external/Twitter
            auth.AddTwitter(options =>
            {
                options.ClientId = configuration["Twitter:ClientId"]
                    ?? throw new InvalidOperationException("Twitter:ClientId no configurado (user-secrets)");
                options.ClientSecret = configuration["Twitter:ClientSecret"]
                    ?? throw new InvalidOperationException("Twitter:ClientSecret no configurado (user-secrets)");
            });
            Log.Information("Puerta de login externo activa: Twitter/X");
        }

        if (configuration.GetValue<bool>("Auth:Microsoft:Enabled"))
        {
            // AddMicrosoftAccount viene en caja del SDK: no necesita paquete NuGet extra
            auth.AddMicrosoftAccount(options =>
            {
                options.ClientId = configuration["Microsoft:ClientId"]
                    ?? throw new InvalidOperationException("Microsoft:ClientId no configurado (user-secrets)");
                options.ClientSecret = configuration["Microsoft:ClientSecret"]
                    ?? throw new InvalidOperationException("Microsoft:ClientSecret no configurado (user-secrets)");
            });
            Log.Information("Puerta de login externo activa: Microsoft");
        }

        return services;
    }
}
