# 06 - Excepciones de Dominio y Middleware

API REST con **excepciones de dominio** y **middleware global** de manejo de errores en ASP.NET Core (.NET 10).

## Descripción

Ejemplo de control de errores sin try/catch en el controller: el servicio lanza excepciones de dominio (`NotFoundException`, `ValidationException`, `ConflictException`) y un middleware global (`ExceptionHandlerMiddleware`) las traduce a respuestas **ProblemDetails** JSON.

- Seed con 5 productos (ids 1–5, mismo que el 04)
- Reglas: `nombre` obligatorio, `precio > 0` (0 no vale), **409** si el nombre ya existe
- Mismas rutas de consulta que el 04 (`search`, `filter`, `order`, `group`, `stats`)
- PATCH de precio con **número JSON crudo** en `/{id}/precio`
- Sin Swagger ni Docker: solo SDK de .NET

## Ejecución manual

```bash
dotnet run --project ProductosExcepciones
```

Escucha en `http://localhost:5000`.

## Pruebas automatizadas

```bash
node automation/test-runner.mjs
```

Node.js nativo: `dotnet restore`/`build`/`run` → polling ≤20 s → **9 tests** HTTP → limpieza en `finally`.

- `exit 0` → todos los tests OK
- `exit 1` → fallo estructurado (endpoint, esperado vs obtenido, body)

> Sin `docker-compose.yml`: sin base de datos externa.

## Endpoints

Prefijo: `http://localhost:5000/api/productos`

| Método | Ruta | Parámetros | Cuerpo de petición | Respuesta exitosa | Errores |
|--------|------|------------|--------------------|-------------------|---------|
| GET | `/api/productos` | ( | ) | `200` `Producto[]` (5 seed) | — |
| GET | `/api/productos/{id}` | `id` (long) | — | `200` `Producto` | `404` problem |
| POST | `/api/productos` | — | `{ "nombre", "precio", "categoria", "imagen"? }` | `201` + `Location` | `400` validación, `409` duplicado |
| PUT | `/api/productos/{id}` | `id` (long) | `Producto` completo | `200` | `400`, `404`, `409` |
| PATCH | `/api/productos/{id}/precio` | `id` (long) | **número** p.ej. `799.99` | `200` | `400` (precio≤0), `404` |
| DELETE | `/api/productos/{id}` | `id` (long) | — | `204` | `404` |
| GET | `/api/productos/search` | Query `termino` **obligatorio** | — | `200` `Producto[]` | `400` si falta |
| GET | `/api/productos/filter/categoria` | Query `categoria` **obligatorio** | — | `200` `Producto[]` | `400` si falta |
| GET | `/api/productos/filter/precio` | Query `min`, `max` | ( | `200` `Producto[]` | ) |
| GET | `/api/productos/order/precio` | Query `descendente` (default `false`) | ( | `200` `Producto[]` | ) |
| GET | `/api/productos/group/categoria` | ( | ) | `200` **array** `[{categoria, productos}]` | — |
| GET | `/api/productos/stats` | ( | ) | `200` `{totalProductos, precioMedio, ...}` | — |

### Seed data

| id | nombre | precio | categoria |
|----|--------|--------|-----------|
| 1 | Portátil | 899.99 | Electrónica |
| 2 | Ratón | 29.99 | Electrónica |
| 3 | Teclado | 79.99 | Electrónica |
| 4 | Silla | 199.99 | Mobiliario |
| 5 | Escritorio | 349.99 | Mobiliario |

### Respuestas de error (middleware)

```json
{ "status": 404, "title": "Not Found", "detail": "Producto con ID 999 no encontrado", "instance": "/api/productos/999" }
```

```json
{ "status": 400, "title": "Validation Error", "detail": "El nombre del producto es obligatorio", "instance": "/api/productos" }
```

```json
{ "status": 409, "title": "Conflict", "detail": "Ya existe un producto con el nombre 'Portátil'", "instance": "/api/productos" }
```

### Reglas de negocio

| Condición | Status |
|-----------|--------|
| `nombre` vacío | `400` |
| `precio <= 0` | `400` |
| `nombre` duplicado (ignore-case) | **`409`** |
| id inexistente | `404` |

## Estructura

```
06-ProductosExcepciones/
├── ProductosExcepciones.slnx
├── ProductosExcepciones/
│   ├── Controllers/ProductosController.cs   ← sin try/catch
│   ├── Errors/                              ← excepciones de dominio
│   ├── Middleware/ExceptionHandlerMiddleware.cs
│   ├── Models/ Producto.cs
│   ├── Repositories/ Services/ Infrastructure/
│   └── Program.cs
├── automation/test-runner.mjs
└── README.md
```

## Vs ejemplo 04

| Aspecto | 04 | 06 |
|---------|----|----|
| Errores | `return` + `{"error"}` | excepciones → ProblemDetails |
| Controller | lógica de error inline | limpio, sin try/catch |
| Precio 0 | válido | `400` |
| Nombre duplicado | permitido | `409` |
