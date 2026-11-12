namespace AuthMicroService.Dtos;

/// <summary>
/// DTO para registro de usuario.
/// </summary>
public record RegisterDto(string Email, string Password, string Nombre, string Apellidos);

/// <summary>
/// DTO para inicio de sesion.
/// </summary>
public record LoginDto(string Email, string Password);

/// <summary>
/// DTO de respuesta de autenticacion (token JWT).
/// </summary>
public record AuthResponseDto(string Token, string Email, string Nombre, string Rol, DateTime Expiration);

/// <summary>
/// DTO de informacion de usuario.
/// </summary>
public record UserDto(long Id, string Email, string Nombre, string Apellidos, string Rol);
