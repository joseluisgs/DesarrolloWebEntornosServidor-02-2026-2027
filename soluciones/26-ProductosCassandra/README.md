# 26 - Productos Cassandra (modelado por consultas)

API REST de productos sobre **Apache Cassandra** en ASP.NET Core (.NET 10), con la misma estructura MVC (Controller → Service → Repository) que el resto de ejemplos de la unidad. La lección no es la API: es que en Cassandra **el modelo sale de las consultas**, no al revés.

## Descripción

En un relacional diseñas entidades y el motor se busca la vida para responder cualquier pregunta. En Cassandra pasa lo contrario: cada tabla se diseña para una consulta concreta (query-first), y las consultas que no diseñaste simplemente no existen. Este ejemplo traduce cinco consultas de la tienda en cinco tablas, y expone cada una por su endpoint.

- Infra: Cassandra 5.0 (un nodo, `docker-compose.yml`)
- Sin ORM: driver nativo de DataStax (`CassandraCSharpDriver`), porque no hay proveedor de EF Core mantenido
- Esquema con `CREATE ... IF NOT EXISTS` (no hay migraciones)
- Sembrado de datos en Development
- Tests de integración con **Testcontainers**: un Cassandra real y efímero

## Las consultas y sus tablas

| Consulta | Descripción | Tabla que la atiende |
|----------|-------------|----------------------|
| **Q1** | Productos de la categoría 3 | Leer la partición `categoria_id` entera |
| **Q2** | Detalle de un producto por su id | Tabla auxiliar `productos_por_id` |
| **Q3** | Listado de categorías | Tabla simple `categorias` |
| **Q4** | Histórico de precios de un producto | Partición `producto_id`, con `CLUSTERING ORDER BY (fecha DESC)`: el orden, guardado |
| **Q5** | Qué precios cambiaron hoy (informe) | La partition key es el día: la partición tiene techo |

Y el esquema real (está en `Infrastructures/CassandraSchemaExtensions.cs`):

```sql
-- Q3: las categorías sueltas
CREATE TABLE categorias (
    id          bigint PRIMARY KEY,
    nombre      text,
    descripcion text
);

-- Q1: la consulta estrella. Partition key = categoria_id; clustering key = producto_id
CREATE TABLE productos_por_categoria (
    categoria_id      bigint,
    producto_id       bigint,
    nombre            text,
    precio            double,
    categoria_nombre  text,   -- denormalización: el nombre se copia en cada fila
    PRIMARY KEY (categoria_id, producto_id)
);

-- Q2: el producto suelto
CREATE TABLE productos_por_id (
    id                bigint PRIMARY KEY,
    nombre            text,
    precio            double,
    categoria_id      bigint,
    categoria_nombre  text
);

-- Q4: serie temporal. El orden temporal está GUARDADO en la tabla
CREATE TABLE historial_precios_por_producto (
    producto_id       bigint,
    fecha             timestamp,
    precio            double,
    precio_anterior   double,   -- la fila ya sabe cuánto cambió
    PRIMARY KEY (producto_id, fecha)
) WITH CLUSTERING ORDER BY (fecha DESC);

-- Q5: el informe por día. La partición ES el día: bucketing de serie
CREATE TABLE cambios_precio_por_fecha (
    fecha             timestamp,  -- el día completo, a medianoche (ver el truco del driver)
    producto_id       bigint,
    precio            double,
    PRIMARY KEY (fecha, producto_id)
);
```

## Ejecución manual

```bash
# 1. Infra (espera ~30-60 s: es una JVM)
docker compose up -d

# 2. API (Development para que siembre datos)
dotnet run --project ProductosCassandra
```

Escucha en `http://localhost:5080`. Swagger en `http://localhost:5080/swagger`.

## Tests de integración

```bash
dotnet test
```

Testcontainers levanta un Cassandra 5.0 real y efímero por ejecución (arranca la suite entera; los tres fixtures comparten el contenedor). **18 tests**: repositorio de productos, repositorio de categorías y capa de servicios.

## Endpoints

### Categorías — `api/Categorias`

| Método | Ruta | Consulta | Éxito | Errores |
|--------|------|----------|-------|---------|
| GET | `/api/Categorias` | Q3 | `200` lista | — |
| GET | `/api/Categorias/{id}` | — | `200` | `404` |
| GET | `/api/Categorias/{id}/productos` | **Q1** | `200` (partición entera, ordenada de serie) | — |

### Productos — `api/Productos`

| Método | Ruta | Consulta | Cuerpo | Éxito | Errores |
|--------|------|----------|--------|-------|---------|
| GET | `/api/Productos/{id}` | Q2 | — | `200` | `404` |
| GET | `/api/Productos/{id}/historial-precios` | **Q4** | — | `200` (del más reciente al más antiguo) | — |
| GET | `/api/Productos/precios-cambios/{fecha}` | **Q5** | — | `200` (informe del día) | — |
| POST | `/api/Productos` | escritura | `{ id, nombre, precio, categoriaId }` | `201` | `400`, `404` |
| PUT | `/api/Productos/{id}/precio` | escritura | `{ precio }` | `204` | `400`, `404` |
| DELETE | `/api/Productos/{id}?categoriaId=1` | escritura | — | `204` | — |

