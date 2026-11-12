using Microsoft.AspNetCore.Identity;

namespace AuthMicroService.Models;

/// <summary>
/// Modelo de usuario para ASP.NET Core Identity.
/// </summary>
public class AppUser : IdentityUser
{
    public string Nombre { get; set; } = string.Empty;

    public string Apellidos { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
