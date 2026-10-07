# 07 - Productos Result (ROP) + DomainError

API REST de productos con **Rail-Oriented Programming (ROP)** usando `CSharpFunctionalExtensions` en ASP.NET Core (.NET 10).

## Descripción

Flujo sin excepciones en la capa de presentación: el servicio devuelve `Result<T, DomainError>` y un método de extensión genérico `ToHttpResult<T>()` mapea el error a la respuesta HTTP correspondiente y devuelve `ActionResult<T>` tipado (lo documenta Swagger).

- Jerarquía: `NotFoundError`→404 · `ValidationError`→400 `{message, errors}` · `ConflictError`→409 · `BusinessRuleError`→422
- `GlobalExceptionHandler` para excepciones no controladas (`{errorId, status, message, ...}`)
- PATCH de precio con **número JSON crudo** en `/{id}/precio`
- Sin seed (arranca vacío); sin Docker
- Swagger solo en Development; el runner lanza con `ASPNETCORE_ENVIRONMENT=Development`

## Ejecución manual

```bash
dotnet run --project ProductosResult
```

Escucha en `http://localhost:5000` (el runner fija `ASPNETCORE_URLS`).

## Pruebas automatizadas

```bash
node automation/test-runner.mjs
```

Node.js nativo: restore/build/run → polling ≤20 s → **12 tests** → limpieza en `finally`.

- `exit 0` → todos OK · `exit 1` → detalle del fallo

> Sin `docker-compose.yml`.

## Endpoints

Prefijo: `http://localhost:5000/api/productos`

| Método | Ruta | Parámetros | Cuerpo de petición | Respuesta exitosa | Errores |
|--------|------|------------|--------------------|-------------------|---------|
| GET | `/api/productos` | ( | ) | `200` `Producto[]` | — |
| GET | `/api/productos/{id}` | `id` (long) | — | `200` `Producto` | `404` `{"message"}` |
| POST | `/api/productos` | — | `{ "nombre", "precio", "categoria", "imagen"? }` | `201` + `Location` | `400` `{"message","errors"}` |
| PUT | `/api/productos/{id}` | `id` (long) | `Producto` completo | `200` | `400`, `404` |
| PATCH | `/api/productos/{id}/precio` | `id` (long) | **número** p.ej. `42.5` | `200` `Producto` | `400` (precio<0), `404` |
| DELETE | `/api/productos/{id}` | `id` (long) | — | `204` | `404` |
| GET | `/api/productos/search` | Query `termino` (obligatorio de negocio) | — | `200` `Producto[]` | `400` `{"message":"El término de búsqueda es obligatorio"}` |

### Validaciones de negocio

| Condición | Status | Body |
|-----------|--------|------|
| `nombre` vacío | `400` | `{"message":"El nombre del producto es obligatorio","errors":null}` |
| `precio < 0` | `400` | `{"message":"El precio X debe ser mayor que cero","errors":null}` |
| `termino` ausente/vacío en search | `400` | `{"message":"El término de búsqueda es obligatorio"}` |
| id inexistente | `404` | `{"message":"..."}` |

### Ejemplo de cuerpo (POST/PUT)

```json
{ "nombre": "Guitarra", "precio": 159.99, "categoria": "Instrumentos", "imagen": null }
```

## Paquetes NuGet

- `CSharpFunctionalExtensions` → `Result<T,TError>`, `UnitResult<TError>`
- `Swashbuckle.AspNetCore` → Swagger (solo Development)

## Estructura

```
07-ProductosResult/
├── ProductosResult.slnx
├── ProductosResult/
│   ├── Controllers/ProductosController.cs   ← sin try/catch
│   ├── Errors/DomainError.cs, ProductoError.cs
│   ├── Extensions/DomainErrorExtensions.cs  ← ToHttpResult<T>()
│   ├── Models/Producto.cs
│   ├── Repositories/ Services/ Infrastructure/
│   └── Program.cs
├── automation/test-runner.mjs
└── README.md
```

## ROP en un vistazo

```
Éxito          → Result.Success(value)  → Ok / Created          (ActionResult<T>)
Error (payload)→ Result.Failure(error)  → error.ToHttpResult<T>() (ActionResult<T>)
Error (204)    → UnitResult failure     → switch inline           (ActionResult)
```

En los endpoints con payload la firma es `ActionResult<T>` y el error se mapea con `error.ToHttpResult<T>()`. En `DELETE`, que devuelve **204 sin cuerpo**, no hay `T`: se usa `ActionResult` plano y un `switch` de dos brazos (`NotFoundError` → 404, resto → 500).
