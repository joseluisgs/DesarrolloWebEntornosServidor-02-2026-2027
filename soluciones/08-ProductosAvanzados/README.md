# 08 - Productos Avanzados (DTOs, Validaciones, HATEOAS)

API REST de productos con **DTOs**, **Mappers**, **validaciones**, **query params**, **HTTP QUERY simulado** y **HATEOAS** en ASP.NET Core (.NET 10).

## Descripción

Ejemplo intermedio-alto que separa dominio y presentación con DTOs, valida en dos niveles (Data Annotations + FluentValidation + atributo propio `NoAdmin`), expone filtros/paginación y añade links HATEOAS en la cabecera `Link` (RFC 8288).

- Negociación de contenido: JSON y XML; `Accept: text/csv` → `406`
- Sin seed (arranca vacío); sin Docker
- Swagger solo en Development (`ASPNETCORE_ENVIRONMENT=Development`)

## Ejecución manual

```bash
dotnet run --project ProductosAvanzados
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
| POST | `/api/productos` | — | `CreateProductoDto` | `201` + `Location` | `400` ValidationProblemDetails |
| PUT | `/api/productos/{id}` | `id` (long) | `UpdateProductoDto` | `200` | `400`, `404` |
| PATCH | `/api/productos/{id}/precio` | `id` (long) | **número** | `200` | `400` (precio<0), `404` |
| DELETE | `/api/productos/{id}` | `id` (long) | — | `204` | `404` |
| GET | `/api/productos/search` | Query `q` **obligatorio** | — | `200` `ProductoDto[]` | `400` si falta `q` |
| GET | `/api/productos/filter` | Query `nombre`, `categoria`, `precioMin`, `precioMax` (opcionales) | ( | `200` `ProductoDto[]` | ) |
| GET | `/api/productos/paged` | Query `page=1`, `pageSize=10` (≤100) | ( | `200` `{data, pagination}` + header **`Link`** | ) |
| POST | `/api/productos/query` | — | `{ nombre?, categoria?, precioMin?, precioMax? }` | `200` `ProductoDto[]` | `400` sin body |

### DTOs

**CreateProductoDto (POST):**

```json
{
  "nombre": "Monitor",
  "precio": 299.00,
  "categoria": "Electrónica",
  "imagen": "https://..."
}
```

| Campo | Obligatorio | Reglas activas (DA) |
|-------|-------------|---------------------|
| `nombre` | SÍ | `MaxLength(100)`, `[NoAdmin]` (rechaza admin/root/sistema) |
| `precio` | SÍ | `Range(0.01, 999999.99)` |
| `categoria` | SÍ | Required |
| `imagen` | no | — |

**UpdateProductoDto (PUT):** igual pero sin `[NoAdmin]`.

**ProductoDto (respuesta):**

```json
{
  "id": 1, "nombre": "...", "precio": 45.0, "categoria": "...",
  "imagen": null, "createdAt": "...", "updatedAt": null, "isActivo": true
}
```

**Paginación:**

```json
{
  "data": [ /* ProductoDto */ ],
  "pagination": { "page": 1, "pageSize": 10, "totalPages": 1, "totalItems": 0 }
}
```

Cabecera: `Link: </api/productos/paged?page=1&pageSize=10>; rel="first", ...; rel="last"`

El techo de paginación (página ≥ 1 y tamaño entre 1 y 100) y el `Skip`/`Take` viven en el **repositorio**, no en el servicio: el servicio solo envuelve el resultado en `Result` y el controlador traduce a HTTP. Así, cualquier llamante que construya el filtro en código (un job, un test, una consulta GraphQL) también queda limitado.

## Estructura

```
08-ProductosAvanzados/
├── ProductosAvanzados.slnx
├── ProductosAvanzados/
│   ├── Controllers/ Dtos/ Errors/ Extensions/
│   ├── Helpers/PaginationHelper.cs   ← Link HATEOAS
│   ├── Infrastructure/ Mappers/ Middleware/
│   ├── Models/ Repositories/ Services/ Validators/
│   └── Program.cs
├── automation/test-runner.mjs
└── README.md
```

## Paquetes NuGet

- `CSharpFunctionalExtensions` → Result ROP
- `FluentValidation.AspNetCore` → validación imperativa
- `Swashbuckle.AspNetCore` → Swagger (Development)
