namespace ProductosEFCore.Dtos;

public record LoginDto(string Email, string Password);
public record SignupDto(string Email, string Password, string Nombre, string Apellidos);
public record AuthResponseDto(string Token, string Email, string Role, DateTime ExpiresAt);
public record UserDto(long Id, string Email, string Nombre, string Apellidos, string Role);
