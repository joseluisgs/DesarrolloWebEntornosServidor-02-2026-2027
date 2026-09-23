- [27. Logging y Monitoreo](#27-logging-y-monitoreo)
  - [27.1. Fundamentos de Logging](#271-fundamentos-de-logging)
    - [27.1.1. Que es el Logging](#2711-que-es-el-logging)
    - [27.1.2. Logging Estructurado vs Texto Plano](#2712-logging-estructurado-vs-texto-plano)
  - [27.2. Serilog: Logging Estructurado](#272-serilog-logging-estructurado)
    - [27.2.1. Configuracion basica](#2721-configuracion-basica)
    - [27.2.2. Configuracion desde appsettings.json](#2722-configuracion-desde-appsettingsjson)
    - [27.2.3. Sinks y Formateadores](#2723-sinks-y-formateadores)
    - [27.2.4. Enriquecedores de Log](#2724-enriquecedores-de-log)
  - [27.3. Uso de Logging en Servicios](#273-uso-de-logging-en-servicios)
    - [27.3.1. Inyeccion de ILogger](#2731-inyeccion-de-ilogger)
    - [27.3.2. Niveles de Log](#2732-niveles-de-log)
    - [27.3.3. Scopes de Log](#2733-scopes-de-log)
    - [27.3.4. Logueo de Excepciones](#2734-logueo-de-excepciones)
  - [27.4. Correlation ID para Trazabilidad](#274-correlation-id-para-trazabilidad)
  - [27.5. Health Checks](#275-health-checks)
    - [27.5.1. Health Checks basicos](#2751-health-checks-basicos)
    - [27.5.2. Custom Health Check](#2752-custom-health-check)
  - [27.6. Buenas Practicas de Logging](#276-buenas-practicas-de-logging)
    - [27.6.1. Cierre seguro del Logger](#2761-cierre-seguro-del-logger)
  - [27.7. Reto: Implementa Logging en FunkoApp](#277-reto-implementa-logging-en-funkoapp)



# 27. Logging y Monitoreo

> 💡 **Punto de partida:** Cuando tu aplicación falla en producción y no tienes logs, es como intentar arreglar un coche a ciegas. Los logs son el cuadro de mando que te dice qué está pasando en tiempo real. Un buen sistema de logging te permite debugear errores, auditar seguridad y optimizar rendimiento.

En este punto aprenderás a configurar Serilog para logging estructurado, implementar correlation IDs para trazabilidad y configurar health checks para monitorizar la salud de la aplicacion.

**Objetivos de aprendizaje:**
- Comprender la diferencia entre logging de texto plano y estructurado
- Configurar Serilog con multiples sinks
- Implementar `ILogger` con inyeccion de dependencias
- Usar correlation IDs para trazabilidad de requests
- Configurar health checks para monitorizar la aplicacion

## 27.1. Fundamentos de Logging

### 27.1.1. Que es el Logging

El **logging** es el proceso de registrar eventos, errores e informacion relevante que ocurre durante la ejecucion de una aplicacion. Estos registros son fundamentales para el debugging, la auditoria de seguridad y la resolucion de problemas en produccion.

📌 **Ejemplo real:** Cuando Instagram tiene un error en produccion, los desarrolladores revisan los logs para ver que paso. Sin logs, tendrian que adivinar que fallo.

| Problema | Impacto | Solucion con Logging |
|----------|---------|----------------------|
| Errores no detectados | Tiempo de inactividad | Logs de errores + alertas |
| Sin trazabilidad | Dificultad para debuggear | Correlation ID |
| Sin metricas | Decisiones sin datos | Metricas + dashboards |

### 27.1.2. Logging Estructurado vs Texto Plano

El **logging estructurado** almacena los logs en formato JSON con campos clave-valor, permitiendo queries eficientes y analisis.

| Aspecto | Texto Plano | Logging Estructurado |
|---------|-------------|----------------------|
| **Formato** | Lineas de texto libre | JSON con campos clave-valor |
| **Busqueda** | Dificil, regex | Facil, por campos |
| **Analisis** | Limitado | Estadisticas avanzadas |
| **Tamanio** | Pequeno | Algo mayor |

> 💡 **Consejo:** El logging de texto plano es como escribir notas en un cuaderno desordenado. El logging estructurado es como usar una base de datos donde cada dato tiene su campo.

## 27.2. Serilog: Logging Estructurado

Serilog es la biblioteca de logging estructurado mas popular en .NET.

### 27.2.1. Configuracion basica

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

### 27.2.2. Configuracion desde appsettings.json

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

### 27.2.3. Sinks y Formateadores

Los **sinks** determinan donde se envian los logs.

| Sink | Uso | Ejemplo |
|------|-----|---------|
| **Console** | Desarrollo | `WriteTo.Console()` |
| **File** | Persistencia local | `WriteTo.File("logs/api-.log")` |
| **Seq** | Busqueda avanzada | `WriteTo.Seq("http://localhost:5341")` |
| **PostgreSQL** | Base de datos | `WriteTo.PostgreSQL()` |

📌 **Ejemplo real:** Seq es como un Google Analytics pero para logs. Permite buscar, filtrar y analizar todos los logs de tu aplicacion en una interfaz web.

### 27.2.4. Enriquecedores de Log

Los **enriquecedores** añaden informacion contextual a todos los logs.

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

## 27.3. Uso de Logging en Servicios

### 27.3.1. Inyeccion de ILogger

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

### 27.3.2. Niveles de Log

| Nivel | Uso | Cuando Usar |
|-------|-----|-------------|
| **Debug** | Informacion detallada | Debugging, desarrollo |
| **Information** | Eventos normales | Requests exitosos |
| **Warning** | Situaciones anomolas | Retries, timeouts |
| **Error** | Errores recoverable | Excepciones capturadas |
| **Critical** | Errores graves | Crash inminente |

```csharp
logger.LogDebug("Consultando funko {FunkoId}", id);
logger.LogInformation("Funko {FunkoId} obtenido exitosamente", id);
logger.LogWarning("Stock bajo para funko {FunkoId}: {Stock}", id, stock);
logger.LogError(ex, "Error al procesar funko {FunkoId}", id);
logger.LogCritical("Error critico: {Message}", ex.Message);
```

### 27.3.3. Scopes de Log

Los **scopes** agrupan logs relacionados bajo un contexto comun.

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

### 27.3.4. Logueo de Excepciones

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

> ⚠️ **Advertencia:** Nunca loguees datos sensibles como contrasenas, tokens JWT o datos personales. Esto es un riesgo de seguridad y puede violar regulaciones como GDPR.

## 27.4. Correlation ID para Trazabilidad

El **correlation ID** es un identificador unico que sigue una request a traves de todos los servicios y logs, permitiendo reconstruir el flujo completo de una operacion.

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

Los health checks permiten a orquestadores como Kubernetes o Azure monitorizar la salud de la aplicacion.

### 27.5.1. Health Checks basicos

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

### 27.5.2. Custom Health Check

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

## 27.6. Buenas Practicas de Logging

| Practica | Descripcion |
|----------|-------------|
| **Usar logging estructurado** | Serilog con formato JSON para busquedas eficientes |
| **Incluir correlation ID** | En todos los logs para trazabilidad |
| **Configurar niveles por ambiente** | Debug en desarrollo, Warning en produccion |
| **No loguear datos sensibles** | Nunca contrasenas, tokens ni datos personales |
| **Usar enrichers** | Para contexto adicional automatico |
| **Implementar health checks** | Para monitorizar la salud de la aplicacion |
| **Configurar alertas** | En produccion para detectar problemas proactivamente |
| **Loguear excepciones con contexto** | Incluir el tipo de excepcion y datos relevantes |

> ⚠️ **Advertencia:** Los logs en produccion deben tener nivel Warning o superior. Los logs Debug e Information en produccion generan demasiado volumen y pueden impactar el rendimiento.

### 27.6.1. Cierre seguro del Logger

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

## 27.7. Reto: Implementa Logging en FunkoApp

> Antes de irte, implementa un sistema completo de logging y monitoreo para tu API de Funkos.

### Contexto

Tu API de Funkos necesita un sistema de logging para debugear errores en desarrollo y monitorizar en produccion.

### Ejercicio

1. Configura Serilog con Console y File sinks
2. Implementa middleware de Correlation ID
3. Añade logging a todos los servicios (FunkoService, CategoriaService)
4. Implementa health checks para la base de datos
5. Configura niveles de log diferentes por entorno

> 💡 **Consejo:** Usa `logger.LogInformation` para eventos normales, `logger.LogWarning` para situaciones anomolas y `logger.LogError` para errores. Nunca uses `logger.LogDebug` en produccion.

---

**Resumen del punto:**

| Concepto | Descripcion |
|----------|-------------|
| **Logging** | Registro de eventos para debugging y auditoria |
| **Serilog** | Biblioteca de logging estructurado para .NET |
| **Logging Estructurado** | Logs en formato JSON con campos clave-valor |
| **Niveles de Log** | Debug, Information, Warning, Error, Critical |
| **Correlation ID** | Identificador unico para trazabilidad de requests |
| **Health Checks** | Verificacion de salud de la aplicacion y dependencias |
| **Sinks** | Destinos de los logs (Console, File, Seq, etc.) |
| **Enrichers** | Informacion contextual añadida automaticamente a los logs |

**¿Qué viene después?**

En el siguiente punto veremos **Testing de Servicios Web**: como escribir tests unitarios con NUnit, usar FluentAssertions para aserciones legibles, crear mocks con Moq y implementar tests de integracion con TestContainers.
