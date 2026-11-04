using CSharpFunctionalExtensions;
using AuthMicroService.Dtos;
using AuthMicroService.Errors;

namespace AuthMicroService.Services;

/// <summary>
/// Servicio de autenticacion: registro, login y consulta de usuario.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Registra un nuevo usuario.
    /// </summary>
    Task<Result<AuthResponseDto, DomainError>> RegisterAsync(RegisterDto dto);

    /// <summary>
    /// Inicia sesion con email y contrasena.
    /// </summary>
    Task<Result<AuthResponseDto, DomainError>> LoginAsync(LoginDto dto);
}
