using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using AuthMicroService.Config;
using AuthMicroService.Dtos;
using AuthMicroService.Entity;
using AuthMicroService.Errors;
using AuthMicroService.Models;
using Serilog;

namespace AuthMicroService.Services;

/// <summary>
/// Implementacion del servicio de autenticacion con JWT y Result pattern (ROP).
/// </summary>
public class AuthService(
    UserManager<AppUser> userManager,
    SignInManager<AppUser> signInManager,
    IOptions<JwtSettings> jwtSettings
) : IAuthService
{
    /// <inheritdoc />
    public async Task<Result<AuthResponseDto, DomainError>> RegisterAsync(RegisterDto dto)
    {
        Log.Information("[Auth] registro recibido de {Email}", dto.Email);

        var existingUser = await userManager.FindByEmailAsync(dto.Email);
        if (existingUser is not null)
        {
            Log.Warning("[Auth] registro RECHAZADO: {Email} ya existe", dto.Email);
            return Result.Failure<AuthResponseDto, DomainError>(
                new ConflictError($"Ya existe un usuario con el email '{dto.Email}'"));
        }

        var user = new AppUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            Nombre = dto.Nombre,
            Apellidos = dto.Apellidos,
            CreatedAt = DateTime.UtcNow
        };

        var result = await userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            Log.Warning("[Auth] registro RECHAZADO de {Email}: {Errors}", dto.Email, errors);
            return Result.Failure<AuthResponseDto, DomainError>(
                new BusinessRuleError($"Error al crear el usuario: {errors}"));
        }

        await userManager.AddToRoleAsync(user, "USER");

        var token = GenerateJwtToken(user, "USER");

        Log.Information("[Auth] registro OK: {Email} con rol USER · token emitido", dto.Email);
        return Result.Success<AuthResponseDto, DomainError>(token);
    }

    /// <inheritdoc />
    public async Task<Result<AuthResponseDto, DomainError>> LoginAsync(LoginDto dto)
    {
        Log.Information("[Auth] login recibido de {Email}", dto.Email);

        var user = await userManager.FindByEmailAsync(dto.Email);
        if (user is null)
        {
            Log.Warning("[Auth] login RECHAZADO: {Email} no existe", dto.Email);
            return Result.Failure<AuthResponseDto, DomainError>(
                UnauthorizedError.InvalidCredentials());
        }

        var result = await signInManager.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: false);
        if (!result.Succeeded)
        {
            Log.Warning("[Auth] login RECHAZADO: contraseña incorrecta de {Email}", dto.Email);
            return Result.Failure<AuthResponseDto, DomainError>(
                UnauthorizedError.InvalidCredentials());
        }

        var role = (await userManager.GetRolesAsync(user)).FirstOrDefault() ?? "USER";
        var token = GenerateJwtToken(user, role);

        Log.Information("[Auth] login OK: {Email} con rol {Role} · token emitido", dto.Email, role);
        return Result.Success<AuthResponseDto, DomainError>(token);
    }

    /// <summary>
    /// Genera un token JWT con los claims del usuario.
    /// </summary>
    private AuthResponseDto GenerateJwtToken(AppUser user, string role)
    {
        var settings = jwtSettings.Value;
        var expires = DateTime.UtcNow.AddMinutes(settings.ExpirationMinutes);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email ?? ""),
            new(ClaimTypes.Role, role),
            new(ClaimTypes.Name, $"{user.Nombre} {user.Apellidos}".Trim())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: settings.Issuer,
            audience: settings.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: credentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return new AuthResponseDto(tokenString, user.Email ?? "", user.Nombre, role, expires);
    }
}
