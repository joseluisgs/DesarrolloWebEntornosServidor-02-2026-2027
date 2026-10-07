# 10 - Productos Test (NUnit + Moq + Docker)

API REST de productos con **tests unitarios** (NUnit, FluentAssertions, Moq), **Serilog** y **Docker / Docker Compose** en ASP.NET Core (.NET 10).

## Descripción

Misma base de endpoints y DTOs que los ejemplos 08–09 (CRUD + search/filter/paged/query + HATEOAS `Link`), con un proyecto de tests separado que usa mocks de repositorio/servicio.

- API sin seed (arranca vacío)
- Runner: `dotnet run` + Docker solo si hay `docker-compose.yml` (BD/infra)
- Swagger solo en Development (`ASPNETCORE_ENVIRONMENT=Development`)

## Ejecución manual

```bash
dotnet run --project ProductosTest
# http://localhost:5000
```

## Pruebas automatizadas (runner)

```bash
node automation/test-runner.mjs
```

Node.js nativo: restore/build/run → polling ≤20 s → **14 tests** → limpieza en `finally`.

- `exit 0` → todos OK · `exit 1` → detalle del fallo

## Tests unitarios (NUnit)

```bash
dotnet test ProductosTest.slnx

# Con cobertura
dotnet test ProductosTest.slnx --collect:"XPlat Code Coverage"
```

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

## Docker / podman

```bash
docker compose up --build
# http://localhost:5000 · http://localhost:5000/swagger

# Solo imagen
docker build -t productos-test .
docker run -p 5000:8080 productos-test
```

## Estructura

```
10-ProductosTest/
├── ProductosTest.slnx
├── Dockerfile
├── docker-compose.yml
├── .dockerignore
├── ProductosTest/
│   ├── Controllers/ Dtos/ Errors/ Helpers/ Models/
│   ├── Repositories/ Services/ Validators/ Program.cs
├── ProductosTest.Test/
│   └── Services/ProductoServiceTests.cs
├── automation/test-runner.mjs
└── README.md
```

## Paquetes NuGet

- API: `CSharpFunctionalExtensions`, `FluentValidation.AspNetCore`, `Serilog.AspNetCore`, `Swashbuckle.AspNetCore`
- Test: `NUnit`, `FluentAssertions`, `Moq`
