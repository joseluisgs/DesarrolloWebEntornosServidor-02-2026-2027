# 12 - Productos MongoDB (Driver + EF Core)

API REST de productos con **MongoDB**: doble implementación de repositorio (**Driver nativo** y **EF Core MongoDB**) en ASP.NET Core (.NET 10).

## Descripción

Dos formas de relacionar documentos en el mismo modelo (`Models/Producto.cs`):

- **Composición**: `categoria` va **embebida** dentro del producto (`[Owned]` en EF Core)
- **Referencia**: `proveedorId` apunta a la colección `proveedores` — solo el `_id` viaja en el producto

IDs `ObjectId` de 24 hex, patrón **Result** y Serilog.

- En Development el seed **borra y recrea** las colecciones (5 proveedores + 10 productos; ObjectIds nuevos en cada arranque, salvo los proveedores que usan **ID fijo** para que las referencias no se rompan)
- `Productos:RepositoryType`: `MongoDbNative` (default) o `MongoDbEfCore`
- Swagger solo en Development

## Ejecución manual

```bash
# 1. MongoDB
docker compose up -d db

# 2. API
dotnet run --project ProductosMongo
```

Escucha en `http://localhost:5000`.

## Pruebas automatizadas (runner)

```bash
node automation/test-runner.mjs
```

Node.js nativo: `docker compose up -d db` → restore/build/run → polling ≤20 s → **11 tests** → `taskkill /T /F` + `docker compose down -v`.

- `exit 0` → todos OK · `exit 1` → detalle del fallo
- Fallback: `docker compose -f docker-compose.api.yml up -d --build`

## Tests unitarios

```bash
dotnet test
```

## Endpoints

Prefijo: `http://localhost:5000/api/productos`

| Método | Ruta | Parámetros | Cuerpo de petición | Respuesta exitosa | Errores |
|--------|------|------------|--------------------|-------------------|---------|
| GET | `/api/productos` | ( | ) | `200` lista (≥10 seed) | — |
| GET | `/api/productos/{id}` | `id` ObjectId (24 hex) | — | `200` | `404` |
| POST | `/api/productos` | — | `Producto` (ver abajo) | `201` + Location | `400`, `409` |
| PUT | `/api/productos/{id}` | `id` (URL) | `Producto` con **mismo `id`** | `200` | `400` id mismatch / validación, `404`, `409` |
| DELETE | `/api/productos/{id}` | `id` ObjectId | — | `204` (soft delete) | `404` |
| GET | `/api/productos/deleted` | ( | ) | `200` borrados | — |
| GET | `/api/productos/search` | Query `q` | — | `200` (nombre o categoría) | `400` si `q` ausente |
| GET | `/api/productos/filter` | Query opcionales `nombre`, `precioMin`, `precioMax` | ( | `200` | ) |

### Cuerpo de producto (POST/PUT)

```json
{
  "nombre": "Producto Nuevo Mongo",
  "precio": 42.5,
  "imagen": null,
  "categoria": { "nombre": "Test", "descripcion": "Categoría de prueba" },
  "proveedorId": "64b000000000000000000001"
}
```

| Campo | Obligatorio | Reglas |
|-------|-------------|--------|
| `nombre` | SÍ | Required; unique → `409` |
| `precio` | SÍ | > 0 → `400` |
| `imagen` | no | nullable |
| `categoria` | SÍ | `{ nombre, descripcion }` **embebida** (composición) |
| `proveedorId` | SÍ | ObjectId de la colección `proveedores` **referenciado**; vacío → `400` |

| Status | Cuerpo típico |
|--------|----------------|
| `400` | `{ message, errors }` validación |
| `404` | `{ message: "Producto con ID … no encontrado" }` |
| `409` | `{ message: "Ya existe un producto con el valor '…'" }` |

## Docker

```bash
docker compose up -d db
docker compose -f docker-compose.api.yml up -d --build   # api + db
docker compose down -v
```

> 📝 **Nota:** Los dos compose son **proyectos distintos** (cada fichero lleva su `name:`), así que no se pisan: levantar el stack completo no destruye el contenedor local, y al revés.
>
> - **Desarrollo** (`docker compose up -d db`): MongoDB en `localhost:27017`. Es lo que necesitas con el play de Rider.
> - **Stack completo** (`-f docker-compose.api.yml`): api en el `5000` y MongoDB en el `27018`, porque dentro de la red de Docker la app se conecta al servicio `db`, no a `localhost`.
>
> ⚠️ El nombre del contenedor es propio de esta solución, pero el **puerto 27017 es común**: no levantes dos soluciones a la vez o la segunda fallará con `port is already allocated`.

## Estructura

```
12-ProductosMongo/
├── ProductosEFCore.slnx
├── Dockerfile
├── docker-compose.yml          ← solo db
├── docker-compose.api.yml      ← api + db
├── ProductosMongo/
│   ├── Controllers/ Converters/ Entity/ Errors/ Extensions/
│   ├── Infrastructures/ Models/ Seed/ Services/
│   ├── Repositories/
│   │   ├── EfCore/
│   │   └── Native/
│   └── Program.cs
├── ProductosMongo.Test/
├── automation/test-runner.mjs
└── README.md
```

## Paquetes NuGet

- `MongoDB.Driver`, `MongoDB.EntityFrameworkCore`
- `CSharpFunctionalExtensions`, `Serilog.AspNetCore`, `Swashbuckle.AspNetCore`
