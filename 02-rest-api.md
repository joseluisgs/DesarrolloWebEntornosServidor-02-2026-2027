# 2. APIs REST y Controladores en ASP.NET Core

## Indice

- [2. APIs REST y Controladores en ASP.NET Core](#2-apis-rest-y-controladores-en-aspnet-core)
  - [2.1. Fundamentos de REST](#21-fundamentos-de-rest)
    - [2.1.1. Principios Fundamentales de REST](#211-principios-fundamentales-de-rest)
    - [2.1.2. Arquitectura Cliente-Servidor en REST](#212-arquitectura-cliente-servidor-en-rest)
    - [2.1.3. Componentes de una API REST](#213-componentes-de-una-api-rest)
      - [Request (Solicitud)](#request-solicitud)
      - [Response (Respuesta)](#response-respuesta)
  - [2.2. Recursos y Endpoints](#22-recursos-y-endpoints)
    - [2.2.1. Convenciones para Diseñar Endpoints](#221-convenciones-para-diseñar-endpoints)
    - [2.2.2. Ejemplos de Endpoints](#222-ejemplos-de-endpoints)
  - [2.3. Métodos HTTP](#23-métodos-http)
    - [2.3.1. Descripción Detallada de Métodos](#231-descripción-detallada-de-métodos)
  - [2.4. Respuestas HTTP](#24-respuestas-http)
    - [2.4.1. Codigos de Estado Comunes](#241-codigos-de-estado-comunes)
    - [2.4.2. Ejemplo de Respuestas en ASP.NET Core](#242-ejemplo-de-respuestas-en-aspnet-core)
  - [2.5. Controladores REST](#25-controladores-rest)
    - [2.5.1. Anatomia de un Controlador](#251-anatomia-de-un-controlador)
    - [2.5.2. Routing por Atributos vs Convenciones](#252-routing-por-atributos-vs-convenciones)
    - [2.5.3. Verbos HTTP y Metodos de Accion](#253-verbos-http-y-metodos-de-accion)
  - [2.6. Model Binding](#26-model-binding)
    - [2.6.1. Fuentes de Datos para Binding](#261-fuentes-de-datos-para-binding)
    - [2.6.2. Validación Automática de Modelos](#262-validación-automática-de-modelos)
  - [2.7. Tipos de Respuesta](#27-tipos-de-respuesta)
    - [2.7.1. IActionResult vs ActionResult vs Typed Results](#271-iactionresult-vs-actionresult-vs-typed-results)
    - [2.7.2. Helpers de Respuesta](#272-helpers-de-respuesta)
  - [2.8. Mejores Practicas Avanzadas](#28-mejores-practicas-avanzadas)
    - [2.8.1. Versionado de APIs](#281-versionado-de-apis)
    - [2.8.2. Paginacion](#282-paginacion)
    - [2.8.3. HATEOAS](#283-hateoas)
    - [2.8.4. Sobres de Respuesta](#284-sobres-de-respuesta)
    - [2.8.5. Filtrado y Cadenas de Consulta](#285-filtrado-y-cadenas-de-consulta)
    - [2.8.6. ETag para Almacenamiento en Cache](#286-etag-para-almacenamiento-en-cache)
  - [2.9. Manejo de Errores](#29-manejo-de-errores)
  - [2.10. Filtros en Controladores](#210-filtros-en-controladores)
  - [2.11. Resumen](#211-resumen)
  - [2.12. Ejercicio Propuesto](#212-ejercicio-propuesto)

---

## 2.1. Fundamentos de REST

**REST (Representational State Transfer)** es un estilo de arquitectura para sistemas de software que se utiliza principalmente en el desarrollo de servicios web. Los servicios que siguen los principios de REST se denominan **servicios web RESTful**. REST usa HTTP de manera semántica, donde cada recurso se identifica por una URI y las operaciones se realizan mediante métodos HTTP estandarizados.

🧠 **Analogía**: REST es como un restaurante con un menú fijo. Pides lo que quieres (GET), pides algo nuevo (POST), cambias tu pedido (PUT), o cancelas (DELETE). Todo sigue un protocolo simple y predecible, como el menú de un restaurante donde cada plato tiene su número y tú solo necesitas pedir el número y decir qué quieres hacer con él.

```mermaid
flowchart LR
    subgraph "Cliente"
        A[App Web<br/>Mobile<br/>Postman]
    end
    
    subgraph "Internet"
        B[HTTP<br/>Stateless]
    end
    
    subgraph "Servidor REST"
        C[Recursos<br/>Endpoints]
    end
    
    A -->|Solicitud HTTP| B -->|Request| C
    C -->|Response| B -->|Respuesta HTTP| A
    
    style A fill:#2196F3
    style C fill:#4CAF50
```

### 2.1.1. Principios Fundamentales de REST

REST se basa en **seis principios fundamentales** que definen su arquitectura. Estos principios no son opcionales, son la esencia de lo que hace que una API sea verdaderamente RESTful. Entender estos principios te ayudará a diseñar APIs que sean escalables, mantenibles y fáciles de usar.

```mermaid
flowchart TD
    A[Principios REST] --> B[1. Cliente-Servidor]
    A --> C[2. Sin Estado Stateless]
    A --> D[3. Cacheable]
    A --> E[4. Sistema en Capas]
    A --> F[5. Interfaz Uniforme]
    A --> G[6. Código bajo demanda]
    
    B --> B1[Separación responsabilidades]
    C --> C1[Cada petición independiente]
    D --> D1[Mejor eficiencia]
    E --> E1[Proxies, balanceadores]
    F --> F1[Recursos + Métodos HTTP]
    G --> G1[Opcional extensiones]
    
    style B fill:#FF9800
    style C fill:#4CAF50
    style D fill:#2196F3
    style E fill:#9C27B0
    style F fill:#F44336
    style G fill:#607D8B
```

**1. Arquitectura Cliente-Servidor:**

El **cliente** envía solicitudes HTTP al servidor y el **servidor** procesa la solicitud y devuelve una respuesta HTTP. La separación permite que cliente y servidor evolucionen de forma independiente, lo que es crucial para el desarrollo moderno donde la interfaz de usuario puede cambiar frecuentemente mientras la lógica de negocio permanece estable.

📝 **Nota del Profesor**: Esta separación es clave para la escalabilidad. Puedes cambiar el cliente (de web a móvil) sin afectar el servidor, y viceversa. Esto permite que equipos diferentes trabajen en paralelo en el cliente y el servidor.

**2. Sin Estado (Stateless):**

Cada solicitud del cliente al servidor debe contener **toda la información necesaria** para procesarla. El servidor **no almacena ningún estado** del cliente entre solicitudes, lo que mejora la **escalabilidad** y simplifica la arquitectura del servidor. Esto significa que cada petición es completamente independiente de las demás.

🧠 **Analogía**: Cada carta que envías por correo debe contener toda la información. El cartero no recuerda cartas anteriores. Si necesitas contexto, debes incluirlo en cada carta. Esto permite que cualquier cartero pueda entregar cualquier carta, no solo el que conoce tu historial.

**3. Cacheable:**

Las respuestas del servidor pueden ser almacenadas en caché por el cliente o por intermediarios. Esto mejora la **eficiencia** y reduce la carga del servidor. Los encabezados HTTP (`Cache-Control`, `ETag`) controlan el comportamiento del caché y permiten indicar si una respuesta puede ser cacheada y por cuánto tiempo.

💡 **Tip del Examinador**: El caching es una de las ventajas más importantes de REST para rendimiento. En el examen, recuerda que las respuestas GET deben ser cacheables mientras que POST, PUT, DELETE y PATCH no lo son generalmente.

**4. Sistema en Capas:**

Un servicio REST puede estar compuesto por **varias capas** de servidores (proxies, balanceadores de carga, gateways). Cada capa tiene una responsabilidad específica. Esto mejora la **escalabilidad** y **seguridad** al permitir agregar capas intermedias sin afectar ni al cliente ni al servidor final.

**5. Interfaz Uniforme:**

Los servicios REST utilizan un conjunto **limitado y bien definido de métodos HTTP** (GET, POST, PUT, DELETE, PATCH). Los recursos se identifican por **URIs (Uniform Resource Identifiers)** únicas y se utilizan **representaciones estándar** (JSON, XML) para intercambiar datos. Esta uniformidad es lo que hace que las APIs REST sean predecibles y fáciles de usar.

**6. Código bajo demanda (opcional):**

El servidor puede enviar código ejecutable al cliente (JavaScript, por ejemplo). Esta característica es opcional y raramente usada en APIs REST puras. Su inclusión como principio indica que REST no es exclusivamente para APIs que devuelven datos, sino que puede extenderse para incluir lógica ejecutable.

### 2.1.2. Arquitectura Cliente-Servidor en REST

La arquitectura REST sigue el patron clasico de cliente-servidor, pero con caracteristicas especificas que la hacen ideal para APIs modernas. El modelo sin estado permite que cualquier servidor de la pool pueda atender cualquier peticion, lo que facilita el balanceo de carga y la escalabilidad horizontal.

```mermaid
sequenceDiagram
    participant C as Cliente
    participant L as Load Balancer
    participant S1 as Servidor 1
    participant S2 as Servidor 2
    participant B as Base de Datos
    
    C->>L: GET /api/usuarios/1
    L->>S1: GET /api/usuarios/1
    S1->>B: SELECT * FROM usuarios WHERE id=1
    B->>S1: {id:1, nombre:"Juan"}
    S1->>L: {id:1, nombre:"Juan"}
    L->>C: {id:1, nombre:"Juan"}
    
    Note over C,L: Sin estado - cada peticion es independiente
    Note over S1,B: El servidor puede cambiar, el cliente no lo nota
```

Esta arquitectura permite que si el Servidor 1 falla, el Load Balancer puede enviar la siguiente peticion al Servidor 2 sin que el cliente note la diferencia, porque toda la informacion necesaria viene en la peticion misma.

### 2.1.3. Componentes de una API REST

Una API REST se comunica mediante solicitudes y respuestas HTTP. Cada peticion y respuesta tiene componentes específicos que debes entender para diseñar APIs efectivas.

#### Request (Solicitud)

Una solicitud REST se compone de cuatro elementos principales que viajan juntos para formar una peticion completa al servidor.

```mermaid
flowchart LR
    subgraph Request
        A[Método HTTP] --> B[URI]
        B --> C[Headers]
        C --> D[Body]
    end
    
    style A fill:#2196F3
    style B fill:#4CAF50
    style C fill:#FF9800
    style D fill:#9C27B0
```

**Componentes de una solicitud HTTP:**

1. **Metodo HTTP**: Indica la accion a realizar sobre el recurso. Los metodos son verbos que describen la operacion.

| Metodo   | Descripcion             | Idempotente | Seguro |
| -------- | ----------------------- | ----------- | ------ |
| `GET`    | Obtener un recurso      | ✅ Si        | ✅ Si   |
| `POST`   | Crear un nuevo recurso  | ❌ No        | ❌ No   |
| `PUT`    | Actualizar/reemplazar   | ✅ Si        | ❌ No   |
| `PATCH`  | Actualizar parcialmente | ❌ No        | ❌ No   |
| `DELETE` | Eliminar un recurso     | ✅ Si        | ❌ No   |

2. **URI**: Identifica el recurso de manera unica en el servidor.

```
https://api.ejemplo.com/api/usuarios/1
├── Protocolo: https://
├── Dominio: api.ejemplo.com
└── Ruta: /api/usuarios/1 (recurso específico)
```

3. **Headers**: Metadatos sobre la solicitud que proporcionan informacion adicional sobre el contexto de la peticion.

| Header            | Descripción                | Ejemplo            |
| ----------------- | -------------------------- | ------------------ |
| `Content-Type`    | Tipo de contenido enviado  | `application/json` |
| `Authorization`   | Token de autenticación     | `Bearer eyJhbG...` |
| `Accept`          | Tipo de contenido aceptado | `application/json` |
| `User-Agent`      | Información del cliente    | `MiApp/1.0`        |
| `Accept-Language` | Idioma preferido           | `es-ES`            |

4. **Body**: Datos enviados al servidor (opcional, en POST, PUT, PATCH). Para APIs REST modernas, el body suele ser JSON.

**Ejemplo de solicitud:**

```http
POST /api/usuarios HTTP/1.1
Host: api.ejemplo.com
Content-Type: application/json
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Accept: application/json
User-Agent: MiApp/1.0

{
  "nombre": "Juan Perez",
  "email": "juan@ejemplo.com",
  "edad": 30
}
```

#### Response (Respuesta)

Una respuesta REST incluye tres elementos principales que el cliente recibe después de procesar su solicitud.

```mermaid
flowchart LR
    subgraph Response
        A[Código Estado] --> B[Headers]
        B --> C[Body JSON]
    end
    
    style A fill:#F44336
    style B fill:#FF9800
    style C fill:#4CAF50
```

**Componentes de una respuesta HTTP:**

1. **Código de Estado HTTP**: Indica el resultado de la solicitud de manera estandarizada.

| Rango   | Significado        | Ejemplos      |
| ------- | ------------------ | ------------- |
| **2xx** | Éxito              | 200, 201, 204 |
| **3xx** | Redirección        | 301, 304      |
| **4xx** | Error del Cliente  | 400, 401, 404 |
| **5xx** | Error del Servidor | 500, 503      |

2. **Headers**: Metadatos sobre la respuesta que proporcionan contexto adicional.

| Header           | Descripción            | Ejemplo            |
| ---------------- | ---------------------- | ------------------ |
| `Content-Type`   | Tipo de contenido      | `application/json` |
| `Content-Length` | Tamaño del body        | `125`              |
| `Location`       | URL del recurso creado | `/api/usuarios/1`  |
| `Cache-Control`  | Directivas de caché    | `max-age=3600`     |
| `ETag`           | Versión del recurso    | `"abc123"`         |

3. **Body**: Representación del recurso en JSON/XML que contiene los datos solicitados o el resultado de la operación.

**Ejemplo de respuesta:**

```http
HTTP/1.1 201 Created
Content-Type: application/json
Content-Length: 85
Location: /api/usuarios/1

{
  "id": 1,
  "nombre": "Juan Pérez",
  "email": "juan@ejemplo.com",
  "creado_en": "2024-01-15T10:30:00Z"
}
```

## 2.2. Recursos y Endpoints

Un **recurso** en REST es cualquier objeto que queremos gestionar (usuarios, productos, pedidos, etc.). Cada recurso tiene una **URL única** (endpoint) que lo identifica. Los endpoints son la interfaz pública de tu API y deben ser diseñados cuidadosamente para ser intuitivos y consistentes.

### 2.2.1. Convenciones para Diseñar Endpoints

**Reglas de oro para diseñar APIs REST profesionales:**

✅ **Usa sustantivos en plural** para los nombres de recursos. Esto es una convención ampliamente aceptada que hace que las URLs sean consistentes y predecibles.

```bash
# Correcto
GET    /api/usuarios      # Lista de usuarios
GET    /api/productos     # Lista de productos
POST   /api/pedidos       # Crear pedido

# Incorrecto
GET    /api/usuario       # No es consistente con otras rutas
GET    /api/obtenerUsuarios # Verboso, usar métodos HTTP
```

✅ **Evita verbos en las URLs** porque los verbos ya están representados por los métodos HTTP. Las URLs deben identificar recursos, no acciones.

```bash
# Correcto
GET    /api/usuarios      # Obtener usuarios
POST   /api/usuarios      # Crear usuario

# Incorrecto
GET    /api/getUsuarios   # Redundante, GET ya implica obtener
POST   /api/crearUsuario  # Verboso, POST ya implica crear
```

✅ **Usa IDs en la URL** para identificar recursos específicos dentro de una colección.

```bash
GET /api/usuarios/1       # Usuario con ID 1
GET /api/productos/123    # Producto con ID 123
DELETE /api/pedidos/456   # Eliminar pedido con ID 456
```

✅ **Usa recursos anidados** para representar relaciones jerárquicas entre recursos.

```bash
GET /api/usuarios/1/pedidos       # Pedidos del usuario 1
GET /api/productos/5/reviews      # Reviews del producto 5
GET /api/pedidos/10/items         # Items del pedido 10
GET /api/categorias/3/productos   # Productos de la categoría 3
```

✅ **Mantén URLs consistentes y predecibles** para que los desarrolladores puedan adivinar los endpoints sin consultar la documentación.

```bash
/api/usuarios           # Colección completa
/api/usuarios/1         # Recurso individual
/api/usuarios/1/pedidos # Recurso relacionado (pedidos del usuario)
/api/pedidos            # Otra colección
/api/pedidos/2          # Recurso individual
```

### 2.2.2. Ejemplos de Endpoints

La siguiente tabla muestra los endpoints típicos para un recurso CRUD completo en una API REST profesional.

| Recurso                 | Endpoint                  | Método | Descripción                   |
| ----------------------- | ------------------------- | ------ | ----------------------------- |
| Usuarios                | `/api/usuarios`           | GET    | Obtener todos los usuarios    |
| Usuario específico      | `/api/usuarios/1`         | GET    | Obtener usuario con ID 1      |
| Crear usuario           | `/api/usuarios`           | POST   | Crear nuevo usuario           |
| Actualizar usuario      | `/api/usuarios/1`         | PUT    | Actualizar usuario con ID 1   |
| Actualizar parcialmente | `/api/usuarios/1`         | PATCH  | Actualizar campos específicos |
| Eliminar usuario        | `/api/usuarios/1`         | DELETE | Eliminar usuario con ID 1     |
| Pedidos de usuario      | `/api/usuarios/1/pedidos` | GET    | Obtener pedidos del usuario 1 |

📝 **Nota del Profesor**: Al diseñar endpoints, piensa en cómo los consumirá un desarrollador frontend. Deben ser intuitivos y seguir las convenciones de la industria. Si un desarrollador puede adivinar tus endpoints sin leer la documentación, has un buen trabajo.

## 2.3. Métodos HTTP

Los métodos HTTP (también llamados verbos HTTP) son la forma en que REST expresa las operaciones sobre recursos. Usar el método correcto es fundamental para crear APIs RESTful correctas y predecibles.

### 2.3.1. Descripción Detallada de Métodos

**GET**: Obtener información de un recurso. Debe ser idempotente (llamarlo múltiples veces tiene el mismo efecto que llamarlo una vez) y seguro (no modifica el recurso). No debe tener body en la petición.

**POST**: Crear un nuevo recurso. El servidor decide la URL del nuevo recurso y la devuelve en el header Location. No es idempotente (llamar dos veces crea dos recursos). Idealmente devuelve 201 Created.

```csharp
// POST /api/usuarios
[HttpPost]
public ActionResult<Usuario> Create(UsuarioDto dto)
{
    if (dto == null)
        return BadRequest();
        
    var usuario = new Usuario 
    { 
        Id = _usuarios.Max(u => u.Id) + 1,
        Nombre = dto.Nombre,
        Email = dto.Email
    };
    _usuarios.Add(usuario);
    
    return CreatedAtAction(nameof(GetById), new { id = usuario.Id }, usuario);
}
```

**PUT**: Actualizar o reemplazar un recurso completo. Es idempotente (llamar multiples veces tiene el mismo efecto). Si el recurso existe, se reemplaza completamente. Si no existe, algunos servidores crean uno nuevo (debes documentar el comportamiento).

```csharp
// PUT /api/usuarios/1
[HttpPut("{id}")]
public IActionResult Update(int id, UsuarioDto dto)
{
    var usuario = _usuarios.FirstOrDefault(u => u.Id == id);
    if (usuario == null)
        return NotFound();
    
    // Reemplazo completo
    usuario.Nombre = dto.Nombre;
    usuario.Email = dto.Email;
    
    return NoContent();
}
```

**PATCH**: Actualizar parcialmente un recurso. A diferencia de PUT, PATCH solo modifica los campos especificados en el documento JSON que se envia. Es mas eficiente para actualizaciones pequenas pero requiere un formato especial.

```csharp
// PATCH /api/usuarios/1
[HttpPatch("{id}")]
public IActionResult PartialUpdate(int id, JsonPatchDocument<Usuario> patchDoc)
{
    var usuario = _usuarios.FirstOrDefault(u => u.Id == id);
    if (usuario == null)
        return NotFound();
    
    patchDoc.ApplyTo(usuario);
    return NoContent();
}
```

**DELETE**: Eliminar un recurso. Es idempotente (eliminar algo que ya no existe devuelve exito en llamadas subsecuentes). Devuelve 204 No Content si se elimino correctamente.

```csharp
// DELETE /api/usuarios/1
[HttpDelete("{id}")]
public IActionResult Delete(int id)
{
    var usuario = _usuarios.FirstOrDefault(u => u.Id == id);
    if (usuario == null)
        return NotFound();
    
    _usuarios.Remove(usuario);
    return NoContent();
}
```

💡 **Tip del Examinador**: En el examen, asegurate de usar el metodo HTTP correcto para cada operacion. PUT es para reemplazo completo, PATCH para actualizacion parcial. Si tienes dudas, usa PUT para actualizaciones grandes y PATCH para pequenas.

## 2.4. Respuestas HTTP

Las respuestas HTTP utilizan codigos de estado estandarizados para comunicar el resultado de la peticion. Usar los codigos correctos es fundamental para que los clientes de tu API puedan manejar los resultados apropiadamente.

### 2.4.1. Codigos de Estado Comunes

**2xx - Exito:**

| Codigo | Significado | Descripcion                                  |
| ------ | ----------- | -------------------------------------------- |
| 200    | OK          | Solicitud exitosa, respuesta en body         |
| 201    | Created     | Recurso creado exitosamente                  |
| 204    | No Content  | Solicitud exitosa sin contenido que devolver |

**3xx - Redireccion:**

| Codigo | Significado        | Descripcion            |
| ------ | ------------------ | ---------------------- |
| 301    | Moved Permanently  | Redireccion permanente |
| 304    | Not Modified       | Usar version cacheada  |
| 307    | Temporary Redirect | Redireccion temporal   |

**4xx - Errores del Cliente:**

| Codigo | Significado          | Descripcion                                 |
| ------ | -------------------- | ------------------------------------------- |
| 400    | Bad Request          | Solicitud mal formada o datos invalidos     |
| 401    | Unauthorized         | Autenticacion requerida                     |
| 403    | Forbidden            | Autenticado pero sin permisos               |
| 404    | Not Found            | Recurso no existe                           |
| 405    | Method Not Allowed   | Metodo HTTP no permitido para este recurso  |
| 409    | Conflict             | Conflicto con el estado actual del recurso  |
| 422    | Unprocessable Entity | Datos válidos pero reglas de negocio fallan |
| 429    | Too Many Requests    | Rate limiting, demasiadas peticiones        |

**5xx - Errores del Servidor:**

| Codigo | Significado           | Descripcion                          |
| ------ | --------------------- | ------------------------------------ |
| 500    | Internal Server Error | Error generico del servidor          |
| 502    | Bad Gateway           | Gateway invalido                     |
| 503    | Service Unavailable   | Servicio temporalmente no disponible |

### 2.4.2. Ejemplo de Respuestas en ASP.NET Core

```csharp
[HttpGet("{id}")]
public ActionResult<Usuario> GetById(int id)
{
    var usuario = _usuarios.FirstOrDefault(u => u.Id == id);
    
    if (usuario == null)
        return NotFound();  // 404
    
    return Ok(usuario);     // 200
}

[HttpPost]
public ActionResult<Usuario> Create(UsuarioDto dto)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState);  // 400
    
    var usuario = new Usuario { /* ... */ };
    _usuarios.Add(usuario);
    
    return CreatedAtAction(nameof(GetById), new { id = usuario.Id }, usuario);  // 201
}

[HttpDelete("{id}")]
public IActionResult Delete(int id)
{
    var usuario = _usuarios.FirstOrDefault(u => u.Id == id);
    if (usuario == null)
        return NotFound();  // 404
    
    _usuarios.Remove(usuario);
    return NoContent();     // 204
}
```

## 2.5. Controladores REST

Los controladores son el punto de entrada de las peticiones HTTP en una API ASP.NET Core. Son clases que reciben las peticiones, delegan el procesamiento a los servicios de negocio, y devuelven respuestas HTTP apropiadas. Un controlador bien diseñado es delgado, delegando toda la logica de negocio a los servicios.

### 2.5.1. Anatomia de un Controlador

Un controlador es una clase que hereda de **ControllerBase** (para APIs sin vistas) o **Controller** (para APIs con vistas MVC). En APIs REST usamos exclusivamente ControllerBase porque no necesitamos views, solo JSON APIs.

**Estructura basica de un controlador:**

```csharp
using Microsoft.AspNetCore.Mvc;
using EjemploApi.Core.Interfaces;

namespace EjemploApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductosController(
    IProductoService productoService,
    ILogger<ProductosController> logger) : ControllerBase
{
    private readonly IProductoService _productoService = productoService;
    private readonly ILogger<ProductosController> _logger = logger;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        _logger.LogInformation("GET /api/productos - Obteniendo todos");
        
        var productos = await _productoService.GetAllAsync();
        
        return Ok(productos);
    }
    
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var producto = await _productoService.GetByIdAsync(id);
        
        if (producto == null)
            return NotFound(new { error = $"Producto {id} no encontrado" });
        
        return Ok(producto);
    }
}
```

**Partes del controlador:**

La primera parte es la **declaracion de la clase** con sus atributos. El atributo `[ApiController]` activa comportamientos convenientes como la validacion automatica de modelos y el binding de parametros especializado. El atributo `[Route("api/[controller]")]` define la ruta base del controlador, donde `[controller]` se reemplaza por el nombre de la clase sin el sufijo "Controller".

La segunda parte son los **servicios injectados** en el constructor primario. Estos servicios contienen la logica de negocio y son la unica dependencia que un controlador debe tener. El controlador debe ser "delgado" (thin), lo que significa que no contiene logica de negocio, solo coordina llamadas a servicios.

La tercera parte son los **metodos de accion** marcados con atributos de verbo HTTP como `[HttpGet]`, `[HttpPost]`, `[HttpPut]`, `[HttpDelete]`. Cada metodo maneja un tipo específico de operacion HTTP.

**Diferencia entre Controller y ControllerBase:**

```csharp
// ✅ CORRECTO para API REST
[ApiController]
[Route("api/[controller]")]
public class ProductosController : ControllerBase
{
    // Solo metodos relacionados con API
}

// ❌ NO NECESARIO para API REST (pero funciona)
[ApiController]
[Route("api/[controller]")]
public class ProductosController : Controller
{
    // Hereda funcionalidad extra que no usas (vistas, etc.)
}
```

📝 **Nota del Profesor**: Siempre usa ControllerBase para APIs REST. Controller esta diseñado para MVC con vistas y añade funcionalidad innecesaria que puede causar confusion o pequeno overhead.

### 2.5.2. Routing por Atributos vs Convenciones

ASP.NET Core soporta dos estilos de routing: **routing por atributos** (usando atributos en los metodos) y **routing por convenciones** (definiendo rutas en Program.cs). El routing por atributos es mas explícito, facil de entender, y se integra mejor con la documentacion automatica de Swagger.

**Routing por atributos:**

Con routing por atributos, cada metodo de accion tiene un atributo que define su ruta relativa. Esto hace el codigo auto-documentado porque la ruta esta justo encima del metodo que la maneja.

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductosController : ControllerBase
{
    // GET /api/productos
    [HttpGet]
    public async Task<IActionResult> GetAll() { }
    
    // GET /api/productos/5
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id) { }
    
    // GET /api/productos/categoria/5
    [HttpGet("categoria/{categoriaId:long}")]
    public async Task<IActionResult> GetByCategoria(long categoriaId) { }
    
    // POST /api/productos
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProductoDto dto) { }
    
    // PUT /api/productos/5
    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] ProductoDto dto) { }
    
    // DELETE /api/productos/5
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id) { }
}
```

**Templates de ruta:**

Los templates de ruta permiten definir parametros, restricciones, y valores opcionales. Los parametros se definen entre llaves `{}` y pueden tener restricciones para validar el formato.

```csharp
[ApiController]
[Route("api/[controller]")]
public class PedidosController : ControllerBase
{
    // Parametro simple
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id) { }
    
    // Multiples parametros
    [HttpGet("cliente/{clienteId}/estado/{estado}")]
    public async Task<IActionResult> GetByClienteYEstado(long clienteId, string estado) { }
    
    // Parametro opcional (con ?)
    [HttpGet("buscar")]
    public async Task<IActionResult> Search([FromQuery] string q, [FromQuery] int? page) { }
    
    // Ruta catch-all (captura segmentos multiples)
    [HttpGet("archivos/{**path}")]
    public async Task<IActionResult> GetFile(string path) { }
}
```

**Restricciones de ruta:**

Las restricciones de ruta indican al framework que tipos de valores acepta cada parametro. Si un valor no cumple la restriccion, el framework devuelve 404 en lugar de llamar al metodo.

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductosController : ControllerBase
{
    // long: solo numeros enteros
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id) { }
    
    // int: alias para long
    [HttpGet("int/{id:int}")]
    public async Task<IActionResult> GetByIntId(int id) { }
    
    // guid: UUIDs
    [HttpGet("uuid/{id:guid}")]
    public async Task<IActionResult> GetByGuid(Guid id) { }
    
    // min(length): minimo de caracteres
    [HttpGet("buscar/{termino:minlength(3)}")]
    public async Task<IActionResult> Search(string termino) { }
    
    // range(min, max): rango numerico
    [HttpGet("precio/{precio:range(0, 1000000)}")]
    public async Task<IActionResult> GetByPrice(decimal precio) { }
    
    // regex: expresion regular
    [HttpGet("codigo/{codigo:regex(^[A-Z]{3}-\d{4}$)}")]
    public async Task<IActionResult> GetByCodigo(string codigo) { }
}
```

### 2.5.3. Verbos HTTP y Metodos de Accion

Los verbos HTTP indican la accion que se quiere realizar sobre un recurso. Usar el verbo correcto es fundamental para crear APIs RESTful correctas y predecibles.

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductosController(IProductoService service) : ControllerBase
{
    // GET: Obtener recursos
    [HttpGet]
    public async Task<IActionResult> GetAll() 
    {
        var productos = await service.GetAllAsync();
        return Ok(productos);  // 200 OK
    }
    
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var producto = await service.GetByIdAsync(id);
        if (producto == null)
            return NotFound();  // 404
        return Ok(producto);    // 200 OK
    }
    
    // POST: Crear nuevo recurso
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProductoCreateDto dto)
    {
        var producto = await service.CreateAsync(dto);
        return CreatedAtAction(  // 201 Created
            nameof(GetById),
            new { id = producto.Id },
            producto);
    }
    
    // PUT: Reemplazar recurso completo
    [HttpPut("{id:long}")]
    public async Task<IActionResult> Put(long id, [FromBody] ProductoUpdateDto dto)
    {
        await service.UpdateAsync(id, dto);
        return NoContent();  // 204 No Content
    }
    
    // PATCH: Modificar parcialmente
    [HttpPatch("{id:long}")]
    public async Task<IActionResult> Patch(long id, [FromBody] JsonPatchDocument<ProductoUpdateDto> patch)
    {
        var producto = await service.GetByIdAsync(id);
        if (producto == null)
            return NotFound();
            
        patch.ApplyTo(producto);
        return NoContent();
    }
    
    // DELETE: Eliminar recurso
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await service.DeleteAsync(id);
        return NoContent();  // 204 No Content
    }
}
```

**Cuándo usar cada verbo:**

| Verbo  | Cuando usar                 | Idempotente | Devuelve         |
| ------ | --------------------------- | ----------- | ---------------- |
| GET    | Obtener informacion         | Si          | 200 OK, 404      |
| POST   | Crear nuevo recurso         | No          | 201 Created, 400 |
| PUT    | Reemplazar recurso completo | Si          | 200/204, 404     |
| PATCH  | Modificar parcialmente      | Si*         | 200/204, 404     |
| DELETE | Eliminar recurso            | Si          | 204, 404         |

💡 **Tip del Examinador**: GET debe ser siempre seguro (no modifica datos) e idempotente (llamarlo multiples veces no cambia el resultado). POST no es idempotente (crea nuevos recursos cada vez).

## 2.6. Model Binding

El model binding es el proceso mediante el cual ASP.NET Core convierte los datos de la peticion HTTP (JSON en el body, query parameters, route parameters, headers) en objetos .NET que puedes usar en tus controladores. Entender como funciona el binding es esencial para recibir datos correctamente desde los clientes.

### 2.6.1. Fuentes de Datos para Binding

El binding puede tomar datos de cinco fuentes diferentes, en este orden de precedencia: 1) Formularios (from form), 2) Route values (de la URL), 3) Query strings (parametros en la URL), 4) Headers, 5) Body (JSON/XML).

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductosController : ControllerBase
{
    // De la ruta: /api/productos/5
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        // id viene del template de ruta {id:long}
        var producto = await _service.GetByIdAsync(id);
        return Ok(producto);
    }
    
    // Del query string: /api/productos?categoria=electronica&pagina=1
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string categoria, 
        [FromQuery] int pagina = 1)
    {
        // categoria y pagina vienen de la query string
        var productos = await _service.GetByCategoriaAsync(categoria, pagina);
        return Ok(productos);
    }
    
    // Del body JSON: {"nombre": "Laptop", "precio": 999.99}
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProductoCreateDto dto)
    {
        // dto viene del body de la peticion
        var resultado = await _service.CreateAsync(dto);
        return Ok(resultado);
    }
    
    // De la ruta Y del body
    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(
        long id,                          // De la ruta
        [FromBody] ProductoUpdateDto dto) // Del body
    {
        var resultado = await _service.UpdateAsync(id, dto);
        return Ok(resultado);
    }
    
    // De headers
    [HttpGet("exportar")]
    public async Task<IActionResult> Exportar([FromHeader(Name = "Accept-Language")] string language)
    {
        // language viene del header Accept-Language
        var bytes = await _service.ExportarAsync(language);
        return File(bytes, "application/pdf");
    }
    
    // De formulario (multipart/form-data para uploads)
    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromForm] IFormFile archivo)
    {
        // archivo viene de un formulario multipart
        await _storage.SaveAsync(archivo);
        return Ok();
    }
}
```

**Binding de DTOs complejos:**

Cuando el body de la petición contiene JSON anidado, el binder automáticamente mapea las propiedades del JSON a las propiedades del DTO, siempre que los nombres coincidan (usando camelCase por convención).

```csharp
// DTO para crear un pedido
public class PedidoCreateDto
{
    public long ClienteId { get; set; }
    public DireccionDto DireccionEnvio { get; set; } = new();
    public List<PedidoItemDto> Items { get; set; } = new();
    public string? Notas { get; set; }
}

public class DireccionDto
{
    public string Calle { get; set; } = string.Empty;
    public string Ciudad { get; set; } = string.Empty;
    public string CodigoPostal { get; set; } = string.Empty;
    public string Pais { get; set; } = string.Empty;
}

public class PedidoItemDto
{
    public long ProductoId { get; set; }
    public int Cantidad { get; set; }
}
```

```json
// JSON que el cliente envia
{
  "clienteId": 123,
  "direccionEnvio": {
    "calle": "Calle Principal 123",
    "ciudad": "Madrid",
    "codigoPostal": "28001",
    "pais": "España"
  },
  "items": [
    { "productoId": 1, "cantidad": 2 },
    { "productoId": 5, "cantidad": 1 }
  ],
    "notas": "Después de las 18:00"
}
```

**Personalizar el binding:**

A veces necesitas personalizar cómo se bindean los datos. Puedes usar el atributo `[FromQuery]` para forzar binding desde query string, `[FromRoute]` para forzar desde ruta, o `[FromBody]` para forzar desde el body.

```csharp
[ApiController]
[Route("api/[controller]")]
public class ReportesController : ControllerBase
{
    // Forzar desde query string aunque haya body
    [HttpGet("buscar")]
    public IActionResult Buscar([FromQuery] string termino)
    {
        return Ok(_service.Buscar(termino));
    }
    
    // Forzar desde ruta
    [HttpGet("cliente/{clienteId}/pedidos")]
    public IActionResult GetPedidos([FromRoute] long clienteId, [FromQuery] DateTime? desde)
    {
        return Ok(_service.GetPedidos(clienteId, desde));
    }
}
```

### 2.6.2. Validación Automática de Modelos

Cuando usas `[ApiController]`, ASP.NET Core automáticamente valida los Data Annotations del modelo. Si la validación falla, devuelve 400 Bad Request con los errores sin que tengas que escribir código adicional.

```csharp
public class ProductoCreateDto
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 200 caracteres")]
    public string Nombre { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "El precio es obligatorio")]
    [Range(0.01, 1000000, ErrorMessage = "El precio debe ser mayor a 0")]
    public decimal Precio { get; set; }
    
    [Required]
    public long CategoriaId { get; set; }
    
    public string? Descripcion { get; set; }
}

[HttpPost]
public async Task<IActionResult> Create([FromBody] ProductoCreateDto dto)
{
    // Si dto es null o las anotaciones fallan,
    // ASP.NET Core devuelve automáticamente 400 Bad Request
    // con los errores de validación
    var resultado = await _service.CreateAsync(dto);
    return Ok(resultado);
}
```

```json
// Respuesta cuando la validación falla
// HTTP 400 Bad Request
{
  "errors": {
    "Nombre": ["El nombre es obligatorio"],
    "Precio": ["El precio debe ser mayor a 0"]
  }
}
```

🧠 **Analogía**: La validación automática es como un guardia de seguridad en la entrada de un edificio. Revisa que todos los datos进来的 (del cliente) cumplan con las reglas básicas (documentación válida) antes de dejar pasar la petición al interior (al servicio de negocio).

## 2.7. Tipos de Respuesta

ASP.NET Core ofrece varias formas de devolver respuestas desde los controladores. IActionResult es el tipo base más flexible, ActionResult<T> combina flexibilidad con tipado fuerte, y Typed Results (introducido en .NET 7) ofrece la sintaxis más concisa para respuestas tipadas.

### 2.7.1. IActionResult vs ActionResult<T> vs Typed Results

**IActionResult (el tradicional):**

IActionResult es una interfaz que representa cualquier resultado de acción. Es útil cuando necesitas devolver diferentes tipos de respuestas condicionalmente o cuando trabajas con código legacy.

**ActionResult<T> (mezcla de tipos):**

ActionResult<T> combina IActionResult con un tipo específico, permitiéndote devolver tanto resultados tipados como errores. Es ideal cuando la respuesta exitosa siempre tiene el mismo tipo.

**Typed Results (.NET 7+):**

Los Typed Results proporcionan métodos de extensión strongly-typed como Ok<T>(value), NotFound<T>(value), etc. Esto hace el código más limpio y permite inferencia de tipos.

```mermaid
flowchart LR
    subgraph "IActionResult"
        A1["Más flexible"]
        A2["Necesario Cast"]
        A3["Código más largo"]
    end
    
    subgraph "ActionResult<T>"
        B1["Tipado"]
        B2["Sin Cast"]
        B3["Balanceado"]
    end
    
    subgraph "Typed Results"
        C1["Más conciso"]
        C2["Inferencia automática"]
        C3["Recomendado .NET 7+"]
    end
    
    A1 --> B1 --> C1
    A2 --> B2 --> C2
    A3 --> B3 --> C3
```

**Cuándo usar cada uno:**

Usa IActionResult cuando trabajes con código legacy o cuando necesites máxima flexibilidad para devolver tipos muy diferentes. Usa ActionResult<T> cuando quieras tipado fuerte pero flexibilidad para devolver errores. Usa Typed Results cuando puedas, porque es la sintaxis más limpia y moderna.

### 2.7.2. Helpers de Respuesta

ASP.NET Core proporciona métodos helper para devolver respuestas comunes. Estos métodos crean automáticamente el IActionResult apropiado con el código de estado y formato correctos.

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductosController : ControllerBase
{
    private readonly IProductoService _service;

    public ProductosController(IProductoService service)
    {
        _service = service;
    }

    // 200 OK - Respuesta exitosa
    [HttpGet]
    public async Task<ActionResult<List<ProductoDto>>> GetAll()
    {
        var productos = await _service.GetAllAsync();
        return Ok(productos);  // 200 con el objeto
    }
    
    // 201 Created - Recurso creado
    [HttpPost]
    public async Task<ActionResult<ProductoDto>> Create([FromBody] ProductoCreateDto dto)
    {
        var producto = await _service.CreateAsync(dto);
        return CreatedAtAction(
            nameof(GetById),
            new { id = producto.Id },
            producto);  // 201 con Location header
    }
    
    // 204 No Content - Sin contenido que devolver
    [HttpDelete("{id:long}")]
    public async Task<ActionResult> Delete(long id)
    {
        await _service.DeleteAsync(id);
        return NoContent();  // 204 sin body
    }
    
    // 400 Bad Request - Error del cliente
    [HttpPost]
    public async Task<ActionResult<ProductoDto>> CreateWithValidation([FromBody] ProductoCreateDto dto)
    {
        if (string.IsNullOrEmpty(dto.Nombre))
            return BadRequest(new { error = "El nombre es obligatorio" });
            
        var producto = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = producto.Id }, producto);
    }
    
    // 404 Not Found - Recurso no existe
    [HttpGet("{id:long}")]
    public async Task<ActionResult<ProductoDto>> GetById(long id)
    {
        var producto = await _service.GetByIdAsync(id);
        if (producto == null)
            return NotFound(new { error = $"Producto {id} no encontrado" });
            
        return Ok(producto);
    }
    
    // 401 Unauthorized - No autenticado
    [HttpGet("admin")]
    public async Task<ActionResult<string>> AdminOnly()
    {
        if (!User.IsInRole("Admin"))
            return Unauthorized(new { error = "Debes iniciar sesion" });
            
        return Ok("Datos sensibles");
    }
    
    // 403 Forbidden - Autenticado pero sin permisos
    [HttpDelete("{id:long}")]
    public async Task<ActionResult> DeleteWithPermission(long id)
    {
        if (!User.HasPermission("Producto.Delete"))
            return StatusCode(403, new { error = "No tienes permiso para eliminar" });
            
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
```

**CreatedAtAction y CreatedAtRoute:**

CreatedAtAction es especialmente util para devolver 201 Created con un header Location que apunta al recurso creado. El cliente puede usar esta URL para obtener el recurso recien creado.

```csharp
[HttpPost]
public async Task<ActionResult<ProductoDto>> Create([FromBody] ProductoCreateDto dto)
{
    var producto = await _service.CreateAsync(dto);
    
    // Genera: Location: /api/productos/123
    return CreatedAtAction(
        actionName: nameof(GetById),
        routeValues: new { id = producto.Id },
        value: producto);
}
```

## 2.8. Mejores Practicas Avanzadas

### 2.8.1. Versionado de APIs

El versionado permite evolucionar la API sin romper clientes existentes. Existen varias estrategias para versionar una API, cada una con sus pros y contras.

```mermaid
flowchart TD
    A["Como versionar?"] --> B["URL Path"]
    A --> C["Query String"]
    A --> D["Header Custom"]
    A --> E["Accept Header"]
    
    B --> B1["GET /api/v1/productos"]
    C --> C1["GET /api/productos?version=1"]
    D --> D1["GET /api/productos<br/>X-API-Version: 1"]
    E --> E1["GET /api/productos<br/>Accept: application/vnd.api.v1+json"]
```

**Comparacion de Estrategias:**

| Estrategia       | Pros                      | Contras             | Uso               |
| ---------------- | ------------------------- | ------------------- | ----------------- |
| **URL Path**     | Visible, facil de cachear | Polluta URLs        | Mas comun         |
| **Query String** | URLs limpias              | No visible en cache | APIs internas     |
| **Header**       | URLs limpias              | Menos discoverable  | APIs sofisticadas |
| **Accept**       | Estandar HTTP             | Complejidad         | APIs enterprise   |

**Implementacion con URL Path:**

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader());
});

var app = builder.Build();

// Versionado por URL
app.MapGroup("/api/v{version:apiVersion}/productos")
   .MapProductsApiV1();

app.MapGroup("/api/v{version:apiVersion}/pedidos")
   .MapPedidosApiV1();

app.Run();
```

```csharp
// ProductsApiV1.cs
public static class ProductsApiV1
{
    public static RouteGroupBuilder MapProductsApiV1(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetProducts);
        group.MapGet("/{id:long}", GetProduct);
        group.MapPost("/", CreateProduct);
        group.MapPut("/{id:long}", UpdateProduct);
        group.MapDelete("/{id:long}", DeleteProduct);
        
        return group;
    }

    private static async Task<Results<Ok<List<ProductoDto>>, NotFound>> 
        GetProducts([AsParameters] GetProductsQuery query)
    {
        // Implementacion...
        return Ok(await _service.GetProductsAsync(query));
    }
}

// ProductsApiV2.cs (nueva version con cambios)
public static class ProductsApiV2
{
    public static RouteGroupBuilder MapProductsApiV2(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetProducts);
        // Nuevos endpoints o cambios...
        
        return group;
    }
}
```

**Deprecation Headers:**

```csharp
app.MapGet("/api/productos", () => Results.Ok(new { }))
    .AddEndpointFilter(async (context, next) =>
    {
        var response = await next(context);
        response.HttpContext.Response.Headers["Deprecation"] = "true";
        response.HttpContext.Response.Headers["Sunset"] = 
            "Thu, 31 Dec 2025 23:59:59 GMT";
        response.HttpContext.Response.Headers["Link"] = 
            "</api/v2/productos>; rel=\"alternate\"";
        return response;
    });
```

### 2.8.2. Paginacion

La paginacion es esencial para APIs que devuelven grandes conjuntos de datos. Evita devolver miles de registros de una vez, lo cual puede saturar la memoria del cliente y la red.

```mermaid
flowchart TD
    subgraph "Offset-Based"
        A1["SKIP 20"]
        A2["TAKE 10"]
        A3["Datos 21-30"]
        A4["Problema: lento con SKIP grande"]
    end
    
    subgraph "Cursor-Based"
        B1["AFTER cursor_abc"]
        B2["TAKE 10"]
        B3["Datos despues del cursor"]
        B4["Mejor rendimiento"]
    end
```

**Offset Paginacion:**

```csharp
// Request con parametros de paginacion
public record GetProductsQuery(
    int Page = 1,
    int PageSize = 20,
    string? SortBy = "createdAt",
    string SortOrder = "desc",
    string? Search = null,
    long? CategoriaId = null,
    decimal? MinPrecio = null,
    decimal? MaxPrecio = null
);

// Response paginado
public record PagedResponse<T>(
    List<T> Items,
    int TotalItems,
    int Page,
    int PageSize,
    int TotalPages,
    bool HasNextPage,
    bool HasPreviousPage,
    PaginationLinks? Links
);

public record PaginationLinks(
    string? First,
    string? Previous,
    string? Next,
    string? Last
);

// Controller
[HttpGet]
public async Task<IActionResult> GetProducts([FromQuery] GetProductsQuery query)
{
    var result = await _productService.GetProductsAsync(query);
    
    var response = new PagedResponse<ProductoDto>(
        Items: result.Items.Select(p => p.ToDto()),
        TotalItems: result.TotalItems,
        Page: query.Page,
        PageSize: query.PageSize,
        TotalPages: (int)Math.Ceiling(result.TotalItems / (double)query.PageSize),
        HasNextPage: query.Page * query.PageSize < result.TotalItems,
        HasPreviousPage: query.Page > 1,
        Links: CreatePaginationLinks(query, result.TotalItems)
    );
    
    return Ok(response);
}

private PaginationLinks? CreatePaginationLinks(GetProductsQuery query, int totalItems)
{
    var baseUrl = "/api/productos";
    var totalPages = (int)Math.Ceiling(totalItems / (double)query.PageSize);
    
    return new PaginationLinks(
        First: $"{baseUrl}?page=1&pageSize={query.PageSize}",
        Previous: query.Page > 1 
            ? $"{baseUrl}?page={query.Page - 1}&pageSize={query.PageSize}" 
            : null,
        Next: query.Page < totalPages 
            ? $"{baseUrl}?page={query.Page + 1}&pageSize={query.PageSize}" 
            : null,
        Last: $"{baseUrl}?page={totalPages}&pageSize={query.PageSize}"
    );
}
```

**Cursor Paginacion:**

```csharp
public record CursorPagedResponse<T>(
    List<T> Items,
    string? NextCursor,
    int? RemainingCount
);

// Implementacion de cursor pagination
public async Task<CursorPagedResponse<ProductoDto>> GetProductsCursorAsync(
    string? cursor, int limit)
{
    IQueryable<Producto> query = _context.Productos;
    
    if (!string.IsNullOrEmpty(cursor))
    {
        var cursorValue = DecodeCursor(cursor);
        query = query.Where(p => p.Id > cursorValue);
    }
    
    var items = await query
        .OrderBy(p => p.Id)
        .Take(limit + 1)
        .ToListAsync();
    
    var hasMore = items.Count > limit;
    var resultItems = items.Take(limit).ToList();
    var nextCursor = hasMore ? EncodeCursor(resultItems.Last().Id) : null;
    
    return new CursorPagedResponse<ProductoDto>
    {
        Items = resultItems.Select(p => p.ToDto()),
        NextCursor = nextCursor,
        RemainingCount = hasMore ? await query.CountAsync() : null
    };
}
```

### 2.8.3. HATEOAS

HATEOAS (Hypermedia as the Engine of Application State) permite que los clientes descubcan las acciones disponibles a través de enlaces hypermedia. Esto hace que la API sea auto-descubrible y mas facil de usar.

```mermaid
flowchart LR
    A["GET /api/pedidos/123"] --> B["Respuesta con enlaces"]
    B --> C["_links.self.href"]
    B --> D["_links.cancel.href"]
    B --> E["_links.pagar.href"]
```

**Implementacion de HATEOAS:**

```csharp
// Response con HATEOAS
public record PedidoResponse(
    long Id,
    PedidoEstado Estado,
    decimal Total,
    DateTime CreatedAt,
    List<PedidoItemResponse> Items,
    Links? Links
);

public record Links(
    string Self,
    string? Cancel,
    string? Pagar,
    string? Factura,
    string? Tracking
);

// Builder de links
public class HateoasLinkBuilder
{
    private readonly HttpContext _httpContext;

    public HateoasLinkBuilder(HttpContext httpContext)
    {
        _httpContext = httpContext;
    }

    public Links BuildPedidoLinks(Pedido pedido)
    {
        var baseUrl = $"{_httpContext.Request.Scheme}://{_httpContext.Request.Host}";
        
        return new Links(
            Self: $"{baseUrl}/api/pedidos/{pedido.Id}",
            Cancel: pedido.Estado == PedidoEstado.Pendiente 
                ? $"{baseUrl}/api/pedidos/{pedido.Id}/cancelar" 
                : null,
            Pagar: pedido.Estado == PedidoEstado.Pendiente 
                ? $"{baseUrl}/api/pedidos/{pedido.Id}/pagar" 
                : null,
            Factura: pedido.Estado == PedidoEstado.Entregado 
                ? $"{baseUrl}/api/pedidos/{pedido.Id}/factura" 
                : null,
            Tracking: pedido.Estado == PedidoEstado.Enviado 
                ? $"{baseUrl}/api/pedidos/{pedido.Id}/tracking" 
                : null
        );
    }
}

// En el controller
[HttpGet("{id:long}")]
public async Task<IActionResult> GetPedido(long id)
{
    var pedido = await _pedidoService.GetByIdAsync(id);
    if (pedido == null)
        return NotFound();
    
    var linkBuilder = new HateoasLinkBuilder(HttpContext);
    var links = linkBuilder.BuildPedidoLinks(pedido);
    
    var response = new PedidoResponse(
        Id: pedido.Id,
        Estado: pedido.Estado,
        Total: pedido.Total,
        CreatedAt: pedido.CreatedAt,
        Items: pedido.Items.Select(i => new PedidoItemResponse(
            i.ProductoId, i.Cantidad, i.PrecioUnitario)).ToList(),
        Links: links
    );
    
    return Ok(response);
}
```

```json
// Ejemplo de response con HATEOAS
{
  "id": 123,
  "estado": "pendiente",
  "total": 99.99,
  "links": {
    "self": "/api/pedidos/123",
    "cancel": "/api/pedidos/123/cancelar",
    "pagar": "/api/pedidos/123/pagar"
  }
}
```

### 2.8.4. Sobres de Respuesta

Los sobres de respuesta (Response Envelopes) estandarizan todas las respuestas de la API, proporcionando consistencia en el formato de respuesta tanto para exitos como para errores.

```csharp
public record ApiResponse<T>(
    bool Success,
    T? Data,
    ApiError? Error,
    DateTime Timestamp,
    PaginationInfo? Pagination,
    Dictionary<string, string>? Meta
);

public record ApiError(
    string Code,
    string Message,
    string? Details,
    List<ValidationError>? ValidationErrors
);

public record ValidationError(
    string Field,
    string Message,
    string? Code
);

public record PaginationInfo(
    int Page,
    int PageSize,
    int TotalItems,
    int TotalPages
);
```

**Helper para crear responses:**

```csharp
public static class ApiResponseHelper
{
    public static ApiResponse<T> Ok<T>(T data, PaginationInfo? pagination = null)
    {
        return new ApiResponse<T>(
            Success: true,
            Data: data,
            Error: null,
            Timestamp: DateTime.UtcNow,
            Pagination: pagination,
            Meta: null
        );
    }

    public static ApiResponse<T> Error<T>(string code, string message, 
        string? details = null)
    {
        return new ApiResponse<T>(
            Success: false,
            Data: default,
            Error: new ApiError(code, message, details, null),
            Timestamp: DateTime.UtcNow,
            Pagination: null,
            Meta: null
        );
    }

    public static ApiResponse<T> ValidationError<T>(List<ValidationError> errors)
    {
        return new ApiResponse<T>(
            Success: false,
            Data: default,
            Error: new ApiError(
                "VALIDATION_ERROR",
                "La solicitud contiene errores de validacion",
                null,
                errors),
            Timestamp: DateTime.UtcNow,
            Pagination: null,
            Meta: null
        );
    }
}
```

**En el Controller:**

```csharp
[HttpGet]
public async Task<IActionResult> GetProducts([FromQuery] GetProductsQuery query)
{
    var result = await _productService.GetProductsAsync(query);
    
    var pagination = new PaginationInfo(
        Page: query.Page,
        PageSize: query.PageSize,
        TotalItems: result.TotalItems,
        TotalPages: (int)Math.Ceiling(result.TotalItems / (double)query.PageSize)
    );
    
    return Ok(ApiResponseHelper.Ok(
        result.Items.Select(p => p.ToDto()),
        pagination));
}

[HttpPost]
public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
{
    var result = await _productService.CreateAsync(request);
    
    return result.Match(
        producto => CreatedAtAction(
            nameof(GetProduct),
            new { id = producto.Id },
            ApiResponseHelper.Ok(producto.ToDto())),
        error => BadRequest(ApiResponseHelper.Error(
            error.Code, error.Message))
    );
}
```

**Ejemplo de Respuesta:**

```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "nombre": "Producto 1",
      "precio": 29.99
    },
    {
      "id": 2,
      "nombre": "Producto 2",
      "precio": 49.99
    }
  ],
  "error": null,
  "timestamp": "2024-01-15T10:30:00Z",
  "pagination": {
    "page": 1,
    "pageSize": 20,
    "totalItems": 100,
    "totalPages": 5
  },
  "meta": null
}
```

### 2.8.5. Filtrado y Cadenas de Consulta

**Parametros de Filtrado:**

```csharp
// Filtros de producto
public record GetProductsQuery(
    [FromQuery] int Page = 1,
    [FromQuery] int PageSize = 20,
    [FromQuery] string SortBy = "createdAt",
    [FromQuery] string SortOrder = "desc",
    
    // Filtros
    [FromQuery] string? Search = null,
    [FromQuery] long? CategoriaId = null,
    [FromQuery] decimal? MinPrecio = null,
    [FromQuery] decimal? MaxPrecio = null,
    [FromQuery] bool? Activo = null,
    [FromQuery] List<long>? Tags = null
);

// Implementacion del servicio
public async Task<PagedResult<ProductoDto>> GetProductsAsync(GetProductsQuery query)
{
    var queryable = _context.Productos.AsQueryable();
    
    // Busqueda por texto
    if (!string.IsNullOrEmpty(query.Search))
    {
        var searchTerm = query.Search.ToLower();
        queryable = queryable.Where(p => 
            p.Nombre.ToLower().Contains(searchTerm) ||
            p.Descripcion.ToLower().Contains(searchTerm));
    }
    
    // Filtros exactos
    if (query.CategoriaId.HasValue)
        queryable = queryable.Where(p => p.CategoriaId == query.CategoriaId);
    
    if (query.MinPrecio.HasValue)
        queryable = queryable.Where(p => p.Precio >= query.MinPrecio);
    
    if (query.MaxPrecio.HasValue)
        queryable = queryable.Where(p => p.Precio <= query.MaxPrecio);
    
    if (query.Activo.HasValue)
        queryable = queryable.Where(p => p.Activo == query.Activo);
    
    // Filtro por lista (tags)
    if (query.Tags != null && query.Tags.Any())
    {
        queryable = queryable.Where(p => 
            p.ProductoTags.Any(pt => query.Tags.Contains(pt.TagId)));
    }
    
    // Ordenamiento
    queryable = query.SortOrder.ToLower() == "asc"
        ? queryable.OrderBy(p => EF.Property<object>(p, query.SortBy))
        : queryable.OrderByDescending(p => EF.Property<object>(p, query.SortBy));
    
    // Conteo total
    var totalItems = await queryable.CountAsync();
    
    // Paginacion
    var items = await queryable
        .Skip((query.Page - 1) * query.PageSize)
        .Take(query.PageSize)
        .ToDtoAsync();
    
    return new PagedResult<ProductoDto>(
        Items: items,
        TotalItems: totalItems,
        Page: query.Page,
        PageSize: query.PageSize);
}
```

### 2.8.6. ETag para Almacenamiento en Cache

```csharp
[HttpGet("{id:long}")]
[ProducesResponseType(typeof(ProductoDto), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status304NotModified)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<IActionResult> GetProduct(long id)
{
    var producto = await _productService.GetByIdAsync(id);
    if (producto == null)
        return NotFound();
    
    var dto = producto.ToDto();
    var etag = $"\"{producto.Version}\"";
    
    // Verificar If-None-Match
    if (Request.Headers.IfNoneMatch.Contains(etag))
    {
        return StatusCode(StatusCodes.Status304NotModified);
    }
    
    Response.Headers.ETag = etag;
    Response.Headers.CacheControl = "public, max-age=300";
    
    return Ok(dto);
}
```

## 2.9. Manejo de Errores

Es fundamental proporcionar **mensajes de error claros** y códigos HTTP apropiados. Los errores deben ser consistentes y seguir un formato estándar para que los clientes puedan manejarlos apropiadamente.

**Formato estándar de error (RFC 7807):**

## 2.10. Filtros en Controladores

Los filtros permiten ejecutar código antes o después de ciertas etapas del pipeline de procesamiento de la petición. Son ideales para lógica transversal como logging, manejo de errores, validación de permisos, y caché de respuestas.

## 2.11. Resumen

1. **REST** es un estilo arquitectónico basado en HTTP y sus métodos, con principios como stateless, cachéable, e interfaz uniforme.

2. **Componentes de request**: método, URI, headers, body. Cada petición debe contener toda la información necesaria.

3. **Códigos de estado**: 2xx (éxito), 3xx (redirección), 4xx (cliente), 5xx (servidor).

4. **Métodos HTTP**: GET (leer), POST (crear), PUT (reemplazar), PATCH (modificar parcialmente), DELETE (eliminar).

5. **Endpoints** usan sustantivos plurales, sin verbos, con IDs para recursos específicos.

6. **Controladores** en ASP.NET Core heredan de ControllerBase y usan atributos para routing y verbos HTTP.

7. **Model Binding** convierte automáticamente JSON a objetos .NET desde body, query string, route, o headers.

8. **Tipos de respuesta**: IActionResult (flexible), ActionResult<T> (tipado), Typed Results (conciso, .NET 7+).

9. **Buenas prácticas**: versionado de APIs, paginación, HATEOAS, response envelopes, filtrado, ETag para caché.

10. **Versionado** es esencial para evoluciones de API sin romper clientes existentes.

## 2.12. Ejercicio Propuesto

**Objetivo:** Diseñar una API REST completa para gestionar **Funkos** (figuras coleccionables).

**Tareas:**

1. Diseñar los **endpoints completos** para realizar CRUD sobre el recurso `Funkos`
2. Especificar para cada endpoint: método HTTP, URL, body, código de respuesta, body de respuesta, errores posibles
3. Implementar un controlador con paginación y filtrado

**Modelo de datos del Funko:**

**Tabla a completar:**

| Endpoint             | Método HTTP | Body                    | Código         | Body Respuesta                       | Errores       |
| -------------------- | ----------- | ----------------------- | -------------- | ------------------------------------ | ------------- |
| `/api/funkos`        | GET         | N/A                     | 200 OK         | `[{"id":1, "nombre":"Batman", ...}]` | -             |
| `/api/funkos?page=1` | GET         | N/A                     | 200 OK         | `{items:[...], pagination:{...}}`    | -             |
| `/api/funkos`        | POST        | `{"nombre":"...", ...}` | 201 Created    | `{"id":2, ...}`                      | 400, 422      |
| `/api/funkos/1`      | GET         | N/A                     | 200 OK         | `{"id":1, ...}`                      | 404           |
| `/api/funkos/1`      | PUT         | `{"nombre":"...", ...}` | 200 OK         | `{"id":1, ...}`                      | 400, 404, 422 |
| `/api/funkos/1`      | PATCH       | `{"precio":12.99}`      | 200 OK         | `{"id":1, ...}`                      | 400, 404, 422 |
| `/api/funkos/1`      | DELETE      | N/A                     | 204 No Content | N/A                                  | 404           |

**Criterios de Evaluación:**

✅ Endpoints siguen convenciones REST (sustantivos plurales, métodos HTTP correctos)

✅ Códigos de estado apropiados para cada operación

✅ Body de respuesta coherente con la operación

✅ Errores documentados con códigos y mensajes claros

✅ Uso correcto de HTTP (idempotencia, seguridad de métodos)

✅ Paginación implementada correctamente

✅ Filtrado por categoría y rango de precio

✅ Controlador bien estructurado con servicios inyectados

✅ Documentación Swagger con comentarios XML

✅ Manejo de errores global con filtro

✅ Versionado de API preparado para futuras versiones
