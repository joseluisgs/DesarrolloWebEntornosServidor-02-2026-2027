using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;
using ProductosEFCore.Dtos;
using ProductosEFCore.Errors;

namespace ProductosEFCore.Services;

/// <summary>
/// Servicio de autenticación: login local, login externo, signup y consulta de usuario.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Inicia sesión con email y contraseña.
    /// </summary>
    Task<Result<AuthResponseDto, DomainError>> LoginAsync(LoginDto dto);

    /// <summary>
    /// Convierte un login externo (Google, GitHub o Microsoft) en un usuario
    /// local de Identity y devuelve el JWT de esta API.
    /// </summary>
    Task<Result<AuthResponseDto, DomainError>> LoginExternalAsync(ExternalLoginInfo info);

    /// <summary>
    /// Registra un nuevo usuario.
    /// </summary>
    Task<Result<AuthResponseDto, DomainError>> SignupAsync(SignupDto dto);

    /// <summary>
    /// Obtiene la información del usuario autenticado.
    /// </summary>
    Task<Result<UserDto, DomainError>> GetMeAsync(long userId);
}
