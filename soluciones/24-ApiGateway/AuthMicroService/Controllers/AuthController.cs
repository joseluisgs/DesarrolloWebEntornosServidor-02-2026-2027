using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using AuthMicroService.Dtos;
using AuthMicroService.Services;

namespace AuthMicroService.Controllers;

/// <summary>
/// API de Autenticacion: registro y login.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    /// <summary>
    /// Registra un nuevo usuario.
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var result = await authService.RegisterAsync(dto);
        if (result.IsFailure)
            return BadRequest(new { error = result.Error.Message });

        return Ok(result.Value);
    }

    /// <summary>
    /// Inicia sesion y devuelve un token JWT.
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await authService.LoginAsync(dto);
        if (result.IsFailure)
            return Unauthorized(new { error = result.Error.Message });

        return Ok(result.Value);
    }
}
