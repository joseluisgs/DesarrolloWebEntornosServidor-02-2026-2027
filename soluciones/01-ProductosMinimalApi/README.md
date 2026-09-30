# 01 - Productos Minimal API

API REST de productos con **Minimal APIs** en ASP.NET Core (.NET 10).

## Descripción

Ejemplo introductorio de Minimal APIs: CRUD completo de productos con almacenamiento en memoria (`Dictionary<long, Producto>`), consultas LINQ (búsqueda, filtrado, ordenación, agrupación y estadísticas) y eliminación lógica mediante `DeletedAt`.

- Rutas definidas en un grupo (`Routes/ProductosRoutes.cs`) con `MapGroup("/api/productos")`
- Serialización JSON en `camelCase`
- Sin validación de negocio en este primer ejemplo (solo errores de binding)
- Sin Swagger ni Docker: solo se necesita el SDK de .NET

## Ejecución manual

```bash
dotnet run --project ProductosMinimalApi
```

La API escucha en `http://localhost:5000`.

## Pruebas automatizadas

Desde la raíz del proyecto (al lado del `.slnx`):

```bash
node automation/test-runner.mjs
```

El script (Node.js nativo, sin dependencias):

1. Compila y arranca la API con `dotnet run` en segundo plano
2. Espera a que responda (hasta 20 s)
3. Ejecuta la suite HTTP (15 tests: CRUD, consultas y errores)
4. Detiene el proceso y libera el puerto en un bloque `finally`

Código de salida: `0` si todo pasa, `1` si algún test falla.

> Este proyecto **no incluye `docker-compose.yml`**: no hay base de datos externa.

## Endpoints

Prefijo base: `http://localhost:5000/api/productos`

| Método | Ruta | Parámetros | Cuerpo de petición | Respuesta exitosa | Errores |
|--------|------|------------|--------------------|-------------------|---------|
| GET | `/api/productos` | — | — | `200` `Producto[]` (solo activos) | — |
| GET | `/api/productos/{id}` | `id` (long, ruta) | — | `200` `Producto` | `404` |
| POST | `/api/productos` | — | `{ "nombre", "precio", "categoria", "imagen"? }` | `201` `Producto` + `Location` | `400` (JSON inválido) |
| PUT | `/api/productos/{id}` | `id` (long, ruta) | `Producto` completo | `200` `Producto` | `404` |
| PATCH | `/api/productos/{id}` | `id` (long, ruta) | `{ "precio": number }` | `200` `Producto` | `400` (sin `precio`), `404` |
| DELETE | `/api/productos/{id}` | `id` (long, ruta) | — | `204` sin cuerpo | `404` |
| GET | `/api/productos/search` | Query `nombre` (opcional) | — | `200` `Producto[]` | — |
| GET | `/api/productos/categoria/{categoria}` | `categoria` (ruta) | — | `200` `Producto[]` | — |
| GET | `/api/productos/precio` | Query `min`, `max` (opcionales) | — | `200` `Producto[]` | `400` (valores no numéricos) |
| GET | `/api/productos/ordenar` | Query `asc` (bool, default `true`) | — | `200` `Producto[]` | — |
| GET | `/api/productos/grupo-categoria` | — | — | `200` objeto `{ categoria: Producto[] }` | — |
| GET | `/api/productos/estadisticas` | — | — | `200` `{ total, precioMedio, porCategoria }` | — |

### Ejemplo de cuerpo (POST/PUT)

```json
{
  "nombre": "Monitor",
  "precio": 299.99,
  "categoria": "Electrónica",
  "imagen": "https://ejemplo.com/monitor.png"
}
```

### Modelo de respuesta

```json
{
  "id": 1,
  "nombre": "Monitor",
  "precio": 299.99,
  "categoria": "Electrónica",
  "imagen": null,
  "createdAt": "2026-09-23T12:00:00Z",
  "updatedAt": null,
  "deletedAt": null,
  "isActivo": true
}
```

> 📝 `id`, `createdAt` los asigna el servidor; `isActivo` es de solo lectura.

## Estructura

```
01-ProductosMinimalApi/
├── ProductosMinimalApi.slnx
├── ProductosMinimalApi/
│   ├── Program.cs
│   ├── Models/Producto.cs
│   └── Routes/ProductosRoutes.cs
├── automation/test-runner.mjs
└── README.md
```
