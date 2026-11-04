using CSharpFunctionalExtensions;
using ProductosEFCore.Dtos;
using ProductosEFCore.Errors;

namespace ProductosEFCore.Services;

/// <summary>
/// Servicio de autenticación: login, signup y consulta de usuario.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Inicia sesión con email y contraseña.
    /// </summary>
    Task<Result<AuthResponseDto, DomainError>> LoginAsync(LoginDto dto);

    /// <summary>
    /// Registra un nuevo usuario.
    /// </summary>
    Task<Result<AuthResponseDto, DomainError>> SignupAsync(SignupDto dto);

    /// <summary>
    /// Obtiene la información del usuario autenticado.
    /// </summary>
    Task<Result<UserDto, DomainError>> GetMeAsync(long userId);
}
