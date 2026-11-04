using Microsoft.AspNetCore.Mvc;
using ProductosGraphQL.Dtos;
using ProductosGraphQL.Services;

namespace ProductosGraphQL.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await authService.LoginAsync(dto);
        return result.IsSuccess
            ? Ok(result.Value)
            : Unauthorized(new { error = result.Error.Message });
    }

    [HttpPost("signup")]
    public async Task<IActionResult> Signup([FromBody] SignupDto dto)
    {
        var result = await authService.SignupAsync(dto);
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { error = result.Error.Message });
    }
}
