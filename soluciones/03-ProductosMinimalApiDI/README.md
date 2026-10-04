# 03 - Productos Minimal API con DI

API REST de productos con **Minimal APIs** e **Inyección de Dependencias** en ASP.NET Core (.NET 10).

## Descripción

Evolución del ejemplo 01: misma API de productos, pero con arquitectura en capas y DI:

- **Repository** (`Singleton`): `Dictionary<long, Producto>` en memoria
- **Service** (`Scoped`): lógica de negocio y **validaciones** (`nombre`, `precio`, `categoria`)
- **Config classes** en `Infrastructure/` para registrar dependencias
- Rutas en `Routes/ProductosRoutes.cs` con `MapGroup`
- Errores de validación → `400` con body `{"error": "..."}`

Sin Swagger ni Docker: solo SDK de .NET.

## Ejecución manual

```bash
dotnet run --project ProductosMinimalApiDI
```

Escucha en `http://localhost:5000`.

## Pruebas automatizadas

```bash
node automation/test-runner.mjs
```

Node.js nativo (sin dependencias): arranca la API con `dotnet run`, espera hasta 20 s, ejecuta **18 tests** (CRUD, consultas y validaciones) y limpia en `finally`.

- `exit 0` → todos los tests OK
- `exit 1` → fallo detallado (endpoint, esperado vs obtenido, body)

> Sin `docker-compose.yml`: no hay base de datos externa.

## Endpoints

Prefijo: `http://localhost:5000/api/productos`

| Método | Ruta | Parámetros | Cuerpo de petición | Respuesta exitosa | Errores |
|--------|------|------------|--------------------|-------------------|---------|
| GET | `/api/productos` | ( | ) | `200` `Producto[]` | — |
| GET | `/api/productos/{id}` | `id` (long) | — | `200` `Producto` | `404` |
| POST | `/api/productos` | — | `{ "nombre", "precio", "categoria", "imagen"? }` | `201` + `Location` | `400` `{"error"}`, binding |
| PUT | `/api/productos/{id}` | `id` (long) | `Producto` completo | `200` | `400` (valida **antes** que 404), `404` |
| PATCH | `/api/productos/{id}` | `id` (long) | `{ "precio": number }` | `200` | `400` (sin precio o negativo), `404` |
| DELETE | `/api/productos/{id}` | `id` (long) | — | `204` | `404` |
| GET | `/api/productos/search` | Query `nombre` (opcional) | ( | `200` `Producto[]` | ) |
| GET | `/api/productos/categoria/{categoria}` | `categoria` (ruta) | ( | `200` `Producto[]` | ) |
| GET | `/api/productos/precio` | Query `min`, `max` | — | `200` `Producto[]` | `400` (no numérico) |
| GET | `/api/productos/ordenar` | Query `asc` (default `true`) | ( | `200` `Producto[]` | ) |
| GET | `/api/productos/grupo-categoria` | ( | ) | `200` `{ categoria: Producto[] }` | — |
| GET | `/api/productos/estadisticas` | ( | ) | `200` `{ total, precioMedio, porCategoria }` | — |

### Validaciones de negocio (POST/PUT/PATCH)

| Condición | Status | Body |
|-----------|--------|------|
| `nombre` vacío | `400` | `{"error":"El nombre es obligatorio."}` |
| `precio < 0` | `400` | `{"error":"El precio no puede ser negativo."}` |
| `categoria` vacía | `400` | `{"error":"La categoría es obligatoria."}` |
| PATCH sin `precio` | `400` | (comprueba **antes** que la existencia) |

### Ejemplo de cuerpo (POST/PUT)

```json
{
  "nombre": "Monitor",
  "precio": 299.99,
  "categoria": "Electrónica",
  "imagen": null
}
```

## Estructura

```
03-ProductosMinimalApiDI/
├── ProductosMinimalApiDI.slnx
├── ProductosMinimalApiDI/
│   ├── Program.cs
│   ├── Models/Producto.cs
│   ├── Repositories/
│   ├── Services/
│   ├── Infrastructure/
│   └── Routes/ProductosRoutes.cs
├── automation/test-runner.mjs
└── README.md
```

## Ciclo de vida (DI)

| Componente | Lifetime | Motivo |
|------------|----------|--------|
| `ProductoRepository` | Singleton | Dictionary en memoria entre requests |
| `ProductoService` | Scoped | Lógica de negocio por request |
