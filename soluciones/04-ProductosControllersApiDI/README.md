# 04 - Controllers con Inyección de Dependencias

API REST de productos con **Controladores MVC**, **Inyección de Dependencias** y **Config classes** en ASP.NET Core (.NET 10).

## Descripción

Variante del ejemplo 02 con DI por capas y un conjunto de rutas de consulta **diferente** (`search`, `filter`, `order`, `group`, `stats`). Incluye **seed data** con 5 productos (ids 1–5) y validaciones de negocio en el servicio.

- Repository `Singleton` + Service `Scoped` registrados con métodos de extensión (`AddRepositories()`, `AddServices()`)
- Errores de negocio → `400` `{"error": "..."}`
- PATCH de precio con **número JSON crudo** en `/{id}/precio`
- Sin Swagger ni Docker: solo SDK de .NET

## Ejecución manual

```bash
dotnet run --project ProductosControllersApiDI
```

Escucha en `http://localhost:5000`.

## Pruebas automatizadas

```bash
node automation/test-runner.mjs
```

Script Node.js nativo: `dotnet restore`/`build`/`run` → polling ≤20 s → **9 tests** HTTP → limpieza en `finally`.

- `exit 0` → todos los tests OK
- `exit 1` → fallo estructurado (endpoint, esperado vs obtenido, body)

> Sin `docker-compose.yml`: sin base de datos externa.

## Endpoints

Prefijo: `http://localhost:5000/api/productos`

| Método | Ruta | Parámetros | Cuerpo de petición | Respuesta exitosa | Errores |
|--------|------|------------|--------------------|-------------------|---------|
| GET | `/api/productos` | ( | ) | `200` `Producto[]` (5 seed) | — |
| GET | `/api/productos/{id}` | `id` (long) | — | `200` `Producto` | `404` |
| POST | `/api/productos` | — | `{ "nombre", "precio", "categoria", "imagen"? }` | `201` + `Location` | `400` `{"error"}`, binding |
| PUT | `/api/productos/{id}` | `id` (long) | `Producto` completo | `200` | `400` (valida antes), `404` |
| PATCH | `/api/productos/{id}/precio` | `id` (long) | **número** p.ej. `799.99` | `200` | `400`, `404` |
| DELETE | `/api/productos/{id}` | `id` (long) | — | `204` | `404` |
| GET | `/api/productos/search` | Query `termino` **obligatorio** | — | `200` `Producto[]` | `400` si falta |
| GET | `/api/productos/filter/categoria` | Query `categoria` **obligatorio** | — | `200` `Producto[]` | `400` si falta |
| GET | `/api/productos/filter/precio` | Query `min`, `max` (default `0`/`MaxValue`) | ( | `200` `Producto[]` | ) |
| GET | `/api/productos/order/precio` | Query `descendente` (default `false`) | ( | `200` `Producto[]` | ) |
| GET | `/api/productos/group/categoria` | ( | ) | `200` **array** `[{categoria, productos}]` | — |
| GET | `/api/productos/stats` | ( | ) | `200` `{totalProductos, precioMedio, precioMinimo, precioMaximo, categorias}` | — |

### Seed data

| id | nombre | precio | categoria |
|----|--------|--------|-----------|
| 1 | Portátil | 899.99 | Electrónica |
| 2 | Ratón | 29.99 | Electrónica |
| 3 | Teclado | 79.99 | Electrónica |
| 4 | Silla | 199.99 | Mobiliario |
| 5 | Escritorio | 349.99 | Mobiliario |

### Validaciones (POST/PUT)

| Condición | Status | Body |
|-----------|--------|------|
| `nombre` vacío | `400` | `{"error":"El nombre del producto es obligatorio."}` |
| `precio < 0` | `400` | `{"error":"El precio no puede ser negativo."}` |

### Ejemplos

```bash
curl -X POST http://localhost:5000/api/productos \
  -H "Content-Type: application/json" \
  -d '{"nombre": "Monitor", "precio": 299.99, "categoria": "Electrónica"}'

curl -X PATCH http://localhost:5000/api/productos/1/precio \
  -H "Content-Type: application/json" -d 799.99
```

## Estructura

```
04-ProductosControllersApiDI/
├── ProductosControllersApiDI.slnx
├── ProductosControllersApiDI/
│   ├── Controllers/ProductosController.cs
│   ├── Models/Producto.cs
│   ├── Repositories/
│   ├── Services/
│   ├── Infrastructure/
│   └── Program.cs
├── automation/test-runner.mjs
└── README.md
```

## Ciclo de vida (DI)

| Componente | Lifetime | Motivo |
|------------|----------|--------|
| `ProductoRepository` | Singleton | Dictionary en memoria |
| `ProductoService` | Scoped | Lógica de negocio por request |
