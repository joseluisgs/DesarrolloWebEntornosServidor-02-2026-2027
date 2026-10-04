- [26. Organización de Program.cs](#26-organización-de-programcs)
  - [26.1. El problema del Program.cs monolítico](#261-el-problema-del-programcs-monolítico)
    - [26.1.1. Ejemplo de Program.cs monolítico](#2611-ejemplo-de-programcs-monolítico)
  - [26.2. Patrón de extension methods para configuración](#262-patrón-de-extension-methods-para-configuración)
    - [26.2.1. Concepto fundamental](#2621-concepto-fundamental)
    - [26.2.2. Beneficios del patrón](#2622-beneficios-del-patrón)
    - [26.2.3. Organización por módulos funcionales](#2623-organización-por-módulos-funcionales)
  - [26.3. Estructura de carpetas: infrastructures](#263-estructura-de-carpetas-infrastructures)
  - [26.4. Ejemplos de implementación](#264-ejemplos-de-implementación)
    - [26.4.1. Configuración de base de datos](#2641-configuración-de-base-de-datos)
    - [26.4.2. Configuración de autenticación JWT](#2642-configuración-de-autenticación-jwt)
  - [26.5. Program.cs refactorizado](#265-programcs-refactorizado)
    - [26.5.1. Comparación de métricas](#2651-comparación-de-métricas)
  - [26.6. Otras formas de estructurar el startup](#266-otras-formas-de-estructurar-el-startup)
  - [26.7. Buenas prácticas](#267-buenas-prácticas)
  - [26.8. Ficheros de organización del repo](#268-ficheros-de-organización-del-repo)
    - [26.8.1. El fichero .editorconfig](#2681-el-fichero-editorconfig)
    - [26.8.2. Directory.Build.props](#2682-directorybuildprops)
    - [26.8.3. Directory.Packages.props (CPM)](#2683-directorypackagesprops-cpm)
    - [26.8.4. Global.json](#2684-globaljson)
  - [26.9. Reto: refactoriza el Program.cs de FunkoApp](#269-reto-refactoriza-el-programcs-de-funkoapp)



# 26. Organización de Program.cs

> 💡 **Punto de partida:** Cuando tu cocina tiene todos los ingredientes, utensilios y recetas en una sola habitación desordenada, cocinar es caótico. Pero si organizas: ingredientes en un área, utensilios en otra, recetas en un libro, todo fluye. Un Program.cs monolítico es como esa cocina desordenada: cuesta encontrar lo que necesitas y es fácil romper algo.

En este punto aprenderás a refactorizar un Program.cs monolítico usando extension methods y el patrón Infrastructure para mantener el código limpio y mantenible.

**Objetivos de aprendizaje:**
- Identificar los problemas de un Program.cs monolítico
- Aplicar el patrón de extension methods para configuración
- Organizar configuraciones en carpetas Infrastructures
- Implementar configuraciones reutilizables para bases de datos, autenticación y más

## 26.1. El problema del Program.cs monolítico

Cuando una aplicación ASP.NET Core crece, el archivo `Program.cs` puede volverse monolítico y difícil de mantener. Los problemas principales son:

- **Dificultad de navegación**: Un archivo de 500+ líneas dificulta encontrar configuraciones
- **Acoplamiento temporal**: Todas las configuraciones en el mismo archivo, difícil reutilizar
- **Dificultad de testing**: Imposible probar una configuración de forma aislada
- **Falta de cohesión**: Configuraciones de naturaleza completamente diferente mezcladas

### 26.1.1. Ejemplo de Program.cs monolítico

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

📌 **Ejemplo real:** Cuando un proyecto como Spotify Backend crece, tener todo en un solo archivo hace que 5 desarrolladores trabajando en el mismo archivo se pisen constantemente. La organización modular evita esto.

## 26.2. Patrón de extension methods para configuración

El patrón de extension methods consiste en crear métodos de extension para `IServiceCollection` (servicios) y `WebApplication` (middlewares), agrupando configuraciones relacionadas en archivos separados.

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

### 26.2.2. Beneficios del patrón

| Beneficio | Descripción |
|-----------|-------------|
| **Lectura mejorada** | Program.cs se convierte en un índice legible |
| **Reutilización** | Configuraciones pueden reutilizarse en otros proyectos |
| **Testing simplificado** | Cada configuración puede probarse de forma aislada |
| **Separación de responsabilidades** | Cada archivo tiene una única responsabilidad |
| **Facilidad de navegación** | Encontrar configuración es tan simple como abrir el archivo |

### 26.2.3. Organización por módulos funcionales

| Módulo | Contenido |
|--------|-----------|
| **Core** | Controladores, validación, versionado de API |
| **API** | Swagger, CORS, middleware de excepciones |
| **Data** | Bases de datos, cache, repositorios |
| **Auth** | Autenticación JWT, autorización por roles |
| **Business** | Servicios de negocio específicos |

## 26.3. Estructura de carpetas: infrastructures

La carpeta `Infrastructures/` es el lugar recomendado para almacenar todos los métodos de extension de configuración.

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

## 26.4. Ejemplos de implementación

### 26.4.1. Configuración de base de datos

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

### 26.4.2. Configuración de autenticación JWT

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

## 26.5. Program.cs refactorizado

Después de aplicar el patrón, el Program.cs queda limpio y legible:

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

📌 **Ejemplo real:** El proyecto TiendaApi usa este patrón exactamente. Cada configuración está en su propio archivo dentro de `Infrastructures/`, y Program.cs tiene menos de 120 líneas.

### 26.5.1. Comparación de métricas

| Métrica | Antes | Después |
|---------|-------|---------|
| Líneas de Program.cs | ~400 | ~40 |
| Archivos de configuración | 1 | 10+ |
| Tiempo para encontrar configuración | ~2 minutos | ~5 segundos |
| Reutilización entre proyectos | Difícil | Fácil |
| Testing de configuración | Prácticamente imposible | Aislado y sencillo |

## 26.6. Otras formas de estructurar el startup

| Enfoque | Pros | Contras |
|---------|------|---------|
| **Extension Methods** (nuestro enfoque) | Simple, familiar, extensible | Requiere múltiples archivos |
| **Clase Startup** | Estructura tradicional de ASP.NET Core | Menos flexible para proyectos pequeños |
| **Directorios por módulo** | Muy organizado para proyectos grandes | Mayor complejidad inicial |
| **Registros fluidos** | Sintaxis muy legible | Puede ser confuso para principiantes |

## 26.7. Buenas prácticas

| Práctica | Descripción |
|----------|-------------|
| **Responsabilidad única** | Cada archivo de configuración debe tener una única razón para cambiar |
| **Convención sobre configuración** | Seguir convenciones consistentes reduce la carga cognitiva |
| **Documentación XML** | Cada extension method debe incluir documentación XML descriptiva |
| **Validar configuración** | Los extension methods deben validar parámetros requeridos |
| **Organizar por tamaño** | Pequeño: Infrastructures/ simple. Mediano: subcarpetas |
| **Reutilizar configuraciones** | Las configuraciones comunes deben poder reutilizarse en otros proyectos |
| **Testing de configuraciones** | Cada configuración debe poder probarse de forma aislada |

> ⚠️ **Advertencia:** No sobre-organices. Para proyectos pequeños, un Program.cs bien estructurado puede ser suficiente. La organización modular es para proyectos que crecen.

## 26.8. Ficheros de organización del repo

Hasta ahora hemos organizado **el interior del proyecto**: `Program.cs`, `Infrastructures/`, carpetas... Pero cuando el repositorio crece (API + tests + cliente, o varias APIs), aparecen **cuatro ficheros que viven en la RAÍZ** y organizan todo el repo de forma automática, sin que tengas que repetir nada en cada proyecto.

| Fichero | Qué fija | Quién lo lee |
|---------|----------|--------------|
| `.editorconfig` | Estilo del código: sangría, saltos de línea, codificación | Tu IDE y `dotnet format` |
| `Directory.Build.props` | Propiedades de compilación comunes | MSBuild (se aplica solo) |
| `Directory.Packages.props` | Versiones de los paquetes NuGet (CPM) | `dotnet restore` |
| `global.json` | Versión del SDK de .NET | La CLI `dotnet` |

> 💡 **Analogía:** Es como el reglamento de un edificio: no pegas el cartel "no fumar" en cada sala; lo pones en la entrada y aplica a todas las plantas. Los cuatro ficheros son el "cartel de la entrada" de tu repo.

📌 **Ejemplo real:** TiendaAPI es un repositorio con la API, sus proyectos de tests y un cliente Blazor. Con estos cuatro ficheros en la raíz, **todos los proyectos** compilan con el mismo SDK, con las mismas propiedades y con las mismas versiones de paquetes.

### 26.8.1. El fichero .editorconfig

El `.editorconfig` define **un único estilo canónico** para todo el repo: codificación, saltos de línea, tamaño de sangría y qué ficheros admiten excepciones.

```ini
# Estilo canónico del repo; verificación: dotnet format
root = true

[*]
charset = utf-8
end_of_line = lf
indent_style = space
indent_size = 4
insert_final_newline = true
trim_trailing_whitespace = true

# JSON, YAML y JS van con 2 espacios (formato de la industria)
[*.{json,yml,yaml,js,mjs}]
indent_size = 2

[*.md]
# Markdown: los dos espacios finales significan salto de línea
trim_trailing_whitespace = false
```

- `root = true`: deja de buscar un `.editorconfig` en carpetas superiores.
- `[*]`: reglas para **todos** los ficheros; los bloques de abajo sobreescriben por tipo.
- `end_of_line = lf`: finales de línea Unix en todos los sistemas (Windows no introduce `\r\n`).

**Verificación en equipo** (para que nadie lo olvide): el IDE lo aplica al guardar, y en CI o antes de commitear se comprueba que nadie lo haya roto:

```bash
# Verificar sin modificar ficheros (0 avisos = OK)
dotnet format --verify-no-changes
```

📌 **Ejemplo real:** TiendaAPI envuelve ese comando en `scripts/check-style.ps1` y lo ejecuta en cada verificación: si alguien formateó mal, el script falla **antes** del commit.

> 💡 **Consejo:** Rider y VS Code leen el `.editorconfig` automáticamente; no tienes que configurar el estilo a mano en cada IDE.

### 26.8.2. Directory.Build.props

Este fichero MSBuild se aplica **automáticamente a todos los `.csproj`** de la carpeta donde está y de sus subcarpetas. Es la solución a la repetición de propiedades:

```xml
<!-- ❌ MALO: las mismas 4 props repetidas en cada uno de los 5 proyectos -->
<PropertyGroup>
    <LangVersion>14</LangVersion>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
</PropertyGroup>
```

```xml
<Project>
  <!-- ✅ BUENO: raíz del repo. MSBuild la inyecta en todos los .csproj -->
  <PropertyGroup>
    <LangVersion>14</LangVersion>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
  </PropertyGroup>
</Project>
```

Y cada proyecto solo declara **lo que es suyo**:

```xml
<PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
</PropertyGroup>
```

Los proyectos **pueden sobreescribir** lo que necesiten (p. ej., un proyecto de tests end-to-end que tolerate avisos): basta con declarar la propiedad en su `.csproj`.

> ⚠️ **Advertencia:** El nombre es **exacto**: `Directory.Build.props`. Si lo escribes mal (`Directory.Build.Prop`, con un espacio...), MSBuild lo ignora **en silencio** y "desaparecen" tus propiedades sin ningún error.

> 📝 **Nota:** En los proyectos sueltos del curso dejamos estas propiedades en cada `.csproj` (proyecto único, todo visible). Cuando el repo tenga varios proyectos, muévelas aquí: ambas formas son válidas, pero repetirlas en N proyectos no lo es.

### 26.8.3. Directory.Packages.props (CPM)

**Central Package Management (CPM)** mueve las **versiones** de los paquetes NuGet a un único fichero de la raíz. El problema que resuelve es el **drift de versiones**: dos proyectos del mismo repo usando versiones distintas del mismo paquete.

📌 **Ejemplo real:** Antes de centralizar, TiendaAPI tenía estas versiones **dispares dentro del mismo repositorio**:

| Paquete | Versión A | Versión B |
|---------|-----------|-----------|
| `NUnit` | 4.3.1 | 4.6.1 |
| `Microsoft.NET.Test.Sdk` | 17.12.0 | 18.10.1 |
| `NUnit3TestAdapter` | 4.6.0 | 6.3.0 |
| `Moq` | 4.20.72 | 4.21.0 |
| `FluentAssertions` | 7.0.0 | 7.2.2 |

Con CPM, la raíz declara **una sola vez** cada versión:

```xml
<Project>
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
  </PropertyGroup>
  <ItemGroup>
    <!-- Las versiones viven AQUÍ, una sola vez -->
    <PackageVersion Include="NUnit" Version="4.6.1" />
    <PackageVersion Include="FluentAssertions" Version="7.2.2" />
    <PackageVersion Include="Moq" Version="4.21.0" />
  </ItemGroup>
</Project>
```

Y los `.csproj` **ya no llevan `Version=`**:

```xml
<!-- ✅ Sin Version: la versión la aporta Directory.Packages.props -->
<PackageReference Include="NUnit" />
<PackageReference Include="FluentAssertions" />
```

> 💡 **Analogía:** Es el catálogo de precios del almacén: el precio se fija **una vez** en el catálogo, no en cada etiqueta de cada balda. Si cambia el precio, cambia en toda la tienda.

> ⚠️ **Advertencia:** Si añades un `<PackageReference>` sin meter su `<PackageVersion>` correspondiente en la raíz, el `dotnet restore` **falla** avisándote de que falta la versión. Es molesto la primera vez y salvador después.

> 💡 **Truco:** Para desactivarlo puntualmente en un proyecto heredado, añade en su `.csproj`: `<ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally>`.

### 26.8.4. Global.json

Fija **qué versión del SDK de .NET** se espera para compilar el repo:

```json
{
  "sdk": {
    "version": "10.0.401",
    "rollForward": "latestFeature"
  }
}
```

- `version`: versión deseada del SDK (no es la del framework `net10.0`, es la de la **herramienta** `dotnet`).
- `rollForward: "latestFeature"`: si la `10.0.401` exacta no está instalada, usa cualquier SDK **`10.0.x`** instalado (el de banda más alta); **no salta** a una `10.1` ni a un `.NET 11`.

| Valor `rollForward` | Comportamiento |
|---------------------|----------------|
| `latestPatch` | Solo parches de la misma banda (10.0.4xx) |
| `latestFeature` | Cualquier banda 10.0.x instalada (el que usamos) |
| `latestMinor` | También 10.1.x, 10.2.x... |
| `latestMajor` | También .NET 11, 12... (muy permisivo) |

📌 **Ejemplo real:** TiendaAPI fija `10.0.401` con `latestFeature` para que el equipo y la CI compilen con la misma herramienta aunque cada máquina instale parches distintos.

```bash
# Qué SDK estoy usando ahora
dotnet --version

# Qué SDKs tengo instalados (¿coincide con global.json?)
dotnet --list-sdks
```

> ⚠️ **Advertencia:** Si pones una versión inexistente y el `rollForward` no permite salto, `dotnet` se niega a compilar con un error claro. No lo borres "para que funcione": comprueba primero con `dotnet --list-sdks`.

**Resumen de la sección:**

| Fichero | Aporta | Coste de olvidarlo |
|---------|--------|--------------------|
| `.editorconfig` | Estilo uniforme | Estilos distintos por IDE |
| `Directory.Build.props` | Props sin repetir | Props duplicadas en N proyectos |
| `Directory.Packages.props` | Versiones sin drift | "En mi máquina funciona" por paquete |
| `global.json` | Mismo SDK | "A mí me compila, a ti no" |

## 26.9. Reto: refactoriza el Program.cs de FunkoApp

> Antes de irte, refactoriza un Program.cs monolítico utilizando el patrón de extension methods.

### Contexto

Tu API de Funkos tiene un Program.cs creciente con configuraciones de base de datos, autenticación, Swagger, CORS y más.

### Ejercicio

1. Identifica las secciones del Program.cs actual
2. Crea archivos de configuración separados en `Infrastructures/`:
   - `DatabaseConfig.cs`
   - `AuthenticationConfig.cs`
   - `SwaggerConfig.cs`
   - `CorsConfig.cs`
   - `RepositoriesConfig.cs`
   - `ServicesConfig.cs`
3. Refactoriza Program.cs para que use los extension methods
4. Mantén Program.cs con menos de 50 líneas
5. Añade documentación XML a cada extension method

> 💡 **Consejo:** El objetivo es que Program.cs sea un índice legible, no un archivo de configuración. Cada línea debe representar un módulo funcional claro.

---

**Resumen del punto:**

| Concepto | Descripción |
|----------|-------------|
| **Program.cs monolítico** | Archivo grande, difícil de mantener y navegar |
| **Extension methods** | Métodos de extension para encapsular configuraciones |
| **Infrastructures/** | Carpeta recomendada para archivos de configuración |
| **Separación de responsabilidades** | Cada archivo maneja una única configuración |
| **Reutilización** | Las configuraciones pueden reutilizarse en otros proyectos |
| **Testabilidad** | Cada configuración puede probarse de forma aislada |
| **Legibilidad** | Program.cs se convierte en un índice claro |

**¿Qué viene después?**

En el siguiente punto veremos **Logging y Monitoreo**: cómo configurar Serilog para logging estructurado, implementar correlation IDs para trazabilidad y configurar health checks para monitorizar la salud de la aplicación.
