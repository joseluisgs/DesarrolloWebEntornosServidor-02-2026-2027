- [2. APIs REST: Recursos, Métodos y Respuestas](#2-apis-rest-recursos-métodos-y-respuestas)
  - [2.1. ¿Qué es REST?](#21-qué-es-rest)
    - [2.1.1. Principios de REST](#211-principios-de-rest)
    - [2.1.2. REST no es solo HTTP](#212-rest-no-es-solo-http)
  - [2.2. Recursos y Endpoints](#22-recursos-y-endpoints)
    - [2.2.1. Convenciones de rutas](#221-convenciones-de-rutas)
    - [2.2.2. Parámetros en la URL](#222-parámetros-en-la-url)
  - [2.3. Métodos HTTP](#23-métodos-http)
    - [2.3.1. GET: Leer recursos](#231-get-leer-recursos)
    - [2.3.2. POST: Crear recursos](#232-post-crear-recursos)
    - [2.3.3. PUT: Actualizar completo](#233-put-actualizar-completo)
    - [2.3.4. PATCH: Actualizar parcial](#234-patch-actualizar-parcial)
    - [2.3.5. DELETE: Eliminar recursos](#235-delete-eliminar-recursos)
    - [2.3.6. Idempotencia](#236-idempotencia)
  - [2.4. Códigos de respuesta](#24-códigos-de-respuesta)
    - [2.4.1. 2xx: Éxito](#241-2xx-éxito)
    - [2.4.2. 4xx: Error del cliente](#242-4xx-error-del-cliente)
    - [2.4.3. 5xx: Error del servidor](#243-5xx-error-del-servidor)
    - [2.4.4. ¿Cuándo usar cada código?](#244-cuándo-usar-cada-código)
  - [2.5. Request y Response](#25-request-y-response)
    - [2.5.1. Estructura de un request](#251-estructura-de-un-request)
    - [2.5.2. Estructura de un response](#252-estructura-de-un-response)
    - [2.5.3. Headers comunes](#253-headers-comunes)
  - [2.6. Buenas prácticas](#26-buenas-prácticas)
  - [2.7. Reto: Diseña la API de Funkos](#27-reto-diseña-la-api-de-funkos)

---

# 2. APIs REST: Recursos, Métodos y Respuestas

> 💡 **Punto de partida:** Cuando usas Spotify y buscas un artista, tu aplicación envía una petición a la API de Spotify. El servidor busca en millones de canciones y te devuelve una lista en milisegundos. Eso es una API REST: un contrato claro entre cliente y servidor que usa HTTP y JSON.

En este punto aprenderás a diseñar APIs REST: qué son los recursos, cómo se estructuran las rutas, qué métodos HTTP existen y cuándo usar cada código de respuesta.

**Objetivos de aprendizaje:**

- Comprender los principios de REST y por qué se usa
- Diseñar endpoints siguiendo convenciones
- Conocer los métodos HTTP y cuándo usar cada uno
- Elegir el código de respuesta correcto para cada operación
- Entender la estructura de request y response

## 2.1. ¿Qué es REST?

**REST (Representational State Transfer)** es un estilo arquitectónico para APIs. No es un protocolo ni una librería: es un **conjunto de convenciones** que hacen que las APIs sean predecibles y fáciles de usar.

> 💡 **Analogía:** REST es como un restaurante con menú fijo. Tú pides por número (recurso), dices qué quieres hacer (método), y el camarero te trae lo que pediste (respuesta). No necesitas saber cocinar: solo seguir el protocolo.

```mermaid
flowchart LR
    A["🌐 Cliente"] -->|HTTP Request| B["📡 API REST"]
    B -->|HTTP Response| A
    style A fill:#2196F3,color:#fff
    style B fill:#4CAF50,color:#fff
```

📌 **Ejemplo real:** La API de Twitter/X sigue REST. Cuando abres la app, envía `GET /api/tweets` para obtener los tweets. Cuando publicas, envía `POST /api/tweets` con el texto. Todo sigue el mismo patrón.

### 2.1.1. Principios de REST

```mermaid
flowchart TD
    A["🌐 REST"] --> B["👥 Cliente-Servidor"]
    A --> C["📭 Sin Estado"]
    A --> D["💾 Cacheable"]
    A --> E["🧩 Uniforme"]
    style A fill:#9C27B0,color:#fff
    style B fill:#4CAF50,color:#fff
    style C fill:#2196F3,color:#fff
    style D fill:#FF9800,color:#fff
    style E fill:#607D8B,color:#fff
```

| Principio | Significado |
|-----------|-------------|
| **Cliente-Servidor** | Separación de responsabilidades. El cliente pide, el servidor responde |
| **Sin Estado** | Cada petición es independiente. El servidor no recuerda peticiones anteriores |
| **Cacheable** | Las respuestas pueden cachéarse para ir más rápido |
| **Uniforme** | Mismos métodos HTTP para todo: GET, POST, PUT, DELETE |

> 💡 **Consejo:** El principio más importante es **sin estado**. Cada petición debe contener toda la información necesaria. El servidor no puede pensar "ah, el cliente me pidió esto antes".

### 2.1.2. REST no es solo HTTP

REST usa HTTP como transporte, pero REST es un **estilo arquitectónico**. Puedes hacer REST con HTTP, pero también con otros protocolos. Lo que define a REST es cómo organizas los recursos y las operaciones sobre ellos.

## 2.2. Recursos y Endpoints

En REST, todo es un **recurso**. Un producto es un recurso. Una categoría es un recurso. Un usuario es un recurso. Cada recurso se identifica con una **URL** (endpoint).

> 💡 **Analogía:** Los recursos son como las páginas de un catálogo. Cada página tiene una dirección única (URL). Para acceder a un producto, vas a la página que le corresponde.

### 2.2.1. Convenciones de rutas

| Convención | Correcto | Incorrecto |
|------------|----------|------------|
| Usar **sustantivos plurales** | `/api/productos` | `/api/producto` o `/api/getProductos` |
| **No usar verbos** en la URL | `GET /api/productos` | `/api/getAllProductos` |
| Usar **IDs** para recursos específicos | `/api/productos/1` | `/api/productos?id=1` |
| **No añadir extensión** | `/api/productos` | `/api/productos.json` |

📌 **Ejemplo real:** La API de GitHub usa `/api/repos` para repositorios, `/api/users` para usuarios, `/api/issues` para issues. Siempre plurales, siempre sin verbos.

```mermaid
flowchart LR
    A["/api/productos"] -->|GET| B["Lista todos"]
    A -->|GET /1| C["Uno solo"]
    A -->|POST| D["Crear nuevo"]
    A -->|PUT /1| E["Actualizar"]
    A -->|DELETE /1| F["Eliminar"]
    style A fill:#2196F3,color:#fff
    style B fill:#4CAF50,color:#fff
    style C fill:#4CAF50,color:#fff
    style D fill:#FF9800,color:#fff
    style E fill:#FF9800,color:#fff
    style F fill:#f44336,color:#fff
```

### 2.2.2. Parámetros en la URL

Los parámetros pueden ir en distintos sitios de la URL:

| Tipo | Ejemplo | Uso |
|------|---------|-----|
| **Ruta** | `/api/productos/1` | Identificar un recurso específico |
| **Query string** | `/api/productos?categoria=musica` | Filtrar, buscar, paginar |
| **Body** | `{ "nombre": "Guitarra", "precio": 299.99 }` | Enviar datos (POST, PUT, PATCH) |

```
https://api.tienda.com/api/productos?categoria=musica&page=1&pageSize=10
\_____/ \____________/ \________/ \___________________________________/
  host      dominio       ruta              query string (filtros)
```

> ⚠️ **Advertencia:** Los query string son para **filtrar y buscar**, no para identificar recursos. Usa `/api/productos/1` para obtener un producto, no `/api/productos?id=1`.

## 2.3. Métodos HTTP

Cada método HTTP tiene un significado claro en REST. No son intercambiables.

### 2.3.1. GET: Leer recursos

**GET** obtiene un recurso o una lista de recursos. **No modifica nada**. Es seguro e idempotente.

```http
GET /api/productos HTTP/1.1
Host: api.tienda.com
Accept: application/json
```

```http
HTTP/1.1 200 OK
Content-Type: application/json

[
  { "id": 1, "nombre": "Guitarra", "precio": 299.99 },
  { "id": 2, "nombre": "Batería", "precio": 499.99 }
]
```

📌 **Ejemplo real:** `GET /api/productos/1` → Obtener el producto con ID 1.

### 2.3.2. POST: Crear recursos

**POST** crea un nuevo recurso. Devuelve **201 Created** con el recurso creado y un header `Location` con la URL del nuevo recurso.

```http
POST /api/productos HTTP/1.1
Host: api.tienda.com
Content-Type: application/json

{
  "nombre": "Guitarra",
  "precio": 299.99,
  "categoria": "Instrumentos"
}
```

```http
HTTP/1.1 201 Created
Content-Type: application/json
Location: /api/productos/1

{
  "id": 1,
  "nombre": "Guitarra",
  "precio": 299.99,
  "categoria": "Instrumentos"
}
```

> ⚠️ **Advertencia:** POST **no** es idempotente. Si envías el mismo POST dos veces, crearás **dos recursos** distintos. Por eso no se usa para actualizar.

### 2.3.3. PUT: Actualizar completo

**PUT** reemplaza un recurso completo. Debes enviar **todos** los campos. Es idempotente.

```http
PUT /api/productos/1 HTTP/1.1
Host: api.tienda.com
Content-Type: application/json

{
  "nombre": "Guitarra",
  "precio": 349.99,
  "categoria": "Instrumentos"
}
```

```http
HTTP/1.1 200 OK
Content-Type: application/json

{
  "id": 1,
  "nombre": "Guitarra",
  "precio": 349.99,
  "categoria": "Instrumentos"
}
```

> 💡 **Consejo:** PUT reemplaza el recurso completo. Si olvidas un campo, ese campo se pierde. Por eso se usa **PATCH** para actualizaciones parciales.

### 2.3.4. PATCH: Actualizar parcial

**PATCH** modifica solo los campos que envías. El resto se mantiene igual.

```http
PATCH /api/productos/1 HTTP/1.1
Host: api.tienda.com
Content-Type: application/json

{
  "precio": 39.99
}
```

```http
HTTP/1.1 200 OK
Content-Type: application/json

{
  "id": 1,
  "nombre": "Guitarra",
  "precio": 39.99,
  "categoria": "Instrumentos"
}
```

📌 **Ejemplo real:** En Netflix, cuando cambias solo tu contraseña, usa PATCH. No necesitas enviar nombre, email y todo lo demás.

### 2.3.5. DELETE: Eliminar recursos

**DELETE** elimina un recurso. Devuelve **204 No Content** (sin cuerpo de respuesta).

```http
DELETE /api/productos/1 HTTP/1.1
Host: api.tienda.com
```

```http
HTTP/1.1 204 No Content
```

> 💡 **Consejo:** DELETE es idempotente. Si eliminas el mismo recurso dos veces, la segunda vez devuelve 404 (no existe), pero el estado final es el mismo.

### 2.3.6. Idempotencia

**Idempotente** = hacer la misma petición varias veces produce el **mismo resultado**.

| Método | Idempotente | ¿Por qué? |
|--------|:-----------:|-----------|
| **GET** | Sí | Solo lee, no modifica nada |
| **PUT** | Sí | Reemplaza el recurso completo, siempre el mismo resultado |
| **DELETE** | Sí | Elimina el recurso, siempre el mismo resultado |
| **POST** | No | Cada petición crea un **nuevo** recurso |
| **PATCH** | No* | Depende de la implementación |

> ⚠️ **Advertencia:** PATCH **puede** ser idempotente si el body es siempre el mismo, pero la especificación HTTP no lo garantiza. Por seguridad, trata PATCH como no idempotente.

📌 **Ejemplo real:** Si envías `DELETE /api/productos/1` tres veces, el producto se elimina la primera vez. Las siguientes devuelven 404. Pero el estado final es el mismo: el producto no existe.

## 2.4. Códigos de respuesta

Los códigos de estado HTTP comunican al cliente qué ha pasado con su petición. Usar el código correcto es **fundamental** para que el cliente sepa cómo reaccionar.

### 2.4.1. 2xx: Éxito

| Código | Nombre | Cuándo usarlo |
|--------|--------|---------------|
| **200** | OK | La petición fue exitosa y devuelves datos |
| **201** | Created | Creaste un nuevo recurso (con POST) |
| **204** | No Content | La petición fue exitosa pero **no devuelves datos** (con DELETE) |

#### 200 vs 201 vs 204

| Operación | Código | ¿Qué devuelve? | ¿Por qué? |
|-----------|:------:|----------------|-----------|
| `GET /api/productos` | 200 | Lista de productos | Estás devolviendo datos |
| `POST /api/productos` | 201 | Producto creado + Location | Creaste algo nuevo |
| `PUT /api/productos/1` | 200 | Producto actualizado | Estás devolviendo el resultado |
| `PATCH /api/productos/1` | 200 | Producto modificado | Estás devolviendo el resultado |
| `DELETE /api/productos/1` | 204 | Nada | Solo confirmas que se eliminó |

> ⚠️ **Advertencia:** **204 No Content** no tiene cuerpo de respuesta. No envíes `{}` ni `null`. Simplemente no hay body. Si necesitas devolver el recurso eliminado, usa 200 con el body.

> 💡 **Consejo:** **201 Created** siempre debe incluir el header `Location` con la URL del recurso creado. Si no lo incluyes, el cliente no sabe dónde está el nuevo recurso.

### 2.4.2. 4xx: Error del cliente

| Código | Nombre | Cuándo usarlo |
|--------|--------|---------------|
| **400** | Bad Request | La petición está mal formada (JSON inválido, campos obligatorios vacíos) |
| **401** | Unauthorized | No estás autenticado (no envías token) |
| **403** | Forbidden | Estás autenticado pero no tienes permisos |
| **404** | Not Found | El recurso no existe |
| **409** | Conflict | Conflicto (ej: intentar crear un producto que ya existe) |
| **422** | Unprocessable Entity | Los datos son válidos syntácticamente pero tienen errores de negocio |

#### 400 vs 422

| Código | Significado | Ejemplo |
|--------|-------------|---------|
| **400** | El cliente envió algo **malformado** | JSON con errores de sintaxis, campo "edad" con texto en vez de número |
| **422** | El formato es correcto pero los datos **no tienen sentido de negocio** | Precio negativo, email sin @, nombre vacío |

> ⚠️ **Advertencia:** No uses 400 para todo. Si el JSON es correcto pero el precio es negativo, es un **422**, no un 400. El 400 es para errores de formato, el 422 para errores de validación.

📌 **Ejemplo real:** En Amazon, si envías un formulario sin rellenar el nombre → 400. Si pones un precio de -5€ → 422.

#### 401 vs 403

| Código | Significado | Ejemplo |
|--------|-------------|---------|
| **401** | No sabes quién eres (sin token) | Intentar acceder a `/api/admin` sin iniciar sesión |
| **403** | Sabes quién eres pero no puedes (sin permisos) | Un usuario normal intentando acceder a `/api/admin` |

### 2.4.3. 5xx: Error del servidor

| Código | Nombre | Cuándo usarlo |
|--------|--------|---------------|
| **500** | Internal Server Error | Error genérico del servidor |
| **502** | Bad Gateway | El servidor de proxy recibió una respuesta inválida |
| **503** | Service Unavailable | El servicio está temporalmente no disponible (mantenimiento) |

> 💡 **Consejo:** Los errores 5xx **nunca** deberían llegar al cliente en producción. Usa logs y monitoring para detectarlos antes.

### 2.4.4. ¿Cuándo usar cada código?

```mermaid
flowchart TD
    A["Petición recibida"] --> B{"¿JSON válido?"}
    B -->|No| C["400 Bad Request"]
    B -->|Sí| D{"¿Recurso existe?"}
    D -->|No| E{"¿Es POST?"}
    E -->|Sí| F["201 Created"]
    E -->|No| G["404 Not Found"]
    D -->|Sí| H{"¿Autorizado?"}
    H -->|No| I{"¿Autenticado?"}
    I -->|No| J["401 Unauthorized"]
    I -->|Sí| K["403 Forbidden"]
    H -->|Sí| L{"¿Datos válidos?"}
    L -->|No| M["422 Unprocessable"]
    L -->|Sí| N{"¿Es DELETE?"}
    N -->|Sí| O["204 No Content"]
    N -->|No| P["200 OK"]
    style C fill:#FF9800,color:#fff
    style F fill:#4CAF50,color:#fff
    style G fill:#f44336,color:#fff
    style J fill:#f44336,color:#fff
    style K fill:#f44336,color:#fff
    style M fill:#FF9800,color:#fff
    style O fill:#4CAF50,color:#fff
    style P fill:#4CAF50,color:#fff
```

## 2.5. Request y Response

### 2.5.1. Estructura de un request

```http
POST /api/productos HTTP/1.1
Host: api.tienda.com
Content-Type: application/json
Accept: application/json
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...

{
  "nombre": "Guitarra",
  "precio": 299.99,
  "categoria": "Instrumentos"
}
```

| Parte | Descripción |
|-------|-------------|
| **Método + URL** | `POST /api/productos` — qué quieres hacer y dónde |
| **Headers** | Info adicional: tipo de contenido, token de auth |
| **Body** | Los datos que envías (solo en POST, PUT, PATCH) |

### 2.5.2. Estructura de un response

```http
HTTP/1.1 201 Created
Content-Type: application/json
Location: /api/productos/1

{
  "id": 1,
  "nombre": "Guitarra",
  "precio": 299.99,
  "categoria": "Instrumentos"
}
```

| Parte | Descripción |
|-------|-------------|
| **Código de estado** | `201 Created` — qué pasó con tu petición |
| **Headers** | Info adicional: tipo de contenido, ubicación del recurso |
| **Body** | Los datos que devuelves (en 204 no hay body) |

### 2.5.3. Headers comunes

| Header | Dirección | Descripción |
|--------|:---------:|-------------|
| `Content-Type` | Ambos | Tipo del body (`application/json`) |
| `Authorization` | Request | Token de autenticación (`Bearer <token>`) |
| `Accept` | Request | Tipo que el cliente acepta |
| `Location` | Response | URL del recurso creado (en 201) |
| `Cache-Control` | Response | Directivas de caché |
| `ETag` | Response | Versión del recurso para caché |

## 2.6. Buenas prácticas

- **Nouns, no verbs:** Los endpoints son sustantivos (`/api/productos`), no acciones (`/api/getProductos`)
- **Plural siempre:** Usa `/api/productos`, nunca `/api/producto`
- **Códigos correctos:** 200 para lectura, 201 para creación, 204 para eliminación
- **Idempotencia:** GET, PUT, DELETE son idempotentes; POST no lo es
- **Documenta tus códigos:** Cada endpoint debe indicar qué códigos devuelve y por qué
- **Consistencia:** Si un endpoint devuelve `{ data: ... }`, todos deben hacerlo

## 2.7. Reto: Diseña la API de Funkos

> Antes de irte, diseña los endpoints de tu API. No escribas código: piensa en el diseño.

### Contexto

Vas a construir una API REST para gestionar una **colección de Funkos**. La API permitirá ver, crear, modificar y eliminar Funkos.

### Modelo de datos

Un Funko tiene estas propiedades:

| Propiedad | Tipo | Obligatorio |
|-----------|------|:-----------:|
| `id` | long | Sí (autogenerado) |
| `nombre` | string | Sí |
| `precio` | decimal | Sí |
| `categoria` | string | Sí |
| `imagen` | string | No |
| `creadoEn` | DateTime | Sí (autogenerado) |

### Ejercicio: Completa la tabla

**Para cada operación, rellena:** endpoint, método, datos que envías, código de respuesta, qué devuelve y si necesita autorización.

| Acción | Endpoint | Método | Datos que envías | Código | ¿Qué devuelve? | ¿Autorizado? |
|--------|----------|--------|------------------|--------|----------------|:------------:|
| Listar todos los Funkos | | | | | | |
| Consultar un Funko por ID | | | | | | |
| Crear un Funko | | | | | | |
| Actualizar un Funko completo | | | | | | |
| Actualizar solo el precio | | | | | | |
| Eliminar un Funko | | | | | | |
| Buscar Funkos por nombre | | | | | | |

> 💡 **Consejo:** Piensa en el código correcto para cada operación. ¿Sabes cuándo se devuelve 200, 201 o 204? ¿Qué datos necesitas en el body de request y response?

---

**Resumen del punto:**

| Concepto | Descripción |
|----------|-------------|
| **REST** | Estilo arquitectónico basado en HTTP + JSON |
| **Recurso** | Todo lo que se puede identificar con una URL |
| **Endpoint** | URL que representa un recurso (`/api/productos`) |
| **GET** | Leer recursos (seguro, idempotente) |
| **POST** | Crear recursos (no idempotente) |
| **PUT** | Actualizar completo (idempotente) |
| **PATCH** | Actualizar parcial |
| **DELETE** | Eliminar recursos (idempotente) |
| **200** | Éxito con datos |
| **201** | Recurso creado |
| **204** | Éxito sin datos |
| **400** | JSON mal formado |
| **401** | No autenticado |
| **403** | No autorizado |
| **404** | Recurso no encontrado |
| **422** | Datos inválidos (validación) |

**¿Qué viene después?**

En el siguiente punto veremos **Minimal APIs**: una forma simplificada de crear endpoints en ASP.NET Core sin necesidad de controladores.
