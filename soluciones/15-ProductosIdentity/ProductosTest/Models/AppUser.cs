using Microsoft.AspNetCore.Identity;

namespace ProductosEFCore.Models;

/// <summary>
/// Usuario personalizado que hereda de IdentityUser.
/// Los campos Nombre y Apellidos se usan en el JWT y en la API.
/// </summary>
public class AppUser : IdentityUser<long>
{
    public string Nombre { get; set; } = string.Empty;

    public string Apellidos { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
