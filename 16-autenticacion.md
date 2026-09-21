# 16. Autenticación JWT y BCrypt

## Tabla de Contenidos

- [16. Autenticación JWT y BCrypt](#16-autenticación-jwt-y-bcrypt)
  - [16.1. Introducción](#161-introducción)
    - [16.1.1. ¿Qué es la Autenticación?](#1611-qué-es-la-autenticación)
    - [16.1.2. Stateless vs Stateful](#1612-stateless-vs-stateful)
    - [16.1.3. Por qué JWT para APIs](#1613-por-qué-jwt-para-apis)
    - [16.1.4. Flujo Completo de Autenticación](#1614-flujo-completo-de-autenticación)
  - [16.2. JWT en Profundidad](#162-jwt-en-profundidad)
    - [16.2.1. Estructura del JWT](#1621-estructura-del-jwt)
    - [16.2.2. Claims](#1622-claims)
    - [16.2.3. Validación de Tokens](#1623-validación-de-tokens)
  - [16.3. BCrypt: Hash de Contraseñas](#163-bcrypt-hash-de-contraseñas)
    - [16.3.1. Por qué no MD5 ni SHA256](#1631-por-qué-no-md5-ni-sha256)
    - [16.3.2. BCrypt en C#](#1632-bcrypt-en-c)
    - [16.3.3. Work Factor](#1633-work-factor)
    - [16.3.4. Comparativa de Algoritmos](#1634-comparativa-de-algoritmos)
  - [16.4. Enfoque Manual (Estilo Tienda)](#164-enfoque-manual-estilo-tienda)
    - [16.4.1. Modelo de Usuario](#1641-modelo-de-usuario)
    - [16.4.2. JwtService](#1642-jwtservice)
    - [16.4.3. AuthService](#1643-authservice)
    - [16.4.4. Configuración en DI (AuthenticationConfig.cs)](#1644-configuración-en-di-authenticationconfigcs)
    - [16.4.5. AuthController](#1645-authcontroller)
    - [16.4.6. Program.cs](#1646-programcs)
  - [16.5. Enfoque Identity](#165-enfoque-identity)
    - [16.5.1. Instalación](#1651-instalación)
    - [16.5.2. Configuración](#1652-configuración)
    - [16.5.3. Ventajas y Desventajas](#1653-ventajas-y-desventajas)
  - [16.6. Comparación de Enfoques](#166-comparación-de-enfoques)
  - [16.7. Buenas Prácticas](#167-buenas-prácticas)
  - [16.8. Reto](#168-reto)
  - [16.9. Resumen](#169-resumen)

---

# 16. Autenticación JWT y BCrypt

> 💡 **Punto de partida:** Cuando abres Instagram, introduces tu email y contraseña. Instagram comprueba que eres quien dices ser y te da acceso a tu feed. Ese proceso se llama **autenticación**. Pero en una API REST, ¿cómo sabe el servidor quién eres en cada petición? La respuesta está en JWT y BCrypt.

## 16.1. Introducción

### 16.1.1. ¿Qué es la Autenticación?

La **autenticación** es el proceso de verificar la identidad de un usuario. Es la pregunta: **¿Quién eres?**

> 📝 **Nota:** No confundas autenticación con autorización. La autenticación verifica la identidad (¿quién eres?), mientras que la autorización determina qué puedes hacer (¿qué permisos tienes?). En el siguiente tema veremos autorización en profundidad.

📌 Ejemplo real: Cuando inicias sesión en **Netflix**, introduces tu email y contraseña. Netflix verifica esas credenciales contra su base de datos. Si son correctas, te permite acceder a tu perfil y tu lista de contenido. Eso es autenticación.

### 16.1.2. Stateless vs Stateful

Existen dos modelos fundamentales para mantener la sesión de un usuario:

```mermaid
flowchart LR
    subgraph "Stateful (Con Estado)"
        A1["Cliente"] -->|1. Login| A2["Servidor"]
        A2 -->|2. Session ID (cookie)| A1
        A1 -->|3. Request + Session ID| A3["Servidor"]
        A3 -->|4. Buscar sesión en memoria/BD| A3
        A3 -->|5. Responder| A1
    end

    subgraph "Stateless (Sin Estado)"
        B1["Cliente"] -->|1. Login| B2["Servidor"]
        B2 -->|2. JWT Token| B1
        B1 -->|3. Request + JWT| B4["Servidor"]
        B4 -->|4. Verificar JWT localmente| B4
        B4 -->|5. Responder| B1
    end

    style A1 fill:#9C27B0,color:#fff
    style A2 fill:#2196F3,color:#fff
    style A3 fill:#2196F3,color:#fff
    style B1 fill:#9C27B0,color:#fff
    style B2 fill:#4CAF50,color:#fff
    style B4 fill:#4CAF50,color:#fff
```

| Aspecto | Stateful (Sesión) | Stateless (JWT) |
|---------|-------------------|-----------------|
| **Almacenamiento** | Sesión en servidor | Token en cliente |
| **Escalabilidad** | Difícil (sticky sessions) | Fácil (cualquier servidor) |
| **Rendimiento** | Lookup de sesión en cada request | Verificación de firma local |
| **Memoria del servidor** | Crece con usuarios | Constante |
| **Mobile/API** | Complicado (cookies) | Natural (header Bearer) |
| **Revocación** | Inmediata (borrar sesión) | Difícil (esperar expiración) |

> 💡 **Consejo:** Para APIs REST que sirven a clientes SPA, móviles o微servicios, **stateless con JWT** es casi siempre la mejor opción. Las sesiones stateful son útiles en aplicaciones Razor Pages o Blazor Server donde el navegador maneja las cookies automáticamente.

### 16.1.3. Por qué JWT para APIs

**JWT (JSON Web Token)** es el estándar de facto para autenticación en APIs REST. Las razones:

1. **Autocontenido**: El token lleva toda la información (claims) dentro
2. **Firado criptográficamente**: No se puede falsificar sin la clave secreta
3. **Sin estado**: El servidor no necesita almacenar nada para validar
4. **Interoperable**: Funciona entre diferentes lenguajes y plataformas
5. **Estándar**: Basado en RFC 7519, soportado por todas las frameworks

📌 Ejemplo real: **Stripe** usa JWT para autenticar las llamadas a su API de pagos. Cada petición de un cliente lleva un token Bearer que Stripe valida localmente sin consultar una base de datos.

### 16.1.4. Flujo Completo de Autenticación

```mermaid
flowchart TD
    A["Usuario introduce credenciales"] --> B["POST /api/auth/login"]
    B --> C["Servidor verifica credenciales"]
    C -->|Credenciales incorrectas| D["401 Unauthorized"]
    C -->|Credenciales correctas| E["Generar JWT Token"]
    E --> F["Devolver token al cliente"]
    F --> G["Cliente almacena token"]
    G --> H["Request con Header: Bearer token"]
    H --> I["Servidor valida JWT"]
    I -->|Token inválido/expirado| J["401 Unauthorized"]
    I -->|Token válido| K["Procesar petición"]
    K --> L["200 OK + datos"]

    style A fill:#9C27B0,color:#fff
    style D fill:#f44336,color:#fff
    style E fill:#4CAF50,color:#fff
    style F fill:#4CAF50,color:#fff
    style J fill:#f44336,color:#fff
    style L fill:#4CAF50,color:#fff
```

> 📝 **Nota:** El flujo se repite en cada petición. La diferencia con sesiones es que el servidor NO almacena nada: el token es autocontenido y se valida únicamente con la clave secreta.

---

## 16.2. JWT en Profundidad

### 16.2.1. Estructura del JWT

Un JWT se compone de **tres partes** separadas por puntos: `HEADER.PAYLOAD.SIGNATURE`

```mermaid
flowchart TD
    subgraph "JWT Completo"
        T["eyJhbGciOiJIUzI1NiJ9.eyJzdWIiOiIxIn0.SflKxwRJSMeKKF2QT4fwpM"]
    end

    subgraph "Header (Base64Url)"
        H1["{\"alg\":\"HS256\",\"typ\":\"JWT\"}"]
    end

    subgraph "Payload (Base64Url)"
        P1["{\"sub\":\"1\",\"name\":\"Juan\",\"exp\":1700000000}"]
    end

    subgraph "Signature"
        S1["HMAC-SHA256(base64(header) + \".\" + base64(payload), secret_key)"]
    end

    T -->|Parte 1| H1
    T -->|Parte 2| P1
    T -->|Parte 3| S1

    style T fill:#607D8B,color:#fff
    style H1 fill:#1B5E20,color:#fff
    style P1 fill:#0D47A1,color:#fff
    style S1 fill:#E65100,color:#fff
```

**Header** — Define el algoritmo de firma y tipo de token:

```json
{
  "alg": "HS256",
  "typ": "JWT"
}
```

**Payload** — Contiene los claims (información del usuario):

```json
{
  "sub": "1",
  "name": "Juan García",
  "email": "juan@email.com",
  "role": "ADMIN",
  "iat": 1700000000,
  "exp": 1700000900
}
```

**Signature** — Firma criptográfica que garantiza la integridad:

```csharp
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

var token = new JwtSecurityToken(
    issuer: "MiApi",
    audience: "MiApiClients",
    claims: userClaims,
    expires: DateTime.UtcNow.AddMinutes(15),
    signingCredentials: creds
);
```

> ⚠️ **Advertencia:** El payload de un JWT **NO está cifrado**, solo codificado en Base64Url. Cualquiera puede leerlo. **Nunca** incluyas información sensible (contraseñas, números de tarjeta) en el payload. Solo incluye claims que no sean secretos.

### 16.2.2. Claims

Los **claims** son pares clave-valor que transportan información sobre el usuario y el token:

| Claim | Nombre | Descripción |
|-------|--------|-------------|
| `sub` | Subject | Identificador principal del usuario (ID) |
| `iss` | Issuer | Quién emite el token |
| `aud` | Audience | Para quién está destinado el token |
| `exp` | Expiration | Fecha de expiración (Unix timestamp) |
| `nbf` | Not Before | Fecha a partir de la cual es válido |
| `iat` | Issued At | Fecha de emisión del token |
| `jti` | JWT ID | Identificador único del token |

Claims personalizados que añadimos nosotros:

```csharp
var claims = new List<Claim>
{
    new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
    new(JwtRegisteredClaimNames.Email, user.Email),
    new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
    new(JwtRegisteredClaimNames.Iat,
        DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
        ClaimValueTypes.Integer64),
    new("username", user.Username),
    new("role", user.Role),
    new("displayName", $"{user.FirstName} {user.LastName}")
};
```

> 📝 **Nota:** El claim `sub` es el estándar para el identificador del usuario. Usar `user.Id` como valor de `sub` permite al servidor identificar al usuario en cada petición sin consultar la base de datos.

### 16.2.3. Validación de Tokens

```mermaid
flowchart TD
    A["Request con Bearer token"] --> B["Extraer token del Header"]
    B --> C["Parsear JWT"]
    C -->|Formato inválido| D["401: Token malformado"]
    C -->|OK| E["Verificar firma"]
    E -->|Firma inválida| F["401: Token falsificado"]
    E -->|OK| G["Verificar expiración"]
    G -->|Expirado| H["401: Token expirado"]
    G -->|OK| I["Verificar issuer"]
    I -->|Issuer inválido| J["401: Issuer no válido"]
    I -->|OK| K["Verificar audience"]
    K -->|Audience inválido| L["401: Audience no válido"]
    K -->|OK| M["Extraer claims"]
    M --> N["ClaimsPrincipal válido"]

    style A fill:#607D8B,color:#fff
    style D fill:#f44336,color:#fff
    style F fill:#f44336,color:#fff
    style H fill:#f44336,color:#fff
    style J fill:#f44336,color:#fff
    style L fill:#f44336,color:#fff
    style N fill:#4CAF50,color:#fff
```

La validación en ASP.NET Core se configura con `TokenValidationParameters`:

```csharp
options.TokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuer = true,
    ValidIssuer = jwtSettings["Issuer"],
    ValidateAudience = true,
    ValidAudience = jwtSettings["Audience"],
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(secretKey)),
    ValidateLifetime = true,
    ClockSkew = TimeSpan.Zero
};
```

> 💡 **Consejo:** Establecer `ClockSkew = TimeSpan.Zero` elimina la tolerancia de 5 minutos por defecto. Si el token expira a las 12:00:00, se considera expirado exactamente a las 12:00:00. Sin esto, un token expirado seguiría siendo válido durante 5 minutos más.

---

## 16.3. BCrypt: Hash de Contraseñas

### 16.3.1. Por qué no MD5 ni SHA256

Almacenar contraseñas en texto plano es un error gravísimo. Pero **¿por qué no usar MD5 o SHA256?**

| Algoritmo | Velocidad | Problema |
|-----------|-----------|----------|
| **MD5** | ~50.000 millones/segundo | Se puede probar un diccionario entero en segundos |
| **SHA256** | ~10.000 millones/segundo | Igual de rápido, igual de vulnerable |
| **BCrypt** | ~17.000/segundo | Diseñado para ser **lento** intencionadamente |

MD5 y SHA256 son algoritmos **rápidos**. Un atacante con una GPU puede probar **miles de millones** de contraseñas por segundo. BCrypt es **lento a propósito**: cada hash tarda ~100ms, lo que hace un ataque de fuerza bruta inviable prácticamente.

> 📝 **Nota:** MD5 y SHA256 son excelentes para verificar integridad de archivos, pero **nunca** para contraseñas. La velocidad es una virtud para integridad, pero un defecto para passwords.

### 16.3.2. BCrypt en C#

El paquete `BCrypt.Net-Next` proporciona las dos operaciones fundamentales:

```csharp
using BCrypt.Net;

// Registrar usuario: hashear la contraseña
string password = "MiContraseña123!";
string hash = BCrypt.Net.BCrypt.HashPassword(password, workFactor: 11);
// Resultado: "$2y$11$N9qo8uLOickgx2ZMRZoMye..."
// El hash incluye: algoritmo + work factor + salt + hash

// Login: verificar la contraseña
bool isValid = BCrypt.Net.BCrypt.Verify(password, hash);
// true si coincide, false si no
```

```mermaid
flowchart LR
    subgraph "Registro (Signup)"
        R1["Contraseña: Test1234"] --> R2["BCrypt.HashPassword(wf:11)"]
        R2 --> R3["$2y$11$LJ3m4..."]
        R3 --> R4["Guardar en BD"]
    end

    subgraph "Login"
        L1["Contraseña: Test1234"] --> L2["Leer hash de BD"]
        L2 --> L3["BCrypt.Verify(password, hash)"]
        L3 --> L4{"¿Coincide?"}
        L4 -->|Sí| L5["Login OK"]
        L4 -->|No| L6["Credenciales inválidas"]
    end

    style R1 fill:#4CAF50,color:#fff
    style R2 fill:#4CAF50,color:#fff
    style R3 fill:#4CAF50,color:#fff
    style R4 fill:#4CAF50,color:#fff
    style L1 fill:#2196F3,color:#fff
    style L2 fill:#2196F3,color:#fff
    style L3 fill:#2196F3,color:#fff
    style L4 fill:#FF9800,color:#fff
    style L5 fill:#4CAF50,color:#fff
    style L6 fill:#f44336,color:#fff
```

> 💡 **Analogía:** BCrypt es como un molino de café. Cuando registras una contraseña, la pasas por el molino 21 veces (work factor 21) para obtener el polvo (hash). Verificar es pasar el café por el mismo molino y comparar. Un atacante tendría que moler cada contraseña 21 veces para intentar adivinarla.

### 16.3.3. Work Factor

El **work factor** determina cuántas iteraciones se realizan. Cada incremento **duplica** el tiempo de cálculo:

| Work Factor | Iteraciones | Tiempo aprox. | Uso recomendado |
|-------------|-------------|---------------|-----------------|
| 8 | 256 | ~10ms | Desarrollo/pruebas |
| 10 | 1.024 | ~40ms | Testing |
| **11** | **2.048** | **~100ms** | **Producción (recomendado)** |
| 12 | 4.096 | ~200ms | Alta seguridad |

```csharp
// Producción: work factor 11 (~100ms por hash)
string hash = BCrypt.Net.BCrypt.HashPassword(password, workFactor: 11);
```

> 💡 **Consejo:** Un work factor de **11** es el punto óptimo: lo suficientemente lento para impedir fuerza bruta (~100ms), pero lo suficientemente rápido para no afectar la experiencia de usuario en login.

### 16.3.4. Comparativa de Algoritmos

| Algoritmo | Velocidad hash | Resistencia rainbow tables | Salt automático | Recomendado |
|-----------|----------------|---------------------------|-----------------|-------------|
| **MD5** | Muy rápido | Baja | No | ❌ Nunca |
| **SHA256** | Rápido | Media | No | ❌ Nunca para passwords |
| **PBKDF2** | Lento (configurable) | Buena | Sí | ✅ Aceptable |
| **BCrypt** | Lento (work factor) | Excelente | Sí | ✅ Recomendado |
| **Argon2** | Muy lento | Excelente | Sí | ✅ El mejor (ganador Password Hashing Competition) |

> 📝 **Nota:** BCrypt genera un salt único automáticamente en cada hash. No necesitas almacenar el salt por separado: está incrustado en el propio hash. Esto protege contra ataques de rainbow tables, donde un atacante usa tablas precalculadas de hashes comunes.

---

## 16.4. Enfoque Manual (Estilo Tienda)

Este enfoque implementa autenticación JWT completa **sin usar ASP.NET Core Identity**. Es más ligero, más flexible y ideal para APIs REST.

### 16.4.1. Modelo de Usuario

```csharp
using System.ComponentModel.DataAnnotations;

namespace FunkoApi.Models;

/// <summary>
/// Usuario del sistema con autenticación JWT.
/// </summary>
public class User
{
    public long Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>Rol del usuario: "USER" o "ADMIN"</summary>
    [MaxLength(50)]
    public string Role { get; set; } = "USER";

    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiry { get; set; }
}

/// <summary>
/// Constantes para los roles de usuario.
/// </summary>
public static class UserRoles
{
    public const string ADMIN = "ADMIN";
    public const string USER = "USER";
}
```

### 16.4.2. JwtService

```csharp
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace FunkoApi.Services;

/// <summary>
/// Servicio para generación y validación de JWT tokens.
/// </summary>
public class JwtService(IConfiguration configuration)
{
    private readonly string _secretKey = configuration["Jwt:Secret"]
        ?? throw new InvalidOperationException("JWT Secret no configurado");
    private readonly string _issuer = configuration["Jwt:Issuer"] ?? "FunkoApi";
    private readonly string _audience = configuration["Jwt:Audience"] ?? "FunkoApiClients";
    private readonly int _accessTokenExpiryMinutes = configuration.GetValue<int>("Jwt:ExpirationMinutes", 15);
    private readonly int _refreshTokenExpiryDays = configuration.GetValue<int>("Jwt:RefreshTokenExpiryDays", 7);

    /// <summary>
    /// Genera un JWT access token para el usuario dado.
    /// </summary>
    public string GenerateToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64),
            new("username", user.Username),
            new("role", user.Role)
        };

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_accessTokenExpiryMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Genera un refresh token aleatorio de 32 bytes.
    /// </summary>
    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber)
            .Replace("/", "_")
            .Replace("+", "-");
    }

    /// <summary>
    /// Valida un JWT token y devuelve el ClaimsPrincipal o null.
    /// </summary>
    public ClaimsPrincipal? ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));

            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out _);

            return principal;
        }
        catch
        {
            return null;
        }
    }
}
```

### 16.4.3. AuthService

```csharp
using BCrypt.Net;

namespace FunkoApi.Services;

/// <summary>
/// DTO para respuesta de autenticación con tokens.
/// </summary>
public record AuthResponse(
    string AccessToken,
    string RefreshToken,
    string TokenType,
    int ExpiresIn,
    long UserId,
    string Username,
    string Role);

/// <summary>
/// DTO para solicitud de login.
/// </summary>
public record LoginRequest(string Email, string Password);

/// <summary>
/// DTO para solicitud de registro.
/// </summary>
public record SignupRequest(string Username, string Email, string Password);

/// <summary>
/// Servicio de autenticación con BCrypt y JWT.
/// </summary>
public class AuthService(
    IUserRepository userRepository,
    JwtService jwtService,
    ILogger<AuthService> logger)
{
    /// <summary>
    /// Registra un usuario nuevo con contraseña hasheada con BCrypt.
    /// </summary>
    public async Task<AuthResponse> SignupAsync(SignupRequest request)
    {
        var existing = await userRepository.FindByEmailAsync(request.Email);
        if (existing != null)
        {
            throw new InvalidOperationException("El email ya está registrado");
        }

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password, workFactor: 11),
            Role = UserRoles.USER,
            CreatedAt = DateTime.UtcNow
        };

        user = await userRepository.CreateAsync(user);

        logger.LogInformation("Usuario registrado: {UserId} ({Username})", user.Id, user.Username);

        return new AuthResponse(
            jwtService.GenerateToken(user),
            jwtService.GenerateRefreshToken(),
            "Bearer",
            15 * 60,
            user.Id,
            user.Username,
            user.Role);
    }

    /// <summary>
    /// Autentica un usuario con email y contraseña.
    /// </summary>
    public async Task<AuthResponse> LoginAsync(string email, string password)
    {
        var user = await userRepository.FindByEmailAsync(email);
        if (user == null || user.IsDeleted)
        {
            throw new UnauthorizedAccessException("Credenciales inválidas");
        }

        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            logger.LogWarning("Intento de login fallido para: {Email}", email);
            throw new UnauthorizedAccessException("Credenciales inválidas");
        }

        user.LastLoginAt = DateTime.UtcNow;
        await userRepository.UpdateAsync(user);

        logger.LogInformation("Login exitoso: {UserId} ({Username})", user.Id, user.Username);

        return new AuthResponse(
            jwtService.GenerateToken(user),
            jwtService.GenerateRefreshToken(),
            "Bearer",
            15 * 60,
            user.Id,
            user.Username,
            user.Role);
    }
}
```

### 16.4.4. Configuración en DI (AuthenticationConfig.cs)

```csharp
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace FunkoApi.Infrastructure;

/// <summary>
/// Configuración de autenticación y autorización estilo Tienda.
/// Extension method para registrar servicios de autenticación JWT.
/// </summary>
public static class AuthenticationConfig
{
    public static IServiceCollection AddAuthenticationJwt(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtSection = configuration.GetSection("Jwt");
        var secretKey = jwtSection["Secret"]
            ?? throw new InvalidOperationException("JWT Secret no configurado");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtSection["Issuer"],
                ValidateAudience = true,
                ValidAudience = jwtSection["Audience"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(secretKey)),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            options.Events = new JwtBearerEvents
            {
                OnTokenValidated = context =>
                {
                    var logger = context.HttpContext.RequestServices
                        .GetRequiredService<ILogger<Program>>();
                    logger.LogDebug("Token validado para: {User}",
                        context.Principal?.Identity?.Name);
                    return Task.CompletedTask;
                },
                OnAuthenticationFailed = context =>
                {
                    var logger = context.HttpContext.RequestServices
                        .GetRequiredService<ILogger<Program>>();
                    logger.LogWarning("Fallo de autenticación: {Message}",
                        context.Exception.Message);
                    return Task.CompletedTask;
                }
            };
        });

        services.AddAuthorizationBuilder()
            .AddPolicy("AdminOnly", policy =>
                policy.RequireRole(UserRoles.ADMIN))
            .AddPolicy("UserOrAdmin", policy =>
                policy.RequireRole(UserRoles.USER, UserRoles.ADMIN));

        services.AddScoped<JwtService>();
        services.AddScoped<AuthService>();

        return services;
    }
}
```

### 16.4.5. AuthController

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FunkoApi.Controllers;

/// <summary>
/// Controller de autenticación: login, signup y perfil de usuario.
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthController(AuthService authService) : ControllerBase
{
    /// <summary>
    /// Registra un usuario nuevo.
    /// </summary>
    [HttpPost("signup")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Signup([FromBody] SignupRequest request)
    {
        try
        {
            var result = await authService.SignupAsync(request);
            return CreatedAtAction(nameof(GetCurrentUser), new { }, result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ProblemDetails
            {
                Title = "Conflicto",
                Detail = ex.Message,
                Status = StatusCodes.Status409Conflict
            });
        }
    }

    /// <summary>
    /// Inicia sesión y devuelve un JWT token.
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var result = await authService.LoginAsync(request.Email, request.Password);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new ProblemDetails
            {
                Title = "No autorizado",
                Detail = ex.Message,
                Status = StatusCodes.Status401Unauthorized
            });
        }
    }

    /// <summary>
    /// Devuelve el usuario autenticado actual.
    /// </summary>
    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult GetCurrentUser()
    {
        return Ok(new
        {
            Id = User.FindFirst("sub")?.Value,
            Username = User.FindFirst("username")?.Value,
            Email = User.FindFirst("email")?.Value,
            Role = User.FindFirst("role")?.Value
        });
    }
}
```

### 16.4.6. Program.cs

```csharp
using FunkoApi.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Autenticación JWT (ANTES de Authorization)
builder.Services.AddAuthenticationJwt(builder.Configuration);

// Repositorios y servicios
builder.Services.AddScoped<IUserRepository, UserRepository>();

var app = builder.Build();

// Middleware (EL ORDEN IMPORTA)
app.UseAuthentication();  // Primero: ¿quién eres?
app.UseAuthorization();   // Segundo: ¿qué puedes hacer?

app.MapControllers();

app.Run();
```

> ⚠️ **Advertencia:** El orden de `UseAuthentication()` y `UseAuthorization()` es **crítico**. Si los inviertes, la autorización no funcionará porque no habrá identidad que verificar. **Siempre** autenticación primero, autorización segundo.

---

## 16.5. Enfoque Identity

### 16.5.1. Instalación

```bash
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet aspnet-codegenerator identity -dc AppDbContext
```

### 16.5.2. Configuración

```csharp
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FunkoApi.Entity;

/// <summary>
/// Usuario personalizado que hereda de IdentityUser.
/// </summary>
public class User : IdentityUser<long>
{
    [PersonalData]
    public string? FirstName { get; set; }

    [PersonalData]
    public string? LastName { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Rol personalizado con descripción.
/// </summary>
public class Role : IdentityRole<long>
{
    public string? Description { get; set; }
}

/// <summary>
/// DbContext que integra Identity con el modelo de la aplicación.
/// </summary>
public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<User, Role, long>(options)
{
    public DbSet<Funko> Funkos { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Mapeo de tablas de Identity
        modelBuilder.Entity<User>().ToTable("users");
        modelBuilder.Entity<Role>().ToTable("roles");
        modelBuilder.Entity<IdentityUserClaim<long>>().ToTable("user_claims");
        modelBuilder.Entity<IdentityUserRole<long>>().ToTable("user_roles");
        modelBuilder.Entity<IdentityUserLogin<long>>().ToTable("user_logins");
        modelBuilder.Entity<IdentityUserToken<long>>().ToTable("user_tokens");
        modelBuilder.Entity<IdentityRoleClaim<long>>().ToTable("role_claims");
    }
}
```

**Controller con UserManager:**

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FunkoApi.Controllers;

/// <summary>
/// Controller de autenticación usando ASP.NET Core Identity.
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthIdentityController(
    UserManager<User> userManager,
    SignInManager<User> signInManager) : ControllerBase
{
    /// <summary>
    /// Registra un usuario nuevo con Identity.
    /// </summary>
    [HttpPost("signup")]
    public async Task<IActionResult> Signup([FromBody] SignupRequest request)
    {
        var user = new User
        {
            UserName = request.Username,
            Email = request.Email,
            FirstName = request.Username
        };

        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Error de registro",
                Detail = string.Join(", ", result.Errors.Select(e => e.Description)),
                Status = StatusCodes.Status400BadRequest
            });
        }

        await userManager.AddToRoleAsync(user, UserRoles.USER);

        return CreatedAtAction(nameof(GetCurrentUser), new { }, new
        {
            user.Id,
            user.UserName,
            user.Email
        });
    }

    /// <summary>
    /// Inicia sesión y genera un JWT token.
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return Unauthorized(new ProblemDetails
            {
                Title = "Credenciales inválidas",
                Status = StatusCodes.Status401Unauthorized
            });
        }

        var result = await signInManager.CheckPasswordSignInAsync(
            user, request.Password, lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            return Unauthorized(new ProblemDetails
            {
                Title = "Credenciales inválidas",
                Status = StatusCodes.Status401Unauthorized
            });
        }

        // Generar JWT (usando JwtSecurityTokenHandler directamente)
        var roles = await userManager.GetRolesAsync(user);
        var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        // ... generación del token ...

        return Ok(new { Token = "jwt-token-aqui", UserId = user.Id });
    }

    /// <summary>
    /// Devuelve el usuario autenticado actual.
    /// </summary>
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        var roles = await userManager.GetRolesAsync(user);

        return Ok(new
        {
            user.Id,
            user.UserName,
            user.Email,
            user.FirstName,
            user.LastName,
            Roles = roles,
            user.CreatedAt,
            user.LastLoginAt
        });
    }
}
```

**Configuración en Program.cs:**

```csharp
using Microsoft.AspNetCore.Identity;
using FunkoApi.Entity;

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddIdentity<User, Role>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;
    options.User.RequireUniqueEmail = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();
```

### 16.5.3. Ventajas y Desventajas

| Aspecto | Manual (JWT + BCrypt) | Identity |
|---------|----------------------|----------|
| **Tablas BD** | 1 tabla (`users`) | 7+ tablas (`AspNetUsers`, `AspNetRoles`, etc.) |
| **Hash de contraseñas** | BCrypt (más lento = más seguro) | PBKDF2 (estándar .NET) |
| **External Logins** | Requiere implementación manual | Integrado (Google, Facebook, etc.) |
| **2FA** | Requiere implementación manual | Integrado |
| **Confirmación email** | Requiere implementación manual | Integrado |
| **Lockout por intentos** | Requiere implementación manual | Integrado |
| **Flexibilidad** | Total control | Limitado por el framework |
| **Código** | ~200 líneas | ~50 líneas (scaffolding) |
| **Curva aprendizaje** | Baja | Media-Alta |
| **Mantenimiento** | Lo mantienes tú | Microsoft mantiene |

> 💡 **Consejo:** Para APIs REST simples donde solo necesitas login con email/password, el enfoque **manual** es más ligero y fácil de entender. Usa **Identity** cuando necesites features avanzadas como 2FA, external logins o confirmación de email sin implementarlas desde cero.

---

## 16.6. Comparación de Enfoques

```mermaid
flowchart TD
    A["¿Qué tipo de aplicación?"] --> B{¿Necesitas external logins?}
    B -->|Sí (Google, Facebook)| C["Identity"]
    B -->|No| D{¿Necesitas 2FA?}
    D -->|Sí| C
    D -->|No| E{¿Tienes UI de login?}
    E -->|Sí (Razor, Blazor)| C
    E -->|No (solo API REST)| F["Manual + BCrypt"]
    C --> G["ASP.NET Core Identity"]
    F --> H["JwtService + AuthService"]
    G --> I["7+ tablas, features completas"]
    H --> J["1 tabla, control total"]

    style C fill:#FF9800,color:#fff
    style F fill:#4CAF50,color:#fff
    style G fill:#FF9800,color:#fff
    style H fill:#4CAF50,color:#fff
    style I fill:#E65100,color:#fff
    style J fill:#1B5E20,color:#fff
```

| Escenario | Recomendación | Razón |
|-----------|---------------|-------|
| **API REST simple** | Manual ✅ | Ligero, control total, 1 tabla |
| **SPA + API Backend** | Manual ✅ | JWT es natural para SPAs |
| **Razor Pages** | Identity ✅ | Cookies + UI login integrada |
| **Blazor Server** | Identity ✅ | Integración completa con .NET |
| **Google/Facebook Login** | Identity ✅ | External logins incluidos |
| **2FA obligatorio** | Identity ✅ | Integrado sin código extra |
| **Microservicios** | Manual ✅ | Mínima sobrecarga por servicio |
| **Auditoría de seguridad** | Identity ✅ | Logs y lockout integrados |

> 📝 **Nota:** Ambos enfoques usan el **mismo middleware** de autenticación/autorización de ASP.NET Core. Los atributos `[Authorize]`, `[Authorize(Roles="Admin")]` y la inyección de `HttpContext.User` funcionan exactamente igual. Solo cambia cómo se gestiona el usuario y se genera el token.

---

## 16.7. Buenas Prácticas

| Práctica | Descripción |
|----------|-------------|
| **HTTPS siempre** | Nunca enviar tokens por HTTP en producción |
| **Secret >= 32 caracteres** | La clave JWT debe ser larga y aleatoria |
| **Expiration corto (15-30 min)** | Access tokens con vida breve para minimizar riesgo |
| **ClockSkew = 0** | Expiración exacta sin tolerancia de 5 minutos |
| **BCrypt workFactor: 11** | Punto óptimo: ~100ms por hash |
| **No localStorage para JWT** | Usar httpOnly cookies o memoria del navegador |
| **Rate limiting en login** | Limitar intentos para evitar fuerza bruta |
| **Refresh tokens** | Renovar access tokens sin re-login |
| **No logear passwords** | Nunca incluir contraseñas en logs |
| **Seed de usuarios** | Crear usuario admin y de prueba en desarrollo |

> ⚠️ **Advertencia:** **Nunca** almacenes JWT en `localStorage` del navegador. Si un atacante logra inyectar JavaScript (XSS), puede robar el token. Usa **httpOnly cookies** o almacén en memoria del SPA. El `localStorage` es accesible desde cualquier script en la página.

> 💡 **Consejo:** Para el seed de usuarios en desarrollo, crea un servicio `SeedService` que se ejecute al iniciar la aplicación:

```csharp
public static class SeedService
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var repository = scope.ServiceProvider
            .GetRequiredService<IUserRepository>();

        var existing = await repository.FindByEmailAsync("admin@funko.com");
        if (existing == null)
        {
            await repository.CreateAsync(new User
            {
                Username = "admin",
                Email = "admin@funko.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123", workFactor: 11),
                Role = UserRoles.ADMIN
            });

            await repository.CreateAsync(new User
            {
                Username = "user",
                Email = "user@funko.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("user123", workFactor: 11),
                Role = UserRoles.USER
            });
        }
    }
}
```

---

## 16.8. Reto

> Implementa autenticación completa en FunkoApp con ambos enfoques.

**Añade a tu API:**

1. Modelo `User` con `PasswordHash` (BCrypt) — tabla `users` en tu BD
2. **Enfoque Manual:** `JwtService` + `AuthService` + `AuthController` con endpoints `POST /auth/login`, `POST /auth/signup`, `GET /auth/me`
3. **Enfoque Identity:** `IdentityDbContext` + `UserManager<User>` + `SignInManager<User>` + configuración de roles
4. Endpoints: `POST /auth/login`, `POST /auth/signup`, `GET /auth/me`
5. Configuración JWT en `appsettings.json` (`Jwt:Secret`, `Jwt:Issuer`, `Jwt:Audience`, `Jwt:ExpirationMinutes`)
6. Seed de usuarios: admin (`admin@funko.com` / `admin123`) y user (`user@funko.com` / `user123`)
7. Tests: login exitoso, login con credenciales incorrectas, token expirado, acceso sin token

**Puntos extra:**

- Refresh tokens: endpoint `POST /auth/refresh` que renueva el access token
- Rate limiting en login (máximo 5 intentos por minuto por IP)
- Validación de fortaleza de contraseña (mínimo 8 caracteres, mayúscula, minúscula, número)

---

## 16.9. Resumen

| Concepto | Descripción |
|----------|-------------|
| **Autenticación** | Verificar la identidad del usuario (¿quién eres?) |
| **Stateless** | Cada request contiene toda la información de autenticación |
| **JWT** | Token autocontenido con claims firmados criptográficamente |
| **Header** | Algoritmo de firma (HS256) y tipo (JWT) |
| **Payload** | Claims: sub, exp, iat, role, custom claims |
| **Signature** | Firma HMAC-SHA256 con clave secreta |
| **BCrypt** | Hash lento y seguro para contraseñas |
| **Work Factor** | Nivel de dificultad BCrypt (11 recomendado para producción) |
| **Enfoque Manual** | JwtService + AuthService, 1 tabla, control total |
| **Enfoque Identity** | ASP.NET Core Identity, 7+ tablas, features completas |
| **Access Token** | Token corto (15-30 min) para acceso a APIs |
| **Refresh Token** | Token largo para renovar access tokens |
| **TokenValidationParameters** | Configuración de validación en JwtBearer |
| **ClockSkew** | Tolerancia de tiempo en validación (0 = expiración exacta) |

**¿Qué viene después?**

En el siguiente punto veremos **Autorización**: una vez que sabemos QUIÉN es el usuario (autenticación), veremos QUÉ puede hacer. Roles, Claims, Políticas de autorización y cómo proteger endpoints con `[Authorize]`.
