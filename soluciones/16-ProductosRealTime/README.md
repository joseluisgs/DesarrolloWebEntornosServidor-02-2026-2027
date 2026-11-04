# 16 - Productos RealTime (SignalR + JWT)

API REST de productos con **SignalR** (notificaciones en tiempo real) y autenticación **JWT** en ASP.NET Core (.NET 10).

## Descripción

Misma base Auth+CRUD que el 15 (roles `ADMIN`/`USER`), más un Hub SignalR con grupos e `IHubContext` para emitir eventos en el CRUD.

- Seed Development: productos (≥10) + usuarios (admin `admin@admin.com` / `Admin123!`)
- Infra: PostgreSQL + Redis vía Docker
- Swagger solo en Development

## Ejecución manual

```bash
# 1. Infra
docker compose up -d

# 2. API
dotnet run --project ProductosTest
```

Escucha en `http://localhost:5110` · Hub: `/hubs/...` (ver `Hubs/`).

## Pruebas automatizadas (runner)

```bash
node automation/test-runner.mjs
```

Node.js nativo: `docker compose up -d db redis` → restore/build/run → polling ≤20 s → **12 tests** → limpieza.

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

**Login admin:** `admin@admin.com` / `Admin123!` → `role: "ADMIN"`.

### Productos — `/api/Productos`

| Método | Ruta | Auth / Roles | Cuerpo | Éxito | Errores |
|--------|------|--------------|--------|-------|---------|
| GET | `/api/Productos` | público | ( | `200` (≥10) | ) |
| GET | `/api/Productos/{id}` | público | — | `200` | `404` |
| POST | `/api/Productos` | **ADMIN** | `{ nombre, precio, categoriaId }` | `201` | `401`, `400`, `409` |
| PUT | `/api/Productos/{id}` | **ADMIN, USER** | `UpdateProductoDto` | `200` | `401`, `403`, `404` |
| DELETE | `/api/Productos/{id}` | **ADMIN** | — | `204` | `401`, `404` |
| POST | `/api/Productos/{id}/restore` | **ADMIN** | — | `200` `{message}` | `401`, `404` |
| GET | `/api/Productos/deleted` | público | ( | `200` | ) |
| GET | `/api/Productos/search` | Query `q` **obligatorio** | — | `200` | `400` |
| GET | `/api/Productos/filter` | Query opcionales | ( | `200` | ) |
| GET | `/api/Productos/paged` | Query `page`, `pageSize` | ( | `200` `{data, pagination}` | ) |
| GET | `/api/Productos/categorias` | público | ( | `200` | ) |

### SignalR

- Hub(s) bajo `/hubs/...` (carpeta `Hubs/`)
- El CRUD emite eventos server→client vía `IHubContext`
- Autenticación JWT compatible con el hub

```http
GET /hubs/productos
Authorization: Bearer <token>
```

## Docker

```bash
docker compose up -d
docker compose -f docker-compose.api.yml up -d --build
docker compose down -v
```

## Estructura

```
16-ProductosRealTime/
├── ProductosEFCore.slnx
├── Dockerfile / docker-compose.yml / docker-compose.api.yml
├── ProductosTest/
│   ├── Controllers/  (Auth, Productos)
│   ├── Hubs/         ← SignalR
│   ├── Seed/
│   └── …
├── ProductosTest.Test/
├── automation/test-runner.mjs
└── README.md
```

## Paquetes NuGet

- SignalR (`Microsoft.AspNetCore.SignalR` incluido en framework)
- JWT + Identity, EF Core/PostgreSQL, Serilog, Swashbuckle
