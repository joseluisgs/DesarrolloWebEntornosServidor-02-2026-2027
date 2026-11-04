# 19 - Productos Email (notificaciones)

API REST de productos con **envío de emails** (`IEmailService`) en cada operación CRUD, templates HTML y config por entorno en ASP.NET Core (.NET 10).

## Descripción

- Auth JWT + roles (admin `admin@admin.com` / `Admin123!`)
- Development: `MemoryEmailService` (emails en memoria/log, no SMTP real)
- Email en create/update/delete con `EmailTemplates`
- Infra: PostgreSQL + Redis; seed ≥1 productos + Identity

> ⚠️ **Advertencia:** Requiere carpeta `wwwroot/` (storage de imágenes heredado). Ya incluida.

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

**Login admin:** `admin@admin.com` / `Admin123!`.

### Productos — `/api/Productos`

| Método | Ruta | Auth | Cuerpo | Éxito | Errores |
|--------|------|------|--------|-------|---------|
| GET | `/api/Productos` | público | ( | `200` lista | ) |
| GET | `/api/Productos/{id}` | público | — | `200` | `404` |
| POST | `/api/Productos` | **ADMIN** | `{ nombre, precio, categoriaId }` | `201` (+ email) | `401`, `400`, `409` |
| PUT | `/api/Productos/{id}` | **ADMIN, USER** | `UpdateProductoDto` | `200` (+ email) | `401`, `404` |
| DELETE | `/api/Productos/{id}` | **ADMIN** | — | `204` (+ email) | `401`, `404` |
| POST | `/api/Productos/{id}/restore` | **ADMIN** | — | `200` `{message}` | `401`, `404` |
| GET | `/api/Productos/deleted` | público | ( | `200` | ) |
| GET | `/api/Productos/search` | Query `q` **obligatorio** | — | `200` | `400` si falta |
| GET | `/api/Productos/filter` | Query opcionales | ( | `200` | ) |
| GET | `/api/Productos/paged` | Query `page`, `pageSize` | ( | `200` `{data, pagination}` | ) |
| GET | `/api/Productos/categorias` | público | ( | `200` | ) |

**Ejemplo create:**

```json
{ "nombre": "Producto Test 171234", "precio": 19.99, "categoriaId": 1 }
```

## Emails

| Evento | Cuándo |
|--------|--------|
| Create | tras `repository.Add` |
| Update | tras update |
| Delete | tras soft delete |

- **Dev:** `MemoryEmailService` → log Serilog (sin red)
- **Prod:** MailKit + `Email:Smtp` en `appsettings.Production.json`

## Docker

```bash
docker compose up -d
docker compose -f docker-compose.api.yml up -d --build
docker compose down -v
```

## Estructura

```
19-ProductosEmail/
├── ProductosEFCore.slnx
├── Dockerfile / docker-compose.yml / docker-compose.api.yml
├── ProductosTest/
│   ├── Controllers/ (Auth, Productos)
│   ├── Services/Email/   ← IEmailService + templates
│   ├── Storage/ wwwroot/
│   └── …
├── ProductosTest.Test/
├── automation/test-runner.mjs
└── README.md
```

## Paquetes NuGet

- EF Core + PostgreSQL, Redis, Identity/JWT
- MailKit (prod), `CSharpFunctionalExtensions`, Serilog, Swashbuckle
