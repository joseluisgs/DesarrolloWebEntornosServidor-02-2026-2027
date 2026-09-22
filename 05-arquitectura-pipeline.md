- [5. Arquitectura y Pipeline HTTP](#5-arquitectura-y-pipeline-http)
  - [5.1. ASP.NET Core: El framework](#51-aspnet-core-el-framework)
    - [5.1.1. ¿Qué es ASP.NET Core?](#511-qué-es-aspnet-core)
    - [5.1.2. Características principales](#512-características-principales)
  - [5.2. Arquitectura de una Web API](#52-arquitectura-de-una-web-api)
    - [5.2.1. Capas de la aplicación](#521-capas-de-la-aplicación)
    - [5.2.2. Controllers](#522-controllers)
    - [5.2.3. Services](#523-services)
    - [5.2.4. Repositories](#524-repositories)
    - [5.2.5. Models y DTOs](#525-models-y-dtos)
  - [5.3. Pipeline HTTP](#53-pipeline-http)
    - [5.3.1. ¿Qué es el pipeline?](#531-qué-es-el-pipeline)
    - [5.3.2. Middlewares](#532-middlewares)
    - [5.3.3. Orden de los middlewares](#533-orden-de-los-middlewares)
  - [5.4. Kestrel: El servidor web](#54-kestrel-el-servidor-web)
    - [5.4.1. ¿Qué es Kestrel?](#541-qué-es-kestrel)
    - [5.4.2. Kestrel con Reverse Proxy](#542-kestrel-con-reverse-proxy)
  - [5.5. Program.cs: El punto de entrada](#55-programcs-el-punto-de-entrada)
    - [5.5.1. Estructura básica](#551-estructura-básica)
    - [5.5.2. Registro de servicios](#552-registro-de-servicios)
    - [5.5.3. Configuración del pipeline](#553-configuración-del-pipeline)
  - [5.6. Buenas Prácticas](#56-buenas-prácticas)
  - [5.7. Reto: Traza una petición HTTP](#57-reto-traza-una-petición-http)

---

# 5. Arquitectura y Pipeline HTTP

> 💡 **Punto de partida:** Cuando envías un mensaje por WhatsApp, ese mensaje pasa por varios pasos: sale de tu móvil, viaja por internet, llega al servidor de Meta, se procesa, y la respuesta vuelve a ti. ASP.NET Core funciona igual: cada petición HTTP recorre un **pipeline** de middlewares antes de llegar al endpoint.

En este punto aprenderás cómo está organizada una aplicación ASP.NET Core y qué le pasa a una petición HTTP desde que llega hasta que se devuelve la respuesta.

**Objetivos de aprendizaje:**

- Comprender qué es ASP.NET Core y sus características
- Conocer la arquitectura en capas de una Web API
- Entender el pipeline HTTP y los middlewares
- Saber qué es Kestrel y cómo sirve las peticiones
- Conocer la estructura de Program.cs

## 5.1. ASP.NET Core: El framework

### 5.1.1. ¿Qué es ASP.NET Core?

**ASP.NET Core** es el framework de Microsoft para crear aplicaciones web y APIs. Es multiplataforma, de código abierto y de alto rendimiento.

> 💡 **Analogía:** Si .NET es una caja de herramientas completa, ASP.NET Core es la herramienta especializada para hacer páginas web y APIs. Todo lo que necesitas para el desarrollo web está ahí.

```mermaid
flowchart TD
    A["🌐 ASP.NET Core"] --> B["🏗️ MVC"]
    A --> C["⚡ Minimal APIs"]
    A --> D["📄 Razor Pages"]
    A --> E["🎮 Blazor"]
    A --> F["📡 Web API"]
    style A fill:#9C27B0,color:#fff
    style B fill:#4CAF50,color:#fff
    style C fill:#4CAF50,color:#fff
    style D fill:#2196F3,color:#fff
    style E fill:#2196F3,color:#fff
    style F fill:#FF9800,color:#fff
```

📌 **Ejemplo real:** Netflix, Spotify y GitHub usan APIs construidas con tecnologías similares a ASP.NET Core. Cada petición que haces a estos servicios pasa por un pipeline similar al que verás aquí.

### 5.1.2. Características principales

| Característica | Significado |
|----------------|-------------|
| **Multiplataforma** | Funciona en Windows, Linux y macOS |
| **Alto rendimiento** | Uno de los frameworks más rápidos según benchmarks |
| **Modular** | Solo instalas lo que necesitas (paquetes NuGet) |
| **DI integrada** | Inyección de dependencias de fábrica |
| **Código abierto** | Desarrollo público en GitHub |

## 5.2. Arquitectura de una Web API

### 5.2.1. Capas de la aplicación

Una Web API bien organizada separa las responsabilidades en **capas**:

```mermaid
flowchart TD
    A["🌐 Cliente"] -->|HTTP| B["🎮 Controller"]
    B -->|Llama| C["⚙️ Service"]
    C -->|Consulta| D["🗄️ Repository"]
    D -->|Accede a| E["📦 Database"]
    C -->|Usa| F["📋 Model"]
    B -->|Devuelve| G["📤 DTO"]
    style A fill:#2196F3,color:#fff
    style B fill:#FF9800,color:#fff
    style C fill:#4CAF50,color:#fff
    style D fill:#9C27B0,color:#fff
    style E fill:#607D8B,color:#fff
    style F fill:#4CAF50,color:#fff
    style G fill:#FF9800,color:#fff
```

📌 **Ejemplo real:** En Amazon, cuando buscas un producto: el **Controller** recibe tu petición, el **Service** busca en el catálogo, el **Repository** accede a la base de datos, y el **DTO** te devuelve solo los datos que necesitas (sin passwords ni datos internos).

### 5.2.2. Controllers

El **Controller** es la puerta de entrada. Recibe la petición HTTP y coordina la respuesta.

```
Cliente → Controller → Service → Repository → Base de Datos
```

| Responsabilidad | Qué hace |
|-----------------|----------|
| Recibir peticiones | Parsea la URL, el body y los headers |
| Validar | Comprueba que los datos son correctos |
| Llamar al service | Delega la lógica de negocio |
| Devolver respuesta | Ok(), NotFound(), BadRequest()... |

> ⚠️ **Advertencia:** El Controller **nunca** debe contener lógica de negocio. Solo coordina. Si estás escribiendo "if/else" complejos en un controller, algo estás haciendo mal.

### 5.2.3. Services

El **Service** contiene la **lógica de negocio**. Es donde ocurre la magia: validar reglas, transformar datos, coordenar operaciones.

```
Service = "Qué hay que hacer"
Repository = "Dónde están los datos"
```

| Responsabilidad | Qué hace |
|-----------------|----------|
| Lógica de negocio | Reglas, validaciones, cálculos |
| Coordinación | Llama a uno o varios repositories |
| Transformación | Convierte entre modelos y DTOs |

📌 **Ejemplo real:** En una tienda online, el `PedidoService` valida stock, calcula descuentos, actualiza inventario y confirma el pedido. Toda esa lógica vive en el service.

### 5.2.4. Repositories

El **Repository** accede a los datos. Puede ser una base de datos, un fichero JSON, una API externa... El repository **oculta** de dónde vienen los datos.

```mermaid
flowchart LR
    A["⚙️ Service"] -->|"GetAll()"| B["🗄️ Repository"]
    B -->|"SELECT *"| C["📦 SQL Server"]
    B -->|"find()"| D["🍃 MongoDB"]
    B -->|"Get()"| E["📄 JSON File"]
    style A fill:#4CAF50,color:#fff
    style B fill:#9C27B0,color:#fff
    style C fill:#2196F3,color:#fff
    style D fill:#4CAF50,color:#fff
    style E fill:#FF9800,color:#fff
```

> 💡 **Consejo:** El patrón Repository permite cambiar la fuente de datos sin tocar el service. Hoy es SQL Server, mañana puede ser MongoDB. El service no se entera.

### 5.2.5. Models y DTOs

| Tipo | Qué es | Ejemplo |
|------|--------|---------|
| **Model** | Entidad completa (con todo) | `Producto` con `Id`, `Nombre`, `Password`, `Email` |
| **DTO** | Lo que se envía al cliente (solo lo necesario) | `ProductoDto` con `Id`, `Nombre` |

> ⚠️ **Advertencia:** **Nunca** devuelvas el Model directamente al cliente. Usa DTOs para filtrar datos sensibles (passwords, tokens, datos internos).

## 5.3. Pipeline HTTP

### 5.3.1. ¿Qué es el pipeline?

El **pipeline** es el camino que sigue una petición HTTP desde que llega al servidor hasta que se devuelve la respuesta. Cada paso es un **middleware**.

```mermaid
flowchart LR
    A["📥 Petición"] --> B["🔒 HTTPS"]
    B --> C["📝 Logging"]
    C --> D["⚡ Errores"]
    D --> E["🔐 Auth"]
    E --> F["🗺️ Routing"]
    F --> G["🎯 Endpoint"]
    G --> H["📤 Respuesta"]
    style A fill:#2196F3,color:#fff
    style B fill:#f44336,color:#fff
    style C fill:#607D8B,color:#fff
    style D fill:#FF9800,color:#fff
    style E fill:#9C27B0,color:#fff
    style F fill:#4CAF50,color:#fff
    style G fill:#4CAF50,color:#fff
    style H fill:#2196F3,color:#fff
```

📌 **Ejemplo real:** Es como una cadena de montaje. Cada estación (middleware) hace su trabajo y pasa el producto a la siguiente. Si alguna estación falla, el producto no llega al final.

### 5.3.2. Middlewares

Un **middleware** es un componente que procesa la petición antes o después de que llegue al endpoint.

```csharp
// Cada middleware hace una cosa
app.UseHttpsRedirection();  // Redirige HTTP a HTTPS
app.UseCors();              // Permite peticiones de otros dominios
app.UseAuthentication();    // Identifica al usuario
app.UseAuthorization();     // Comprueba permisos
```

| Middleware | Qué hace |
|-----------|----------|
| `UseHttpsRedirection()` | Fuerza HTTPS |
| `UseCors()` | Permite peticiones cross-origin |
| `UseAuthentication()` | Identifica quién eres |
| `UseAuthorization()` | Comprueba si puedes hacer eso |
| `UseStaticFiles()` | Sirve ficheros estáticos (CSS, JS, imágenes) |

### 5.3.3. Orden de los middlewares

El **orden importa**. Los middlewares se ejecutan en el orden que se registran:

```csharp
// ✅ CORRECTO: Orden adecuado
app.UseExceptionHandler("/error");
app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// ❌ MAL: Orden incorrecto
app.MapControllers();          // No tiene sentido mapear antes de autenticar
app.UseAuthentication();       // Nunca se ejecuta
app.UseAuthorization();        // Nunca se ejecuta
```

> ⚠️ **Advertencia:** Si `UseAuthentication` va después de `UseAuthorization`, la autorización fallará porque no sabe quién eres. Siempre: Authentication → Authorization.

## 5.4. Kestrel: El servidor web

### 5.4.1. ¿Qué es Kestrel?

**Kestrel** es el servidor web que viene integrado en ASP.NET Core. Es el que escucha las peticiones HTTP y las pasa al pipeline.

```mermaid
flowchart LR
    A["🌐 Internet"] --> B["🛡️ Reverse Proxy<br/>(Nginx/IIS)"]
    B -->|HTTP| C["⚡ Kestrel"]
    C -->|Pipeline| D["🎯 Endpoint"]
    style A fill:#2196F3,color:#fff
    style B fill:#f44336,color:#fff
    style C fill:#4CAF50,color:#fff
    style D fill:#FF9800,color:#fff
```

📌 **Ejemplo real:** Kestrel es como el portero de un edificio. Recibe a todos los visitantes (peticiones), les pasa al sistema de seguridad (middleware), y los dirige al destino correcto (endpoint).

### 5.4.2. Kestrel con Reverse Proxy

En producción, Kestrel **nunca** se expone directamente a internet. Se usa detrás de un **reverse proxy** (Nginx, IIS, Apache):

| Componente | Función |
|------------|---------|
| **Reverse Proxy** | Recibe peticiones de internet, balancea carga, termina SSL |
| **Kestrel** | Procesa las peticiones internamente |

> 💡 **Consejo:** En desarrollo, Kestrel funciona solo. En producción, siempre detrás de un reverse proxy.

## 5.5. Program.cs: El punto de entrada

### 5.5.1. Estructura básica

`Program.cs` es el archivo que arranca tu aplicación. Tiene dos partes claras:

```csharp
// 1. Configurar servicios
var builder = WebApplication.CreateBuilder(args);
// ... registrar servicios ...

// 2. Configurar el pipeline
var app = builder.Build();
// ... configurar middlewares ...

app.Run();
```

```mermaid
flowchart TD
    A["Program.cs"] --> B["builder.Build()<br/>Configurar servicios"]
    B --> C["app.*<br/>Configurar pipeline"]
    C --> D["app.Run()<br/>Arrancar servidor"]
    style A fill:#9C27B0,color:#fff
    style B fill:#4CAF50,color:#fff
    style C fill:#FF9800,color:#fff
    style D fill:#2196F3,color:#fff
```

### 5.5.2. Registro de servicios

Los servicios se registran en la **DI container** (Inyección de Dependencias):

```csharp
var builder = WebApplication.CreateBuilder(args);

// Servicios de la aplicación
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Tus servicios
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
```

> 💡 **Consejo:** Los servicios se registran **antes** de `builder.Build()`. Los middlewares **después**.

### 5.5.3. Configuración del pipeline

Los middlewares se configuran **después** de `builder.Build()`:

```csharp
var app = builder.Build();

// Middleware de errores (siempre primero)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

// Endpoints
app.MapControllers();

app.Run();
```

## 5.6. Buenas Prácticas

- **Separación de responsabilidades:** Cada capa (Controller, Service, Repository) tiene una única responsabilidad. El Controller coordina, el Service aplica lógica de negocio, el Repository accede a datos
- **Principio de dependencia:** Las dependencias siempre fluyen hacia abajo (Controller → Service → Repository). Nunca al revés
- **Single Responsibility:** Una clase = una razón para cambiar. Si un servicio hace validación, persistencia y notificaciones, hay que dividirlo
- **DTOs para expuestos:** Nunca devuelvas entidades de dominio directamente al cliente. Usa DTOs para filtrar datos sensibles
- **Pipeline ordenado:** El orden de middlewares importa. Siempre: ExceptionHandler → HTTPS → CORS → Authentication → Authorization → Routing → Endpoints
- **Config classes para DI:** Organiza el registro de dependencias en clases estáticas (RepositoriesConfig, ServicesConfig) en lugar de llenar Program.cs

## 5.7. Reto: Traza una petición HTTP

> Antes de irte, dibuja el camino completo de una petición.

### Contexto

Un cliente envía esta petición a FunkoApp:

```http
GET /api/productos/1 HTTP/1.1
Host: localhost:5001
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

### Ejercicio 1: Dibuja el pipeline

Dibuja el camino que sigue esta petición desde que llega al servidor hasta que se devuelve la respuesta. Incluye:

- Middlewares por los que pasa
- Orden de ejecución
- Qué middleware hace qué cosa

### Ejercicio 2: Identifica las capas

Para esta petición, indica qué componente de cada capa se ejecuta:

| Capa | Componente | Qué hace |
|------|------------|----------|
| **Controller** | ¿Cuál? | ¿Qué método? |
| **Service** | ¿Cuál? | ¿Qué operación? |
| **Repository** | ¿Cuál? | ¿Qué consulta? |
| **Model** | ¿Cuál? | ¿Qué datos devuelve? |

> 💡 **Consejo:** Piensa en el orden: Controller → Service → Repository → Base de Datos. La respuesta vuelve por el mismo camino.

---

**Resumen del punto:**

| Concepto | Descripción |
|----------|-------------|
| **ASP.NET Core** | Framework multiplataforma para web y APIs |
| **Controller** | Puerta de entrada, coordina la respuesta |
| **Service** | Lógica de negocio |
| **Repository** | Acceso a datos |
| **Model** | Entidad completa |
| **DTO** | Datos que se envían al cliente |
| **Pipeline** | Camino de la petición HTTP |
| **Middleware** | Componente que procesa la petición |
| **Kestrel** | Servidor web integrado |
| **Program.cs** | Punto de entrada de la aplicación |

**¿Qué viene después?**

En el siguiente punto veremos la **Inyección de Dependencias**: cómo conectar las capas de la aplicación de forma flexible y testeable.
