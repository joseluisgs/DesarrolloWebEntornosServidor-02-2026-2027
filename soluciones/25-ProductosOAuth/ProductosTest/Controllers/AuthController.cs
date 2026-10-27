using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProductosEFCore.Dtos;
using ProductosEFCore.Extensions;
using ProductosEFCore.Models;
using ProductosEFCore.Services;

namespace ProductosEFCore.Controllers;

/// <summary>
/// Controller de autenticación: login local, login externo, signup y perfil.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController(
    IAuthService authService,
    SignInManager<AppUser> signInManager,
    IAuthenticationSchemeProvider schemeProvider) : ControllerBase
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
    /// Inicia el login externo: redirige el navegador al proveedor pedido
    /// ("Google", "GitHub" o "Microsoft"), si su puerta está activa.
    /// </summary>
    [HttpGet("external/{provider}")]
    public async Task<IActionResult> ExternalLogin([FromRoute] string provider)
    {
        var scheme = await schemeProvider.GetSchemeAsync(provider);
        if (scheme is null)
            return NotFound(new { message = $"La puerta '{provider}' no está activa: revisa Auth:{provider}:Enabled" });

        var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Auth");
        var properties = signInManager
            .ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return Challenge(properties, provider);
    }

    /// <summary>
    /// Destino del proveedor tras el consentimiento: convierte el login
    /// externo en usuario local de Identity y devuelve el JWT de esta API.
    /// El navegador llega aquí con un GET; la respuesta es el token.
    /// </summary>
    [AllowAnonymous]
    [HttpGet("external-callback")]
    public async Task<ActionResult<AuthResponseDto>> ExternalLoginCallback()
    {
        var info = await signInManager.GetExternalLoginInfoAsync();
        if (info is null)
            return Unauthorized(new { message = "No se pudo leer la información del proveedor" });

        var result = await authService.LoginExternalAsync(info);
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
