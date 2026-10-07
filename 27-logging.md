- [27. Logging y monitoreo](#27-logging-y-monitoreo)
  - [27.1. Fundamentos de logging](#271-fundamentos-de-logging)
    - [27.1.1. Qué es el logging](#2711-qué-es-el-logging)
    - [27.1.2. Logging estructurado vs texto plano](#2712-logging-estructurado-vs-texto-plano)
  - [27.2. Serilog: logging estructurado](#272-serilog-logging-estructurado)
    - [27.2.1. Configuración básica](#2721-configuración-básica)
    - [27.2.2. Configuración desde appsettings.json](#2722-configuración-desde-appsettingsjson)
    - [27.2.3. Sinks y formateadores](#2723-sinks-y-formateadores)
    - [27.2.4. Enriquecedores de log](#2724-enriquecedores-de-log)
  - [27.3. Uso de logging en servicios](#273-uso-de-logging-en-servicios)
    - [27.3.1. Inyección de ILogger](#2731-inyección-de-ilogger)
    - [27.3.2. Niveles de log](#2732-niveles-de-log)
    - [27.3.3. Scopes de log](#2733-scopes-de-log)
    - [27.3.4. Logueo de excepciones](#2734-logueo-de-excepciones)
  - [27.4. Correlation ID para trazabilidad](#274-correlation-id-para-trazabilidad)
  - [27.5. Health Checks](#275-health-checks)
    - [27.5.1. Health Checks básicos](#2751-health-checks-básicos)
    - [27.5.2. Custom health check](#2752-custom-health-check)
  - [27.6. Endpoint de versión: GET /version](#276-endpoint-de-versión-get-version)
  - [27.7. Buenas prácticas de logging](#277-buenas-prácticas-de-logging)
    - [27.7.1. Cierre seguro del logger](#2771-cierre-seguro-del-logger)
  - [27.8. Reto: implementa logging en FunkoApp](#278-reto-implementa-logging-en-funkoapp)



# 27. Logging y monitoreo

> 💡 **Punto de partida:** Cuando tu aplicación falla en producción y no tienes logs, es como intentar arreglar un coche a ciegas. Los logs son el cuadro de mando que te dice qué está pasando en tiempo real. Un buen sistema de logging te permite debugear errores, auditar seguridad y optimizar rendimiento.

En este punto aprenderás a configurar Serilog para logging estructurado, implementar correlation IDs para trazabilidad y configurar health checks para monitorizar la salud de la aplicación.

**Objetivos de aprendizaje:**
- Comprender la diferencia entre logging de texto plano y estructurado
- Configurar Serilog con múltiples sinks
- Implementar `ILogger` con inyección de dependencias
- Usar correlation IDs para trazabilidad de requests
- Configurar health checks para monitorizar la aplicación

## 27.1. Fundamentos de logging

### 27.1.1. Qué es el logging

El **logging** es el proceso de registrar eventos, errores e información relevante que ocurre durante la ejecución de una aplicación. Estos registros son fundamentales para el debugging, la auditoría de seguridad y la resolución de problemas en producción.

📌 **Ejemplo real:** Cuando Instagram tiene un error en producción, los desarrolladores revisan los logs para ver qué pasó. Sin logs, tendrían que adivinar qué falló.

| Problema | Impacto | Solución con Logging |
|----------|---------|----------------------|
| Errores no detectados | Tiempo de inactividad | Logs de errores + alertas |
| Sin trazabilidad | Dificultad para debuggear | Correlation ID |
| Sin métricas | Decisiones sin datos | Métricas + dashboards |

### 27.1.2. Logging estructurado vs texto plano

El **logging estructurado** almacena los logs en formato JSON con campos clave-valor, permitiendo queries eficientes y análisis.

| Aspecto | Texto Plano | Logging Estructurado |
|---------|-------------|----------------------|
| **Formato** | Líneas de texto libre | JSON con campos clave-valor |
| **Búsqueda** | Difícil, regex | Fácil, por campos |
| **Análisis** | Limitado | Estadísticas avanzadas |
| **Tamaño** | Pequeño | Algo mayor |

> 💡 **Consejo:** El logging de texto plano es como escribir notas en un cuaderno desordenado. El logging estructurado es como usar una base de datos donde cada dato tiene su campo.

## 27.2. Serilog: logging estructurado

Serilog es la biblioteca de logging estructurado más popular en .NET.

### 27.2.1. Configuración básica

```bash
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.Console
dotnet add package Serilog.Sinks.File
dotnet add package Serilog.Exceptions
dotnet add package Serilog.Enrichers.Environment
```

```csharp
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithExceptionDetails()
    .Enrich.WithProperty("Application", "FunkoApp")
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Scopes:j}{NewLine}{Exception}")
    .WriteTo.File(
        path: "logs/funkoapp-.log",
        rollingInterval: RollingInterval.Day,
        rollOnFileSizeLimit: true,
        fileSizeLimitBytes: 10_000_000,
        retainedFileCountLimit: 30)
    .CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

// Middleware para logging de requests
app.UseSerilogRequestLogging();

app.Run();
```

### 27.2.2. Configuración desde appsettings.json

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Debug",
      "Override": {
        "Microsoft": "Information",
        "Microsoft.AspNetCore": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "outputTemplate": "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Scopes:j}{NewLine}{Exception}"
        }
      },
      {
        "Name": "File",
        "Args": {
          "path": "logs/funkoapp-.log",
          "rollingInterval": "Day"
        }
      }
    ],
    "Properties": {
      "Application": "FunkoApp"
    }
  }
}
```

```csharp
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();
```


**Qué hace `UseSerilog()`: sustituye el logger de Microsoft.**

ASP.NET Core trae un `ILogger` por defecto que escribe en consola. `builder.Host.UseSerilog()` lo **sustituye** por completo: a partir de ese momento, todos los `ILogger<T>` que inyectes en servicios, controladores o páginas escriben en los sinks de Serilog (consola, fichero, etc.), no en el proveedor por defecto.

```csharp
// ANTES de UseSerilog: los ILogger escriben en consola (proveedor por defecto)
// DESPUÉS de UseSerilog: los ILogger escriben donde diga Serilog (consola + fichero + ...)
builder.Host.UseSerilog();
```

> ⚠️ **Advertencia:** `UseSerilog()` solo cambia el proveedor. No toca los `ILogger<T>` que ya inyectaste: siguen funcionando igual, pero su salida va a los sinks de Serilog.

> 📝 **Nota:** Si no llamas a `UseSerilog()`, la configuración `"Serilog"` del `appsettings.json` se ignora: el logger por defecto sigue mandando.
### 27.2.3. Sinks y formateadores

Los **sinks** determinan dónde se envían los logs.

| Sink | Uso | Ejemplo |
|------|-----|---------|
| **Console** | Desarrollo | `WriteTo.Console()` |
| **File** | Persistencia local | `WriteTo.File("logs/api-.log")` |
| **Seq** | Búsqueda avanzada | `WriteTo.Seq("http://localhost:5341")` |
| **PostgreSQL** | Base de datos | `WriteTo.PostgreSQL()` |

📌 **Ejemplo real:** Seq es como un Google Analytics pero para logs. Permite buscar, filtrar y analizar todos los logs de tu aplicación en una interfaz web.

### 27.2.4. Enriquecedores de log

Los **enriquecedores** añaden información contextual a todos los logs.

```csharp
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithExceptionDetails()
    .Enrich.WithProperty("Application", "FunkoApp")
    .Enrich.WithEnvironmentName()
    .Enrich.WithMachineName()
    .CreateLogger();
```

> ⚠️ **Advertencia:** `WithEnvironmentName()` y `WithMachineName()` provienen de **Serilog.Enrichers.Environment** (paquete no incluido en Serilog.AspNetCore). Si no lo instalas, el código no compila:

```bash
dotnet add package Serilog.Enrichers.Environment
```

## 27.3. Uso de logging en servicios

### 27.3.1. Inyección de ILogger

```csharp
// ❌ MALO: Logger como campo estatico
public static class BadLogger
{
    private static readonly ILogger _logger = null!; // No se puede inyectar
}

// ✅ BUENO: Logger con inyeccion de dependencias
public class FunkoService(
    IFunkoRepository repository,
    ILogger<FunkoService> logger) : IFunkoService
{
    public async Task<Funko?> GetByIdAsync(long id)
    {
        logger.LogInformation("Buscando funko {FunkoId}", id);

        var funko = await repository.GetByIdAsync(id);

        if (funko is null)
        {
            logger.LogWarning("Funko {FunkoId} no encontrado", id);
        }

        return funko;
    }
}
```

### 27.3.2. Niveles de log

| Nivel | Uso | Cuándo Usar |
|-------|-----|-------------|
| **Debug** | Información detallada | Debugging, desarrollo |
| **Information** | Eventos normales | Requests exitosos |
| **Warning** | Situaciones anómalas | Retries, timeouts |
| **Error** | Errores recuperables | Excepciones capturadas |
| **Critical** | Errores graves | Crash inminente |

```csharp
logger.LogDebug("Consultando funko {FunkoId}", id);
logger.LogInformation("Funko {FunkoId} obtenido exitosamente", id);
logger.LogWarning("Stock bajo para funko {FunkoId}: {Stock}", id, stock);
logger.LogError(ex, "Error al procesar funko {FunkoId}", id);
logger.LogCritical("Error critico: {Message}", ex.Message);
```

### 27.3.3. Scopes de log

Los **scopes** agrupan logs relacionados bajo un contexto común.

```csharp
public async Task<List<Funko>> GetByCategoriaAsync(long categoriaId)
{
    using var _ = logger.BeginScope("Obteniendo funkos por categoria {CategoriaId}", categoriaId);

    logger.LogDebug("Iniciando consulta de funkos");

    var funkos = await repository.GetByCategoriaIdAsync(categoriaId);

    logger.LogDebug("Consulta completada. {Count} funkos encontrados", funkos.Count);

    return funkos;
}
```

### 27.3.4. Logueo de excepciones

```csharp
// ❌ MALO: Logear sin contexto
logger.LogError("Error");

// ✅ BUENO: Logear con excepcion y contexto
try
{
    var result = await repository.AddAsync(funko);
}
catch (DbUpdateException ex)
{
    logger.LogError(ex, "Error de base de datos al crear funko {Nombre}", funko.Nombre);
    throw;
}
```

> ⚠️ **Advertencia:** Nunca loguees datos sensibles como contraseñas, tokens JWT o datos personales. Esto es un riesgo de seguridad y puede violar regulaciones como GDPR.

## 27.4. Correlation ID para trazabilidad

El **correlation ID** es un identificador único que sigue una request a través de todos los servicios y logs, permitiendo reconstruir el flujo completo de una operación.

```csharp
public class CorrelationIdMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault()
            ?? Guid.NewGuid().ToString();

        context.Response.Headers["X-Correlation-ID"] = correlationId;
        context.Items["CorrelationId"] = correlationId;

        using var logScope = Log.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId
        });

        await next(context);
    }
}

// Registrar middleware
app.UseMiddleware<CorrelationIdMiddleware>();
```

📌 **Ejemplo real:** Cuando un usuario reporta un error en Spotify, el equipo de soporte usa el correlation ID para reconstruir toda la secuencia de eventos que llevaron al error, desde la request inicial hasta la respuesta final.

## 27.5. Health Checks

Los health checks permiten a orquestadores como Kubernetes o Azure monitorizar la salud de la aplicación.

### 27.5.1. Health Checks básicos

```bash
dotnet add package AspNetCore.HealthChecks.NpgSql
dotnet add package AspNetCore.HealthChecks.Redis
```

```csharp
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy())
    .AddNpgSql(
        connectionString: builder.Configuration.GetConnectionString("PostgreSQL"),
        name: "postgresql",
        tags: ["database"])
    .AddRedis(
        connectionString: builder.Configuration.GetConnectionString("Redis"),
        name: "redis",
        tags: ["cache"]);

app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});
```

### 27.5.2. Custom health check

```csharp
public class CustomHealthCheck(IServiceScopeFactory scopeFactory) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<FunkoDbContext>();

            var canConnect = await db.Database.CanConnectAsync(cancellationToken);

            return canConnect
                ? HealthCheckResult.Healthy("Base de datos conectada")
                : HealthCheckResult.Degraded("No se puede conectar a la base de datos");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(
                $"Error verificando base de datos: {ex.Message}");
        }
    }
}

builder.Services.AddHealthChecks()
    .AddCheck<CustomHealthCheck>("database-check");
```

## 27.6. Endpoint de versión: GET /version

Los health checks responden **¿está viva?**. La otra pregunta clásica de un incidente es **¿qué versión está corriendo?** — sobre todo si tienes varias réplicas o despliegues azul/verde: ¿se completó el despliegue o una instancia se quedó en el build anterior?

📌 **Ejemplo real:** TiendaAPI expone `GET /version` como un extension method en `Infrastructures/VersionConfig.cs` (el patrón de Infrastructures que viste en el tema 26), con `.AllowAnonymous()`: la versión no es información sensible.

**Respuesta de ejemplo:**

```json
{
  "version": "1.2.3+abc1234",
  "fileVersion": "1.2.3.0",
  "runtime": "10.0.1",
  "framework": ".NET 10.0.1"
}
```

| Campo | De dónde sale | Para qué sirve |
|-------|---------------|----------------|
| `version` | `AssemblyInformationalVersionAttribute` | Versión semántica; a menudo incluye `+hash` del commit |
| `fileVersion` | `AssemblyFileVersionAttribute` | Versión del fichero `.dll` |
| `runtime` | `Environment.Version` | Versión del runtime .NET en ejecución |
| `framework` | `RuntimeInformation.FrameworkDescription` | Descripción legible del framework |

**Implementación:**

```csharp
using System.Reflection;
using Serilog;

namespace TuProyecto.Infrastructures;

public static class VersionConfig
{
    /// <summary>
    /// Mapea el endpoint GET /version con la información de build.
    /// </summary>
    public static IEndpointRouteBuilder MapVersionEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/version", () =>
        {
            var assembly = Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version?.ToString() ?? "0.0.0";
            var fileVersion = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version ?? version;
            var informationVersion = assembly
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? version;

            // Log estructurado: queda registrado quién preguntó por la versión
            Log.Information("Consultada versión de la API: {Version}", informationVersion);

            return Results.Ok(new
            {
                version = informationVersion,
                fileVersion,
                runtime = Environment.Version.ToString(),
                framework = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription
            });
        })
        .WithName("GetVersion")
        .WithDescription("Devuelve la versión de la API y el runtime")
        .AllowAnonymous();

        return endpoints;
    }
}
```

```csharp
// Program.cs - mismo patrón que el resto de configuraciones
app.MapVersionEndpoint();
```

**Diagnóstico rápido en terminal:**

```bash
curl -s http://localhost:5000/version | jq .version
```

> 💡 **Consejo:** ¿Varias réplicas? Recorre `curl /version` en cada una: si no todas devuelven el mismo `version`, **el despliegue no ha terminado**.

> ⚠️ **Advertencia:** `AllowAnonymous` está bien para metadatos de build (versión, runtime), pero **nunca** expongas aquí rutas internas, connection strings o datos de configuración. Pregunta al endpoint: ¿esto ayudaría a un atacante? Si la respuesta es "no", puede quedar.

> 📝 **Nota:** El trifecta de monitorización: **`/health`** (¿vive?), **`/health/ready`** (¿puede recibir tráfico?), **`/version`** (¿qué versión?). Juntos responden en tres curl lo que de otro modo exigiría entrar en el servidor.

## 27.7. Buenas prácticas de logging

| Práctica | Descripción |
|----------|-------------|
| **Usar logging estructurado** | Serilog con formato JSON para búsquedas eficientes |
| **Incluir correlation ID** | En todos los logs para trazabilidad |
| **Configurar niveles por ambiente** | Debug en desarrollo, Warning en producción |
| **No loguear datos sensibles** | Nunca contraseñas, tokens ni datos personales |
| **Usar enrichers** | Para contexto adicional automático |
| **Implementar health checks** | Para monitorizar la salud de la aplicación |
| **Configurar alertas** | En producción para detectar problemas proactivamente |
| **Loguear excepciones con contexto** | Incluir el tipo de excepción y datos relevantes |

> ⚠️ **Advertencia:** Los logs en producción deben tener nivel Warning o superior. Los logs Debug e Information en producción generan demasiado volumen y pueden impactar el rendimiento.

### 27.7.1. Cierre seguro del logger

Si la aplicación muere por una excepción no controlada, el logger global (estático) puede perder los últimos mensajes. Registra el error fatal y **cierra el logger** para garantizar el flush:

```csharp
try
{
    app.Run();
}
catch (Exception ex)
{
    // Último recurso: la aplicación termina inesperadamente
    Log.Fatal(ex, "Host terminated unexpectedly");
    throw;
}
finally
{
    // Asegura que queden mensajes pendientes (ficheros, red...)
    Log.CloseAndFlush();
}
```

> 💡 **Consejo:** `Log.CloseAndFlush()` es imprescindible con sinks como File o Seq: sin él, los eventos en cola pueden perderse si el proceso muere bruscamente. `Log.Fatal` captura el error que habría quedado sin registrar al romper el host.

## 27.8. Reto: implementa logging en FunkoApp

> Antes de irte, implementa un sistema completo de logging y monitoreo para tu API de Funkos.

### Contexto

Tu API de Funkos necesita un sistema de logging para debugear errores en desarrollo y monitorizar en producción.

### Ejercicio

1. Configura Serilog con Console y File sinks
2. Implementa middleware de Correlation ID
3. Añade logging a todos los servicios (FunkoService, CategoriaService)
4. Implementa health checks para la base de datos
5. Configura niveles de log diferentes por entorno

> 💡 **Consejo:** Usa `logger.LogInformation` para eventos normales, `logger.LogWarning` para situaciones anómalas y `logger.LogError` para errores. Nunca uses `logger.LogDebug` en producción.

---

**Resumen del punto:**

| Concepto | Descripción |
|----------|-------------|
| **Logging** | Registro de eventos para debugging y auditoría |
| **Serilog** | Biblioteca de logging estructurado para .NET |
| **Logging Estructurado** | Logs en formato JSON con campos clave-valor |
| **Niveles de Log** | Debug, Information, Warning, Error, Critical |
| **Correlation ID** | Identificador único para trazabilidad de requests |
| **Health Checks** | Verificación de salud de la aplicación y dependencias |
| **Sinks** | Destinos de los logs (Console, File, Seq, etc.) |
| **Enrichers** | Información contextual añadida automáticamente a los logs |

**¿Qué viene después?**

En el siguiente punto veremos **Testing de Servicios Web**: cómo escribir tests unitarios con NUnit, usar FluentAssertions para aserciones legibles, crear mocks con Moq y implementar tests de integración con TestContainers.
