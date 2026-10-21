# 14 - Productos Concurrencia (RowVersion + Redis)

API REST de productos con **concurrencia optimista**, **ETag/If-Match**, **distributed lock** e **HiLo/Sequential IDs** en ASP.NET Core (.NET 10).

## Descripción

Tres controllers: Productos (CRUD con `categoriaId` Guid), Concurrency (optimistic/ETag) y Proveedor (CRUD con id de 11 chars).

- PostgreSQL + Redis vía Docker; seed Development: ≥10 productos
- Swagger solo en Development (`ASPNETCORE_ENVIRONMENT=Development`)

## Ejecución manual

```bash
# 1. Infra
docker compose up -d

# 2. API
dotnet run --project ProductosTest
```

Escucha en `http://localhost:5000`.

## Pruebas automatizadas (runner)

```bash
node automation/test-runner.mjs
```

Node.js nativo: infra → restore/build/run → polling ≤20 s → **15 tests** → limpieza.

- `exit 0` → todos OK · `exit 1` → detalle del fallo

## Tests unitarios

```bash
dotnet test
```

## Endpoints

### Productos; `http://localhost:5000/api/Productos`

| Método | Ruta | Parámetros | Cuerpo de petición | Respuesta exitosa | Errores |
|--------|------|------------|--------------------|-------------------|---------|
| GET | `/api/Productos` | ( | ) | `200` `ProductoDto[]` (≥10) |; |
| GET | `/api/Productos/{id}` | `id` (long) |; | `200` | `404` |
| POST | `/api/Productos` |; | `{ nombre, precio, categoriaId(Guid), stock?, imagen? }` | `201` | `400`, `409` |
| PUT | `/api/Productos/{id}` | `id` (long) | `UpdateProductoDto` | `200` | `400`, `404`, `409` |
| DELETE | `/api/Productos/{id}` | `id` (long) |; | `204` | `404` |
| POST | `/api/Productos/{id}/restore` | `id` (long) |; | `200` `{message}` | `404` |
| GET | `/api/Productos/deleted` | ( | ) | `200` | — |
| GET | `/api/Productos/search` | Query `q` **obligatorio** | — | `200` | `400` |
| GET | `/api/Productos/filter` | Query opcionales `nombre`, `categoriaId`(Guid), `precioMin`, `precioMax` | ( | `200` | ) |
| GET | `/api/Productos/paged` | Query `page`, `pageSize` (≤100) | ( | `200` `{data, pagination}` | ) |
| GET | `/api/Productos/categorias` | ( | ) | `200` lista (Guids) | — |
| GET | `/api/Productos/categorias/{id}` | `id` Guid | — | `200` | `404` |

### Concurrencia — `/api/Concurrency`

| Método | Ruta | Cuerpo | Respuesta | Errores |
|--------|------|--------|-----------|---------|
| PUT | `/api/Concurrency/optimistic/{id}` | `{ newStock: 999 }` | `200` producto o `409` conflicto RowVersion | `404` id inexistente |

### Proveedor — `/api/Proveedor`

| Método | Ruta | Cuerpo | Respuesta | Errores |
|--------|------|--------|-----------|---------|
| GET | `/api/Proveedor` | ( | `200` array (sin seed) | ) |
| GET | `/api/Proveedor/{id}` | — | `200` | `404` |
| POST | `/api/Proveedor` | `{ nombre, contacto }` | `201` + id **11 chars** | `400`, `409` |
| PUT | `/api/Proveedor/{id}` | `{ nombre, contacto }` | `200` | `400`, `404`, `409` |
| DELETE | `/api/Proveedor/{id}` | — | `204` | `404` |

**Ejemplo create producto:**

```json
{ "nombre": "Producto Concurrency Test", "precio": 55.5, "categoriaId": "<guid>", "stock": 10 }
```

## Docker

```bash
docker compose up -d
docker compose -f docker-compose.api.yml up -d --build
docker compose down -v
```

## Estructura

```
14-ProductosConcurrency/
├── ProductosEFCore.slnx
├── Dockerfile
├── docker-compose.yml / docker-compose.api.yml
├── ProductosTest/
│   ├── Controllers/  (Productos, Concurrency, Proveedor)
│   ├── IdGenerators/  ← HiLo, Sequential
│   └── …
├── ProductosTest.Test/
├── automation/test-runner.mjs
└── README.md
```

## Paquetes NuGet

- EF Core + PostgreSQL, Redis (lock)
- `CSharpFunctionalExtensions`, `Serilog.AspNetCore`, `Swashbuckle.AspNetCore`
