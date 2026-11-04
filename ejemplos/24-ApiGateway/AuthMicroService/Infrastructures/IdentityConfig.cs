using AuthMicroService.Config;
using AuthMicroService.Entity;
using AuthMicroService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace AuthMicroService.Infrastructures;

/// <summary>
/// Configuracion de Identity y JWT.
/// </summary>
public static class IdentityConfig
{
    /// <summary>
    /// Registra Identity, JWT y PostgreSQL.
    /// </summary>
    public static IServiceCollection AddIdentityConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        Log.Information("Configurando Identity y JWT...");

        // PostgreSQL
        var connectionString = configuration.GetConnectionString("PostgresConnection")
            ?? "Host=localhost;Database=auth_db;Username=admin;Password=admin123";
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Identity
        // AddIdentityCore: Identity sin UI (no registra la cookie Identity.Application),
        // el mecanismo oficial para APIs; roles y signInManager se añaden a mano.
        services.AddIdentityCore<AppUser>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 6;
            options.User.RequireUniqueEmail = true;
        })
        .AddRoles<IdentityRole>()
        .AddSignInManager()
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        // JWT
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        // Authentication
        // Fijamos TODOS los esquemas a Bearer: sin DefaultForbidScheme, un 403
        // (usuario autenticado sin rol) caería en la cookie de Identity y
        // redirigiría en vez de devolver el JSON que espera una API.
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
            options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
            options.DefaultForbidScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            var jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()!;
            options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                    System.Text.Encoding.UTF8.GetBytes(jwtSettings.Key))
            };
        });

        services.AddAuthorization();

        Log.Information("Identity y JWT configurados correctamente.");
        return services;
    }

    /// <summary>
    /// Inicializa los roles (ADMIN, USER).
    /// </summary>
    public static async Task InitializeRolesAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        // Asegurar esquema Identity (tablas AspNet*)
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.EnsureCreatedAsync();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        string[] roles = { "ADMIN", "USER" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
                Log.Information("Rol '{Role}' creado", role);
            }
        }

        // Seed admin (necesario para POST/PUT/DELETE /api/productos con rol ADMIN)
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var adminEmail = "admin@admin.com";
        if (await userManager.FindByEmailAsync(adminEmail) is null)
        {
            var admin = new AppUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                Nombre = "Admin",
                Apellidos = "Gateway",
                CreatedAt = DateTime.UtcNow
            };
            var createResult = await userManager.CreateAsync(admin, "Admin123!");
            if (createResult.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "ADMIN");
                Log.Information("Usuario admin creado: {Email}", adminEmail);
            }
            else
            {
                Log.Warning("No se pudo crear admin: {Errors}",
                    string.Join(", ", createResult.Errors.Select(e => e.Description)));
            }
        }
    }
}
