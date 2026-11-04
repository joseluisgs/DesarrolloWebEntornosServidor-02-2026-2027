using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProductosGraphQL.Config;
using ProductosGraphQL.Entity;
using ProductosGraphQL.Models;
using Serilog;

namespace ProductosGraphQL.Infrastructures;

public static class AuthorizationConfig
{
    public static IServiceCollection AddAuthConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        Log.Information("Configurando autenticacion JWT...");

        var jwtSettings = configuration
            .GetSection(JwtSettings.SectionName)
            .Get<JwtSettings>() ?? new JwtSettings();

        services.Configure<JwtSettings>(
            configuration.GetSection(JwtSettings.SectionName));

        // AddIdentityCore: Identity sin UI (no registra la cookie Identity.Application),
        // el mecanismo oficial para APIs; roles y signInManager se añaden a mano.
        services.AddIdentityCore<AppUser>(options =>
        {
            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.User.RequireUniqueEmail = true;
        })
        .AddRoles<IdentityRole<long>>()
        .AddSignInManager()
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        // JWT
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
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.Key)),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorizationBuilder()
            .SetDefaultPolicy(new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build())
            // Esquema explícito en las políticas: no dependen del DefaultScheme.
            // ⚠️ AddPolicy pasa un builder al lambda y hay que MODIFICARLO. Si el lambda
            // construye una política con `new AuthorizationPolicyBuilder(...).Build()` y la
            // devuelve, esa política se descarta y la registrada queda vacía, lo que revienta
            // el arranque con "AuthorizationPolicy must have at least one requirement".
            .AddPolicy("RequireAdminRole", policy => policy
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .RequireRole("ADMIN"))
            .AddPolicy("RequireUserRole", policy => policy
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .RequireRole("USER", "ADMIN"))
            .AddPolicy("AdminOnly", policy => policy
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .RequireRole("ADMIN"));

        Log.Information("Autenticacion JWT configurada correctamente.");
        return services;
    }
}
