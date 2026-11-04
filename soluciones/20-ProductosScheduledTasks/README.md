# 20 - Scheduled Tasks (newsletter BackgroundService)

API REST con **Identity + File Storage + Email + BackgroundService** para newsletter semanal en ASP.NET Core (.NET 10).

## Descripción

Misma base Auth+CRUD+email que el 19, más `NewsletterBackgroundService` (tarea programada: lunes 8:00 AM UTC, productos de los últimos 7 días).

- Dev: `MemoryEmailService` (log, no SMTP)
- Infra: PostgreSQL + Redis
- Admin: `admin@admin.com` / `Admin123!`

> ⚠️ **Advertencia:** `wwwroot/` incluido; el `BackgroundService` resuelve `IEmailService` **dentro de un scope** (no en el constructor singleton).

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

Node.js nativo: infra → restore/build/run → polling ≤20 s → **14 tests** → limpieza.

- `exit 0` → todos OK · `exit 1` → detalle del fallo

## Tests unitarios

```bash
dotnet test
```

## Endpoints

### Auth — `/api/Auth`

| Método | Ruta | Auth | Cuerpo | Éxito | Errores |
|--------|------|------|--------|-------|---------|
| POST | `/api/Auth/login` | no | `{ email, password }` | `200` `{ token, role }` | `401` |
| POST | `/api/Auth/signup` | no | `{ email, password, nombre, apellidos }` | `200` `{ token }` | `400`/`409` |
| GET | `/api/Auth/me` | **Bearer** | — | `200` | `401` |

### Productos — `/api/Productos`

| Método | Ruta | Auth | Cuerpo | Éxito | Errores |
|--------|------|------|--------|-------|---------|
| GET | `/api/Productos` | público | ( | `200` (≥1 semilla) | ) |
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

### Tarea programada (no es endpoint HTTP)

| Componente | Descripción |
|------------|-------------|
| `NewsletterBackgroundService` | `BackgroundService` singleton |
| Programación | **Lunes 8:00 AM UTC** (`GetNextMonday`) |
| Contenido | productos `CreatedAt >= now-7d`, no borrados |
| Email | `admin@productos.com`, HTML con tabla |

⚠️ **DI:** no inyectar `IEmailService` (scoped) en el constructor del hosted service; crear un scope en cada envío.

## Docker

```bash
docker compose up -d
docker compose -f docker-compose.api.yml up -d --build
docker compose down -v
```

## Estructura

```
20-ProductosScheduledTasks/
├── ProductosEFCore.slnx
├── Dockerfile / docker-compose.yml / docker-compose.api.yml
├── ProductosTest/
│   ├── Controllers/ (Auth, Productos)
│   ├── Services/
│   │   ├── Email/
│   │   └── Scheduler/NewsletterBackgroundService.cs
│   ├── Storage/ wwwroot/
│   └── …
├── ProductosTest.Test/
├── automation/test-runner.mjs
└── README.md
```

## Paquetes NuGet

- EF Core + PostgreSQL, Redis, Identity/JWT, MailKit (prod)
- `CSharpFunctionalExtensions`, Serilog, Swashbuckle
