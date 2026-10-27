# 15 - Productos Identity (JWT + Roles)

API REST de productos con **ASP.NET Core Identity**, **JWT** y autorización por roles en ASP.NET Core (.NET 10).

## Descripción

Lectura pública de productos; escritura (POST/PUT/DELETE/restore) requiere JWT con rol `ADMIN` (PUT también `USER`).

- Seed Development: productos (≥10) + usuarios/roles
- Credenciales admin de prueba: `admin@admin.com` / `Admin123!` (rol `ADMIN`)
- Swagger solo en Development (`ASPNETCORE_ENVIRONMENT=Development`)

## Ejecución manual

```bash
# 1. PostgreSQL
docker compose up -d

# 2. API
dotnet run --project ProductosTest
```

Escucha en `http://localhost:5000`.

## Pruebas automatizadas (runner)

```bash
node automation/test-runner.mjs
```

Node.js nativo: infra → restore/build/run → polling ≤20 s → **11 tests** → limpieza.

- `exit 0` → todos OK · `exit 1` → detalle del fallo

## Tests unitarios

```bash
dotnet test
```

## Endpoints

### Auth — `http://localhost:5000/api/Auth`

| Método | Ruta | Auth | Cuerpo de petición | Respuesta exitosa | Errores |
|--------|------|------|--------------------|-------------------|---------|
| POST | `/api/Auth/login` | no | `{ email, password }` | `200` `{ token, role, ... }` | `401` credenciales |
| POST | `/api/Auth/signup` | no | `{ email, password, nombre, apellidos }` | `200` `{ token, ... }` | `400`/`409` |
| GET | `/api/Auth/me` | **Bearer** | — | `200` perfil + `role` | `401` sin token |

**Login admin:**

```json
{ "email": "admin@admin.com", "password": "Admin123!" }
→ { "token": "…", "role": "ADMIN", … }
```

Envío de token: cabecera `Authorization: Bearer <token>`.

### Productos — `/api/Productos`

| Método | Ruta | Auth / Roles | Cuerpo | Éxito | Errores |
|--------|------|--------------|--------|-------|---------|
| GET | `/api/Productos` | público | ( | `200` (≥10) | ) |
| GET | `/api/Productos/{id}` | público | — | `200` | `404` |
| POST | `/api/Productos` | **ADMIN** | `{ nombre, precio, categoriaId, imagen? }` | `201` | `401` sin token, `400`, `409` |
| PUT | `/api/Productos/{id}` | **ADMIN, USER** | `UpdateProductoDto` | `200` | `401`, `403`, `400`, `404` |
| DELETE | `/api/Productos/{id}` | **ADMIN** | — | `204` | `401`, `404` |
| POST | `/api/Productos/{id}/restore` | **ADMIN** | — | `200` `{message}` | `401`, `404` |
| GET | `/api/Productos/deleted` | público | ( | `200` | ) |
| GET | `/api/Productos/search` | Query `q` **obligatorio** | — | `200` | `400` |
| GET | `/api/Productos/filter` | Query opcionales | ( | `200` | ) |
| GET | `/api/Productos/paged` | Query `page`, `pageSize` | ( | `200` `{data, pagination}` | ) |
| GET | `/api/Productos/categorias` | público | ( | `200` | ) |
| GET | `/api/Productos/categorias/{id}` | público | — | `200` | `404` |

**Ejemplo create con token:**

```http
POST /api/Productos
Authorization: Bearer <admin-token>
Content-Type: application/json

{ "nombre": "Producto Identity Test", "precio": 33.3, "categoriaId": 1 }
```

## Docker

```bash
docker compose up -d
docker compose -f docker-compose.api.yml up -d --build
docker compose down -v
```

> 📝 **Nota:** Los dos compose son **proyectos distintos** (cada fichero lleva su `name:`), así que no se pisan: levantar el stack completo no destruye los contenedores locales, y al revés.
>
> - **Desarrollo** (`docker compose up -d`): PostgreSQL en `localhost:5432` y Redis en `localhost:6379`. Es lo que necesitas con el play de Rider.
> - **Stack completo** (`-f docker-compose.api.yml`): api en el `5000`, PostgreSQL en el `5433` y Redis en el `6380`, porque dentro de la red de Docker la app se conecta a los servicios `db` y `redis`, no a `localhost`. Arranca en `Development` a propósito: así se crean las tablas y se siembra Identity, y ahí vive Swagger.
>
> ⚠️ Los nombres de contenedor son propios de esta solución, pero los **puertos 5432 y 6379 son comunes**: no levantes dos soluciones a la vez o la segunda fallará con `port is already allocated`.

## Estructura

```
15-ProductosIdentity/
├── ProductosEFCore.slnx
├── Dockerfile / docker-compose.yml / docker-compose.api.yml
├── ProductosTest/
│   ├── Controllers/  (Auth, Productos)
│   ├── Seed/IdentitySeeder.cs
│   └── …
├── ProductosTest.Test/
├── automation/test-runner.mjs
└── README.md
```

## Paquetes NuGet

- ASP.NET Core Identity + EF Core/PostgreSQL
- JWT bearer auth, `CSharpFunctionalExtensions`, Serilog, Swashbuckle
