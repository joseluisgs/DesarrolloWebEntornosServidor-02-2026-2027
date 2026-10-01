# 05 - Productos Scrutor API

API REST de productos con **Controladores MVC** y **Scrutor** (registro automático de servicios) en ASP.NET Core (.NET 10).

## Descripción

Misma API de productos que los ejemplos 01–03 (CRUD + consultas LINQ), pero la Inyección de Dependencias se registra de forma **automática** con [Scrutor](https://github.com/khellang/Scrutor) mediante `Scan`, sin Config classes manuales.

- Repository `Singleton` + Service `Scoped` detectados por convención de nombres (`*Repository`, `*Service`)
- Validaciones de negocio: `nombre` obligatorio, `precio > 0` (el 0 **no** vale aquí)
- Errores de validación → `400` con body **string JSON** (`"El nombre es obligatorio"`), no objeto
- Sin Swagger ni Docker: solo SDK de .NET

## Ejecución manual

```bash
dotnet run --project ProductosScrutorApi
```

Escucha en `http://localhost:5000`.

## Pruebas automatizadas

```bash
node automation/test-runner.mjs
```

Node.js nativo: compila y arranca con `dotnet run`, espera ≤20 s, ejecuta **17 tests** y limpia en `finally`.

- `exit 0` → todos los tests OK
- `exit 1` → detalle del fallo (endpoint, esperado vs obtenido, body)

> Sin `docker-compose.yml`: sin base de datos externa.

## Endpoints

Prefijo: `http://localhost:5000/api/productos`

| Método | Ruta | Parámetros | Cuerpo de petición | Respuesta exitosa | Errores |
|--------|------|------------|--------------------|-------------------|---------|
| GET | `/api/productos` | — | — | `200` `Producto[]` | — |
| GET | `/api/productos/{id}` | `id` (long) | — | `200` `Producto` | `404` |
| POST | `/api/productos` | — | `{ "nombre", "precio", "categoria", "imagen"? }` | `201` + `Location` | `400` string, binding |
| PUT | `/api/productos/{id}` | `id` (long) | `Producto` completo | `200` | `400` (valida antes), `404` |
| PATCH | `/api/productos/{id}` | `id` (long) | `{ "precio": number }` | `200` | `400` (sin precio antes que 404) |
| DELETE | `/api/productos/{id}` | `id` (long) | — | `204` | `404` |
| GET | `/api/productos/search` | Query `nombre` (opcional) | — | `200` `Producto[]` | — |
| GET | `/api/productos/categoria/{categoria}` | `categoria` (ruta) | — | `200` `Producto[]` | — |
| GET | `/api/productos/precio` | Query `min`, `max` | — | `200` `Producto[]` | `400` (no numérico) |
| GET | `/api/productos/ordenar` | Query `asc` (default `true`) | — | `200` `Producto[]` | — |
| GET | `/api/productos/grupo-categoria` | — | — | `200` `{ categoria: Producto[] }` | — |
| GET | `/api/productos/estadisticas` | — | — | `200` `{ total, precioMedio, porCategoria }` | — |

### Validaciones (POST/PUT/PATCH)

| Condición | Status | Body |
|-----------|--------|------|
| `nombre` vacío | `400` | `"El nombre es obligatorio"` (string JSON) |
| `precio <= 0` | `400` | `"El precio debe ser mayor que cero"` |

> ⚠️ A diferencia del 03, aquí el **precio 0 está prohibido**.

### Ejemplo de cuerpo

```json
{ "nombre": "Monitor", "precio": 299.99, "categoria": "Electrónica", "imagen": null }
```

## DI con Scrutor

```csharp
builder.Services.Scan(scan => scan
    .FromAssemblyOf<Program>()
    .AddClasses(classes => classes.Where(t => t.Name.EndsWith("Repository")))
        .AsImplementedInterfaces().WithSingletonLifetime()
    .AddClasses(classes => classes.Where(t => t.Name.EndsWith("Service")))
        .AsImplementedInterfaces().WithScopedLifetime());
```

| vs Ejemplo 04 | Aquí |
|----------------|------|
| `Infrastructure/*Config.cs` | No existen |
| `AddRepositories()` / `AddServices()` | `Scan(...)` automático |

## Estructura

```
05-ProductosScrutorApi/
├── ProductosScrutorApi.slnx
├── ProductosScrutorApi/
│   ├── Program.cs
│   ├── Models/Producto.cs
│   ├── Repositories/
│   ├── Services/
│   └── Controllers/ProductosController.cs
├── automation/test-runner.mjs
└── README.md
```

## Ciclo de vida (DI)

| Componente | Lifetime | Motivo |
|------------|----------|--------|
| `ProductoRepository` | Singleton | Dictionary en memoria |
| `ProductoService` | Scoped | Lógica de negocio por request |
