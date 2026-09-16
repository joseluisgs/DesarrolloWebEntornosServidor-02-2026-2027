# 25. Configuracion de Entornos en ASP.NET Core

## Indice

- [25.1. Introduccion a los Entornos](#251-introduccion-a-los-entornos)
  - [25.1.1. Que son los Entornos](#2511-que-son-los-entornos)
  - [25.1.2. Entornos Predefinidos](#2512-entornos-predefinidos)
  - [25.1.3. Prioridad de Configuracion](#2513-prioridad-de-configuracion)
- [25.2. Configuracion por Entorno](#252-configuracion-por-entorno)
  - [25.2.1. Estructura de Archivos](#2521-estructura-de-archivos)
  - [25.2.2. Configuracion Base](#2522-configuracion-base)
  - [25.2.3. Configuracion de Desarrollo](#2523-configuracion-de-desarrollo)
  - [25.2.4. Configuracion de Produccion](#2524-configuracion-de-produccion)
- [25.3. Configuracion de Base de Datos por Entorno](#253-configuracion-de-base-de-datos-por-entorno)
  - [25.3.1. Development (SQLite o LocalDB)](#2531-development-sqlite-o-localdb)
  - [25.3.2. Production (SQL Server)](#2532-production-sql-server)
- [25.4. Variables de Entorno](#254-variables-de-entorno)
  - [25.4.1. Configurar en launchSettings.json](#2541-configurar-en-launchsettingsjson)
  - [25.4.2. Configurar en Sistema Operativo](#2542-configurar-en-sistema-operativo)
  - [25.4.3. Configurar en Docker](#2543-configurar-en-docker)
- [25.5. Uso de IWebHostEnvironment](#255-uso-de-iwebhostenvironment)
  - [25.5.1. En Program.cs](#2551-en-programcs)
  - [25.5.2. En Controladores](#2552-en-controladores)
  - [25.5.3. En Servicios](#2553-en-servicios)
- [25.6. Configuracion de Swagger por Entorno](#256-configuracion-de-swagger-por-entorno)
- [25.7. Configuracion de Logging por Entorno](#257-configuracion-de-logging-por-entorno)
- [25.8. Secretos de Usuario (User Secrets)](#258-secretos-de-usuario-user-secrets)
  - [25.8.1. Inicializar y Usar User Secrets](#2581-inicializar-y-usar-user-secrets)
- [25.9. Configuracion Avanzada](#259-configuracion-avanzada)
  - [25.9.1. Clases de Configuracion Tipadas](#2591-clases-de-configuracion-tipadas)
  - [25.9.2. Validacion de Configuracion](#2592-validacion-de-configuracion)
- [25.10. Azure App Configuration](#2510-azure-app-configuration)
- [25.11. Buenas Practicas](#2511-buenas-practicas)
- [25.12. Resumen](#2512-resumen)
- [25.13. Ejercicio Propuesto](#2513-ejercicio-propuesto)

---

## 25.1. Introduccion a los Entornos

### 25.1.1. Que son los Entornos

Los **entornos** (environments) en ASP.NET Core permiten configurar la aplicación de manera diferente según donde se ejecute. Esto es esencial para mantener la seguridad en producción mientras se facilita el desarrollo.

```mermaid
flowchart LR
    subgraph "Desarrollo"
        A1[Desarrollador] --> A2[Localhost]
        A2 --> A3[Debugging activo]
        A3 --> A4[Logs detallados]
    end
    
    subgraph "Produccion"
        B1[Usuarios] --> B2[Servidor]
        B2 --> B3[Optimizado]
        B3 --> B4[Logs minimos]
    end
    
    style A2 fill:#2E7D32
    style B2 fill:#B71C1C
```

**Beneficios de usar entornos:**

| Beneficio | Descripción |
|-----------|-------------|
| **Configuración específica** | Cada entorno tiene su propia configuración |
| **Base de datos diferente** | Desarrollo y producción nunca comparten BD |
| **Logging adaptado** | Detallado en dev, mínimo en prod |
| **Swagger controlado** | Solo disponible en desarrollo |
| **Seguridad** | Secretos protegidos en producción |

🧠 **Analogía**: Piensa en los entornos como diferentes modos de un videojuego. En "modo desarrollo" tienes opciones de debug, god mode y puedes ver todo. En "modo producción" el juego está optimizado, sin cheats y listo para los jugadores.

### 25.1.2. Entornos Predefinidos

ASP.NET Core tiene tres entornos predefinidos:

| Entorno | Descripción | Uso Típico |
|---------|-------------|------------|
| **Development** | Entorno de desarrollo local | Debug, Swagger, logs detallados |
| **Staging** | Pre-producción | Pruebas finales, validación antes de producción |
| **Production** | Producción real | Configuración optimizada, segura y de alto rendimiento |

**Establecer el entorno:**

```bash
# Windows
set ASPNETCORE_ENVIRONMENT=Development

# Linux/Mac
export ASPNETCORE_ENVIRONMENT=Development

# En launchSettings.json (Visual Studio)
"environmentVariables": {
    "ASPNETCORE_ENVIRONMENT": "Development"
}
```

### 25.1.3. Prioridad de Configuracion

```mermaid
flowchart TD
    A["appsettings.json"] --> B["appsettings.{Environment}.json"]
    B --> C[Variables de Entorno]
    C --> D[User Secrets (solo Development)]
    D --> E[Argumentos de Linea de Comandos]
    
    style A fill:#1565C0
    style B fill:#1565C0
    style C fill:#1565C0
    style D fill:#B71C1C
    style E fill:#1565C0
```

**Orden de prioridad (mayor a menor):**

1. Argumentos de linea de comandos
2. Variables de entorno
3. User Secrets (solo Development)
4. `appsettings.{Environment}.json`
5. `appsettings.json`

📝 **Nota del Profesor**: Los valores que aparecen mas abajo en la lista sobrescriben a los que aparecen arriba. Por ejemplo, una variable de entorno sobrescribe el mismo valor en appsettings.json.

---

## 25.2. Configuracion por Entorno

### 25.2.1. Estructura de Archivos

```
FunkosApi/
├── appsettings.json                      # Configuracion base (comun)
├── appsettings.Development.json          # Desarrollo (sobrescribe base)
├── appsettings.Staging.json              # Staging (opcional)
├── appsettings.Production.json           # Produccion (sobrescribe base)
├── Program.cs
└── FunkosApi.csproj
```

### 25.2.2. Configuracion Base

**appsettings.json:**

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ApplicationName": "Funkos API",
  "Jwt": {
    "Issuer": "https://localhost:5001",
    "Audience": "https://localhost:5001",
    "ExpirationInMinutes": 60
  },
  "Pagination": {
    "DefaultPageSize": 10,
    "MaxPageSize": 100
  }
}
```

### 25.2.3. Configuracion de Desarrollo

**appsettings.Development.json:**

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Information",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=funkos_dev.db",
    "MongoConnection": "mongodb://localhost:27017/FunkosDb_Dev"
  },
  "Jwt": {
    "Secret": "clave-secreta-desarrollo-muy-larga-y-segura-de-al-menos-32-caracteres"
  },
  "EnableSwagger": true,
  "EnableDetailedErrors": true,
  "Cors": {
    "AllowedOrigins": ["http://localhost:3000", "http://localhost:4200"]
  }
}
```

⚠️ **Advertencia**: Nunca uses claves reales o contrasenas de produccion en appsettings.Development.json. Usa User Secrets para datos sensibles.

### 25.2.4. Configuracion de Produccion

**appsettings.Production.json:**

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Error",
      "Microsoft.EntityFrameworkCore": "Error"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=prod-server.database.windows.net;Database=FunkosDb;User Id=admin;Password=${DB_PASSWORD};",
    "MongoConnection": "mongodb+srv://user:${MONGO_PASS}@cluster.mongodb.net/FunkosDb"
  },
  "Jwt": {
    "Secret": "${JWT_SECRET}"
  },
  "EnableSwagger": false,
  "EnableDetailedErrors": false,
  "Cors": {
    "AllowedOrigins": ["https://miaplicacion.com"]
  }
}
```

💡 **Tip del Examinador**: Usa la sintaxis `${VARIABLE}` para referenciar variables de entorno en JSON. Esto es mas seguro que hardcodear valores sensibles.

---

## 25.3. Configuracion de Base de Datos por Entorno

### 25.3.1. Development (SQLite o LocalDB)

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    // SQLite para desarrollo local
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
               .EnableSensitiveDataLogging()
               .EnableDetailedErrors()
    );
}

var app = builder.Build();
```

**Ventajas de SQLite en desarrollo:**

| Ventaja | Descripcion |
|---------|-------------|
| No requiere instalacion | Listo para usar inmediatamente |
| Base de datos en archivo | Facil de compartir y respaldar |
| Rapido para desarrollo local | Ideal para testing rapido |
| Facilidad para reiniciar | Datos de prueba siempre disponibles |

### 25.3.2. Production (SQL Server)

```csharp
if (builder.Environment.IsProduction())
{
    // SQL Server para produccion
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
            sqlServerOptions =>
            {
                sqlServerOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
            })
    );
}
```

**Configuracion en appsettings.Production.json:**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=prod-server;Database=FunkosDb;User Id=admin;Password=${DB_PASSWORD};"
  }
}
```

🧠 **Analogia**: En desarrollo usas un coche de practicas (SQLite) facil de manejar y resetear. En produccion usas el coche de carrera (SQL Server) optimizado para velocidad y fiabilidad.

---

## 25.4. Variables de Entorno

### 25.4.1. Configurar en launchSettings.json

```json
{
  "profiles": {
    "Development": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "launchUrl": "swagger",
      "applicationUrl": "https://localhost:5001;http://localhost:5000",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development",
        "JWT_SECRET": "clave-desarrollo-12345",
        "DB_PASSWORD": "dev-password-123"
      }
    },
    "Production": {
      "commandName": "Project",
      "launchBrowser": false,
      "applicationUrl": "https://localhost:5001",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Production"
      }
    }
  }
}
```

### 25.4.2. Configurar en Sistema Operativo

**Windows (CMD):**
```cmd
setx ASPNETCORE_ENVIRONMENT "Production"
setx JWT_SECRET "mi-clave-secreta-produccion"
```

**Windows (PowerShell):**
```powershell
$env:ASPNETCORE_ENVIRONMENT = "Production"
$env:JWT_SECRET = "mi-clave-secreta"
```

**Linux/Mac:**
```bash
export ASPNETCORE_ENVIRONMENT=Production
export JWT_SECRET=mi-clave-secreta-produccion
```

### 25.4.3. Configurar en Docker

**Dockerfile:**
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

ENV ASPNETCORE_ENVIRONMENT=Production
ENV JWT_SECRET=${JWT_SECRET}

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["FunkosApi/FunkosApi.csproj", "FunkosApi/"]
RUN dotnet restore "FunkosApi/FunkosApi.csproj"
COPY . .
WORKDIR "/src/FunkosApi"
RUN dotnet build "FunkosApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FunkosApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FunkosApi.dll"]
```

**docker-compose.yml:**
```yaml
version: '3.8'
services:
  funkosapi:
    image: funkosapi:latest
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - JWT_SECRET=${JWT_SECRET}
      - ConnectionStrings__DefaultConnection=Server=db;Database=FunkosDb;User=sa;Password=${DB_PASSWORD};
    ports:
      - "8080:80"
    depends_on:
      - db
  
  db:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=${DB_PASSWORD}
    ports:
      - "1433:1433"
```

---

## 25.5. Uso de IWebHostEnvironment

### 25.5.1. En Program.cs

```csharp
var builder = WebApplication.CreateBuilder(args);

// Acceder al entorno
var env = builder.Environment;

Console.WriteLine($"Entorno actual: {env.EnvironmentName}");

// Configurar segun entorno
if (env.IsDevelopment())
{
    Console.WriteLine("Modo DESARROLLO habilitado");
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();
}
else if (env.IsProduction())
{
    Console.WriteLine("Modo PRODUCCION habilitado");
}

var app = builder.Build();

// Configuracion condicional del middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
```

### 25.5.2. En Controladores

```csharp
[ApiController]
[Route("api/[controller]")]
public class DiagnosticController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;
    private readonly IConfiguration _configuration;

    public DiagnosticController(IWebHostEnvironment environment, IConfiguration configuration)
    {
        _environment = environment;
        _configuration = configuration;
    }

    [HttpGet("info")]
    public ActionResult<object> GetInfo()
    {
        return Ok(new
        {
            Environment = _environment.EnvironmentName,
            ApplicationName = _environment.ApplicationName,
            ContentRootPath = _environment.ContentRootPath,
            IsDevelopment = _environment.IsDevelopment(),
            IsProduction = _environment.IsProduction()
        });
    }

    [HttpGet("config")]
    [AllowAnonymous]
    public ActionResult<object> GetConfig()
    {
        // Solo permitir en desarrollo
        if (!_environment.IsDevelopment())
        {
            return Forbid();
        }

        return Ok(new
        {
            EnableSwagger = _configuration.GetValue<bool>("EnableSwagger"),
            AllowedHosts = _configuration["AllowedHosts"]
        });
    }
}
```

### 25.5.3. En Servicios

```csharp
public class EmailService : IEmailService
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IWebHostEnvironment environment, ILogger<EmailService> logger)
    {
        _environment = environment;
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        if (_environment.IsDevelopment())
        {
            // En desarrollo, solo loguear (no enviar emails reales)
            _logger.LogInformation("[DEV] Email simulado a {To}: {Subject}", to, subject);
            _logger.LogDebug("[DEV] Body: {Body}", body);
            await Task.CompletedTask;
        }
        else
        {
            // En produccion, enviar email real
            _logger.LogInformation("[PROD] Enviando email a {To}", to);
            // await _emailClient.SendAsync(to, subject, body);
        }
    }
}
```

---

## 25.6. Configuracion de Swagger por Entorno

```csharp
var builder = WebApplication.CreateBuilder(args);

// Configurar Swagger solo en Development
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Funkos API - Development",
            Version = "v1",
            Description = "API en modo desarrollo con todas las herramientas habilitadas"
        });
    });
}

var app = builder.Build();

// Habilitar Swagger solo en Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Funkos API V1");
        options.RoutePrefix = "swagger";
    });
}
```

⚠️ **Advertencia**: Nunca expongas Swagger en produccion. Swagger revela la estructura completa de tu API, lo cual es un riesgo de seguridad.

---

## 25.7. Configuracion de Logging por Entorno

**appsettings.Development.json:**
```json
{
  "Logging": {
    "      "Default":LogLevel": {
 "Debug",
      "Microsoft": "Information",
      "Microsoft.Hosting.Lifetime": "Information",
      "Microsoft.EntityFrameworkCore": "Information"
    },
    "Console": {
      "IncludeScopes": true,
      "TimestampFormat": "HH:mm:ss "
    }
  }
}
```

**appsettings.Production.json:**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft": "Error",
      "Microsoft.Hosting.Lifetime": "Information"
    },
    "ApplicationInsights": {
      "LogLevel": {
        "Default": "Information"
      }
    }
  }
}
```

💡 **Tip del Examinador**: En produccion, usa Application Insights o un sistema de logging centralizado para monitorear tu aplicacion.

---

## 25.8. Secretos de Usuario (User Secrets)

### 25.8.1. Inicializar y Usar User Secrets

**Inicializar User Secrets:**
```bash
dotnet user-secrets init
```

Esto agrega al .csproj:
```xml
PropertyGroup>
  <UserSecretsId>aspnet-FunkosApi-12345</UserSecretsId>
</PropertyGroup>
```

**Agregar secretos:**
```bash
dotnet user-secrets set "Jwt:Secret" "mi-clave-secreta-desarrollo"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=.;Database=FunkosDb;Trusted_Connection=true"
```

**Usar en Program.cs:**
```csharp
var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

var jwtSecret = builder.Configuration["Jwt:Secret"];
```

📝 **Nota del Profesor**: User Secrets solo funciona en desarrollo y los secretos se almacenan en un archivo JSON separado en el perfil del usuario.

---

## 25.9. Configuracion Avanzada

### 25.9.1. Clases de Configuracion Tipadas

```csharp
// Clase de configuracion tipada
public class JwtSettings
{
    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpirationInMinutes { get; set; } = 60;
}

// Registrar configuracion
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("Jwt")
);

// Uso en servicios con inyeccion
public class JwtService
{
    private readonly JwtSettings _settings;

    public JwtService(IOptions<JwtSettings> settings)
    {
        _settings = settings.Value;
    }
}
```

### 25.9.2. Validacion de Configuracion

```csharp
public class JwtSettings
{
    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;

    public void Validate()
    {
        if (string.IsNullOrEmpty(Secret))
            throw new InvalidOperationException("JWT Secret no configurado");

        if (Secret.Length < 32)
            throw new InvalidOperationException("JWT Secret debe tener al menos 32 caracteres");

        if (string.IsNullOrEmpty(Issuer))
            throw new InvalidOperationError("JWT Issuer no configurado");
    }
}

// Validar al inicio de la aplicacion
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();
jwtSettings?.Validate();
```

---

## 25.10. Azure App Configuration

```bash
dotnet add package Microsoft.Extensions.Configuration.AzureAppConfiguration
```

```csharp
builder.Configuration.AddAzureAppConfiguration(options =>
{
    options.Connect(builder.Configuration["ConnectionStrings:AppConfig"])
           .Select(KeyFilter.Any, LabelFilter.Null)
           .Select(KeyFilter.Any, builder.Environment.EnvironmentName);
});
```

🧠 **Analogia**: Azure App Configuration es como un "almacen centralizado" donde todas tus aplicaciones pueden venir a buscar su configuracion. Si cambias algo en el almacen, todas las aplicaciones se actualizan automaticamente.

---

## 25.11. Buenas Practicas

| Practica | Descripcion |
|----------|-------------|
| **Nunca commits secretos** | Usa User Secrets o variables de entorno, nunca guardes contrasenas en el codigo |
| **Separar configuracion por entorno** | Un archivo appsettings por entorno |
| **Validar configuracion** | Valida la configuracion al inicio de la aplicacion |
| **Usar variables de entorno en produccion** | No hardcodear secretos en archivos de configuracion |
| **Swagger solo en desarrollo** | Deshabilitar Swagger en produccion por seguridad |
| **Logs apropiados** | Detallados en desarrollo, minimos en produccion |
| **Base de datos diferente** | Nunca usar la misma base de datos en desarrollo y produccion |
| **Documentar configuracion** | Documentar todas las variables requeridas en README |
| **Usar IWebHostEnvironment** | Para comportamiento condicional segun el entorno |

---

## 25.12. Resumen

| Concepto | Descripcion |
|----------|-------------|
| **Entornos** | (Development, Staging, Production) permiten configurar la aplicacion de forma diferente |
| **appsettings.json** | y **appsettings.{Environment}.json** organizan la configuracion por entorno |
| **Variables de entorno** | Son la forma mas segura de configurar secretos en produccion |
| **IWebHostEnvironment** | Permite acceder al entorno actual desde cualquier parte del codigo |
| **User Secrets** | Almacenan datos sensibles de forma segura en desarrollo |
| **Swagger** | Debe estar deshabilitado en produccion por seguridad |
| **Logging** | Debe ser detallado en desarrollo y minimo en produccion |
| **Validacion de configuracion** | Previene errores de inicio |
| **Azure App Configuration** | Ofrece configuracion centralizada para multiples aplicaciones |
| **Prioridad de configuracion** | Va de menor a mayor: appsettings.json → variables de entorno → user secrets |

---

## 25.13. Ejercicio Propuesto

**Objetivo:** Configurar correctamente los entornos de desarrollo y produccion para la API de Funkos.

**Requisitos Funcionales:**

1. **Estructura de Archivos:**
   - `appsettings.json` con configuracion base
   - `appsettings.Development.json` con configuracion de desarrollo
   - `appsettings.Production.json` con configuracion de produccion

2. **Configuracion de Desarrollo:**
   - SQLite como base de datos
   - Logs en nivel Debug
   - Swagger habilitado
   - CORS permisivo para localhost
   - User Secrets para JWT secret

3. **Configuracion de Produccion:**
   - SQL Server como base de datos
   - Logs en nivel Warning
   - Swagger deshabilitado
   - CORS restrictivo (solo tu dominio)
   - Variables de entorno para secretos

4. **Comportamiento Condicional:**
   - EmailService que simula emails en desarrollo
   - Endpoint de diagnostico que solo funciona en desarrollo
   - Exception pages diferentes por entorno

5. **Docker:**
   - Dockerfile con variables de entorno
   - docker-compose con configuracion de produccion

**Requisitos No Funcionales:**

| Requisito | Verificacion |
|-----------|--------------|
| Sin secretos en el repositorio (gitignore correcto) | ✅ |
| Documentacion de variables de entorno requeridas | ✅ |
| Validacion de configuracion al inicio | ✅ |
| Tests que verifiquen comportamiento por entorno | ✅ |

**Criterios de Evaluacion:**

| Criterio | Puntos |
|----------|--------|
| Configuracion correcta de Development y Production | 2.0 |
| Base de datos diferente por entorno | 1.5 |
| Swagger solo en desarrollo | 1.0 |
| User Secrets configurados | 1.0 |
| Variables de entorno documentadas | 1.0 |
| No hay secretos en el repositorio | 1.0 |
| Aplicacion funciona en ambos entornos | 1.5 |
| Dockerfile y docker-compose correctos | 1.0 |

**Total: 10 puntos**

**Archivos de Configuracion Esperados:**

```
FunkosApi/
├── appsettings.json                    # Base
├── appsettings.Development.json        # Desarrollo
├── appsettings.Production.json         # Produccion
├── .gitignore                          # Excluye secrets
├── Dockerfile                          # Multi-stage
└── docker-compose.yml                  # Orchestracion
```

**Variables de Entorno para Produccion:**
```bash
ASPNETCORE_ENVIRONMENT=Production
JWT_SECRET=clave-secreta-muy-larga-32-caracteres-minimo
ConnectionStrings__DefaultConnection=Server=db;Database=FunkosDb;User=sa;Password=password;
DB_PASSWORD=password-seguro
```
