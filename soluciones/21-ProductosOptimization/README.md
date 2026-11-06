# 21 - Productos Optimization (Polly, Rate Limiting, Health)

API REST con **Identity + Storage + Email + Optimizaciones**: compresión, Polly, rate limiting y health checks en ASP.NET Core (.NET 10).

## Descripción

Misma base Auth+CRUD+newsletter que el 20, más capas de resiliencia y observabilidad.

- `GET /health` — health checks (API + PostgreSQL)
- Compresión Brotli + Gzip
- Rate limiting: **60 req/min** por cliente (fixed window)
- Polly retry con backoff exponencial
- Dev: `MemoryEmailService`; admin `admin@admin.com` / `Admin123!`
- Infra: PostgreSQL + Redis

> ⚠️ **Advertencia:** `wwwroot/` incluido; newsletter resuelve `IEmailService` en scope.

## Ejecución manual

```bash
# 1. Infra
docker compose up -d

# 2. API
dotnet run --project ProductosTest
```

Escucha en `http://localhost:5110`.

## Pruebas automatizadas (runner)

```bash
node automation/test-runner.mjs
```

Node.js nativo: infra → restore/build/run → polling **`/health`** ≤20 s → **15 tests** → limpieza.

- `exit 0` → todos OK · `exit 1` → detalle del fallo

## Tests unitarios

```bash
dotnet test
```

## Endpoints

### Salud / ops

| Método | Ruta | Auth | Éxito | Errores |
|--------|------|------|-------|---------|
| GET | `/health` | no | `200` Healthy/Degraded | `503` |
| GET | `/swagger` | no | `200` (Development) | `404` |

### Auth — `/api/Auth`

| Método | Ruta | Auth | Cuerpo | Éxito | Errores |
|--------|------|------|--------|-------|---------|
| POST | `/api/Auth/login` | no | `{ email, password }` | `200` `{ token, role }` | `401` |
| POST | `/api/Auth/signup` | no | `{ email, password, nombre, apellidos }` | `200` `{ token }` | `400`/`409` |
| GET | `/api/Auth/me` | **Bearer** | — | `200` | `401` |

### Productos — `/api/Productos`

| Método | Ruta | Auth | Cuerpo | Éxito | Errores |
|--------|------|------|--------|-------|---------|
| GET | `/api/Productos` | público | ( | `200` | ) |
| GET | `/api/Productos/{id}` | público | — | `200` | `404` |
| POST | `/api/Productos` | **ADMIN** | `{ nombre, precio, categoriaId }` | `201` (+ email) | `401`, `400`, `409` |
| PUT | `/api/Productos/{id}` | **ADMIN, USER** | `UpdateProductoDto` | `200` | `401`, `404` |
| DELETE | `/api/Productos/{id}` | **ADMIN** | — | `204` | `401`, `404` |
| POST | `/api/Productos/{id}/restore` | **ADMIN** | — | `200` `{message}` | `401`, `404` |
| GET | `/api/Productos/deleted` | público | ( | `200` | ) |
| GET | `/api/Productos/search` | Query `q` **obligatorio** | — | `200` | `400` |
| GET | `/api/Productos/filter` | Query opcionales | ( | `200` | ) |
| GET | `/api/Productos/paged` | Query `page`, `pageSize` | ( | `200` `{data, pagination}` | ) |
| GET | `/api/Productos/categorias` | público | ( | `200` | ) |

⚠️ **429 Too Many Requests** si superas 60 req/min.

## Optimizaciones

| Técnica | Config |
|---------|--------|
| Compresión | Brotli + Gzip (`UseResponseCompression`) |
| Rate limiting | 60 fixed window / cliente |
| Polly | retry + backoff exponencial (BD transitoria) |
| Health | `/health` incluye check PostgreSQL |

## Docker

```bash
docker compose up -d
docker compose -f docker-compose.api.yml up -d --build
docker compose down -v
```

## Estructura

```
21-ProductosOptimization/
├── ProductosOptimization.slnx
├── Dockerfile / docker-compose.yml / docker-compose.api.yml
├── ProductosTest/
│   ├── Controllers/ (Auth, Productos)
│   ├── Services/{Email,Scheduler}/
│   ├── Storage/ wwwroot/
│   └── Program.cs   ← compression, rate limit, health, Polly
├── ProductosTest.Test/
├── automation/test-runner.mjs
└── README.md
```

## Paquetes NuGet

- Polly, ResponseCompression, RateLimiting, HealthChecks
- EF Core + PostgreSQL, Redis, Identity/JWT, Serilog, Swashbuckle
