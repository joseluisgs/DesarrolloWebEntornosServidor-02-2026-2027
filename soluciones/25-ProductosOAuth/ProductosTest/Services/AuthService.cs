using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProductosEFCore.Config;
using ProductosEFCore.Dtos;
using ProductosEFCore.Entity;
using ProductosEFCore.Errors;
using ProductosEFCore.Models;
using Serilog;

namespace ProductosEFCore.Services;

/// <summary>
/// Implementación del servicio de autenticación con JWT.
/// </summary>
public class AuthService(
    AppDbContext appDbContext,
    UserManager<AppUser> userManager,
    SignInManager<AppUser> signInManager,
    IOptions<JwtSettings> jwtSettings
) : IAuthService
{
    /// <inheritdoc />
    public async Task<Result<AuthResponseDto, DomainError>> LoginAsync(LoginDto dto)
    {
        Log.Information("Intento de login para {Email}", dto.Email);

        var user = await userManager.FindByEmailAsync(dto.Email);
        if (user is null)
        {
            Log.Warning("Login fallido: usuario {Email} no encontrado", dto.Email);
            return Result.Failure<AuthResponseDto, DomainError>(
                UnauthorizedError.InvalidCredentials());
        }

        var result = await signInManager.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: false);
        if (!result.Succeeded)
        {
            Log.Warning("Login fallido: contraseña incorrecta para {Email}", dto.Email);
            return Result.Failure<AuthResponseDto, DomainError>(
                UnauthorizedError.InvalidCredentials());
        }

        var role = (await userManager.GetRolesAsync(user)).FirstOrDefault() ?? "USER";
        var token = GenerateJwtToken(user, role);

        Log.Information("Login exitoso para {Email} con rol {Role}", dto.Email, role);
        return Result.Success<AuthResponseDto, DomainError>(token);
    }

    /// <inheritdoc />
    public async Task<Result<AuthResponseDto, DomainError>> LoginExternalAsync(ExternalLoginInfo info)
    {
        Log.Information("Login externo con {Provider}", info.LoginProvider);

        // El email es el ancla de la cuenta: sin él no hay identidad posible
        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrWhiteSpace(email))
        {
            return Result.Failure<AuthResponseDto, DomainError>(
                new BusinessRuleError("El proveedor no devolvió email; no se puede identificar al usuario"));
        }

        // 1. ¿Ya existe una cuenta vinculada a ese login externo?
        var user = await userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

        if (user is null)
        {
            // 2. ¿Existe ya un usuario local con ese email? Se vincula en vez de duplicar
            user = await userManager.FindByEmailAsync(email);

            // 3. Primera vez: se crea la cuenta local, sin contraseña
            if (user is null)
            {
                user = new AppUser
                {
                    UserName = email,
                    Email = email,
                    Nombre = info.Principal.FindFirstValue(ClaimTypes.Name) ?? string.Empty,
                    CreatedAt = DateTime.UtcNow
                };

                var created = await userManager.CreateAsync(user);
                if (!created.Succeeded)
                {
                    var errors = string.Join(", ", created.Errors.Select(e => e.Description));
                    Log.Warning("Login externo fallido al crear usuario {Email}: {Errors}", email, errors);
                    return Result.Failure<AuthResponseDto, DomainError>(
                        new BusinessRuleError($"Error al crear el usuario: {errors}"));
                }

                await userManager.AddToRoleAsync(user, "USER");
            }

            // 4. Se asocia el proveedor a la cuenta (solo si aún no lo estaba)
            var linked = await userManager.AddLoginAsync(user, info);
            if (!linked.Succeeded)
            {
                var errors = string.Join(", ", linked.Errors.Select(e => e.Description));
                Log.Warning("Login externo fallido al vincular {Provider}: {Errors}", info.LoginProvider, errors);
                return Result.Failure<AuthResponseDto, DomainError>(
                    new BusinessRuleError($"Error al vincular {info.LoginProvider}: {errors}"));
            }
        }

        // De aquí en adelante, es el mismo JWT que emitiría el login local
        var role = (await userManager.GetRolesAsync(user)).FirstOrDefault() ?? "USER";
        var token = GenerateJwtToken(user, role);

        Log.Information("Login externo exitoso para {Email} con {Provider}", email, info.LoginProvider);
        return Result.Success<AuthResponseDto, DomainError>(token);
    }

    /// <inheritdoc />
    public async Task<Result<AuthResponseDto, DomainError>> SignupAsync(SignupDto dto)
    {
        Log.Information("Intento de registro para {Email}", dto.Email);

        var existingUser = await userManager.FindByEmailAsync(dto.Email);
        if (existingUser is not null)
        {
            Log.Warning("Registro fallido: {Email} ya existe", dto.Email);
            return Result.Failure<AuthResponseDto, DomainError>(
                ConflictError.Duplicate("usuario", dto.Email));
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
            Log.Warning("Registro fallido para {Email}: {Errors}", dto.Email, errors);
            return Result.Failure<AuthResponseDto, DomainError>(
                new BusinessRuleError($"Error al crear el usuario: {errors}"));
        }

        await userManager.AddToRoleAsync(user, "USER");

        var role = "USER";
        var token = GenerateJwtToken(user, role);

        Log.Information("Registro exitoso para {Email} con rol {Role}", dto.Email, role);
        return Result.Success<AuthResponseDto, DomainError>(token);
    }

    /// <inheritdoc />
    public async Task<Result<UserDto, DomainError>> GetMeAsync(long userId)
    {
        Log.Debug("Consultando usuario {UserId}", userId);

        var user = await appDbContext.Users.FindAsync(userId);
        if (user is null)
        {
            return Result.Failure<UserDto, DomainError>(
                NotFoundError.FromId(userId, "Usuario"));
        }

        var role = (await userManager.GetRolesAsync(user)).FirstOrDefault() ?? "USER";

        return Result.Success<UserDto, DomainError>(
            new UserDto(user.Id, user.Email ?? "", user.Nombre, user.Apellidos, role));
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

        return new AuthResponseDto(tokenString, user.Email ?? "", role, expires);
    }
}
