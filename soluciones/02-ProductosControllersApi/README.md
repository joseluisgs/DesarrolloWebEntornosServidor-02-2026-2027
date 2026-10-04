# 02 - Productos Controllers API

API REST de productos con **Controladores MVC** en ASP.NET Core (.NET 10).

## Descripción

Misma API que el ejemplo 01 (CRUD + consultas LINQ + eliminación lógica), pero implementada con el patrón **MVC**: un único `ProductosController` con atributos `[HttpGet]`, `[HttpPost]`, `[HttpPut]`, `[HttpPatch]` y `[HttpDelete]`, y prefijo de rutas `[Route("api/[controller]")]`.

- Almacenamiento en memoria (`Dictionary<long, Producto>`) aislado en `Repositories/ProductoRepository.cs`, creado en el propio controlador como campo `static` (**sin inyección de dependencias**)
- Modelo `Producto` como **`record` inmutable** (propiedades `init`): los cambios se hacen con `with`, sin anotaciones de base de datos
- Serialización JSON en `camelCase`
- Sin validación de negocio (solo errores de model binding → `400 ProblemDetails`)
- Sin Swagger ni Docker: solo SDK de .NET

## Ejecución manual

```bash
dotnet run --project ProductosControllersApi
```

Escucha en `http://localhost:5000`.

## Pruebas automatizadas

```bash
node automation/test-runner.mjs
```

El script (Node.js nativo) compila y arranca la API con `dotnet run`, espera su disponibilidad (máx. 20 s), ejecuta 15 tests HTTP (CRUD, consultas y errores) y detiene el proceso en `finally`.

- `exit 0` → todos los tests OK
- `exit 1` → algún test KO (se imprime endpoint, esperado vs obtenido y body)

> Sin `docker-compose.yml`: no hay base de datos externa.

## Endpoints

Prefijo: `http://localhost:5000/api/productos`

| Método | Ruta | Parámetros | Cuerpo de petición | Respuesta exitosa | Errores |
|--------|------|------------|--------------------|-------------------|---------|
| GET | `/api/productos` | ( | ) | `200` `Producto[]` |; |
| GET | `/api/productos/{id}` | `id` (long, ruta) | — | `200` `Producto` | `404` |
| POST | `/api/productos` | — | `{ "nombre", "precio", "categoria", "imagen"? }` | `201` `Producto` + `Location` | `400` (binding) |
| PUT | `/api/productos/{id}` | `id` (long, ruta) | `Producto` completo | `200` `Producto` | `404` |
| PATCH | `/api/productos/{id}` | `id` (long, ruta) | `{ "precio": number }` | `200` `Producto` | `400` (sin `precio`), `404` |
| DELETE | `/api/productos/{id}` | `id` (long, ruta) | — | `204` | `404` |
| GET | `/api/productos/search` | Query `nombre` (opcional) | ( | `200` `Producto[]` | ) |
| GET | `/api/productos/categoria/{categoria}` | `categoria` (ruta) | ( | `200` `Producto[]` | ) |
| GET | `/api/productos/precio` | Query `min`, `max` (opcionales) | — | `200` `Producto[]` | `400` (no numérico) |
| GET | `/api/productos/ordenar` | Query `asc` (bool, default `true`) | ( | `200` `Producto[]` | ) |
| GET | `/api/productos/grupo-categoria` | ( | ) | `200` `{ categoria: Producto[] }` | — |
| GET | `/api/productos/estadisticas` | ( | ) | `200` `{ total, precioMedio, porCategoria }` | — |

### Ejemplo de cuerpo (POST/PUT)

```json
{
  "nombre": "Monitor",
  "precio": 299.99,
  "categoria": "Electrónica",
  "imagen": null
}
```

### PATCH (precio)

```json
{ "precio": 99.99 }
```

## Estructura

```
02-ProductosControllersApi/
├── ProductosControllersApi.slnx
├── ProductosControllersApi/
│   ├── Program.cs
│   ├── Models/
│   │   ├── Producto.cs
│   │   └── ProductoEstadisticas.cs
│   ├── Repositories/
│   │   ├── IProductoRepository.cs
│   │   └── ProductoRepository.cs
│   └── Controllers/ProductosController.cs
├── automation/test-runner.mjs
└── README.md
```
