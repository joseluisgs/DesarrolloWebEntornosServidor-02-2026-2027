# 1. Conceptos de Servicios Web y Configuración de Proyectos .NET

## Indice

- [1. Conceptos de Servicios Web y Configuración de Proyectos .NET](#1-conceptos-de-servicios-web-y-configuración-de-proyectos-net)
  - [1.1. Servicios Web en .NET](#11-servicios-web-en-net)
    - [1.1.1. Servicios Orientados a la Conexión vs Sin Conexión](#111-servicios-orientados-a-la-conexión-vs-sin-conexión)
    - [1.1.2. Servicios Web](#112-servicios-web)
    - [1.1.3. El Protocolo HTTP y sus Ventajas](#113-el-protocolo-http-y-sus-ventajas)
    - [1.1.4. Características Clave de HTTP](#114-características-clave-de-http)
  - [1.2. Componentes de un Servicio Web](#12-componentes-de-un-servicio-web)
    - [1.2.1. Request (Solicitud)](#121-request-solicitud)
    - [1.2.2. Response (Respuesta)](#122-response-respuesta)
    - [1.2.3. Ejemplos de Servicios Web](#123-ejemplos-de-servicios-web)
    - [1.2.4. Arquitecturas de Servicios Web](#124-arquitecturas-de-servicios-web)
  - [1.3. Creación de Soluciones y Proyectos](#13-creación-de-soluciones-y-proyectos)
    - [1.3.1. ¿Por qué usar soluciones?](#131-por-qué-usar-soluciones)
    - [1.3.2. Crear la estructura desde cero](#132-crear-la-estructura-desde-cero)
    - [1.3.3. Establecer referencias entre proyectos](#133-establecer-referencias-entre-proyectos)
    - [1.3.4. Estructura de carpetas resultante](#134-estructura-de-carpetas-resultante)
  - [1.4. Instalación de Librerías con NuGet](#14-instalación-de-librerías-con-nuget)
    - [1.4.1. ¿Qué son los paquetes NuGet?](#141-qué-son-los-paquetes-nuget)
    - [1.4.2. Instalación de paquetes esenciales](#142-instalación-de-paquetes-esenciales)
    - [1.4.3. Comandos útiles de NuGet](#143-comandos-útiles-de-nuget)
  - [1.5. Estructura de Proyectos](#15-estructura-de-proyectos)
    - [1.5.1. ¿Por qué separar en proyectos?](#151-por-qué-separar-en-proyectos)
    - [1.5.2. Estructura completa del proyecto](#152-estructura-completa-del-proyecto)
    - [1.5.3. Flujo de datos entre capas](#153-flujo-de-datos-entre-capas)
  - [1.6. Configuración con appsettings.json](#16-configuración-con-appsettingsjson)
    - [1.6.1. ¿Cómo funciona la configuración?](#161-cómo-funciona-la-configuración)
    - [1.6.2. Archivo appsettings.json básico](#162-archivo-appsettingsjson-básico)
    - [1.6.3. appsettings.Development.json](#163-appsettingsdevelopmentjson)
    - [1.6.4. appsettings.Production.json](#164-appsettingsproductionjson)
    - [1.6.5. Leer configuración en código](#165-leer-configuración-en-código)
    - [1.6.6. Jerarquía de configuración](#166-jerarquía-de-configuración)
  - [1.7. Variables de Entorno y Secrets de Usuario](#17-variables-de-entorno-y-secrets-de-usuario)
    - [1.7.1. ¿Cuándo usar cada método?](#171-cuándo-usar-cada-método)
    - [1.7.2. Variables de entorno en el sistema](#172-variables-de-entorno-en-el-sistema)
    - [1.7.3. Variables de entorno en Docker](#173-variables-de-entorno-en-docker)
    - [1.7.4. Secrets de Usuario para Desarrollo](#174-secrets-de-usuario-para-desarrollo)
    - [1.7.5. Ubicación de User Secrets por sistema operativo](#175-ubicación-de-user-secrets-por-sistema-operativo)
    - [1.7.6. Configurar User Secrets en Program.cs](#176-configurar-user-secrets-en-programcs)
  - [1.8. Patrón de Opciones (IOptions)](#18-patrón-de-opciones-ioptions)
    - [1.8.1. Beneficios del patrón de opciones](#181-beneficios-del-patrón-de-opciones)
    - [1.8.2. Definir clases de opciones](#182-definir-clases-de-opciones)
    - [1.8.3. Registrar opciones en Program.cs](#183-registrar-opciones-en-programcs)
    - [1.8.4. Consumir opciones en servicios](#184-consumir-opciones-en-servicios)
    - [1.8.5. Diferencia entre IOptions, IOptionsSnapshot e IOptionsMonitor](#185-diferencia-entre-ioptions-ioptionssnapshot-e-ioptionsmonitor)
  - [1.9. Testing con NUnit](#19-testing-con-nunit)
    - [1.9.1. ¿Por qué hacer tests?](#191-por-qué-hacer-tests)
    - [1.9.2. Estructura básica de un test NUnit](#192-estructura-básica-de-un-test-nunit)
    - [1.9.3. Comandos para ejecutar tests](#193-comandos-para-ejecutar-tests)
    - [1.9.4. Configurar cobertura con Coverlet](#194-configurar-cobertura-con-coverlet)
    - [1.9.5. Atributos útiles de NUnit](#195-atributos-útiles-de-nunit)
  - [1.10. Hot Reload y dotnet watch run](#110-hot-reload-y-dotnet-watch-run)
    - [1.10.1. ¿Cómo funciona Hot Reload?](#1101-cómo-funciona-hot-reload)
    - [1.10.2. Uso básico de dotnet watch](#1102-uso-básico-de-dotnet-watch)
    - [1.10.3. Configurar Hot Reload en el proyecto](#1103-configurar-hot-reload-en-el-proyecto)
    - [1.10.4. Hot Reload con Docker Compose](#1104-hot-reload-con-docker-compose)
    - [1.10.5. Archivos monitorizados](#1105-archivos-monitorizados)
    - [1.10.6. Limitaciones de Hot Reload](#1106-limitaciones-de-hot-reload)
    - [Comparación de rendimiento](#comparación-de-rendimiento)
    - [Configurar en VS Code](#configurar-en-vs-code)
  - [1.11. Resumen y Buenas Prácticas](#111-resumen-y-buenas-prácticas)
    - [Puntos clave del módulo](#puntos-clave-del-módulo)
    - [Buenas prácticas](#buenas-prácticas)
    - [Siguientes pasos](#siguientes-pasos)
    - [Recursos adicionales](#recursos-adicionales)

---

## 1.1. Servicios Web en .NET

Un **servicio web** es una funcionalidad que se ofrece a través de una interfaz (API) y que puede ser utilizada por otros componentes de software, ya sea para acceder a datos, manejar recursos, o realizar operaciones de negocio.

🧠 **Analogía**: Un servicio web es como un restaurante. El cliente (tu aplicación) hace un pedido (solicitud HTTP), el cocina (servidor) prepara la comida (procesa la solicitud), y el mesero trae la comida (respuesta HTTP). Todo sigue un protocolo establecido: tú pides, te traen lo que pediste.

```mermaid
flowchart LR
    A[Cliente<br/>App, Web, Mobile] -->|HTTP Request| B[Servicio Web<br/>ASP.NET Core]
    B -->|HTTP Response| A
    
    subgraph "Internet"
        A
        B
    end
```

### 1.1.1. Servicios Orientados a la Conexión vs Sin Conexión

Los servicios pueden ser **orientados a la conexión** o **sin conexión** (connectionless), y cada enfoque tiene características distintas.

**Servicios orientados a la conexión:**

Requieren que el cliente y el servidor establezcan una conexión antes de que pueda comenzar la comunicación. La conexión se mantiene activa hasta que se cierra explícitamente.

🧠 **Analogía**: Es como una llamada telefónica. Marcas, esperas que contesten, hablas mientras dure la llamada, y cuando termináis, colgáis. Durante toda la comunicación hay una línea abierta.

**Pros:**

1. **Fiabilidad**: Los datos se entregan en el orden correcto y sin errores (ej: TCP)
2. **Control de Flujo**: Previene que el remitente sobrecargue al receptor
3. **Control de Congestión**: Ayuda a prevenir problemas cuando la red está saturada

**Contras:**

1. **Tiempo de Configuración**: Requiere un tiempo inicial para establecer la conexión
2. **Overhead**: Mayor sobrecarga debido a confirmaciones (ACKs) para cada paquete
3. **Menos Eficiente para Datos Pequeños**: El costo de establecer y cerrar una conexión puede superar los beneficios

📝 **Nota del Profesor**: TCP es el ejemplo clásico de protocolo orientado a la conexión. En servicios web, SOAP sobre TCP es un ejemplo de este enfoque.

**Servicios No Orientados a la Conexión:**

No requieren una conexión establecida previamente. Los mensajes se envían y reciben de forma independiente.

🧠 **Analogía**: Es como enviar cartas por correo. Cada carta es independiente, no necesitas mantener una conexión abierta con el destinatario. Envías tu carta, y eventualmente llegará (con suerte).

**Pros:**

1. **Rapidez**: Los datos se pueden enviar inmediatamente sin conexión previa (ej: UDP, HTTP)
2. **Eficiencia para Datos Pequeños**: Ideal para mensajes individuales sin overhead
3. **Resiliencia**: Más resistentes a problemas de red

**Contras:**

1. **Menos Fiabilidad**: No garantizan entrega en orden o sin errores
2. **Sin Control de Flujo o Congestión**: Pueden provocar problemas cuando la red está saturada
3. **Requiere Mayor Gestión de Errores**: Es necesario implementar mecanismos adicionales

```mermaid
flowchart TD
    subgraph "Orientado a la Conexion TCP"
        A1[Cliente] <--->|Conexion establecida| B1[Servidor]
        A1 -->|Datos 1| B1
        B1 -->|ACK| A1
        A1 -->|Datos 2| B1
        B1 -->|ACK| A1
    end
    
    subgraph "Sin Conexion UDP"
        A2[Cliente] -->|Datagrama 1| B2[Servidor]
        A2 -->|Datagrama 2| B2
        A2 -->|Datagrama 3| B2
    end
```

⚠️ **Advertencia**: HTTP/3 ahora usa QUIC (basado en UDP), lo que cambia esta distinción. Sin embargo, para este curso nos centraremos en HTTP/1.1 y HTTP/2 que son los más comunes en APIs REST.

### 1.1.2. Servicios Web

Los **servicios web** son sistemas de software diseñados para permitir la **interoperabilidad** entre máquinas a través de una red, utilizando principalmente el protocolo **HTTP**.

Proporcionan una forma estandarizada de integrar aplicaciones basadas en web usando formatos como **JSON**, **XML**, y protocolos como **REST**, **SOAP**, **GraphQL**, **gRPC**.

**Ventajas de los servicios web:**

✅ **Interoperabilidad**: Permiten la comunicación entre diferentes aplicaciones, sistemas operativos y lenguajes de programación

✅ **Reducción de costos**: Facilitan la integración de sistemas sin necesidad de desarrollar interfaces personalizadas

✅ **Escalabilidad**: Pueden manejar un gran número de solicitudes simultáneas (especialmente con arquitecturas stateless como REST)

✅ **Reutilización**: Un servicio puede ser consumido por múltiples clientes

✅ **Mantenimiento**: Los cambios en el servicio se reflejan automáticamente en todos los clientes

**Desventajas:**

❌ **Complejidad**: Algunos enfoques (como SOAP) pueden ser complejos de implementar

❌ **Seguridad**: Requieren medidas adicionales (HTTPS, autenticación JWT, OAuth)

❌ **Rendimiento**: La comunicación por red introduce latencia

❌ **Disponibilidad**: El servicio depende de la red

### 1.1.3. El Protocolo HTTP y sus Ventajas

**HTTP (Hypertext Transfer Protocol)** es la base de cualquier intercambio de datos en la Web. Es un protocolo **sin estado** (stateless) y **basado en texto**.

🧠 **Analogía**: HTTP es como el sistema postal. Cada carta (petición) contiene toda la información necesaria para que el destinatario entienda qué quieres. No recuerda cartas anteriores.

**Ventajas de usar HTTP en servicios web:**

1. **Interoperabilidad**: HTTP es universalmente aceptado. Cualquier cliente o servidor puede comunicarse

2. **Simplicidad**: Los mensajes HTTP son legibles por humanos, facilitando la depuración

3. **Compatibilidad con REST**: HTTP es el protocolo subyacente de las APIs RESTful

4. **Seguridad**: A través de **HTTPS**, HTTP proporciona cifrado de datos en tránsito

5. **Caching**: HTTP soporta almacenamiento en caché, mejorando el rendimiento

6. **Soporte de Sesión y Cookies**: Mantener estado entre solicitudes

7. **Escalabilidad**: Su naturaleza sin estado permite manejar muchas solicitudes simultáneas

💡 **Tip del Examinador**: En entrevistas, destacan que HTTP es la base de la Web y que HTTPS es HTTP sobre TLS/SSL para seguridad.

### 1.1.4. Características Clave de HTTP

**Arquitectura Cliente-Servidor:**

- El **cliente** (navegador, aplicación móvil, Postman, etc.) realiza una **solicitud HTTP**
- El **servidor** (ASP.NET Core Web API) procesa la solicitud y devuelve una **respuesta HTTP**

```mermaid
sequenceDiagram
    participant C as Cliente
    participant S as Servidor
    
    C->>S: GET /api/usuarios/1
    S->>C: 200 OK {"id": 1, "nombre": "Juan"}
    
    C->>S: POST /api/usuarios
    S->>C: 201 Created {"id": 2, "nombre": "Maria"}
```

**Sin Estado (Stateless):**

- Cada solicitud HTTP se procesa de forma **independiente**
- El servidor **no mantiene información** sobre solicitudes anteriores
- Esto simplifica la escalabilidad, pero requiere mecanismos adicionales (JWT, cookies) para mantener estado

📝 **Nota del Profesor**: Esta característica es fundamental para entender REST. Cada petición contiene toda la información necesaria, sin depender del servidor.

## 1.2. Componentes de un Servicio Web

La comunicación entre cliente y servidor se basa en **peticiones (requests)** y **respuestas (responses)** enviadas a través de HTTP.

```mermaid
flowchart LR
    subgraph Solicitud
        A1[Método] --> A2[URL] --> A3[Headers] --> A4[Body]
    end
    
    subgraph Respuesta
        B1[Status Code] --> B2[Headers] --> B3[Body]
    end
    
    A4 -->|HTTP| B1
```

### 1.2.1. Request (Solicitud)

Un **request** o **solicitud HTTP** es un mensaje que un cliente envía a un servidor para solicitar una acción específica.

**Componentes de una solicitud HTTP:**

1. **Método HTTP**: Indica la acción a realizar

| Método | Descripción | Idempotente | Safe |
|--------|-------------|-------------|------|
| `GET` | Obtener un recurso | ✅ Sí | ✅ Sí |
| `POST` | Crear un nuevo recurso | ❌ No | ❌ No |
| `PUT` | Actualizar un recurso completo | ✅ Sí | ❌ No |
| `PATCH` | Actualizar parcialmente un recurso | ❌ No | ❌ No |
| `DELETE` | Eliminar un recurso | ✅ Sí | ❌ No |
| `HEAD` | Obtener solo encabezados | ✅ Sí | ✅ Sí |
| `OPTIONS` | Obtener métodos soportados | ✅ Sí | ✅ Sí |

2. **URL (Uniform Resource Locator)**: Especifica el recurso solicitado
   - Ejemplo: `https://api.example.com/api/usuarios/1`

3. **Encabezados (Headers)**: Información adicional

| Header | Descripción |
|--------|-------------|
| `Content-Type` | Tipo de contenido enviado (ej: `application/json`) |
| `Authorization` | Token de autenticación (ej: `Bearer <JWT>`) |
| `Accept` | Tipo de contenido que el cliente puede aceptar |
| `User-Agent` | Información sobre el cliente |
| `Accept-Language` | Idioma preferido |
| `Cache-Control` | Directivas de caché |

4. **Cuerpo (Body)**: Contiene los datos enviados al servidor (en POST, PUT, PATCH)

**Ejemplo de solicitud HTTP:**

```http
POST /api/usuarios HTTP/1.1
Host: api.example.com
Content-Type: application/json
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Accept: application/json
User-Agent: MiApp/1.0

{
  "nombre": "Juan Pérez",
  "email": "juan@example.com",
  "edad": 30
}
```

### 1.2.2. Response (Respuesta)

Un **response** o **respuesta HTTP** es un mensaje que el servidor envía al cliente en respuesta a una solicitud.

**Componentes de una respuesta HTTP:**

1. **Código de Estado (Status Code)**: Indica el resultado de la solicitud

| Rango | Significado | Descripción |
|-------|-------------|-------------|
| **1xx** | Informativo | Solicitud recibida, continuando proceso |
| **2xx** | Éxito | Solicitud procesada correctamente |
| **3xx** | Redirección | Se necesitan más acciones |
| **4xx** | Error del Cliente | El cliente cometió un error |
| **5xx** | Error del Servidor | El servidor falló |

**Códigos más comunes:**

```csharp
// 2xx - Éxito
200 OK          // Solicitud exitosa
201 Created     // Recurso creado exitosamente
204 No Content  // Solicitud exitosa sin contenido

// 4xx - Errores del Cliente
400 Bad Request         // Solicitud mal formada
401 Unauthorized        // Autenticación requerida
403 Forbidden           // Sin permisos
404 Not Found           // Recurso no encontrado
422 Unprocessable Entity // Datos inválidos

// 5xx - Errores del Servidor
500 Internal Server Error // Error genérico
503 Service Unavailable   // Temporalmente no disponible
502 Bad Gateway           // Gateway inválido
```

2. **Encabezados (Headers)**: Información adicional sobre la respuesta

| Header | Descripción |
|--------|-------------|
| `Content-Type` | Tipo de contenido del cuerpo |
| `Content-Length` | Tamaño del cuerpo en bytes |
| `Cache-Control` | Directivas de caché |
| `Location` | URL del recurso creado |
| `ETag` | Identificador de versión del recurso |

3. **Cuerpo (Body)**: Contiene los datos solicitados

**Ejemplo de respuesta HTTP:**

```http
HTTP/1.1 201 Created
Content-Type: application/json
Content-Length: 85
Location: /api/usuarios/1
ETag: "abc123"

{
  "id": 1,
  "nombre": "Juan Pérez",
  "email": "juan@example.com",
  "creado_en": "2024-01-15T10:30:00Z"
}
```

### 1.2.3. Ejemplos de Servicios Web

Algunos ejemplos populares de servicios web que usamos a diario:

- **API de Google Maps**: Permite acceder a datos de mapas, geolocalización y rutas
- **API de Stripe**: Procesamiento de pagos y gestión de suscripciones
- **API de OpenWeather**: Datos meteorológicos en tiempo real
- **API de GitHub**: Interactuar con repositorios, issues y usuarios
- **API de Spotify**: Reproducción de música, playlists y recomendaciones
- **API de WhatsApp/Business**: Mensajería automatizada
- **API de Twilio**: Envío de SMS y llamadas telefónicas

💡 **Tip del Examinador**: Conocer ejemplos reales demuestra comprensión práctica del concepto.

### 1.2.4. Arquitecturas de Servicios Web

Existen diferentes arquitecturas de servicios web, cada una con sus propios protocolos y estándares.

```mermaid
flowchart TD
    A[Arquitecturas de Servicios Web] --> B[SOAP]
    A --> C[REST]
    A --> D[GraphQL]
    A --> E[gRPC]
    A --> F[WebSockets]
    
    B --> B1[XML + WSDL]
    C --> C1[JSON + HTTP]
    D --> D1[Query Language]
    E --> E1[Protocol Buffers]
    F --> F1[TCP persistente]
```

**1. SOAP (Simple Object Access Protocol)**

- Protocolo basado en **XML** para el intercambio de mensajes
- Independiente del lenguaje y del protocolo de transporte (HTTP, SMTP, TCP)
- Usa **WSDL** para describir la funcionalidad del servicio

**Pros:**
- Altamente extensible y flexible
- Soporta operaciones complejas (transacciones, seguridad avanzada WS-Security)
- Contrato formal (WSDL)

**Contras:**
- Más lento y consume más recursos (verbosidad de XML)
- Complejo de implementar y aprender

**Ejemplo de uso:** Sistemas empresariales (ERP, CRM, sistemas financieros bancarios)

**2. REST (Representational State Transfer)**

- Estilo arquitectónico que usa **HTTP** y sus métodos (GET, POST, PUT, DELETE)
- Utiliza **JSON** para el intercambio de datos
- Stateless (sin estado)

🧠 **Analogía**: REST es como pedir comida en un restaurante con un menú fijo. Pides lo que quieres (GET), pides algo nuevo (POST), cambias tu pedido (PUT), o cancelas (DELETE). Todo sigue un protocolo simple.

**Pros:**
- Simple de entender e implementar
- Eficiente y rápido (JSON es ligero)
- Soporta caché de forma nativa
- Ampliamente adoptado

**Contras:**
- Menos seguro que SOAP (requiere HTTPS y JWT)
- No adecuado para operaciones muy complejas
- Over-fetching y under-fetching de datos

**Ejemplo en ASP.NET Core:**

```csharp
namespace Ejemplo.REST.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly List<Usuario> _usuarios = new();

    [HttpGet]
    public ActionResult<IEnumerable<Usuario>> GetUsuarios()
    {
        return Ok(_usuarios);
    }

    [HttpGet("{id}")]
    public ActionResult<Usuario> GetUsuario(int id)
    {
        var usuario = _usuarios.FirstOrDefault(u => u.Id == id);
        if (usuario == null)
            return NotFound();
        return Ok(usuario);
    }

    [HttpPost]
    public ActionResult<Usuario> CrearUsuario(UsuarioDto dto)
    {
        var usuario = new Usuario { Id = _usuarios.Count + 1, Nombre = dto.Nombre };
        _usuarios.Add(usuario);
        return CreatedAtAction(nameof(GetUsuario), new { id = usuario.Id }, usuario);
    }

    [HttpPut("{id}")]
    public ActionResult<Usuario> ActualizarUsuario(int id, UsuarioDto dto)
    {
        var usuario = _usuarios.FirstOrDefault(u => u.Id == id);
        if (usuario == null)
            return NotFound();
        usuario.Nombre = dto.Nombre;
        return Ok(usuario);
    }

    [HttpDelete("{id}")]
    public ActionResult EliminarUsuario(int id)
    {
        var usuario = _usuarios.FirstOrDefault(u => u.Id == id);
        if (usuario == null)
            return NotFound();
        _usuarios.Remove(usuario);
        return NoContent();
    }
}

public record UsuarioDto(string Nombre);
public class Usuario { public int Id { get; set; } public string Nombre { get; set; } = string.Empty; }
```

**3. GraphQL**

- Lenguaje de consulta para APIs que permite a los clientes **especificar exactamente qué datos necesitan**
- Un solo endpoint maneja todas las solicitudes

**Pros:**
- Evita over-fetching y under-fetching de datos
- Más eficiente en algunos escenarios
- Tipado fuerte con esquema definido

**Contras:**
- Más complejo de implementar
- Menor adopción que REST
- Curva de aprendizaje más alta

**Ejemplo de uso:** Facebook (creadores), GitHub API v4

**4. gRPC (Google Remote Procedure Call)**

- Protocolo moderno que usa **HTTP/2** y **Protocol Buffers** (formato binario)
- Comunicación bidireccional y en tiempo real

**Pros:**
- Muy rápido (formato binario)
- Soporta streaming bidireccional
- Tipado fuerte con archivos .proto

**Contras:**
- Menos legible por humanos
- Menor compatibilidad con navegadores (requiere proxy)
- No es ideal para APIs públicas

**Ejemplo de uso:** Microservicios internos, sistemas de alto rendimiento, comunicación app-servidor

**5. WebSockets**

- Protocolo que proporciona **comunicación bidireccional y en tiempo real** sobre una **conexión persistente**

🧠 **Analogía**: Es como una llamada telefónica en tiempo real, donde ambos pueden hablar simultáneamente, en lugar de enviar cartas (HTTP) y esperar respuesta.

**Pros:**
- Comunicación en tiempo real
- Baja latencia
- Bidireccional simultáneo

**Contras:**
- Consume más recursos (conexión siempre abierta)
- No soportado por todos los navegadores/servidores antiguos
- Complejidad en manejo de reconexiones

**Ejemplo de uso:** Chat en vivo, juegos online, dashboards en tiempo real, notificaciones

**Ejemplo en ASP.NET Core con SignalR:**

```csharp
namespace Ejemplo.WebSockets.Hubs;

public class ChatHub : Hub
{
    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }

    public async Task JoinRoom(string roomName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
        await Clients.Group(roomName).SendAsync("ReceiveMessage", "System", $"{Context.ConnectionId} joined {roomName}");
    }
}
```

**Comparación de Arquitecturas:**

| Aspecto | REST | SOAP | GraphQL | gRPC | WebSockets |
|---------|------|------|---------|------|------------|
| **Formato** | JSON | XML | GraphQL | Protobuf | TCP |
| **HTTP** | 1.1/2 | Cualquiera | 1.1/2 | 2 | ws:// |
| **Bidireccional** | No | No | No | Sí | Sí |
| **Tipado** | No | WSDL | Schema | .proto | No |
| **Curva aprendizaje** | Baja | Alta | Media | Media | Media |
| **Rendimiento** | Medio | Bajo | Medio | Alto | Alto |
| **Casos uso** | APIs públicas | Enterprise | APIs flexibles | Microservicios | Tiempo real |

💡 **Tip del Examinador**: Para este curso nos centraremos en **REST** con ASP.NET Core, que es el estándar más utilizado para APIs web.

## 1.3. Creación de Soluciones y Proyectos

Una **solución** en .NET es un contenedor lógico que agrupa uno o más proyectos relacionados. Separar la solución del código fuente permite gestionar mejor las dependencias entre proyectos y facilita el trabajo en equipo. Cuando creas una solución, estás creando un archivo .sln que actúa como índice de todos los proyectos que forman parte de tu aplicación.

### 1.3.1. ¿Por qué usar soluciones?

Imagina que tienes una aplicación de tienda con tres partes: la API que expone los endpoints, la lógica de negocio que contiene las reglas y entidades, y los tests que verifican que todo funcione. Cada una de estas partes es un proyecto independiente, pero están relacionadas entre sí. La solución actúa como el pegamento que las une, permitiéndote abrir todo el código en Visual Studio o Rider con un solo archivo.

### 1.3.2. Crear la estructura desde cero

Abre tu terminal en la carpeta donde quieres crear el proyecto y ejecuta los siguientes comandos. Cada comando crea un proyecto diferente y lo añade automáticamente a la solución:

```bash
# Crear una solución vacía que contendrá todos los proyectos
dotnet new slnx -n TiendaApi

# Crear el proyecto de API (contiene controladores, middleware y configuración)
dotnet new webapi -n TiendaApi.Apis -o ./TiendaApi.Apis

# Crear el proyecto de Core (contiene entidades, servicios e interfaces)
dotnet new classlib -n TiendaApi.Core -o ./TiendaApi.Core

# Crear el proyecto de Tests con NUnit
dotnet new nunit -n TiendaApi.Tests -o ./TiendaApi.Tests

# Añadir cada proyecto a la solución
dotnet slnx add TiendaApi.Apis/TiendaApi.Apis.csproj
dotnet slnx add TiendaApi.Core/TiendaApi.Core.csproj
dotnet slnx add TiendaApi.Tests/TiendaApi.Tests.csproj
```

### 1.3.3. Establecer referencias entre proyectos

Los proyectos necesitan "conocerse" entre sí para poder usar las clases definidas en otros. La API necesita acceder a Core, y los Tests necesitan acceder tanto a API como a Core para poder testearlos:

```bash
# La API referencia a Core porque necesita usar sus servicios
dotnet add TiendaApi.Apis/TiendaApi.Apis.csproj reference TiendaApi.Core/TiendaApi.Core.csproj

# Los Tests referencian a API y Core para poder testearlos
dotnet add TiendaApi.Tests/TiendaApi.Tests.csproj reference TiendaApi.Apis/TiendaApi.Apis.csproj
dotnet add TiendaApi.Tests/TiendaApi.Tests.csproj reference TiendaApi.Core/TiendaApi.Core.csproj
```

### 1.3.4. Estructura de carpetas resultante

```mermaid
flowchart TB
    subgraph "Archivos de solución"
        SLN["TiendaApi.slnx"]
    end
    
    subgraph "Proyecto API"
        API["TiendaApi.Apis/"]
        API --> API_CS["TiendaApi.Apis.csproj"]
        API --> PROGRAM["Program.cs"]
        API --> CONTROLLERS["Controllers/"]
    end
    
    subgraph "Proyecto Core"
        CORE["TiendaApi.Core/"]
        CORE --> CORE_CS["TiendaApi.Core.csproj"]
        CORE --> MODELS["Models/"]
        CORE --> SERVICES["Services/"]
        CORE --> DTOS["Dtos/"]
    end
    
    subgraph "Proyecto Tests"
        TESTS["TiendaApi.Tests/"]
        TESTS --> TESTS_CS["TiendaApi.Tests.csproj"]
        TESTS --> UNIT["UnitTests/"]
    end
    
    SLN -.-> API
    SLN -.-> CORE
    SLN -.-> TESTS
    
    API --> CORE
    TESTS --> API
    TESTS --> CORE
```

## 1.4. Instalación de Librerías con NuGet

NuGet es el gestor de paquetes de .NET. Cuando necesitas una funcionalidad que no viene incluida en el framework, como conectar con PostgreSQL o validar datos con FluentValidation, buscas el paquete correspondiente y lo instalas con un comando. Los paquetes se descargan desde nuget.org y se almacenan en caché en tu máquina.

### 1.4.1. ¿Qué son los paquetes NuGet?

Piensa en NuGet como una biblioteca gigante donde desarrolladores de todo el mundo comparten código reusable. Entity Framework Core para acceder a bases de datos, AutoMapper para convertir entre objetos, o Serilog para registrar logs, todos son paquetes NuGet que puedes incorporar a tu proyecto con un solo comando.

### 1.4.2. Instalación de paquetes esenciales

Estos son los paquetes más utilizados en el proyecto TiendaApi. Ejecuta estos comandos desde la carpeta de cada proyecto:

```bash
# ======= ENTITY FRAMEWORK CORE =======
# Paquete principal de EF Core
dotnet add TiendaApi.Core/TiendaApi.Core.csproj package Microsoft.EntityFrameworkCore

# Proveedor para PostgreSQL
dotnet add TiendaApi.Core/TiendaApi.Core.csproj package Npgsql.EntityFrameworkCore.PostgreSQL

# Herramientas de línea de comandos para migraciones
dotnet add TiendaApi.Apis/TiendaApi.Apis.csproj package Microsoft.EntityFrameworkCore.Tools

# ======= MAPPING Y VALIDACIÓN =======
# AutoMapper para transformar objetos automáticamente
dotnet add TiendaApi.Core/TiendaApi.Core.csproj package AutoMapper
dotnet add TiendaApi.Core/TiendaApi.Core.csproj package AutoMapper.Extensions.Microsoft.DependencyInjection

# FluentValidation para reglas de validación personalizadas
dotnet add TiendaApi.Core/TiendaApi.Core.csproj package FluentValidation
dotnet add TiendaApi.Core/TiendaApi.Core.csproj package FluentValidation.DependencyInjection

# ======= RESULT PATTERN =======
# CSharpFunctionalExtensions para el patrón Result
dotnet add TiendaApi.Core/TiendaApi.Core.csproj package CSharpFunctionalExtensions

# ======= AUTENTICACIÓN =======
# JWT Bearer para autenticación basada en tokens
dotnet add TiendaApi.Apis/TiendaApi.Apis.csproj package Microsoft.AspNetCore.Authentication.JwtBearer

# ======= CACHE =======
# StackExchange.Redis para cache distribuido
dotnet add TiendaApi.Core/TiendaApi.Core.csproj package StackExchange.Redis

# ======= BASE DE DATOS DOCUMENTAL =======
# Controlador oficial de MongoDB
dotnet add TiendaApi.Core/TiendaApi.Core.csproj package MongoDB.Driver

# ======= RESILIENCIA =======
# Polly para reintentos y circuit breaker
dotnet add TiendaApi.Core/TiendaApi.Core.csproj package Polly
dotnet add TiendaApi.Core/TiendaApi.Core.csproj package Polly.Extensions.Http

# ======= DOCUMENTACIÓN =======
# Swashbuckle para generar documentación OpenAPI/Swagger
dotnet add TiendaApi.Apis/TiendaApi.Apis.csproj package Swashbuckle.AspNetCore

# ======= TESTING =======
# NUnit como framework de testing
dotnet add TiendaApi.Tests/TiendaApi.Tests.csproj package NUnit
dotnet add TiendaApi.Tests/TiendaApi.Tests.csproj package NUnit3TestAdapter

# Moq para crear objetos simulados
dotnet add TiendaApi.Tests/TiendaApi.Tests.csproj package Moq

# FluentAssertions para aserciones más legibles
dotnet add TiendaApi.Tests/TiendaApi.Tests.csproj package FluentAssertions
```

### 1.4.3. Comandos útiles de NuGet

```bash
# Restaurar todos los paquetes del proyecto
dotnet restore

# Listar los paquetes instalados en un proyecto
dotnet list package

# Ver versiones disponibles de un paquete
dotnet list package --all-versions | findstr AutoMapper

# Actualizar un paquete a la última versión
dotnet add package AutoMapper

# Actualizar a una versión específica
dotnet add package AutoMapper --version 12.0.0
```

## 1.5. Estructura de Proyectos

Una arquitectura bien organizada separa las responsabilidades en capas claramente definidas. Esto facilita el mantenimiento, las pruebas y la evolución del código. En este proyecto utilizamos una arquitectura por capas donde cada proyecto tiene una responsabilidad específica.

### 1.5.1. ¿Por qué separar en proyectos?

Cuando estás desarrollando, es tentador poner todo en un solo proyecto para ir más rápido. Sin embargo, a medida que la aplicación crece, esta decisión se convierte en un problema. Los tests necesitan acceder a tu código sin arrastrar dependencias de servidor web, y quieres poder reutilizar la lógica de negocio en diferentes aplicaciones (web, móvil, CLI). Separar en proyectos te da esta flexibilidad.

### 1.5.2. Estructura completa del proyecto

```
TiendaApi.sln
│
├── TiendaApi.Apis/                          # Capa de presentación: controladores, middleware y configuración Web
│   ├── Controllers/                          # Controladores REST (Auth, Users, Productos, etc.)
│   ├── Middleware/                           # Middlewares personalizados (Exception Handler, etc.)
│   ├── WebSockets/                           # Hubs de SignalR para tiempo real
│   ├── GraphQL/                              # Tipos y resolvers de GraphQL
│   ├── Program.cs                            # Punto de entrada y configuración
│   ├── appsettings.json                      # Configuración general
│   ├── appsettings.Development.json          # Configuración específica de desarrollo
│   └── TiendaApi.Apis.csproj                 # Archivo de proyecto
│
├── TiendaApi.Core/                           # Capa de negocio: entidades, servicios e interfaces
│   ├── Models/                               # Entidades de dominio (User, Producto, Pedido, etc.)
│   ├── Dtos/                                 # Data Transfer Objects para la API
│   │   ├── Common/                           # DTOs compartidos (PagedResult, etc.)
│   │   ├── Auth/                             # DTOs de autenticación
│   │   ├── Productos/                        # DTOs de productos
│   │   ├── Pedidos/                          # DTOs de pedidos
│   │   └── ...
│   ├── Mappers/                              # Perfiles de AutoMapper
│   ├── Services/                             # Servicios de negocio con lógica de dominio
│   │   ├── Auth/                             # AuthService, JwtService, etc.
│   │   ├── Productos/                        # ProductoService
│   │   ├── Pedidos/                          # PedidosService
│   │   └── ...
│   ├── Repositories/                         # Implementaciones de repositorios
│   │   ├── Productos/
│   │   ├── Pedidos/
│   │   └── ...
│   ├── Interfaces/                           # Contratos (interfaces) para inyección de dependencias
│   │   ├── IServices/
│   │   ├── IRepositories/
│   │   └── ...
│   ├── Validators/                           # Validadores de FluentValidation
│   ├── Errors/                               # Tipos de errores personalizados (DomainError, etc.)
│   ├── Exceptions/                           # Excepciones personalizadas
│   ├── Data/                                 # DbContext y configuración de datos
│   │   ├── Seed/                             # Seeders de datos iniciales
│   │   └── Interceptors/                     # Interceptors de EF Core
│   └── TiendaApi.Core.csproj
│
└── TiendaApi.Tests/                          # Capa de pruebas: unitarias y de integración
    ├── UnitTests/                            # Tests unitarios
    │   ├── Services/
    │   ├── Controllers/
    │   └── Mappers/
    ├── IntegrationTests/                     # Tests de integración
    ├── Fixtures/                             # Clases de configuración para tests
    └── TiendaApi.Tests.csproj
```

### 1.5.3. Flujo de datos entre capas

```mermaid
flowchart TB
    subgraph "TiendaApi.Apis [Capa de Presentación]"
        A1["Controllers"]
        A2["Middleware"]
    end
    
    subgraph "TiendaApi.Core [Capa de Negocio]"
        B1["Services"]
        B2["Repositories"]
        B3["Models"]
        B4["Validators"]
    end
    
    subgraph "Base de Datos"
        C1["PostgreSQL"]
        C2["MongoDB"]
        C3["Redis"]
    end
    
    subgraph "TiendaApi.Tests [Capa de Pruebas]"
        D1["Unit Tests"]
        D2["Integration Tests"]
    end
    
    A1 --> B1
    A2 --> B1
    
    B1 --> B2
    B1 --> B3
    
    B2 --> C1
    B2 --> C2
    B2 --> C3
    
    B3 --> B2
    
    D1 --> B1
    D2 --> A1
```

## 1.6. Configuración con appsettings.json

El archivo `appsettings.json` es el método principal de configuración en ASP.NET Core. Permite definir valores que pueden cambiar entre entornos (desarrollo, producción) sin modificar el código. La configuración se carga en orden jerárquico, donde los valores de entornos más específicos sobrescriben los generales.

### 1.6.1. ¿Cómo funciona la configuración?

Cuando inicias una aplicación ASP.NET Core, el framework lee automáticamente el archivo `appsettings.json` y lo fusiona con `appsettings.{Entorno}.json`. Luego añade las variables de entorno y otros orígenes, creando una jerarquía donde los valores más específicos tienen prioridad. Esto significa que puedes tener valores por defecto en el archivo general y sobrescribirlos específicamente para desarrollo o producción.

### 1.6.2. Archivo appsettings.json básico

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning"
    },
    "Console": {
      "IncludeScopes": true
    }
  },
  "AllowedHosts": "*",
  
  "ConnectionStrings": {
    "PostgreSQL": "Host=localhost;Database=TiendaDb;Username=postgres;Password=secret",
    "Redis": "localhost:6379",
    "MongoDB": "mongodb://localhost:27017"
  },
  
  "Jwt": {
    "SecretKey": "TuClaveSecretaMuyLargaYSegura123456789",
    "Issuer": "TiendaApi",
    "Audience": "TiendaApiClients",
    "ExpiryMinutes": 60,
    "RefreshExpiryDays": 7
  },
  
  "Storage": {
    "BasePath": "./wwwroot/uploads",
    "MaxFileSizeMb": 10,
    "AllowedExtensions": [".jpg", ".jpeg", ".png", ".gif", ".pdf"]
  },
  
  "Email": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderEmail": "noreply@tienda.com",
    "SenderName": "Tienda DAW"
  },
  
  "Cache": {
    "DefaultExpirationMinutes": 5,
    "SlidingExpirationMinutes": 2
  },
  
  "Cors": {
    "AllowedOrigins": ["http://localhost:3000", "https://tienda.com"]
  }
}
```

### 1.6.3. appsettings.Development.json

Este archivo contiene configuración específica para el entorno de desarrollo. En desarrollo quieres ver más información de logs, usar conexiones a bases de datos locales, y tener habilitado el hot reload:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Information",
      "Microsoft.EntityFrameworkCore": "Debug"
    }
  },
  "ConnectionStrings": {
    "PostgreSQL": "Host=localhost;Database=TiendaDb;Username=postgres;Password=postgres",
    "Redis": "localhost:6379",
    "MongoDB": "mongodb://localhost:27017"
  },
  "Jwt": {
    "SecretKey": "ClaveDeDesarrollo123456789012345678901234567890"
  },
  "DetailedErrors": true,
  "BrowserLink": true
}
```

### 1.6.4. appsettings.Production.json

En producción quieres menos logs, conexiones a bases de datos remotas, y configuración optimizada para rendimiento:

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
    "PostgreSQL": "Host=prod-db.example.com;Database=TiendaDb;Username=webuser;Password=${DB_PASSWORD}",
    "Redis": "prod-redis.example.com:6379"
  },
  "AllowedHosts": "tienda.com",
  "DetailedErrors": false
}
```

### 1.6.5. Leer configuración en código

```csharp
var builder = WebApplication.CreateBuilder(args);

// Método 1: Acceso directo a configuración
var connectionString = builder.Configuration.GetConnectionString("PostgreSQL");
var jwtSecret = builder.Configuration["Jwt:SecretKey"];

// Método 2: Obtener una sección completa
var jwtConfig = builder.Configuration.GetSection("Jwt");
var secretKey = jwtConfig["SecretKey"];

// Método 3: Usando opciones patrón (recomendado para objetos complejos)
builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection("Jwt"));

builder.Services.Configure<StorageOptions>(
    builder.Configuration.GetSection("Storage"));

// Método 4: Opciones con validación automática
builder.Services.AddOptions<ConnectionStrings>()
    .Bind(builder.Configuration.GetSection("ConnectionStrings"))
    .ValidateDataAnnotations()
    .ValidateOnStart();
```

### 1.6.6. Jerarquía de configuración

```mermaid
flowchart TB
    subgraph "Orden de precedencia (menor a mayor)"
        A["appsettings.json"]
        B["appsettings.{Environment}.json"]
        C["User Secrets (Development)"]
        D["Environment Variables"]
        E["Command Line Arguments"]
    end
```

## 1.7. Variables de Entorno y Secrets de Usuario

Las variables de entorno permiten configurar la aplicación sin modificar archivos de código, lo cual es esencial para el despliegue en producción donde no tienes acceso directo a los archivos de configuración. Los User Secrets son una forma segura de almacenar configuración sensible durante el desarrollo sin riesgo de accidentalmente commitearla al repositorio.

### 1.7.1. ¿Cuándo usar cada método?

Las variables de entorno son ideales para configuración que cambia entre entornos: cadenas de conexión a bases de datos, URLs de servicios externos, y cualquier secreto que no quieras incluir en archivos de configuración versionados. Los User Secrets son específicos del desarrollo local y están diseñados para mantener la configuración sensible fuera del sistema de control de versiones.

### 1.7.2. Variables de entorno en el sistema

```bash
# En Linux o macOS
export DOTNET_ENVIRONMENT=Production
export ConnectionStrings__PostgreSQL=Host=prod-server;Database=TiendaDb
export Jwt__SecretKey=SuperSecretKeyDeProduccion

# En Windows (Command Prompt)
set ConnectionStrings__PostgreSQL=Host=prod-server;Database=TiendaDb

# En Windows (PowerShell)
$env:ConnectionStrings__PostgreSQL = "Host=prod-server;Database=TiendaDb"
```

### 1.7.3. Variables de entorno en Docker

En Docker Compose, las variables de entorno se definen en el archivo YAML de forma declarativa:

```yaml
services:
  api:
    image: tiendaapi:latest
    container_name: tiendaapi
    environment:
      - DOTNET_ENVIRONMENT=Production
      - ConnectionStrings__PostgreSQL=Host=db;Database=TiendaDb
      - ConnectionStrings__Redis=Host=redis
      - Jwt__SecretKey=${JWT_SECRET_KEY}
      - Jwt__Issuer=TiendaApi
      - Jwt__Audience=TiendaApiClients
    ports:
      - "5000:80"
    depends_on:
      - db
      - redis
    restart: unless-stopped

  db:
    image: postgres:15-alpine
    environment:
      - POSTGRES_DB=TiendaDb
      - POSTGRES_USER=postgres
      - POSTGRES_PASSWORD=${DB_PASSWORD}
    volumes:
      - postgres_data:/var/lib/postgresql/data

  redis:
    image: redis:7-alpine
    volumes:
      - redis_data:/data

volumes:
  postgres_data:
  redis_data:
```

### 1.7.4. Secrets de Usuario para Desarrollo

Los User Secrets almacenan configuración en un archivo JSON especial fuera del directorio del proyecto, por lo que nunca se incluyen en el control de versiones:

```bash
# Desde el directorio del proyecto API
cd TiendaApi.Apis

# Inicializar User Secrets (crea un ID único en el csproj)
dotnet user-secrets init

# Establecer secretos
dotnet user-secrets set "Jwt:SecretKey" "MiClaveSecretaSoloParaDesarrollo"

dotnet user-secrets set "ConnectionStrings:PostgreSQL" "Host=localhost;Database=TiendaDb"

dotnet user-secrets set "ExternalServices:SendGridApiKey" "SG.xxxxxxx"

# Listar todos los secretos
dotnet user-secrets list

# Eliminar un secreto
dotnet user-secrets remove "ExternalServices:SendGridApiKey"

# Eliminar todos los secretos
dotnet user-secrets clear
```

### 1.7.5. Ubicación de User Secrets por sistema operativo

| Sistema Operativo | Ubicación |
|-------------------|-----------|
| Windows | `%APPDATA%\Microsoft\UserSecrets\<user_secrets_id>\secrets.json` |
| Linux | `~/.microsoft/usersecrets/<user_secrets_id>/secrets.json` |
| macOS | `~/.microsoft/usersecrets/<user_secrets_id>/secrets.json` |

### 1.7.6. Configurar User Secrets en Program.cs

```csharp
var builder = WebApplication.CreateBuilder(args);

var environment = builder.Environment.EnvironmentName;

Console.WriteLine($"Entorno actual: {environment}");

// En desarrollo, añadir User Secrets a la configuración
if (environment == "Development")
{
    builder.Configuration.AddUserSecrets<Program>();
}

var app = builder.Build();
```

```mermaid
flowchart LR
    subgraph "Configuración segura"
        A["appsettings.json"] --> B["appsettings.Development.json"]
        B --> C["User Secrets (no en Git)"]
        C --> D["Environment Variables"]
    end
    
    subgraph "Produccion"
        E["appsettings.Production.json"]
        F["Environment Variables"]
        G["Secretos del orquestador (Kubernetes, Docker)"]
    end
    
    style C fill:#4CAF50
    style G fill:#4CAF50
```

## 1.8. Patrón de Opciones (IOptions<T>)

El patrón de opciones proporciona acceso tipado y fuertemente tipado a la configuración. En lugar de leer configuraciones como strings con claves, puedes definir clases que representan tu configuración y acceder a sus propiedades directamente. Además, incluye soporte para validación y cambios en tiempo real.

### 1.8.1. Beneficios del patrón de opciones

Al usar clases tipadas para la configuración, el compilador te ayuda a detectar errores: si escribes mal un nombre de propiedad, el código no compilará. También puedes agregar validación automática que garantice que la configuración necesaria está presente antes de iniciar la aplicación. Esto es mucho más seguro que acceder a configuraciones por strings que pueden fallar en tiempo de ejecución.

### 1.8.2. Definir clases de opciones

```csharp
namespace TiendaApi.Apis.Configuration;

public class JwtOptions
{
    public const string Section = "Jwt";
    
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpiryMinutes { get; set; } = 60;
    public int RefreshTokenExpiryDays { get; set; } = 7;
}

public class StorageOptions
{
    public const string Section = "Storage";
    
    public string BasePath { get; set; } = "./wwwroot/uploads";
    public long MaxFileSizeBytes { get; set; } = 10 * 1024 * 1024; // 10 MB
    public List<string> AllowedExtensions { get; set; } = new() 
        { ".jpg", ".jpeg", ".png", ".gif", ".pdf" };
}

public class CacheOptions
{
    public const string Section = "Cache";
    
    public int DefaultExpirationMinutes { get; set; } = 5;
    public int SlidingExpirationMinutes { get; set; } = 2;
}

public class PaginationOptions
{
    public const string Section = "Pagination";
    
    public int DefaultPageSize { get; set; } = 10;
    public int MaxPageSize { get; set; } = 100;
}
```

### 1.8.3. Registrar opciones en Program.cs

```csharp
using TiendaApi.Apis.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Registrar opciones con configuración
builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection(JwtOptions.Section));

builder.Services.Configure<StorageOptions>(
    builder.Configuration.GetSection(StorageOptions.Section));

builder.Services.Configure<CacheOptions>(
    builder.Configuration.GetSection(CacheOptions.Section));

builder.Services.Configure<PaginationOptions>(
    builder.Configuration.GetSection(PaginationOptions.Section));

// Validación automática
builder.Services.AddOptions<JwtOptions>()
    .Bind(builder.Configuration.GetSection(JwtOptions.Section))
    .ValidateDataAnnotations()
    .ValidateOnStart();

// Configuración de conexión strings
builder.Services.Configure<ConnectionStrings>(
    builder.Configuration.GetSection("ConnectionStrings"));
```

### 1.8.4. Consumir opciones en servicios

```csharp
using Microsoft.Extensions.Options;

namespace TiendaApi.Core.Services.Auth;

public class JwtService(IOptions<JwtOptions> jwtOptions, ILogger<JwtService> logger)
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public string GenerateToken(User user)
    {
        if (string.IsNullOrEmpty(_jwtOptions.SecretKey))
        {
            throw new InvalidOperationException("JWT SecretKey no configurada");
        }

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiryMinutes),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
```

### 1.8.5. Diferencia entre IOptions, IOptionsSnapshot e IOptionsMonitor

```csharp
// IOptions<T>: Singleton, no cambia durante la vida de la aplicación
public class SingletonService(IOptions<JwtOptions> options)
{
    private readonly JwtOptions _options = options.Value;
    // El valor se lee una vez al inicio
}

// IOptionsSnapshot<T>: Scoped, cambia por cada request HTTP
public class RequestService(IOptionsSnapshot<JwtOptions> options)
{
    private readonly JwtOptions _options = options.Value;
    // Se recrea para cada request, útil para configuración que cambia
}

// IOptionsMonitor<T>: Singleton con soporte de callback para cambios
public class MonitorService(IOptionsMonitor<JwtOptions> options)
{
    public MonitorService()
    {
        // Callback cuando la configuración cambia
        options.OnChange(newOptions => 
        {
            Console.WriteLine($"Configuración actualizada: {newOptions.SecretKey}");
        });
    }
}
```

## 1.9. Testing con NUnit

NUnit es un framework de testing unitario ampliamente utilizado en .NET. Permite escribir pruebas automatizadas que verifican el comportamiento de tu código. NUnit usa atributos como `[Test]` y `[SetUp]` para definir pruebas y lógica de preparación, proporcionando una sintaxis clara y expresiva para organizar tus tests.

### 1.9.1. ¿Por qué hacer tests?

Imagina que modificas el método de cálculo de precio total en un pedido. Sin tests, tendrías que probar manualmente todos los escenarios posibles: pedido vacío, un producto, múltiples productos, descuentos, impuestos. Con tests automatizados, puedes ejecutar cientos de pruebas en segundos cada vez que haces un cambio, garantizando que no has roto nada existente.

### 1.9.2. Estructura básica de un test NUnit

```csharp
using NUnit.Framework;
using FluentAssertions;

namespace TiendaApi.Tests.Unit.Services;

[TestFixture]
public class ProductoServiceTests
{
    private Mock<IProductoRepository> _repositoryMock;
    private Mock<ILogger<ProductoService>> _loggerMock;
    private ProductoService _service;

    [SetUp]
    public void SetUp()
    {
        // Arrange: Preparar el contexto antes de cada test
        _repositoryMock = new Mock<IProductoRepository>();
        _loggerMock = new Mock<ILogger<ProductoService>>();
        _service = new ProductoService(
            _repositoryMock.Object,
            _loggerMock.Object);
    }

    [Test]
    public void GetById_ProductoExistente_ReturnsProducto()
    {
        // Arrange
        var productoId = 1L;
        var producto = new Producto { Id = productoId, Nombre = "Laptop" };
        _repositoryMock.Setup(r => r.FindByIdAsync(productoId))
            .ReturnsAsync(producto);

        // Act
        var result = _service.GetByIdAsync(productoId);

        // Assert
        result.Should().NotBeNull();
        result.Result.IsSuccess.Should().BeTrue();
        result.Result.Value.Nombre.Should().Be("Laptop");
    }

    [Test]
    public void GetById_ProductoNoExistente_ReturnsNotFound()
    {
        // Arrange
        var productoId = 999L;
        _repositoryMock.Setup(r => r.FindByIdAsync(productoId))
            .ReturnsAsync((Producto)null!);

        // Act
        var result = _service.GetByIdAsync(productoId);

        // Assert
        result.Result.IsSuccess.Should().BeFalse();
        result.Result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [TearDown]
    public void TearDown()
    {
        // Limpieza después de cada test
        _repositoryMock.VerifyNoOtherCalls();
    }
}
```

### 1.9.3. Comandos para ejecutar tests

```bash
# Ejecutar todos los tests del proyecto
dotnet test

# Ejecutar tests con salida detallada
dotnet test -v normal

# Ejecutar un proyecto específico
dotnet test TiendaApi.Tests/TiendaApi.Tests.csproj

# Filtrar tests por nombre de clase o método
dotnet test --filter "FullyQualifiedName~ProductoServiceTests"
dotnet test --filter "DisplayName~GetById_ProductoExistente"

# Filtrar por categoría
dotnet test --filter "Category=Unit"
dotnet test --filter "Category!=Integration"

# Ejecutar en paralelo (más rápido)
dotnet test --parallel

# Generar reporte de cobertura de código
dotnet test --collect:"XPlat Code Coverage"

# Combinación de opciones
dotnet test -v normal --filter "Category=Unit" --parallel
```

### 1.9.4. Configurar cobertura con Coverlet

Edita el archivo del proyecto de tests para habilitar la cobertura:

```xml
<!-- TiendaApi.Tests.csproj -->
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
    <PackageReference Include="NUnit" Version="3.14.0" />
    <PackageReference Include="NUnit3TestAdapter" Version="4.5.0" />
    <PackageReference Include="FluentAssertions" Version="6.12.0" />
    <PackageReference Include="Moq" Version="4.20.70" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="8.0.0" />
    <PackageReference Include="Coverlet.Collector" Version="6.0.0">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
  </ItemGroup>

  <!-- Configuración de cobertura -->
  <PropertyGroup>
    <CollectCoverage>true</CollectCoverage>
    <CoverletOutputFormat>json,lcov</CoverletOutputFormat>
    <CoverletOutput>./coverage/</CoverletOutput>
    <Threshold>80</Threshold>
    <ThresholdType>line,branch</ThresholdType>
  </PropertyGroup>

</Project>
```

```bash
# Ejecutar tests con cobertura
dotnet test /p:CollectCoverage=true

# Ver reporte en formato HTML
dotnet reportgenerator -reports:coverage/coverage.json -targetdir:coverage/html -reporttypes:Html

# Ver cobertura mínima requerida
dotnet test /p:CollectCoverage=true /p:Threshold=80
```

### 1.9.5. Atributos útiles de NUnit

```csharp
[TestFixture]           // Clase que contiene tests
public class Tests { }

[Test]                  // Método de test
public void Test() { }

[SetUp]                 // Se ejecuta antes de cada test
public void Setup() { }

[TearDown]              // Se ejecuta después de cada test
public void TearDown() { }

[OneTimeSetUp]          // Se ejecuta una vez antes de todos los tests
public void OneTimeSetup() { }

[OneTimeTearDown]       // Se ejecuta una vez después de todos los tests
public void OneTimeTearDown() { }

[TestCase(1, 2, 3)]     // Parámetros para el test
public void TestCase(int a, int b, int expected) { }

[TestCaseSource(nameof(SumaData))]  // Fuente externa de datos
public void TestWithData(int a, int b, int expected) { }

[Ignore("Razón por la que se ignora")]
public void IgnoredTest() { }

[Category("Unit")]      // Categoría para filtrado
public void CategorizedTest() { }

[Description("Descripción del test")]
public void DescribedTest() { }
```

```mermaid
flowchart TB
    subgraph "NUnit Test Lifecycle"
        A["[OneTimeSetUp]<br/>Una vez al inicio"]
        B["[SetUp]<br/>Antes de cada test"]
        C["[Test]<br/>Ejecutar test"]
        D["[TearDown]<br/>Después de cada test"]
        E["[OneTimeTearDown]<br/>Una vez al final"]
    end
    
    A --> B --> C --> D
    B --> C --> D
    B --> C --> D
    E --> D
    
    style A fill:#4CAF50
    style C fill:#2196F3
    style E fill:#4CAF50
```

## 1.10. Hot Reload y dotnet watch run

Hot Reload es una característica de .NET que permite ver los cambios en el código reflejados inmediatamente en la aplicación en ejecución, sin necesidad de detener y reiniciar el servidor. Esto acelera enormemente el ciclo de desarrollo, especialmente cuando estás ajustando la interfaz de usuario o depurando el comportamiento de un endpoint.

### 1.10.1. ¿Cómo funciona Hot Reload?

Cuando ejecutas `dotnet watch run`, el CLI monitoriza los archivos de tu proyecto. Cuando detecta un cambio en un archivo C#, Razor o configuración, recompila solo los archivos modificados y actualiza la aplicación en ejecución. Dependiendo del tipo de cambio, puede que la actualización sea instantánea o requiera un breve reinicio del servidor de desarrollo.

### 1.10.2. Uso básico de dotnet watch

```bash
# Iniciar la aplicación con hot reload habilitado
dotnet watch run

# Especificar entorno
dotnet watch run --environment Development

# Especificar URLs
dotnet watch run --urls "http://localhost:5000"

# Con argumentos adicionales
dotnet watch run -- --urls "http://localhost:5000" --environment Development

# Modo verbose para ver qué archivos se recompilan
dotnet watch run -v verbose
```

### 1.10.3. Configurar Hot Reload en el proyecto

Edita el archivo `.csproj` para habilitar hot reload:

```xml
<PropertyGroup>
  <TargetFramework>net8.0</TargetFramework>
  <Nullable>enable</Nullable>
  <ImplicitUsings>enable</ImplicitUsings>
  <IsPackable>false</IsPackable>
</PropertyGroup>

<!-- Configuración de Hot Reload -->
<PropertyGroup>
  <EnableHotReload>true</EnableHotReload>
</PropertyGroup>
```

### 1.10.4. Hot Reload con Docker Compose

Para desarrollo con Docker, puedes configurar hot reload mapeando el código fuente como volumen:

```dockerfile
# Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS development
WORKDIR /src

# Copiar solo archivos de proyecto primero (para caché de Docker)
COPY ["TiendaApi.Apis/TiendaApi.Apis.csproj", "TiendaApi.Apis/"]
COPY ["TiendaApi.Core/TiendaApi.Core.csproj", "TiendaApi.Core/"]
COPY ["TiendaApi.Tests/TiendaApi.Tests.csproj", "TiendaApi.Tests/"]

RUN dotnet restore

# Copiar todo el código fuente
COPY . .

EXPOSE 80

# Iniciar con watch para hot reload
CMD ["dotnet", "watch", "run", "--urls", "http://0.0.0.0:80", "--environment", "Development"]
```

```yaml
# docker-compose.override.yml
services:
  api:
    build:
      context: .
      dockerfile: Dockerfile
      target: development
    volumes:
      - ./src:/src
      - ~/.nuget/packages:/root/.nuget/packages:ro
    environment:
      - DOTNET_ENVIRONMENT=Development
      - DOTNET_ENABLE_HOT_RELOAD=1
    ports:
      - "5000:80"
    tty: true
```

### 1.10.5. Archivos monitorizados

Hot Reload detecta cambios en los siguientes tipos de archivos:

| Tipo de Archivo | ¿Recarga Automática? |
|-----------------|---------------------|
| Archivos `.cs` (código C#) | Sí, la mayoría de cambios |
| Archivos `.cshtml` (Razor) | Sí, inmediato |
| Archivos `.razor` (Blazor) | Sí, inmediato |
| Archivos `.json` (configuración) | Sí, con reload automático |
| Archivos `.css`, `.js` (estáticos) | Sí, inmediato |
| Archivos `.csproj` (proyecto) | No, requiere reinicio |

### 1.10.6. Limitaciones de Hot Reload

Algunos cambios requieren un reinicio completo de la aplicación porque Hot Reload no puede aplicarlos dinámicamente:

```csharp
// Estos cambios requieren reinicio:
public class ProductoService : IProductoService  // Cambiar interfaz
{
    public void Metodo(int x, int y) {}           // Cambiar firma
    public string NuevaPropiedad { get; set; }    // Añadir propiedad
}

// Estos cambios se aplican en caliente:
public class ProductoService
{
    private readonly ILogger<ProductoService> _logger;  // Modificar implementación
    public async Task<Producto> GetById(long id)       // Modificar cuerpo
    {
        // changes apply automatically
    }
}
```

### Comparación de rendimiento

```mermaid
flowchart LR
    subgraph "Desarrollo sin Hot Reload"
        A["Escribir código"]
        B["Ctrl+Shift+B (Build)"]
        C["Detener depurador"]
        D["Iniciar depurador"]
        E["Probar cambio"]
        A --> B --> C --> D --> E --> A
        E -->|"~2-5 minutos"| A
    end
    
    subgraph "Desarrollo con Hot Reload"
        F["Escribir código"]
        G["Guardar archivo"]
        H["Ver resultado"]
        F --> G --> H --> F
        H -->|"~2-5 segundos"| F
    end
    
    style F fill:#4CAF50
    style G fill:#4CAF50
    style H fill:#4CAF50
```

### Configurar en VS Code

```json
// .vscode/tasks.json
{
  "version": "2.0.0",
  "tasks": [
    {
      "label": "dotnet: watch run",
      "command": "dotnet",
      "args": ["watch", "run"],
      "type": "process",
      "problemMatcher": "$msCompile",
      "isBackground": true,
      "group": {
        "kind": "build",
        "isDefault": true
      },
      "options": {
        "env": {
          "DOTNET_ENVIRONMENT": "Development"
        }
      }
    },
    {
      "label": "      "command": "dotnet",
      "args": ["dotnet: test",
test", "--no-build", "--collect:XPlat Code Coverage"],
      "type": "process",
      "problemMatcher": "$msCompile",
      "group": {
        "kind": "test",
        "isDefault": true
      }
    }
  ]
}
```

```json
// .vscode/launch.json
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": "Watch Run",
      "type": "dotnet",
      "request": "launch",
      "program": "${workspaceFolder}/TiendaApi.Apis/bin/Debug/net8.0/TiendaApi.Apis.dll",
      "args": [],
      "env": {
        "DOTNET_ENVIRONMENT": "Development"
      },
      "preLaunchTask": "dotnet: watch run"
    }
  ]
}
```

## 1.11. Resumen y Buenas Prácticas

A lo largo de este documento hemos aprendido a configurar un proyecto .NET desde cero. Estos son los puntos clave que debes recordar y las prácticas recomendadas que facilitarán tu trabajo diario.

### Puntos clave del módulo

La creación de soluciones y proyectos con la estructura adecuada es el primer paso para un proyecto mantenible. Separar la solución en proyectos con responsabilidades claras (API, Core, Tests) facilita el desarrollo a largo plazo. Los paquetes NuGet extienden la funcionalidad del framework de forma modular. La configuración jerárquica con appsettings.json, variables de entorno y User Secrets permite adaptar la aplicación a cada entorno. El patrón de opciones proporciona acceso tipado y validado a la configuración. NUnit te permite escribir tests automatizados que garantizan la calidad del código. Hot Reload acelera drásticamente el ciclo de desarrollo.

### Buenas prácticas

```mermaid
flowchart TB
    subgraph "Organización"
        A1["Separar proyectos por responsabilidad"]
        A2["API → Core → Tests"]
    end
    
    subgraph "Configuración"
        B1["Nunca commitear secrets"]
        B2["Usar User Secrets en desarrollo"]
        B3["Variables de entorno en producción"]
    end
    
    subgraph "Testing"
        C1["Escribir tests antes o junto al código"]
        C2["Mantener cobertura > 80%"]
        C3["NUnit + Moq + FluentAssertions"]
    end
    
    subgraph "Desarrollo"
        D1["Usar dotnet watch run"]
        D2["Configurar User Secrets"]
        D3["Separar appsettings por entorno"]
    end
    
    A1 --> A2
    B1 --> B2 --> B3
    C1 --> C2 --> C3
    D1 --> D2 --> D3
```

### Siguientes pasos

Con la configuración básica lista, el siguiente paso es entender cómo funciona el pipeline HTTP de ASP.NET Core y cómo se procesa una petición desde que llega al servidor hasta que se devuelve la respuesta. Esto te ayudará a comprender dónde insertar tu lógica de negocio y cómo estructurar tus controladores.

### Recursos adicionales

- Documentación oficial de .NET: https://docs.microsoft.com/dotnet
- Documentación de NUnit: https://docs.nunit.org
- Paquetes NuGet: https://www.nuget.org
- Hot Reload: https://docs.microsoft.com/dotnet/core/tools/dotnet-watch
