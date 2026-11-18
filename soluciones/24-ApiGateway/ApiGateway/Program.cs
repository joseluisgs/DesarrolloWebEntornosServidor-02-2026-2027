using System.Diagnostics;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using Yarp.ReverseProxy.Model;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    // Microsoft/System a Warning: en la demostración solo se ven los logs propios
    // ([GATEWAY], [Auth]...) y los de YARP, no el ruido de Swagger ni el de ASP.NET
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

try
{
    Log.Information("Iniciando ApiGateway...");

    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();

    // ── JWT Authentication ───────────────────────────────────────────────
    var jwtKey = builder.Configuration["Jwt:Key"]
        ?? "SuperSecretKeyForJwtToken2024!SuperSecretKeyForJwtToken2024!";
    var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "AuthMicroService";
    var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "ApiGateway";

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
            };

            // ── Log de demostración: qué hace el gateway con cada token ─────────
            options.Events = new JwtBearerEvents
            {
                // El token es correcto → la petición merece seguir hacia el micro
                OnTokenValidated = contexto =>
                {
                    var principal = contexto.Principal;
                    var email = principal?.FindFirst(ClaimTypes.Email)?.Value
                        ?? principal?.FindFirst("email")?.Value;
                    var rol = principal?.FindFirst(ClaimTypes.Role)?.Value
                        ?? principal?.FindFirst("role")?.Value
                        ?? "sin rol";

                    Log.Information("[GATEWAY] token de {Email} validado (rol {Rol}) → se reenvía la petición", email, rol);
                    return Task.CompletedTask;
                },

                // Llega un token pero no cuadra (firma, caducidad, emisor...)
                OnAuthenticationFailed = contexto =>
                {
                    Log.Information("[GATEWAY] token descartado: {Motivo}", contexto.Exception.Message);
                    return Task.CompletedTask;
                },

                //401: no hay token o la política no se cumple
                OnChallenge = contexto =>
                {
                    var cabecera = contexto.Request.Headers.Authorization.ToString();
                    Log.Information("[GATEWAY] 401 en {Ruta}: {Motivo}",
                        contexto.Request.Path,
                        string.IsNullOrWhiteSpace(cabecera)
                            ? "no llega cabecera Authorization (falta el Bearer)"
                            : $"llega un token pero no vale ({contexto.Error ?? "motivo desconocido"})");
                    return Task.CompletedTask;
                },
            };
        });

    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("authenticated", policy =>
            policy.RequireAuthenticatedUser());
    });

    // ── YARP Reverse Proxy ───────────────────────────────────────────────
    builder.Services.AddReverseProxy()
        .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    // ── Log de demostración: el gateway como puerta única de entrada ──────────
    // Va ANTES de la autenticación/autorización a propósito: así se ve también la
    // petición que el gateway corta aquí mismo (401 sin token, o ruta que no casa
    // con ninguna), que es justo lo que no llega a ningún microservicio.
    app.Use(async (context, next) =>
    {
        if (!context.Request.Path.StartsWithSegments("/api"))
        {
            await next();
            return;
        }

        Log.Information("[GATEWAY] {Metodo} {Ruta} · petición recibida del cliente en el puerto 5100",
            context.Request.Method, context.Request.Path.Value);

        await next();

        if (context.Response.StatusCode >= 400 && context.Features.Get<IReverseProxyFeature>() is null)
        {
            Log.Information("[GATEWAY] {Metodo} {Ruta} → {Status} · respondida aquí mismo, sin llamar a ningún microservicio",
                context.Request.Method, context.Request.Path.Value, context.Response.StatusCode);
        }
    });

    app.UseAuthentication();
    app.UseAuthorization();

    // ── Mapeo del Reverse Proxy ──────────────────────────────────────────
    // Misma canalización que MapReverseProxy() por defecto, con un middleware
    // extra al final que deja constancia de la decisión de enrutado: qué ruta ha
    // casado, a qué destino se ha reenviado la petición y qué ha devuelto.
    app.MapReverseProxy(proxy =>
    {
        proxy.UseSessionAffinity();
        proxy.UseLoadBalancing();
        proxy.UsePassiveHealthChecks();

        proxy.Use(async (context, next) =>
        {
            var reloj = Stopwatch.StartNew();
            await next();
            reloj.Stop();

            var feature = context.Features.Get<IReverseProxyFeature>();
            var ruta = feature?.Route.Config.RouteId ?? "sin-ruta";
            var destino = feature?.ProxiedDestination?.Model.Config.Address
                ?? feature?.AvailableDestinations?.FirstOrDefault()?.Model.Config.Address
                ?? "sin-destino";

            Log.Information("[GATEWAY] ruta '{Ruta}' reenviada a {Destino} · {Status} en {Ms} ms",
                ruta, destino, context.Response.StatusCode, reloj.ElapsedMilliseconds);
        });
    });

    // ── Mapa de enrutado, leído de la propia configuración que usa YARP ───────
    // Es lo primero que conviene ver en clase: dónde acaba cada ruta del gateway.
    var clusters = builder.Configuration.GetSection("ReverseProxy:Clusters").GetChildren()
        .ToDictionary(
            c => c.Key,
            c => c.GetSection("Destinations").GetChildren().FirstOrDefault()?["Address"] ?? "sin destino");

    Log.Information("ApiGateway escuchando en http://localhost:5100 · mapa de rutas (appsettings.json):");
    foreach (var ruta in builder.Configuration.GetSection("ReverseProxy:Routes").GetChildren())
    {
        var clusterId = ruta["ClusterId"] ?? "sin-clúster";
        var destino = clusters.GetValueOrDefault(clusterId, "sin destino");
        var sufijo = string.IsNullOrEmpty(ruta["AuthorizationPolicy"])
            ? string.Empty
            : "  · exige JWT (política 'authenticated')";

        Log.Information("  {Patron}  →  {Destino}{Sufijo}",
            ruta["Match:Path"] ?? "/", destino, sufijo);
    }

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "ApiGateway termino inesperadamente");
}
finally
{
    Log.CloseAndFlush();
}
