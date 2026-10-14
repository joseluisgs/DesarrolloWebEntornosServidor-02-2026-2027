- [24. Documentación con Swagger/OpenAPI](#24-documentación-con-swaggeropenapi)
  - [24.1. CORS (Cross-Origin resource sharing)](#241-cors-cross-origin-resource-sharing)
    - [24.1.1. Qué es CORS](#2411-qué-es-cors)
    - [24.1.2. Configuración de CORS en ASP.NET Core](#2412-configuración-de-cors-en-aspnet-core)
  - [24.2. Swagger y OpenAPI](#242-swagger-y-openapi)
    - [24.2.1. Qué es OpenAPI](#2421-qué-es-openapi)
    - [24.2.2. Qué es Swagger](#2422-qué-es-swagger)
    - [24.2.3. Configuración básica](#2423-configuración-básica)
    - [24.2.4. Configuración avanzada](#2424-configuración-avanzada)
  - [24.3. Documentar endpoints](#243-documentar-endpoints)
    - [24.3.1. Atributos de documentación](#2431-atributos-de-documentación)
    - [24.3.2. Ejemplo completo de endpoint documentado](#2432-ejemplo-completo-de-endpoint-documentado)
    - [24.3.3. Respuesta 201 created en POST](#2433-respuesta-201-created-en-post)
  - [24.4. Documentar modelos y DTOs](#244-documentar-modelos-y-dtos)
  - [24.5. Documentar autenticación JWT](#245-documentar-autenticación-jwt)
  - [24.6. Documentar respuestas de error](#246-documentar-respuestas-de-error)
  - [24.7. Versionado de API](#247-versionado-de-api)
  - [24.8. Ejemplos de solicitudes](#248-ejemplos-de-solicitudes)
  - [24.9. Filtros personalizados](#249-filtros-personalizados)
  - [24.10. Buenas prácticas](#2410-buenas-prácticas)
  - [24.11. Reto: documenta la API de FunkoApp](#2411-reto-documenta-la-api-de-funkoapp)



# 24. Documentación con Swagger/OpenAPI

> 💡 **Punto de partida:** Cuando usas la API de Stripe para cobrar en tu tienda online, necesitas saber qué endpoints existen, qué datos envías y qué respuestas recibes. Stripe proporciona documentación interactiva donde puedes probar cada endpoint en vivo. Eso es lo que Swagger/OpenAPI hace por tu API: documentación automática, interactiva y siempre actualizada.

En este punto aprenderás a configurar CORS, integrar Swagger/OpenAPI en ASP.NET Core, documentar endpoints, modelos, autenticación y errores, versionar tu API y usar filtros personalizados.

**Objetivos de aprendizaje:**
- Configurar CORS para permitir solicitudes cross-origin
- Integrar Swagger/OpenAPI en una aplicación ASP.NET Core
- Documentar endpoints, modelos y respuestas de error
- Configurar autenticación JWT en Swagger
- Implementar versionado de API
- Usar filtros personalizados para personalizar la documentación

## 24.1. CORS (Cross-Origin resource sharing)

### 24.1.1. Qué es CORS

**CORS** es un mecanismo de seguridad de los navegadores que controla las solicitudes HTTP entre diferentes dominios. Sin CORS, los navegadores bloquean solicitudes a dominios diferentes por defecto.

```mermaid
flowchart LR
    subgraph "Sin CORS"
        A[Frontend: https://mi-app.com] -->|Bloqueado| B[API: https://mi-api.com]
        style A fill:#f44336,color:#fff
        style B fill:#f44336,color:#fff
    end

    subgraph "Con CORS"
        C[Frontend: https://mi-app.com] -->|Permitido| D[API: https://mi-api.com]
        style C fill:#4CAF50,color:#fff
        style D fill:#4CAF50,color:#fff
    end
```

📌 **Ejemplo real:** Cuando un frontend React en `localhost:3000` quiere consumir tu API en `localhost:5001`, el navegador bloquea la solicitud si no configuras CORS. Es como un portero que solo deja entrar a personas con autorización.

> 📝 **Nota:** CORS solo afecta a solicitudes desde navegadores. Aplicaciones servidor a servidor no están afectadas por CORS.

### 24.1.2. Configuración de CORS en ASP.NET Core

**Politica permisiva (solo desarrollo):**

```csharp
// ❌ MALO: Usar AllowAll en produccion
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
```

```csharp
// ✅ BUENO: Politica restrictiva para produccion
builder.Services.AddCors(options =>
{
    options.AddPolicy("Production", policy =>
    {
        policy.WithOrigins(
                "https://mi-frontend.com",
                "https://admin.mi-frontend.com"
            )
            .WithMethods("GET", "POST", "PUT", "DELETE")
            .WithHeaders("Content-Type", "Authorization")
            .AllowCredentials();
    });
});
```

```csharp
// Uso en Program.cs
app.UseCors("Production");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
```

> ⚠️ **Advertencia:** Nunca uses "AllowAll" en producción. Esto permite que cualquier dominio acceda a tu API, lo cual es un riesgo de seguridad.

## 24.2. Swagger y OpenAPI

### 24.2.1. Qué es OpenAPI

**OpenAPI** es una especificación estándar para describir APIs REST en formato JSON/YAML. Define la estructura de tu API de forma que pueda ser consumida por herramientas automatizadas.

📌 **Ejemplo real:** La API de Twitter/X sigue OpenAPI. Cuando usas herramientas como Postman para explorar la API de Twitter, estás consumindo la especificación OpenAPI que ellos publican.

| Beneficio | Descripcion |
|-----------|-------------|
| **Documentacion estandarizada** | Machine-readable y auto-descriptiva |
| **Generacion automatica de SDKs** | Clientes en multiples lenguajes |
| **Pruebas interactivas** | Testing desde la propia documentacion |

### 24.2.2. Qué es Swagger

**Swagger** es un conjunto de herramientas que implementa la especificacion OpenAPI:

| Herramienta | Descripcion |
|-------------|-------------|
| **Swagger UI** | Interfaz web interactiva para explorar y probar la API |
| **Swagger Editor** | Editor en linea para crear especificaciones OpenAPI |
| **Swagger Codegen** | Genera codigo cliente y servidor desde la especificacion |

### 24.2.3. Configuración básica

La configuración mínima de Swagger son dos líneas en `Program.cs`: una para registrar el generador y otra para montar la página de documentación.

```bash
dotnet add package Swashbuckle.AspNetCore
dotnet add package Swashbuckle.AspNetCore.Annotations
```

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Habilitar Swagger solo en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
```

Acceder a Swagger UI: `https://localhost:5001/swagger/index.html`

> 💡 **Truco:** Si la URL de Swagger no coincide (porque `Properties/launchSettings.json` cambia el puerto), ejecuta con `dotnet run --no-launch-profile` para usar las URLs por defecto (`https://localhost:5001`) y saber exactamente dónde está la UI.

**Swashbuckle vs `Microsoft.AspNetCore.OpenApi` (embebido en .NET 9/10):**

| | **Swashbuckle.AspNetCore** | **Microsoft.AspNetCore.OpenApi** |
|---|---|---|
| **Origen** | Comunidad (Swagger Tools) | Oficial de Microsoft (incluido en .NET 9/10) |
| **Registro** | `AddSwaggerGen()` + `UseSwagger()` / `UseSwaggerUI()` | `AddOpenApi()` + `MapOpenApi()` |
| **UI interactiva** | Sí, incluida (Swagger UI) | No incluida (solo sirve el documento `/openapi/v1.json`) |
| **Ventaja** | UI lista para probar, filtros y ejemplos | Cero dependencias extra, integración nativa |
| **Cuándo usarlo** | Necesitas probar endpoints desde el navegador | Solo necesitas el documento OpenAPI |

> 📝 **Nota:** En .NET 9/10 las plantillas nuevas usan `Microsoft.AspNetCore.OpenApi` (`AddOpenApi()` / `MapOpenApi()`). Si quieres la UI interactiva tipo Swagger, combínalo con Swashbuckle o con un visor de OpenAPI de terceros. En este tema usamos Swashbuckle porque además nos da los atributos de documentación.

### 24.2.4. Configuración avanzada

La configuración avanzada personaliza los metadatos del esquema: título, versión, descripción y los servidores contra los que se puede probar.

```csharp
using Microsoft.OpenApi.Models;

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1.0.0",
        Title = "ProductosApp API",
        Description = "API REST para gestion de Productos",
        Contact = new OpenApiContact
        {
            Name = "Jose Luis Gonzalez",
            Email = "jose@example.com"
        }
    });

    // Incluir comentarios XML (solo si existe: si no, IncludeXmlComments lanza FileNotFoundException)
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }

    options.EnableAnnotations();
});
```

Habilitar comentarios XML en `.csproj` (si no, no se genera el `.xml` y el `if (File.Exists)` anterior lo salta):

```xml
<PropertyGroup>
  <GenerateDocumentationFile>true</GenerateDocumentationFile>
  <NoWarn>$(NoWarn);1591</NoWarn>
</PropertyGroup>
```

## 24.3. Documentar endpoints

### 24.3.1. Atributos de documentación

Los atributos de documentación describen cada endpoint para Swagger: código de estado, tipo de respuesta y ejemplo de lo que devuelve.

```csharp
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[SwaggerTag("Gestion de Productos")]
public class ProductosController : ControllerBase
{
    /// <summary>
    /// Obtiene todos los productos con paginacion
    /// </summary>
    /// <param name="pageNumber">Numero de pagina (default: 1)</param>
    /// <param name="pageSize">Tamano de pagina (default: 10)</param>
    /// <returns>Lista paginada de productos</returns>
    /// <response code="200">Devuelve la lista de productos</response>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Obtener todos los productos",
        Description = "Devuelve una lista paginada de productos con filtros opcionales",
        OperationId = "GetAllProductos",
        Tags = new[] { "Productos" }
    )]
    [SwaggerResponse(200, "Lista de productos obtenida correctamente", typeof(PageResponse<ProductoResponseDto>))]
    [SwaggerResponse(400, "Parametros invalidos")]
    public async Task<ActionResult<PageResponse<ProductoResponseDto>>> GetAll(
        [FromQuery, SwaggerParameter("Numero de pagina", Required = false)] int pageNumber = 1,
        [FromQuery, SwaggerParameter("Tamano de pagina", Required = false)] int pageSize = 10)
    {
        // ...
    }
```

### 24.3.2. Ejemplo completo de endpoint documentado

Este ejemplo junta todo lo anterior en un endpoint real: XMLDoc en el método, atributos de respuesta y ejemplos para Swagger.

```csharp
/// <summary>
/// Actualiza un producto existente
/// </summary>
/// <param name="id">ID del producto a actualizar</param>
/// <param name="dto">Nuevos datos del producto</param>
/// <returns>Producto actualizado</returns>
[HttpPut("{id}")]
[Authorize(Roles = Roles.Admin)]
[SwaggerOperation(
    Summary = "Actualizar producto",
    Description = "Actualiza todos los campos de un producto. Solo administradores pueden ejecutar esta operacion",
    OperationId = "UpdateProducto"
)]
[SwaggerResponse(200, "Producto actualizado correctamente", typeof(ProductoResponseDto))]
[SwaggerResponse(400, "Datos invalidos o incompletos", typeof(ValidationProblemDetails))]
[SwaggerResponse(401, "Usuario no autenticado")]
[SwaggerResponse(404, "Producto no encontrado", typeof(ProblemDetails))]
public async Task<ActionResult<ProductoResponseDto>> Update(
    [FromRoute, SwaggerParameter("ID del producto", Required = true)] int id,
    [FromBody, SwaggerRequestBody("Datos actualizados del producto", Required = true)] UpdateProductoDto dto)
{
    var result = await _service.UpdateAsync(id, dto);
    return result.Match<ActionResult<ProductoResponseDto>>(
        success => Ok(success),
        error => error switch
        {
            ProductoError.NotFound => NotFound(new ProblemDetails { Detail = error.Message }),
            ProductoError.InvalidData => BadRequest(new ValidationProblemDetails()),
            _ => StatusCode(500)
        }
    );
}
```

📌 **Ejemplo real:** Netflix documenta cada endpoint de su API interna con atributos similares. Cuando un desarrollador necesita consumir un servicio, abre Swagger y ve exactamente qué datos enviar y qué respuestas esperar.

### 24.3.3. Respuesta 201 created en POST

Cuando un `POST` **crea** un recurso, la respuesta correcta es **201 Created** con header `Location`, no un `200 OK`. Swagger lo documenta con `SwaggerResponse(201, ...)`:

```csharp
/// <summary>
/// Crea un producto nuevo
/// </summary>
/// <param name="dto">Datos del producto a crear</param>
/// <returns>Producto recién creado</returns>
[HttpPost]
[SwaggerOperation(
    Summary = "Crear producto",
    Description = "Crea un nuevo producto y devuelve su ubicación (Location)",
    OperationId = "CreateProducto"
)]
[SwaggerResponse(201, "Producto creado correctamente", typeof(ProductoResponseDto))]
[SwaggerResponse(400, "Datos inválidos", typeof(ValidationProblemDetails))]
public async Task<ActionResult<ProductoResponseDto>> Create(
    [FromBody, SwaggerRequestBody("Datos del nuevo producto", Required = true)] CreateProductoDto dto)
{
    var producto = await _service.CreateAsync(dto);

    // 201 + Location apuntando al recurso recién creado (GET /api/productos/{id})
    return CreatedAtAction(
        nameof(GetById),
        new { id = producto.Id },
        producto);
}
```

> 📝 **Nota (201 vs 200):** **201 Created** = el `POST` creó un recurso nuevo (aquí, con `CreatedAtAction` que añade el header `Location`). **200 OK** = operación correcta sin recurso nuevo (por ejemplo, `PUT` que actualiza, o un login). Documentar bien los códigos en Swagger evita clientes que esperen `200` y rompan con `201`.

## 24.4. Documentar modelos y DTOs

Los modelos también se documentan con XMLDoc y data annotations: Swagger usa esas descripciones para mostrar cada campo del cuerpo de la petición y la respuesta.

```csharp
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

/// <summary>
/// DTO para crear un producto
/// </summary>
[SwaggerSchema(Description = "Datos necesarios para crear un producto")]
public record CreateProductoDto
{
    /// <summary>
    /// Nombre del producto
    /// </summary>
    /// <example>Iron Man</example>
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres")]
    [SwaggerSchema(Description = "Nombre del producto", Nullable = false)]
    public string Nombre { get; init; } = string.Empty;

    /// <summary>
    /// Precio del producto en euros
    /// </summary>
    /// <example>29.99</example>
    [Required(ErrorMessage = "El precio es obligatorio")]
    [Range(0.01, 9999.99, ErrorMessage = "El precio debe estar entre 0.01 y 9999.99")]
    [SwaggerSchema(Description = "Precio del producto en euros", Format = "decimal", Nullable = false)]
    public decimal Precio { get; init; }

    /// <summary>
    /// Categoria del producto
    /// </summary>
    /// <example>Marvel</example>
    [Required(ErrorMessage = "La categoria es obligatoria")]
    [SwaggerSchema(Description = "Categoria a la que pertenece el producto", Nullable = false)]
    public string Categoria { get; init; } = string.Empty;

    /// <summary>
    /// URL de la imagen del producto
    /// </summary>
    /// <example>https://example.com/images/ironman.jpg</example>
    [Url(ErrorMessage = "La imagen debe ser una URL valida")]
    [SwaggerSchema(Description = "URL de la imagen del producto", Format = "uri", Nullable = true)]
    public string? Imagen { get; init; }
}
```

> 💡 **Consejo:** Usa el tag `<example>` para mostrar valores de ejemplo en Swagger. Esto ayuda mucho a los desarrolladores que consumen tu API.

## 24.5. Documentar autenticación JWT

Para que Swagger permita probar endpoints protegidos hay que añadirle el esquema de seguridad JWT y el botón de autorización.

```csharp
builder.Services.AddSwaggerGen(options =>
{
    // Definir esquema de seguridad JWT
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese 'Bearer' seguido de un espacio y el token JWT."
    });

    // Aplicar esquema de seguridad globalmente
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
```

📌 **Ejemplo real:** La API de Spotify usa OAuth2 con un flujo similar. Cuando pruebas endpoints protegidos en su documentación, haces clic en "Authorize", introduces tu token y puedes probar endpoints que requieren autenticación.

Pasos para usar autenticación en Swagger UI:
1. Hacer clic en "Authorize" (botón con candado)
2. Ingresar token: `Bearer eyJhbGciOiJIUzI1NiIs...`
3. Probar endpoints protegidos

## 24.6. Documentar respuestas de error

Prioriza **`ProblemDetails`** (estándar **RFC 9457**, que sustituye al antiguo RFC 7807): es el formato de error por defecto de ASP.NET Core, así que Swagger ya lo conoce y los clientes pueden tratarlo de forma genérica.

```csharp
// Program.cs — habilita ProblemDetails globalmente (UseExceptionHandler, 404, etc.)
builder.Services.AddProblemDetails();
```

```csharp
[HttpGet("{id}")]
[SwaggerOperation(Summary = "Obtener producto por ID")]
[SwaggerResponse(200, "Producto encontrado", typeof(ProductoResponseDto))]
[SwaggerResponse(404, "Producto no encontrado", typeof(ProblemDetails))]
[SwaggerResponse(500, "Error interno del servidor", typeof(ProblemDetails))]
public async Task<ActionResult<ProductoResponseDto>> GetById(int id)
{
    var producto = await _service.GetByIdAsync(id);
    if (producto is null)
    {
        // RFC 9457: type + title + status + detail + instance
        return NotFound(new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc9457#section-3",
            Title = "Producto no encontrado",
            Status = StatusCodes.Status404NotFound,
            Detail = $"No existe un producto con ID {id}",
            Instance = $"/api/productos/{id}"
        });
    }
    return Ok(producto);
}
```

Para errores de validación, usa `ValidationProblemDetails` (incluye el diccionario `errors`):

```csharp
return BadRequest(new ValidationProblemDetails(ModelState)
{
    Title = "Datos inválidos",
    Status = StatusCodes.Status400BadRequest
});
```

> 💡 **Consejo:** Un modelo personalizado tipo `ApiError` es una **alternativa válida**, pero si lo usas, alinéalo con `ProblemDetails` (mismos campos `title`, `status`, `detail`) o añádele como extensión. Inventar un formato propio obliga a cada cliente a escribir su parser de errores.

## 24.7. Versionado de API

Paquetes actuales (los antiguos `Microsoft.AspNetCore.Mvc.Versioning*` están obsoletos):

```bash
dotnet add package Asp.Versioning.Http
dotnet add package Asp.Versioning.Mvc.ApiExplorer
```

```csharp
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "ProductosApp API", Version = "v1" });
    options.SwaggerDoc("v2", new OpenApiInfo { Title = "ProductosApp API", Version = "v2" });
});
```

> ⚠️ **Advertencia — Sin `GroupNameFormat`, Swagger sale casi vacío:** El `GroupNameFormat` **no es decorativo**. Sin él, el nombre de grupo de cada endpoint es `ApiVersion.ToString()` → `"1.0"`, y Swashbuckle solo mete en `SwaggerDoc("v1")` los grupos llamados `"v1"`. El resultado es un Swagger con **solo los endpoints sin versionar** (p. ej. `/version`) y **todos los controllers versionados desaparecidos**. Con `'v'VVV`, la versión `1.0` se formatea como `v1` (el minor se omite cuando es `0`) y vuelve a coincidir con el `SwaggerDoc("v1")`.

📌 **Ejemplo real:** este es un bug más común de lo que parece. `.AddApiExplorer()` sin `GroupNameFormat` hace que el Swagger muestre **1 ruta** (`/version`) en lugar de las **22 rutas / 39 operaciones** reales. Una sola línea (`options.GroupNameFormat = "'v'VVV";`) lo arregla.

```csharp
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ProductosV1Controller : ControllerBase
{
    // Endpoints V1 - Version original
}

[ApiController]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ProductosV2Controller : ControllerBase
{
    // Endpoints V2 - Nueva version con cambios
}
```

📌 **Ejemplo real:** La API de GitHub usa versionado en la URL (`/api/v3/`). Cuando lanzan cambios breaking, crean una nueva versión sin romper las aplicaciones existentes.

## 24.8. Ejemplos de solicitudes

`SwaggerRequestExample` e `IExamplesProvider<T>` **no existen en Swashbuckle base**: vienen del paquete **`Swashbuckle.AspNetCore.Filters`**:

```bash
dotnet add package Swashbuckle.AspNetCore.Filters
```

```csharp
// Registrar los ejemplos (junto al resto de AddSwaggerGen)
builder.Services.AddSwaggerGen(options =>
{
    options.ExampleFilters();
});
builder.Services.AddSwaggerExamplesFromAssemblyOf<CreateProductoExample>();
```

```csharp
[HttpPost]
[SwaggerOperation(
    Summary = "Crear producto",
    Description = "Crea un nuevo producto con los datos proporcionados"
)]
[SwaggerRequestExample(typeof(CreateProductoDto), typeof(CreateProductoExample))]
public async Task<ActionResult<ProductoResponseDto>> Create(CreateProductoDto dto)
{
    // ...
}

public class CreateProductoExample : IExamplesProvider<CreateProductoDto>
{
    public CreateProductoDto GetExamples()
    {
        return new CreateProductoDto
        {
            Nombre = "Iron Man Mark 50",
            Precio = 34.99m,
            Categoria = "Marvel",
            Imagen = "https://example.com/images/ironman-mk50.jpg"
        };
    }
}
```

## 24.9. Filtros personalizados

Los filtros permiten personalizar la documentación que genera Swagger automáticamente.

```csharp
public class SwaggerExamplesFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.RequestBody?.Content is not null)
        {
            foreach (var content in operation.RequestBody.Content)
            {
                if (content.Key == "application/json")
                {
                    content.Value.Example = new OpenApiString(@"{
                        ""nombre"": ""Laptop"",
                        ""precio"": 29.99,
                        ""categoria"": ""DC"",
                        ""imagen"": ""https://example.com/batman.jpg""
                    }");
                }
            }
        }
    }
}

// Registrar filtro
builder.Services.AddSwaggerGen(options =>
{
    options.OperationFilter<SwaggerExamplesFilter>();
});
```

## 24.10. Buenas prácticas

| Practica | Descripcion |
|----------|-------------|
| **Documentar todos los endpoints** | Incluye resumenes y descripciones claras |
| **Documentar parametros** | Explica que espera cada parametro y su formato |
| **Documentar respuestas** | Incluye todos los codigos HTTP posibles |
| **Ejemplos claros** | Proporciona ejemplos realistas con valores de ejemplo |
| **Modelos documentados** | Documenta todos los DTOs con descripciones y ejemplos |
| **Versionado** | Versiona tu API desde el inicio para evitar breaking changes |
| **Autenticacion** | Documenta como autenticarse e incluye el esquema JWT |
| **Errores** | Documenta todos los posibles errores que puede retornar la API |
| **CORS restrictivo** | En produccion, permite solo origenes conocidos |
| **Documentacion viva** | Debe mantenerse actualizada con el codigo |

> ⚠️ **Advertencia:** La documentacion desactualizada es peor que ninguna documentacion. Manten siempre sincronizada la documentacion con el codigo.

## 24.11. Reto: documenta la API de FunkoApp

> Antes de irte, documenta completamente la API de Funkos. No escribas codigo: diseña la documentacion.

### Contexto

Vas a documentar una API REST para gestionar una **coleccion de Funkos** con Swagger/OpenAPI.

### Modelo de datos

| Propiedad | Tipo | Obligatorio |
|-----------|------|:-----------:|
| `id` | long | Si (autogenerado) |
| `nombre` | string | Si |
| `precio` | decimal | Si |
| `categoria` | string | Si |
| `imagen` | string | No |
| `creadoEn` | DateTime | Si (autogenerado) |

### Ejercicio

1. Configura Swagger con informacion completa de la API
2. Documenta todos los endpoints con `[SwaggerOperation]`
3. Documenta el `CreateFunkoDto` con atributos de validacion y ejemplos
4. Configura el esquema de seguridad JWT en Swagger
5. Documenta las respuestas de error con `ProblemDetails` (RFC 9457)
6. Configura CORS con politica restrictiva

> 💡 **Consejo:** Usa comentarios XML en todos los modelos y endpoints. Los comentarios XML se convierten automaticamente en la documentacion de Swagger.

---

**Resumen del punto:**

| Concepto | Descripcion |
|----------|-------------|
| **CORS** | Controla el acceso cross-domain entre navegadores y APIs |
| **Swagger/OpenAPI** | Genera documentacion interactiva automaticamente |
| **Swashbuckle** | Integra Swagger en ASP.NET Core |
| **Atributos de documentacion** | Enriquecen la documentacion con descripciones |
| **JWT en Swagger** | Permite probar endpoints protegidos desde la UI |
| **Versionado de API** | Permite evolucionar sin romper clientes existentes |
| **Filtros personalizados** | Permiten customizar la documentacion generada |

**¿Qué viene después?**

En el siguiente punto veremos **Configuracion de Entornos**: cómo manejar diferentes configuraciones para desarrollo y producción, variables de entorno, User Secrets y Azure App Configuration.
