using Microsoft.AspNetCore.Identity;
using ProductosGraphQL.Entity;
using ProductosGraphQL.Models;
using Serilog;

namespace ProductosGraphQL.Seed;

/// <summary>
/// Semilla de Identity: crea los roles y los usuarios iniciales.
/// Sin los roles, el registro falla al llamar a <c>AddToRoleAsync</c>, y sin el
/// usuario admin ninguna mutación GraphQL (política AdminOnly) puede tener éxito.
/// </summary>
public static class IdentitySeeder
{
    /// <summary>
    /// Crea los roles ADMIN y USER y los usuarios de prueba si aún no existen.
    /// </summary>
    /// <param name="scope">Proveedor de servicios del ámbito actual.</param>
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

        // ── Usuario admin (rol ADMIN: puede ejecutar las mutaciones) ─────────
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
                Log.Information("Usuario admin creado: {Email} (contraseña Admin123!)", adminEmail);
            }
            else
            {
                Log.Warning("Error al crear admin: {Errors}",
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        // ── Usuario regular (rol USER: solo lectura) ─────────────────────────
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
                Log.Information("Usuario regular creado: {Email} (contraseña User123!)", userEmail);
            }
            else
            {
                Log.Warning("Error al crear usuario: {Errors}",
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }
}
