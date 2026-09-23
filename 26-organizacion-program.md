- [26. Organizacion de Program.cs](#26-organizacion-de-programcs)
  - [26.1. El Problema del Program.cs Monolitico](#261-el-problema-del-programcs-monolitico)
    - [26.1.1. Ejemplo de Program.cs Monolitico](#2611-ejemplo-de-programcs-monolitico)
  - [26.2. Patron de Extension Methods para Configuracion](#262-patron-de-extension-methods-para-configuracion)
    - [26.2.1. Concepto fundamental](#2621-concepto-fundamental)
    - [26.2.2. Beneficios del patron](#2622-beneficios-del-patron)
    - [26.2.3. Organizacion por modulos funcionales](#2623-organizacion-por-modulos-funcionales)
  - [26.3. Estructura de Carpetas: Infrastructures](#263-estructura-de-carpetas-infrastructures)
  - [26.4. Ejemplos de Implementacion](#264-ejemplos-de-implementacion)
    - [26.4.1. Configuracion de Base de Datos](#2641-configuracion-de-base-de-datos)
    - [26.4.2. Configuracion de Autenticacion JWT](#2642-configuracion-de-autenticacion-jwt)
  - [26.5. Program.cs Refactorizado](#265-programcs-refactorizado)
  - [26.6. Otras Formas de Estructurar el Startup](#266-otras-formas-de-estructurar-el-startup)
  - [26.7. Buenas Practicas](#267-buenas-practicas)
  - [26.8. Reto: Refactoriza el Program.cs de FunkoApp](#268-reto-refactoriza-el-programcs-de-funkoapp)



# 26. Organizacion de Program.cs

> 💡 **Punto de partida:** Cuando tu cocina tiene todos los ingredientes, utensilios y recetas en una sola habitación desordenada, cocinar es caótico. Pero si organizas: ingredientes en un área, utensilios en otra, recetas en un libro, todo fluye. Un Program.cs monolítico es como esa cocina desordenada: cuesta encontrar lo que necesitas y es fácil romper algo.

En este punto aprenderás a refactorizar un Program.cs monolitico usando extension methods y el patron Infrastructure para mantener el codigo limpio y mantenible.

**Objetivos de aprendizaje:**
- Identificar los problemas de un Program.cs monolitico
- Aplicar el patron de extension methods para configuracion
- Organizar configuraciones en carpetas Infrastructures
- Implementar configuraciones reutilizables para bases de datos, autenticacion y mas

## 26.1. El Problema del Program.cs Monolitico

Cuando una aplicacion ASP.NET Core crece, el archivo `Program.cs` puede volverse monolitico y dificil de mantener. Los problemas principales son:

- **Dificultad de navegacion**: Un archivo de 500+ lineas dificulta encontrar configuraciones
- **Acoplamiento temporal**: Todas las configuraciones en el mismo archivo, dificil reutilizar
- **Dificultad de testing**: Imposible probar una configuracion de forma aislada
- **Falta de cohesion**: Configuraciones de naturaleza completamente diferente mezcladas

### 26.1.1. Ejemplo de Program.cs Monolitico

```csharp
var builder = WebApplication.CreateBuilder(args);

// Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();
builder.Host.UseSerilog();

// Controllers
builder.Services.AddControllers();
builder.Services.AddFluentValidation();

// API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
});

// Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

// Database
var connectionString = builder.Configuration.GetConnectionString("PostgreSQL");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// MongoDB
var mongoConnection = builder.Configuration.GetConnectionString("MongoDB");
builder.Services.AddSingleton<IMongoClient>(new MongoClient(mongoConnection));

// Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")));

// Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]))
        };
    });

// Repositories
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();

// Services
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
context.Database.EnsureCreated();

app.Run();
```

📌 **Ejemplo real:** Cuando un proyecto como Spotify Backend crece, tener todo en un solo archivo hace que 5 desarrolladores trabajando en el mismo archivo se pisen constantemente. La organizacion modular evita esto.

## 26.2. Patron de Extension Methods para Configuracion

El patron de extension methods consiste en crear metodos de extension para `IServiceCollection` (servicios) y `WebApplication` (middlewares), agrupando configuraciones relacionadas en archivos separados.

### 26.2.1. Concepto fundamental

```csharp
// ❌ MALO: Configuracion dispersa en Program.cs
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")));
builder.Services.AddSingleton<IMongoClient>(new MongoClient(
    builder.Configuration.GetConnectionString("MongoDB")));

// ✅ BUENO: Extension method encapsula la configuracion
builder.Services.AddDatabases(builder.Configuration);
```

```csharp
using Microsoft.Extensions.DependencyInjection;

namespace FunkoApp.Infrastructures;

public static class DatabaseConfig
{
    public static IServiceCollection AddDatabases(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PostgreSQL");
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        var mongoConnection = configuration.GetConnectionString("MongoDB");
        services.AddSingleton<IMongoClient>(new MongoClient(mongoConnection));

        return services;
    }
}
```

### 26.2.2. Beneficios del patron

| Beneficio | Descripcion |
|-----------|-------------|
| **Lectura mejorada** | Program.cs se convierte en un indice legible |
| **Reutilizacion** | Configuraciones pueden reutilizarse en otros proyectos |
| **Testing simplificado** | Cada configuracion puede probarse de forma aislada |
| **Separacion de responsabilidades** | Cada archivo tiene una unica responsabilidad |
| **Facilidad de navegacion** | Encontrar configuracion es tan simple como abrir el archivo |

### 26.2.3. Organizacion por modulos funcionales

| Modulo | Contenido |
|--------|-----------|
| **Core** | Controladores, validacion, versionado de API |
| **API** | Swagger, CORS, middleware de excepciones |
| **Data** | Bases de datos, cache, repositorios |
| **Auth** | Autenticacion JWT, autorizacion por roles |
| **Business** | Servicios de negocio especificos |

## 26.3. Estructura de Carpetas: Infrastructures

La carpeta `Infrastructures/` es el lugar recomendado para almacenar todos los metodos de extension de configuracion.

```
FunkoApp/
├── Program.cs
├── Infrastructures/
│   ├── SerilogConfig.cs
│   ├── ControllersConfig.cs
│   ├── ApiVersioningConfig.cs
│   ├── SwaggerConfig.cs
│   ├── CorsConfig.cs
│   ├── DatabaseConfig.cs
│   ├── AuthenticationConfig.cs
│   ├── RepositoriesConfig.cs
│   ├── ServicesConfig.cs
│   └── CacheConfig.cs
```

**Convenciones de nomenclatura:**

| Sufijo | Contenido | Ejemplo |
|--------|-----------|---------|
| `*Config.cs` | Configuraciones de servicios (registro en DI) | `DatabaseConfig.cs` |
| `*Extensions.cs` | Configuraciones del pipeline de middlewares | `CorsExtensions.cs` |

## 26.4. Ejemplos de Implementacion

### 26.4.1. Configuracion de Base de Datos

```csharp
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace FunkoApp.Infrastructures;

public static class DatabaseConfig
{
    public static IServiceCollection AddDatabases(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var postgresConnection = configuration.GetConnectionString("PostgreSQL")
            ?? throw new InvalidOperationException(
                "Connection string 'PostgreSQL' no encontrada");

        services.AddDbContext<FunkoDbContext>(options =>
        {
            options.UseNpgsql(postgresConnection);
        });

        var mongoConnection = configuration.GetConnectionString("MongoDB");
        services.AddSingleton<IMongoClient>(new MongoClient(mongoConnection));

        return services;
    }
}
```

### 26.4.2. Configuracion de Autenticacion JWT

```csharp
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace FunkoApp.Infrastructures;

public static class AuthenticationConfig
{
    /// <summary>
    /// Registra la autenticación JWT (nombre propio para no colisionar
    /// con el AddAuthentication() nativo de ASP.NET Core).
    /// </summary>
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("Jwt");
        var secretKey = jwtSettings["Secret"]
            ?? throw new InvalidOperationException("JWT Secret no configurada");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = key
            };
        });

        return services;
    }
}
```

## 26.5. Program.cs Refactorizado

Despues de aplicar el patron, el Program.cs queda limpio y legible:

```csharp
using Serilog;
using FunkoApp.Infrastructures;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

var services = builder.Services;
var configuration = builder.Configuration;

// === CONFIGURACION DE SERVICIOS ===
services.AddMvcControllers();
services.AddFluentValidation();
services.AddApiVersioning();
services.AddSwagger();
services.AddCorsPolicy();
services.AddDatabases(configuration);
services.AddJwtAuthentication(configuration);
services.AddRepositories();
services.AddServices();

// === CONSTRUCCION DE LA APLICACION ===
var app = builder.Build();

// === PIPELINE DE MIDDLEWARES ===
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseGlobalExceptionHandler();
app.UseHttpsRedirection();
app.UseCorsPolicy();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.MapControllers();

// === INICIALIZACION ===
using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<FunkoDbContext>();
context.Database.EnsureCreated();

app.Run();
```

📌 **Ejemplo real:** El proyecto TiendaApi usa este patron exactamente. Cada configuracion esta en su propio archivo dentro de `Infrastructures/`, y Program.cs tiene menos de 120 lineas.

### Comparacion de Metricas

| Metrica | Antes | Despues |
|---------|-------|---------|
| Lineas de Program.cs | ~400 | ~40 |
| Archivos de configuracion | 1 | 10+ |
| Tiempo para encontrar configuracion | ~2 minutos | ~5 segundos |
| Reutilizacion entre proyectos | Dificil | Facil |
| Testing de configuracion | Prácticamente imposible | Aislado y sencillo |

## 26.6. Otras Formas de Estructurar el Startup

| Enfoque | Pros | Contras |
|---------|------|---------|
| **Extension Methods** (nuestro enfoque) | Simple, familiar, extensible | Requiere multiples archivos |
| **Clase Startup** | Estructura tradicional de ASP.NET Core | Menos flexible para proyectos pequenos |
| **Directorios por modulo** | Muy organizado para proyectos grandes | Mayor complejidad inicial |
| **Registros fluidos** | Sintaxis muy legible | Puede ser confuso para principiantes |

## 26.7. Buenas Practicas

| Practica | Descripcion |
|----------|-------------|
| **Responsabilidad unica** | Cada archivo de configuracion debe tener una unica razon para cambiar |
| **Convencion sobre configuracion** | Seguir convenciones consistentes reduce la carga cognitiva |
| **Documentacion XML** | Cada extension method debe incluir documentacion XML descriptiva |
| **Validar configuracion** | Los extension methods deben validar parametros requeridos |
| **Organizar por tamano** | Pequeno: Infrastructures/ simple. Mediano: subcarpetas |
| **Reutilizar configuraciones** | Las configuraciones comunes deben poder reutilizarse en otros proyectos |
| **Testing de configuraciones** | Cada configuracion debe poder probarse de forma aislada |

> ⚠️ **Advertencia:** No sobre-organices. Para proyectos pequenos, un Program.cs bien estructurado puede ser suficiente. La organizacion modular es para proyectos que crecen.

## 26.8. Reto: Refactoriza el Program.cs de FunkoApp

> Antes de irte, refactoriza un Program.cs monolitico utilizando el patron de extension methods.

### Contexto

Tu API de Funkos tiene un Program.cs creciente con configuraciones de base de datos, autenticacion, Swagger, CORS y mas.

### Ejercicio

1. Identifica las secciones del Program.cs actual
2. Crea archivos de configuracion separados en `Infrastructures/`:
   - `DatabaseConfig.cs`
   - `AuthenticationConfig.cs`
   - `SwaggerConfig.cs`
   - `CorsConfig.cs`
   - `RepositoriesConfig.cs`
   - `ServicesConfig.cs`
3. Refactoriza Program.cs para que use los extension methods
4. Mantén Program.cs con menos de 50 lineas
5. Añade documentacion XML a cada extension method

> 💡 **Consejo:** El objetivo es que Program.cs sea un indice legible, no un archivo de configuracion. Cada linea debe representar un modulo funcional claro.

---

**Resumen del punto:**

| Concepto | Descripcion |
|----------|-------------|
| **Program.cs monolitico** | Archivo grande, dificil de mantener y navegar |
| **Extension methods** | Metodos de extension para encapsular configuraciones |
| **Infrastructures/** | Carpeta recomendada para archivos de configuracion |
| **Separacion de responsabilidades** | Cada archivo maneja una unica configuracion |
| **Reutilizacion** | Las configuraciones pueden reutilizarse en otros proyectos |
| **Testabilidad** | Cada configuracion puede probarse de forma aislada |
| **Legibilidad** | Program.cs se convierte en un indice claro |

**¿Qué viene después?**

En el siguiente punto veremos **Logging y Monitoreo**: como configurar Serilog para logging estructurado, implementar correlation IDs para trazabilidad y configurar health checks para monitorizar la salud de la aplicacion.
