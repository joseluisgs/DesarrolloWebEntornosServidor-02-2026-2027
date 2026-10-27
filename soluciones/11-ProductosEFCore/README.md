# 11 - Productos EF Core + PostgreSQL

API REST de productos con **Entity Framework Core** y **PostgreSQL**, patrón **Repository**, DTOs, FluentValidation y patrón **Result** en ASP.NET Core (.NET 10).

## Descripción

Persistencia real en PostgreSQL. El runner levanta solo la BD con `docker compose up -d db` y lanza la API con `dotnet run`. En Development hay seed: 10 productos (id 1 = Teclado Mecánico) y 5 categorías.

- CRUD + soft delete/restore + search + paged + categorías
- CHECK de precio en la base de datos (fluent API) y CHECKs de longitud del paquete `EFCore.CheckConstraints`
- 409 en nombre duplicado
- Swagger solo en Development (`ASPNETCORE_ENVIRONMENT=Development`)

## Ejecución manual

```bash
# 1. PostgreSQL
docker compose up -d db

# 2. API
dotnet run --project ProductosTest
```

Escucha en `http://localhost:5000`.

## Pruebas automatizadas (runner)

```bash
node automation/test-runner.mjs
```

Node.js nativo: `docker compose up -d db` → restore/build/run → polling ≤20 s → **13 tests** → `taskkill /T /F` + `docker compose down -v`.

- `exit 0` → todos OK · `exit 1` → detalle del fallo
- Fallback: `docker compose -f docker-compose.api.yml up -d --build` si el CLI falla

## Tests unitarios

```bash
dotnet test
```

## Endpoints

Prefijo: `http://localhost:5000/api/productos`

| Método | Ruta | Parámetros | Cuerpo de petición | Respuesta exitosa | Errores |
|--------|------|------------|--------------------|-------------------|---------|
| GET | `/api/productos` | ( | ) | `200` lista (≥10 seed) |; |
| GET | `/api/productos/{id}` | `id` (long) |; | `200` | `404` |
| POST | `/api/productos` | — | `{ nombre, precio, categoriaId, imagen? }` | `201` + id | `400`, `409` nombre duplicado |
| PUT | `/api/productos/{id}` | `id` (long) | producto completo | `200` | `400`, `404`, `409` |
| PATCH | `/api/productos/{id}/precio` | `id` (long) | **número** | `200` | `400`, `404` |
| DELETE | `/api/productos/{id}` | `id` (long) | — | `204` (soft delete) | `404` |
| GET | `/api/productos/deleted` | ( | ) | `200` borrados | — |
| POST | `/api/productos/{id}/restore` | `id` (long) | — | `200` `{message}` | `404` |
| GET | `/api/productos/search` | Query `q` **obligatorio** | — | `200` lista | `400` si falta |
| GET | `/api/productos/paged` | Query `page`, `pageSize` | ( | `200` `{data, pagination}` | ) |
| GET | `/api/productos/categorias` | ( | ) | `200` **5** categorías seed | — |
| GET | `/api/productos/categorias/{id}` | `id` (long) | — | `200` | `404` |

### Create/Update producto

```json
{
  "nombre": "Producto Nuevo EF",
  "precio": 42.5,
  "categoriaId": 1,
  "imagen": null
}
```

| Campo | Obligatorio | Reglas |
|-------|-------------|--------|
| `nombre` | SÍ | Required, unique (409) |
| `precio` | SÍ | > 0 |
| `categoriaId` | SÍ | debe existir |
| `imagen` | no | — |

### Errores típicos

| Status | Cuerpo |
|--------|--------|
| `400` | ValidationProblemDetails |
| `404` | `{"message":"..."}` |
| `409` | `{"message":"..."}` nombre duplicado |

## Docker

```bash
# Solo BD (recomendado con el runner)
docker compose up -d db

# Stack completo (api + db)
docker compose -f docker-compose.api.yml up -d --build

# Limpieza
docker compose down -v
```

> 📝 **Nota:** Los dos compose son **proyectos distintos** (cada fichero lleva su `name:`), así que no se pisan: levantar el stack completo no destruye el contenedor local, y al revés.
>
> - **Desarrollo** (`docker compose up -d db`): PostgreSQL en `localhost:5432`. Es lo que necesitas con el play de Rider.
> - **Stack completo** (`-f docker-compose.api.yml`): api en el `5000` y PostgreSQL en el `5433`, porque dentro de la red de Docker la app se conecta al servicio `db`, no a `localhost`.
>
> ⚠️ El nombre del contenedor es propio de esta solución, pero el **puerto 5432 es común a todas**: no levantes dos soluciones a la vez o la segunda fallará con `port is already allocated`.

## Estructura

```
11-ProductosEFCore/
├── ProductosEFCore.slnx
├── Dockerfile
├── docker-compose.yml          ← solo db
├── docker-compose.api.yml      ← api + db
├── ProductosTest/              ← proyecto API (¡ojo con el nombre!)
│   ├── Controllers/ Dtos/ Entity/ Errors/ Extensions/
│   ├── Helpers/ Infrastructures/ Mappers/ Middleware/
│   ├── Models/ Repositories/ Services/ Validators/ Program.cs
├── ProductosTest.Test/
│   └── Services/ProductoServiceTests.cs
├── automation/test-runner.mjs
└── README.md
```

## Paquetes NuGet

- `Npgsql.EntityFrameworkCore.PostgreSQL` / EF Core, `EFCore.CheckConstraints`
- `CSharpFunctionalExtensions`, `FluentValidation.AspNetCore`
- `Serilog.AspNetCore`, `Swashbuckle.AspNetCore`
- Test: `NUnit`, `FluentAssertions`, `Moq`

## Quién manda en la configuración

| Sitio | Qué vive ahí | Ejemplo |
|-------|--------------|---------|
| **DTOs** (FluentValidation) | Validación de entrada, antes de tocar la BD | `precio > 0` → 400 |
| **Entidad** (Data Annotations) | Mapeo básico | `[Key]`, `[StringLength(100, MinimumLength = 1)]` |
| **`AppDbContext`** (Fluent API) | Relaciones, índices, precisión, query filter y CHECK | `CK_Productos_Precio` |
| **`EFCore.CheckConstraints`** | Convierte los atributos de la entidad en CHECK | `LENGTH("Nombre") >= 1` |

Si una anotación y la fluent API se contradicen, **manda la fluent API**: es la última en configurar el modelo. Por eso el precio negativo lo frena el CHECK de `AppDbContext` (el paquete no genera CHECK para `decimal` con `[Range]`) y no se configura nada en dos sitios.

Trampas del paquete que este ejemplo ya tiene resueltas: `[Required]` junto a `[StringLength]` sobre la misma propiedad generan dos CHECK con el mismo nombre y el paquete revienta al arrancar; `[Required]` sobre un `decimal` produciría un `CHECK` de longitud sobre columna numérica; y `[StringLength]` sin `MinimumLength` genera un `LENGTH >= 0` siempre verdadero, por eso las longitudes máximas van con `HasMaxLength` en fluent API.
