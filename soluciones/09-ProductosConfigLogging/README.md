# 09 - Configuración tipada y Serilog Logging

API REST de productos con **configuración fuertemente tipada** (`IOptions<T>`) y **Serilog** en ASP.NET Core (.NET 10).

## Descripción

Misma base de endpoints y DTOs que el ejemplo 08 (CRUD + search/filter/paged/query + HATEOAS), añadiendo:

- **`AppConfig` / `CacheConfig`** leídos de `appsettings.json` con `GetSection()`
- **Serilog** bootstrap + `ReadFrom.Configuration()` (sinks Console y File con rolling diario)
- **`UseSerilogRequestLogging()`**: cada petición HTTP queda registrada
- Logs también en `logs/productos-*.log`

Sin seed; sin Docker; Swagger solo en Development.

## Ejecución manual

```bash
# Development (por defecto con el runner)
dotnet run --project ProductosConfigLogging

# Producción
ASPNETCORE_ENVIRONMENT=Production dotnet run --project ProductosConfigLogging
```

Escucha en `http://localhost:5000`.

## Pruebas automatizadas

```bash
node automation/test-runner.mjs
```

Node.js nativo: restore/build/run → polling ≤20 s → **14 tests** → limpieza en `finally`.

- `exit 0` → todos OK · `exit 1` → detalle del fallo

> Sin `docker-compose.yml`.

## Endpoints

Prefijo: `http://localhost:5000/api/productos`

| Método | Ruta | Parámetros | Cuerpo de petición | Respuesta exitosa | Errores |
|--------|------|------------|--------------------|-------------------|---------|
| GET | `/api/productos` | ( | ) | `200` `ProductoDto[]` | — |
| GET | `/api/productos/{id}` | `id` (long) | — | `200` `ProductoDto` | `404` |
| POST | `/api/productos` | — | `CreateProductoDto` | `201` + `Location` | `400` |
| PUT | `/api/productos/{id}` | `id` (long) | `UpdateProductoDto` | `200` | `400`, `404` |
| PATCH | `/api/productos/{id}/precio` | `id` (long) | **número** | `200` | `400`, `404` |
| DELETE | `/api/productos/{id}` | `id` (long) | — | `204` | `404` |
| GET | `/api/productos/search` | Query `q` **obligatorio** | — | `200` `ProductoDto[]` | `400` si falta |
| GET | `/api/productos/filter` | Query opcionales `nombre`, `categoria`, `precioMin`, `precioMax` | ( | `200` `ProductoDto[]` | ) |
| GET | `/api/productos/paged` | Query `page`, `pageSize` (≤100) | ( | `200` `{data, pagination}` + **`Link`** | ) |
| POST | `/api/productos/query` | — | `{ nombre?, categoria?, precioMin?, precioMax? }` | `200` `ProductoDto[]` | `400` |

### CreateProductoDto (POST)

```json
{ "nombre": "Monitor", "precio": 299.00, "categoria": "Electrónica", "imagen": null }
```

| Campo | Obligatorio | Reglas |
|-------|-------------|--------|
| `nombre` | SÍ | máx 100, `[NoAdmin]` |
| `precio` | SÍ | `0.01`–`999999.99` |
| `categoria` | SÍ | Required |
| `imagen` | no | — |

### Perfiles de entorno (config/serilog)

| Entorno | Nivel | Cache TTL |
|---------|-------|-----------|
| Development | Debug | 2 min |
| Production | Warning | 60 min |
| Default | Information | 5 min |

## Estructura

```
09-ProductosConfigLogging/
├── ProductosConfigLogging.slnx
├── ProductosConfigLogging/
│   ├── Config/AppConfig.cs, CacheConfig.cs
│   ├── Controllers/ Dtos/ Errors/ Extensions/ Helpers/
│   ├── Infrastructure/ Mappers/ Middleware/
│   ├── Models/ Repositories/ Services/ Validators/
│   ├── Program.cs              ← Serilog bootstrap
│   └── appsettings*.json
├── automation/test-runner.mjs
└── README.md
```

## Paquetes NuGet

- `Serilog.AspNetCore` (+ sinks Console/File)
- `CSharpFunctionalExtensions`, `FluentValidation.AspNetCore`, `Swashbuckle.AspNetCore`
