using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using ProductosEFCore.Config;
using ProductosEFCore.Entity;
using ProductosEFCore.Models;
using Serilog;

namespace ProductosEFCore.Infrastructures;

/// <summary>
/// Configuracion de Identity + JWT + Authorization.
/// </summary>
public static class IdentityConfig
{
    /// <summary>
    /// Registra Identity, JWT Bearer y políticas de autorización.
    /// </summary>
    /// <param name="services">Colección de servicios.</param>
    /// <param name="configuration">Configuración de la app.</param>
    /// <param name="environment">Entorno de la aplicación.</param>
    /// <returns>IServiceCollection para encadenar.</returns>
    public static IServiceCollection AddIdentityConfiguration(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        Log.Information("Configurando Identity y JWT...");

        // ── JWT Settings ─────────────────────────────────────────────────────
        services.Configure<JwtSettings>(
            configuration.GetSection(JwtSettings.SectionName));

        var jwtSettings = configuration
            .GetSection(JwtSettings.SectionName)
            .Get<JwtSettings>() ?? new JwtSettings();

        // ── Identity ─────────────────────────────────────────────────────────
        services.AddIdentity<AppUser, IdentityRole<long>>(options =>
        {
            // Password
            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;

            // Lockout
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;

            // User
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        // En este ejemplo se mantiene AddIdentity: el login externo necesita las
        // cookies Identity.External e Identity.Application. Se fijan los cuatro
        // esquemas a Bearer para que ningún camino de la API caiga en la cookie.
        // ── JWT Authentication ───────────────────────────────────────────────
        // Fijamos TODOS los esquemas a Bearer: sin DefaultForbidScheme, un 403
        // (usuario autenticado sin rol) caería en la cookie de Identity y
        // redirigiría en vez de devolver el JSON que espera una API.
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings.Key)),
                ClockSkew = TimeSpan.Zero
            };
        });

        // ── Authorization Policies ───────────────────────────────────────────
        // Esquema explícito en las políticas: no dependen del DefaultScheme
        services.AddAuthorizationBuilder()
            .AddPolicy("RequireAdmin", policy =>
                new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                    .RequireRole("ADMIN")
                    .Build())
            .AddPolicy("RequireUser", policy =>
                new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                    .RequireRole("USER", "ADMIN")
                    .Build());

        Log.Information("Identity y JWT configurados correctamente.");
        return services;
    }
}
