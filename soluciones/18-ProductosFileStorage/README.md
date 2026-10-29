# 18 - Productos File Storage (imágenes)

API REST de productos con **subida de imágenes** (`multipart/form-data`), `IStorageService` y ficheros estáticos en `wwwroot/uploads` en ASP.NET Core (.NET 10).

## Descripción

- Auth JWT + roles (admin `admin@admin.com` / `Admin123!`)
- `POST /api/Productos/imagen` crea producto con archivo
- `PATCH /api/Productos/{id}/imagen` reemplaza la imagen
- Ficheros servidos con `UseStaticFiles` bajo `/uploads/...`
- Desarrollo limpia el directorio de uploads al arrancar
- Infra: PostgreSQL + Redis; BD `productos_filestorage` se crea con `prepareDb` si falta

> ⚠️ El proyecto necesita la carpeta `wwwroot/` (si `WebRootPath` es null, el storage falla). Ya está incluida.

## Ejecución manual

```bash
# 1. Infra + BD
docker compose up -d
docker compose exec -T db psql -U admin -d postgres -c "CREATE DATABASE productos_filestorage;"

# 2. API
dotnet run --project ProductosTest
```

Escucha en `http://localhost:5000`.

## Pruebas automatizadas (runner)

```bash
node automation/test-runner.mjs
```

Node.js nativo: infra + `prepareDb` (tolera BD ya creada) → restore/build/run → polling ≤20 s → **15 tests** (incluye upload PNG 1x1) → limpieza.

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
| GET | `/api/Productos` | público | ( | `200` (≥10 seed) | ) |
| GET | `/api/Productos/{id}` | público | — | `200` | `404` |
| POST | `/api/Productos` | **ADMIN** | `{ nombre, precio, categoriaId, imagen? }` | `201` | `401`, `400`, `409` |
| PUT | `/api/Productos/{id}` | **ADMIN, USER** | `UpdateProductoDto` | `200` | `401`, `404` |
| DELETE | `/api/Productos/{id}` | **ADMIN** | — | `204` | `401`, `404` |
| POST | `/api/Productos/{id}/restore` | **ADMIN** | — | `200` `{message}` | `401`, `404` |
| **POST** | **`/api/Productos/imagen`** | **ADMIN** | **`multipart/form-data`**: `nombre`, `precio`, `categoriaId`, `archivo` | **`201`** | `400` tipo/tamaño, `401` |
| **PATCH** | **`/api/Productos/{id}/imagen`** | **ADMIN** | **`multipart/form-data`**: `archivo` | **`200`** | `400`, `401`, `404` |
| GET | `/api/Productos/deleted` | público | ( | `200` | ) |
| GET | `/api/Productos/search` | Query `q` **obligatorio** | — | `200` | `400` |
| GET | `/api/Productos/filter` | Query opcionales | ( | `200` | ) |
| GET | `/api/Productos/paged` | Query `page`, `pageSize` | ( | `200` `{data, pagination}` | ) |
| GET | `/api/Productos/categorias` | público | ( | `200` | ) |
| GET | `/uploads/productos/{file}` | estático | — | `200` imagen | `404` |

**Ejemplo upload (curl):**

```bash
curl -X POST http://localhost:5000/api/Productos/imagen \
  -H "Authorization: Bearer $TOKEN" \
  -F "nombre=Producto Con Imagen Test" \
  -F "precio=29.99" \
  -F "categoriaId=1" \
  -F "archivo=@test.png;type=image/png"
```

## Docker

```bash
docker compose up -d
docker compose -f docker-compose.api.yml up -d --build
docker compose down -v
```

## Estructura

```
18-ProductosFileStorage/
├── ProductosEFCore.slnx
├── Dockerfile / docker-compose.yml / docker-compose.api.yml
├── ProductosTest/
│   ├── Controllers/ (Auth, Productos)
│   ├── Storage/     ← IStorageService + FileSystem
│   ├── wwwroot/     ← necesario (WebRootPath)
│   └── uploads/     ← ficheros subidos
├── ProductosTest.Test/
├── automation/test-runner.mjs
└── README.md
```

## Paquetes NuGet

- EF Core + PostgreSQL, Redis cache, Identity/JWT
- `CSharpFunctionalExtensions`, Serilog, Swashbuckle
