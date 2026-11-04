using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProductosEFCore.Entity;
using ProductosEFCore.Models;
using Serilog;

namespace ProductosEFCore.Seed;

/// <summary>
/// Semilla de Identity: crea roles y usuarios iniciales.
/// </summary>
public static class IdentitySeeder
{
    /// <summary>
    /// Semilla de roles y usuarios de Identity.
    /// </summary>
    /// <param name="scope">ServiceProvider del scope.</param>
    /// <returns>Task.</returns>
    public static async Task SeedAsync(IServiceProvider scope)
    {
        var roleManager = scope.GetRequiredService<RoleManager<IdentityRole<long>>>();
        var userManager = scope.GetRequiredService<UserManager<AppUser>>();

        // ── Roles ────────────────────────────────────────────────────────────
        string[] roles = ["ADMIN", "USER"];
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<long>(role));
                Log.Information("Rol '{Role}' creado", role);
            }
        }

        // ── Usuario Admin ────────────────────────────────────────────────────
        const string adminEmail = "admin@admin.com";
        if (await userManager.FindByEmailAsync(adminEmail) is null)
        {
            var admin = new AppUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                Nombre = "Admin",
                Apellidos = "Sistema",
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(admin, "Admin123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "ADMIN");
                Log.Information("Usuario admin creado: {Email}", adminEmail);
            }
            else
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                Log.Warning("Error al crear admin: {Errors}", errors);
            }
        }

        // ── Usuario regular ──────────────────────────────────────────────────
        const string userEmail = "user@user.com";
        if (await userManager.FindByEmailAsync(userEmail) is null)
        {
            var user = new AppUser
            {
                UserName = userEmail,
                Email = userEmail,
                Nombre = "Usuario",
                Apellidos = "Regular",
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(user, "User123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "USER");
                Log.Information("Usuario regular creado: {Email}", userEmail);
            }
            else
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                Log.Warning("Error al crear usuario: {Errors}", errors);
            }
        }
    }
}