⚠️ No existe `GET /api/Productos` (listado general) **a propósito**: query-first significa que solo viven las consultas que el modelo prevé, y ninguna tabla devuelve el catálogo entero.

Cosas que verás al usar la API:

- El `POST` escribe en **las dos** tablas con el nombre de la categoría copiado; si la categoría no existe, `404` y no se escribe nada.
- El `PUT .../precio` es **una** petición que se convierte en **cuatro** escrituras (histórico, informe del día y precio actual en las dos tablas de lectura): la factura de la denormalización, hecha visible.
- El `DELETE` borra compensatoriamente en las dos tablas: sin transacciones repartidas, la compensación la orquesta la aplicación.
- El id lo pone el cliente en el `POST`: en Cassandra no hay autoincremento.

## Qué se ve en el código

- **Capas MVC como en el resto de la unidad**: `Controllers/` (solo traduce HTTP ↔ servicio), `Services/` (decisión de negocio: resolver la categoría, qué tablas toca cada operación), `Repositories/Cassandra/` (CQL puro, un método por consulta).
- **Una tabla por consulta**, sin claves foráneas y sin `JOIN`: la denormalización es forzada, el nombre de la categoría viaja copiado en cada fila.
- **Partition key y clustering key con su efecto real**: leer una categoría es una sola llamada, y los productos salen ordenados por `producto_id` de serie, sin ningún `ORDER BY`.
- **Series temporales**: histórico por producto con el orden DESC guardado en la propia tabla, y tabla de cambios por día cuya partition key es el día: bucketing de serie.
- **DTOs y mappers**: la capa de presentación (`Dtos/`, `Mappers/`) no ve los atributos `[Table]`/`[Column]` ni los nombres de columnas de CQL.

## Estructura

```
26-ProductosCassandra/
├── ProductosCassandra.slnx
├── docker-compose.yml
├── ProductosCassandra/
│   ├── Controllers/            # CategoriasController, ProductosController
│   ├── Services/               # ICategoriaService, IProductoService + implementaciones
│   ├── Repositories/
│   │   ├── Cassandra/          # ProductoCassandraRepository, CategoriaCassandraRepository
│   │   ├── ICategoriaRepository.cs
│   │   └── IProductoRepository.cs
│   ├── Dtos/                   # CreateProductoDto, CambioPrecioDto, respuestas...
│   ├── Mappers/ProductoMapper.cs
│   ├── Models/                 # Filas de tablas: ProductoPorCategoria, HistorialPrecio...
│   ├── Config/CassandraSettings.cs
│   ├── Infrastructures/
│   │   ├── CassandraConfig.cs            # sesión singleton del driver
│   │   ├── CassandraSchemaExtensions.cs  # CREATE ... IF NOT EXISTS (el "esquema")
│   │   ├── RepositoriesConfig.cs
│   │   └── ServicesConfig.cs
│   ├── Seed/CassandraSeeder.cs
│   └── Program.cs              # AddControllers + MapControllers; sin endpoints en línea
├── ProductosCassandra.Test/
│   ├── Repositories/           # tests del CQL contra el contenedor
│   ├── Services/               # tests de la capa de negocio sobre los repos reales
│   └── IntegrationTestBase.cs  # contenedor compartido + sembrado
└── README.md
```

## Paquetes NuGet

- `CassandraCSharpDriver` (driver nativo de DataStax)
- `Testcontainers.Cassandra` (Cassandra efímero para tests)
- Serilog + Swashbuckle (igual que el resto de ejemplos)

## Errores y trucos

- **El heap de la JVM**: sin techo, Cassandra pide varios GB y un portátil de clase se ahoga. El compose fija `MAX_HEAP_SIZE=1G` y `HEAP_NEWSIZE=256M`; los tests hacen lo propio con Testcontainers.
- **`date` con el driver de C#**: la columna de día de `cambios_precio_por_fecha` es `timestamp` a medianoche y no `date` de CQL, porque `date` se mapea a `DateOnly` y el driver 3.23 devuelve `Unknown Cassandra target type for CLR type System.DateOnly`. El patrón de bucket diario no cambia: la partición sigue siendo el día completo.
- **`decimal` y el driver**: CQL `decimal` se mapea a `Cassandra.BigDecimal`, incómodo de usar; por eso los precios van en `double`, que el driver serializa directo.
- **El precio vive en tres tablas**: `productos_por_id`, `productos_por_categoria` e histórico. Un cambio de precio son **cuatro escrituras**; el endpoint `PUT /productos/{id}/precio` las muestra todas en un sitio.
- **El contenedor de tests es compartido**: con tres fixtures, disposal del contenedor en el teardown del primero mata el compartido; solo se cierra la sesión por fixture y el contenedor lo reclama Ryuk al salir.
