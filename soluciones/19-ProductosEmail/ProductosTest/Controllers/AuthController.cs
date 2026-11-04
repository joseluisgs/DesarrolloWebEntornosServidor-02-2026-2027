using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductosEFCore.Dtos;
using ProductosEFCore.Extensions;
using ProductosEFCore.Services;

namespace ProductosEFCore.Controllers;

/// <summary>
/// Controller de autenticación: login, signup y perfil.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    /// <summary>
    /// Inicia sesión con email y contraseña.
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
    {
        var result = await authService.LoginAsync(dto);
        return result.IsSuccess ? Ok(result.Value) : result.Error.ToHttpResult<AuthResponseDto>();
    }

    /// <summary>
    /// Registra un nuevo usuario.
    /// </summary>
    [HttpPost("signup")]
    public async Task<ActionResult<AuthResponseDto>> Signup([FromBody] SignupDto dto)
    {
        var result = await authService.SignupAsync(dto);
        return result.IsSuccess ? Ok(result.Value) : result.Error.ToHttpResult<AuthResponseDto>();
    }

    /// <summary>
    /// Obtiene la información del usuario autenticado.
    /// </summary>
    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> GetMe()
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var result = await authService.GetMeAsync(userId);
        return result.IsSuccess ? Ok(result.Value) : result.Error.ToHttpResult<UserDto>();
    }
}
