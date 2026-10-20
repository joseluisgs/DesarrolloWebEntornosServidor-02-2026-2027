# 13 - Productos Cache (MemoryCache + Redis)

API REST de productos con **caché** (`ICacheService`: MemoryCache o Redis) y patrón **Cache-Aside** en ASP.NET Core (.NET 10).

## Descripción

Misma base de endpoints que el 11 (CRUD + soft delete/restore + search/paged/categorías), con caché transparente e invalidación en escrituras.

- PostgreSQL + Redis vía Docker; seed Development: 10 productos, 5 categorías
- Swagger solo en Development (`ASPNETCORE_ENVIRONMENT=Development`)

## Ejecución manual

```bash
# 1. Infra
docker compose up -d db cache   # o: docker compose up -d

# 2. API
dotnet run --project ProductosTest
```

Escucha en `http://localhost:5000`.

## Pruebas automatizadas (runner)

```bash
node automation/test-runner.mjs
```

Node.js nativo: `docker compose up -d …` → restore/build/run → polling ≤20 s → **11 tests** → limpieza.

- `exit 0` → todos OK · `exit 1` → detalle del fallo

## Tests unitarios

```bash
dotnet test
```

## Endpoints

Prefijo: `http://localhost:5000/api/Productos`

| Método | Ruta | Parámetros | Cuerpo de petición | Respuesta exitosa | Errores |
|--------|------|------------|--------------------|-------------------|---------|
| GET | `/api/Productos` | ( | ) | `200` `ProductoDto[]` (≥10) |; |
| GET | `/api/Productos/{id}` | `id` (long) |; | `200` | `404` `{message}` |
| POST | `/api/Productos` | — | `{ nombre, precio, categoriaId, imagen? }` | `201` + id | `400`, `409` duplicado |
| PUT | `/api/Productos/{id}` | `id` (long) | `{ nombre, precio, categoriaId }` | `200` | `400`, `404`, `409` |
| DELETE | `/api/Productos/{id}` | `id` (long) | — | `204` (soft) | `404` |
| POST | `/api/Productos/{id}/restore` | `id` (long) | — | `200` `{message}` | `404` |
| GET | `/api/Productos/deleted` | ( | ) | `200` | — |
| GET | `/api/Productos/search` | Query `q` **obligatorio** | — | `200` | `400` si falta |
| GET | `/api/Productos/filter` | Query opcionales `nombre`, `categoriaId`, `precioMin`, `precioMax` | ( | `200` | ) |
| GET | `/api/Productos/paged` | Query `page`, `pageSize` (≤100) | ( | `200` `{data, pagination}` | ) |
| GET | `/api/Productos/categorias` | ( | ) | `200` **5** | — |
| GET | `/api/Productos/categorias/{id}` | `id` (long) | — | `200` | `404` |

### Create/Update

```json
{ "nombre": "Producto Test Cache", "precio": 42.5, "categoriaId": 1 }
```

| Campo | Obligatorio | Reglas |
|-------|-------------|--------|
| `nombre` | SÍ | unique → `409` |
| `precio` | SÍ | > 0 |
| `categoriaId` | SÍ | debe existir |

## Docker

```bash
docker compose up -d
docker compose -f docker-compose.api.yml up -d --build
docker compose down -v
```

## Estructura

```
13-ProductosCache/
├── ProductosEFCore.slnx
├── Dockerfile
├── docker-compose.yml
├── docker-compose.api.yml
├── ProductosTest/
│   ├── Cache/                    ← ICacheService + decorator
│   ├── Controllers/ Dtos/ Entity/ Errors/ …
│   └── Program.cs
├── ProductosTest.Test/
├── automation/test-runner.mjs
└── README.md
```

## Paquetes NuGet

- EF Core + PostgreSQL, `StackExchange.Redis` / MemoryCache
- `CSharpFunctionalExtensions`, `Serilog.AspNetCore`, `Swashbuckle.AspNetCore`
