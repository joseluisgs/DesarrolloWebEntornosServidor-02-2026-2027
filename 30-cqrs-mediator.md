- [30. CQRS y MediatR](#30-cqrs-y-mediatr)
  - [30.1. ¿Qué es CQRS?](#301-qué-es-cqrs)
    - [30.1.1. Nuestro dominio: Productos, Categorías y Proveedores](#3011-nuestro-dominio-productos-categorías-y-proveedores)
    - [30.1.2. El problema tradicional](#3012-el-problema-tradicional)
    - [30.1.3. La solución CQRS](#3013-la-solución-cqrs)
    - [30.1.4. ¿Por qué separar escrituras y lecturas?](#3014-por-qué-separar-escrituras-y-lecturas)
  - [30.2. Commands y Queries](#302-commands-y-queries)
    - [30.2.1. Commands (Escrituras)](#3021-commands-escrituras)
    - [30.2.2. Queries (Lecturas)](#3022-queries-lecturas)
  - [30.3. SQL para Escrituras](#303-sql-para-escrituras)
    - [30.3.1. Modelo normalizado](#3031-modelo-normalizado)
    - [30.3.2. Producto con Categoria y Proveedor](#3032-producto-con-categoria-y-proveedor)
  - [30.4. MongoDB para Lecturas](#304-mongodb-para-lecturas)
    - [30.4.1. Documentos denormalizados](#3041-documentos-denormalizados)
    - [30.4.2. Producto con relaciones embebidas](#3042-producto-con-relaciones-embebidas)
    - [30.4.3. Otras opciones para lecturas](#3043-otras-opciones-para-lecturas)
  - [30.5. Consistencia Eventual](#305-consistencia-eventual)
    - [30.5.1. Qué es la consistencia eventual](#3051-qué-es-la-consistencia-eventual)
    - [30.5.2. Ventana de inconsistencia](#3052-ventana-de-inconsistencia)
    - [30.5.3. Cómo manejarla](#3053-cómo-manejarla)
    - [30.5.4. Diagrama: Ciclo completo con timestamps](#3054-diagrama-ciclo-completo-con-timestamps)
    - [30.5.5. Diagrama: Lectura durante la ventana de inconsistencia](#3055-diagrama-lectura-durante-la-ventana-de-inconsistencia)
    - [30.5.6. Diagrama: Polling vs Domain Events](#3056-diagrama-polling-vs-domain-events)
    - [30.5.7. Diagrama: Manejo de errores en sincronización](#3057-diagrama-manejo-de-errores-en-sincronización)
  - [30.6. Sincronización SQL a MongoDB](#306-sincronización-sql-a-mongodb)
    - [30.6.1. El problema: ¿Qué pasa si sincronizamos TODO?](#3061-el-problema-qué-pasa-si-sincronizamos-todo)
    - [30.6.2. Enfoque 1: Sync Completo (NO recomendado)](#3062-enfoque-1-sync-completo-no-recomendado)
    - [30.6.3. Enfoque 2: Sync Incremental](#3063-enfoque-2-sync-incremental)
    - [30.6.4. Enfoque 3: Domain Events (Ideal)](#3064-enfoque-3-domain-events-ideal)
    - [30.6.5. Enfoque 4: CDC (Profesional)](#3065-enfoque-4-cdc-profesional)
    - [30.6.6. Enfoque 5: RX.NET con Observables](#3066-enfoque-5-rxnet-con-observables)
    - [30.6.7. Comparativa](#3067-comparativa)
  - [30.7. Ventajas y Desventajas](#307-ventajas-y-desventajas)
  - [30.8. Kafka: la opcion profesional](#308-kafka-la-opcion-profesional)
    - [30.8.1. ¿Qué es Kafka?](#3081-qué-es-kafka)
    - [30.8.2. ¿Qué es Debezium (CDC)?](#3082-qué-es-debezium-cdc)
    - [30.8.3. Flujo completo: PostgreSQL a Kafka a MongoDB](#3083-flujo-completo-postgresql-a-kafka-a-mongodb)
    - [30.8.4. Ventajas y desventajas de Kafka](#3084-ventajas-y-desventajas-de-kafka)
  - [30.9. MediatR: Implementando CQRS](#309-mediatr-implementando-cqrs)
    - [30.9.1. ¿Qué es MediatR?](#3091-qué-es-mediatr)
    - [30.9.2. Commands con MediatR](#3092-commands-con-mediatr)
    - [30.9.3. Queries con MediatR](#3093-queries-con-mediatr)
    - [30.9.4. Pipeline Behaviors](#3094-pipeline-behaviors)
    - [30.9.5. Diagrama: Flujo completo de Commands y Queries](#3095-diagrama-flujo-completo-de-commands-y-queries)
    - [30.9.6. GraphQL también pasa por MediatR](#3096-graphql-también-pasa-por-mediatr)
  - [30.10. Sincronización con Domain Events y MediatR](#3010-sincronización-con-domain-events-y-mediatr)
    - [30.10.1. La idea clave: ya tienes los datos en memoria](#30101-la-idea-clave-ya-tienes-los-datos-en-memoria)
    - [30.10.2. Código de ejemplo](#30102-código-de-ejemplo)
    - [30.10.3. Ventajas de este enfoque](#30103-ventajas-de-este-enfoque)
  - [30.11. Testing de CQRS](#3011-testing-de-cqrs)
  - [30.12. Buenas Prácticas](#3012-buenas-prácticas)
  - [30.13. Reto](#3013-reto)



# 30. CQRS y MediatR

> 💡 **Punto de partida:** En una tienda online, el administrador actualiza precios y stock (escritura), pero miles de clientes buscan productos (lectura). ¿Por qué usar la misma base de datos para las dos cosas si tienen necesidades tan distintas? **CQRS** separa las lecturas de las escrituras para optimizar cada una por separado.

En este punto aprenderás a separar Commands de Queries, usar PostgreSQL para escrituras y MongoDB para lecturas, y entender la consistencia eventual.

**Objetivos de aprendizaje:**
- Entender qué es CQRS y por qué se usa
- Separar Commands (escrituras) de Queries (lecturas)
- Usar PostgreSQL normalizado para escrituras y MongoDB denormalizado para lecturas
- Implementar sincronización entre ambas bases de datos
- Comprender la consistencia eventual y sus implicaciones

## 30.1. ¿Qué es CQRS?

**CQRS** (Command Query Responsibility Segregation) es un patrón de arquitectura que separa las operaciones de **lectura** (Queries) de las de **escritura** (Commands) en modelos diferentes.

📌 Ejemplo real: **Amazon** usa CQRS. Las escrituras van a una base de datos relacional optimizada para transacciones, pero las lecturas (búsqueda de productos, catálogos) van a un sistema optimizado para búsquedas rápidas como Elasticsearch. Cada una tiene su propia base de datos.

### 30.1.1. Nuestro dominio: Productos, Categorías y Proveedores

Vamos a usar un ejemplo concreto para entender CQRS. Imagina una tienda online con estos datos:

```mermaid
classDiagram
    class Producto {
        +long Id
        +string Nombre
        +decimal Precio
        +int Stock
        +long CategoriaId
        +long ProveedorId
        +DateTime CreatedAt
    }
    class Categoria {
        +long Id
        +string Nombre
        +string Descripcion
    }
    class Proveedor {
        +long Id
        +string Nombre
        +string Email
        +string Telefono
    }
    Producto --> Categoria : "categoriaId"
    Producto --> Proveedor : "proveedorId"
```

📌 Ejemplo real: **Mercado Libre** maneja millones de productos con categorías y proveedores. Cada búsqueda debe ser rápida, pero cada actualización debe ser segura y consistente.

**Modelo relacional en PostgreSQL (3 tablas):**

```mermaid
erDiagram
    PRODUCTOS {
        long Id PK
        string Nombre
        decimal Precio
        int Stock
        long CategoriaId FK
        long ProveedorId FK
    }
    CATEGORIAS {
        long Id PK
        string Nombre
        string Descripcion
    }
    PROVEEDORES {
        long Id PK
        string Nombre
        string Email
        string Telefono
    }
    PRODUCTOS }o--|| CATEGORIAS : "tiene"
    PRODUCTOS }o--|| PROVEEDORES : "proviene de"
```

**El problema: ¿Qué pasa cuando queremos devolver un DTO con toda la info?**

Cuando un cliente pide un producto, necesita ver el nombre de la categoría y el nombre del proveedor. Pero esos datos están en **tablas diferentes**. Para devolver un DTO como este:

```json
{
    "id": 1,
    "nombre": "Teclado Mecánico",
    "precio": 89.99,
    "categoria": { "id": 1, "nombre": "Electrónica" },
    "proveedor": { "id": 5, "nombre": "Logitech" }
}
```

PostgreSQL necesita leer **3 tablas**. Puede hacerlo en 3 consultas separadas... o, mejor, en **1 sola consulta con JOINs** (que es justo lo que hace EF Core con `Include`):

```sql
-- Opción A (ineficiente): 3 consultas separadas
SELECT * FROM Productos WHERE Id = 1;
SELECT * FROM Categorias WHERE Id = 1;
SELECT * FROM Proveedores WHERE Id = 5;

-- Opción B (la correcta): 1 consulta con JOINs
SELECT p.*, c.Nombre as CatNombre, pr.Nombre as ProvNombre
FROM Productos p
INNER JOIN Categorias c ON p.CategoriaId = c.Id
INNER JOIN Proveedores pr ON p.ProveedorId = pr.Id
WHERE p.Id = 1;
```

**¿Cuánto cuesta esto?**

| Operación dentro del SELECT | Tiempo estimado | Memoria |
|-----------------------------|----------------|---------|
| Buscar el producto | 1ms | Baja |
| JOIN con Categorias | +2ms | Media |
| JOIN con Proveedores | +2ms | Media |
| **Total (1 SELECT con 2 JOINs)** | **~5ms** | **Media** |

Con 100 productos, la consulta **sigue siendo 1 solo SELECT**, pero las filas a cruzar crecen:

| Productos en la lista | Filas cruzadas (aprox.) | Tiempo estimado |
|----------------|-------------|-----------------|
| 1 | 3 | ~5ms |
| 10 | 30 | ~20ms |
| 100 | 300 | ~100ms |
| 1000 | 3000 | ~500ms |

📌 Ejemplo real: **Amazon** tiene millones de productos. Si cada listado tuviera que cruzar 3 tablas con millones de filas, las consultas tardarían segundos en vez de milisegundos.

> ⚠️ **Advertencia — LINQ oculta el coste real:** Cuando usas LINQ con EF Core, el código parece sencillo:
> ```csharp
> var productos = await context.Productos
>     .Include(p => p.Categoria)
>     .Include(p => p.Proveedor)
>     .ToListAsync();
> ```
> Pero por debajo, EF Core genera **1 sola consulta SQL con 2 JOINs**. Es eficiente... pero el cruce de tablas sigue costando CPU y memoria en el servidor de BD: cuanto más datos, más tarda. El ORM oculta esa complejidad.

**¿Cómo se hacen estas consultas en LINQ?**

Hay 3 formas de cargar datos relacionados en EF Core:

**1. Eager Loading (Include):** Carga todo de golpe con JOINs
```csharp
var productos = await context.Productos
    .Include(p => p.Categoria)      // JOIN con Categorias
    .Include(p => p.Proveedor)      // JOIN con Proveedores
    .ToListAsync();                 // Genera 1 consulta con 2 JOINs
```

**2. Lazy Loading:** Carga cada relación bajo demanda (N+1 queries)
```csharp
var productos = await context.Productos.ToListAsync(); // 1 consulta
foreach (var p in productos)
{
    var cat = p.Categoria;  // Cada acceso = 1 consulta nueva
    var prov = p.Proveedor; // Cada acceso = 1 consulta nueva
}
// 1 + N + N = 2N+1 consultas
```

**3. Explicit Loading:** Carga manualmente bajo demanda
```csharp
var productos = await context.Productos.ToListAsync(); // 1 consulta
foreach (var p in productos)
{
    await context.Entry(p).Reference(x => x.Categoria).LoadAsync(); // 1 por producto
}
```

> 📝 **Nota:** Si tu DTO "monta" datos de 3 tablas (Producto + Categoria + Proveedor), **siempre** vas a leer de las 3 tablas. No hay forma de evitarlo. La única diferencia es cuándo y cómo se hacen esas lecturas. Por eso CQRS es útil: separar las lecturas (que necesitan JOINs) de las escrituras (que no los necesitan).

### 30.1.2. El problema tradicional

En una arquitectura tradicional (CRUD), la misma entidad sirve para leer y escribir. Esto crea varios problemas:

```mermaid
flowchart TD
    subgraph TRADICIONAL["CRUD Tradicional - Todo en una BD"]
        A[Cliente] -->|Lee| B[(PostgreSQL)]
        C[Admin] -->|Escribe| B
        D[Servidor] -->|Consultas complejas| B
    end

    subgraph PROBLEMAS["Problemas"]
        P1["Consultas lentas\nbloquean escrituras"]
        P2["Mismo esquema para\ntodo"]
        P3["No se puede escalar\ncada parte"]
    end

    TRADICIONAL --> PROBLEMAS

    style TRADICIONAL fill:#f44336,color:#fff
    style PROBLEMAS fill:#f44336,color:#fff
```

📌 Ejemplo real: **Netflix** tenía este problema. Sus consultas de catálogo (millones de series) eran lentas porque la misma BD manejaba escrituras de configuración. Al separar CQRS, las lecturas se volvieron 10x más rápidas.

| Problema | Impacto | Ejemplo |
|----------|---------|---------|
| **Consultas lentas** | Los clientes esperan segundos | Buscar "teclado" tarda 3s en vez de 200ms |
| **Mismo esquema** | Compromisos en diseño | No puedes denormalizar para lecturas sin afectar escrituras |
| **No escala** | Crecimiento limitado | No puedes añadir nodos de solo lectura |
| **Bloqueos** | Escrituras bloquean lecturas | Un UPDATE bloquea las consultas |

### 30.1.3. La solución CQRS

CQRS resuelve esto separando en dos modelos optimizados para cada caso:

```mermaid
flowchart TD
    subgraph CQRS["CQRS - Dos modelos separados"]
        A[Admin] -->|Command| B[(PostgreSQL\nEscrituras\nNormalizado)]
        B -->|Sincronizar| C[(MongoDB\nLecturas\nDenormalizado)]
        D[Cliente] -->|Query| C
    end

    subgraph VENTAJAS["Ventajas"]
        V1["Escrituras:\nIntegridad referencial"]
        V2["Lecturas:\nDocumentos anidados\nUltra-rápidas"]
    end

    CQRS --> VENTAJAS

    style CQRS fill:#4CAF50,color:#fff
    style VENTAJAS fill:#2196F3,color:#fff
```

📌 Ejemplo real: **LinkedIn** usa CQRS. Cuando actualizas tu perfil (escritura), va a PostgreSQL. Cuando alguien busca tu perfil (lectura), va a un sistema optimizado para búsquedas rápidas.

### 30.1.4. ¿Por qué separar escrituras y lecturas?

Para entender por qué CQRS es útil, necesitas comprender el **coste real** de cada operación:

#### El coste de las claves foráneas y los JOINs

En PostgreSQL normalizado, cuando consultas un producto con su categoría y proveedor, el motor de BD debe:

1. **Buscar el producto** en la tabla `Productos`
2. **Hacer JOIN** con `Categorias` para obtener el nombre
3. **Hacer JOIN** con `Proveedores` para obtener el nombre
4. **Cargar todo en memoria** para devolver el resultado

```sql
-- Consulta típica en PostgreSQL normalizado
SELECT p.Nombre, p.Precio, c.Nombre, pr.Nombre
FROM Productos p
INNER JOIN Categorias c ON p.CategoriaId = c.Id
INNER JOIN Proveedores pr ON p.ProveedorId = pr.Id
WHERE p.Precio > 50;
```

**Problemas:**
- Cada JOIN **multiplica** el tiempo de ejecución
- Con 3 tablas y 10.000 productos, la consulta puede tardar **50-100ms**
- Con millones de registros, puede tardar **segundos**
- Cada JOIN consume **memoria** en el servidor de BD

#### El coste de tener dos bases de datos

CQRS añade complejidad:

| Coste | Descripción |
|-------|-------------|
| **Mantenimiento** | Dos sistemas que actualizar |
| **Sincronización** | BackgroundService adicional |
| **Consistencia eventual** | Los datos no están al instante sincronizados |
| **Operaciones** | Dos servidores, dos backups, dos monitoreos |
| **Coste económico** | Dos servidores = el doble de coste |

#### ¿Cuándo merece la pena?

| Escenario | CQRS | Por qué |
|-----------|------|---------|
| **API con miles de lecturas por escritura** | ✅ Sí | Las lecturas se vuelven 10x más rápidas |
| **Consultas complejas con muchos JOINs** | ✅ Sí | MongoDB resuelve todo en un documento |
| **Escalabilidad independiente** | ✅ Sí | Puedes añadir nodos de solo lectura |
| **API simple, pocos usuarios** | ❌ No | La complejidad no compensa |
| **Datos que cambian muy rápido** | ⚠️ Dependiendo | La consistencia puede ser un problema |
| **Cache ya resuelve el problema** | ⚠️ Dependiendo | Redis puede ser suficiente |

📌 Ejemplo real: **Amazon** recibe millones de lecturas (buscar productos) por cada escritura (actualizar precio). CQRS les permite escalar las lecturas independientemente de las escrituras.

#### Comparativa detallada

```mermaid
flowchart TD
    subgraph SOLO_SQL["Solo PostgreSQL (CRUD)"]
        S1["Producto → Categoria → Proveedor"]
        S2["3 JOINs por consulta"]
        S3["50-100ms por consulta"]
        S4["Escalable horizontalmente"]
    end

    subgraph CQRS["CQRS: SQL + MongoDB"]
        C1["Escritura: PostgreSQL normalizado"]
        C2["Lectura: MongoDB denormalizado"]
        C3["0 JOINs (datos embebidos)"]
        C4["1-5ms por consulta"]
    end

    SOLO_SQL -->|"Problema: lento con muchos datos"| CQRS

    style SOLO_SQL fill:#f44336,color:#fff
    style CQRS fill:#4CAF50,color:#fff
```

> 📝 **Nota:** CQRS no es la solución para todo. Si tu API tiene 100 usuarios y 50 productos, la complejidad adicional no compensa. CQRS brilla cuando tienes **miles de lecturas por escritura** y necesitas **escalar las lecturas** independientemente.

## 30.2. Commands y Queries

### 30.2.1. Commands (Escrituras)

Un **Command** es una operación que modifica el estado del sistema. Siempre devuelve `true/false` o el objeto creado.

| Command | Qué hace | Ejemplo HTTP |
|---------|----------|--------------|
| `CreateProducto` | Crea un nuevo producto | POST /api/productos |
| `UpdateProducto` | Actualiza un producto existente | PUT /api/productos/1 |
| `DeleteProducto` | Elimina un producto | DELETE /api/productos/1 |

```csharp
// Command: crear producto
public record CreateProductoCommand(
    string Nombre,
    decimal Precio,
    long CategoriaId,
    long ProveedorId
);
```

### 30.2.2. Queries (Lecturas)

Una **Query** es una operación que lee datos sin modificarlos. Devuelve el objeto o una lista.

| Query | Qué hace | Ejemplo HTTP |
|-------|----------|--------------|
| `GetProductoById` | Obtiene un producto por ID | GET /api/productos/1 |
| `GetAllProductos` | Lista todos los productos | GET /api/productos |
| `SearchProductos` | Busca productos | GET /api/productos/search?q=teclado |

```csharp
// Query: buscar productos
public record SearchProductosQuery(string Termino);
```

## 30.3. SQL para Escrituras

### 30.3.1. Modelo normalizado

Las escrituras usan PostgreSQL **normalizado** para garantizar consistencia e integridad referencial:

```mermaid
erDiagram
    PRODUCTOS {
        long Id PK
        string Nombre
        decimal Precio
        long CategoriaId FK
        long ProveedorId FK
    }
    CATEGORIAS {
        long Id PK
        string Nombre
    }
    PROVEEDORES {
        long Id PK
        string Nombre
    }
    PRODUCTOS }o--|| CATEGORIAS : "tiene"
    PRODUCTOS }o--|| PROVEEDORES : "proviene de"
```

📌 Ejemplo real: **Amazon** almacena productos, categorías y proveedores en tablas separadas normalizadas. Cuando un proveedor cambia de nombre, solo se actualiza una fila en la tabla `Proveedores`.

### 30.3.2. Producto con Categoria y Proveedor

```csharp
// Modelo normalizado en PostgreSQL
public class Producto
{
    public long Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public long CategoriaId { get; set; }
    public Categoria Categoria { get; set; } = null!;
    public long ProveedorId { get; set; }
    public Proveedor Proveedor { get; set; } = null!;
}

public class Categoria
{
    public long Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
}

public class Proveedor
{
    public long Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
}
```

❌ **MALO**: Guardar el nombre de la categoría directamente en el producto:

```csharp
public class Producto
{
    public string NombreCategoria { get; set; } = string.Empty; // Duplicado
}
// Si cambia el nombre de la categoría, hay que actualizar TODOS los productos
```

✅ **BUENO**: Usar claves foráneas y relaciones:

```csharp
public class Producto
{
    public long CategoriaId { get; set; } // Referencia, no duplicado
    public Categoria Categoria { get; set; } = null!;
}
```

## 30.4. MongoDB para Lecturas

### 30.4.1. Documentos denormalizados

Las lecturas usan MongoDB **desnormalizado** con documentos que contienen toda la información anidada:

```mermaid
erDiagram
    PRODUCTOS_READ {
        long Id
        string Nombre
        decimal Precio
        Categoria Categoria
        Proveedor Proveedor
    }
    CATEGORIA {
        string Nombre
        string Descripcion
    }
    PROVEEDOR {
        string Nombre
        string Email
    }
    PRODUCTOS_READ ||--|| CATEGORIA : "embebida"
    PRODUCTOS_READ ||--|| PROVEEDOR : "embebida"
```

📌 Ejemplo real: **Instagram** almacena posts con el perfil del usuario embebido. Cuando ves un post, no necesitas hacer JOIN con la tabla de usuarios: todo está en el mismo documento.

### 30.4.2. Producto con relaciones embebidas

```csharp
// Modelo denormalizado en MongoDB
public class ProductoRead
{
    public long Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public CategoriaRead Categoria { get; set; } = null!;
    public ProveedorRead Proveedor { get; set; } = null!;
    public DateTime SyncAt { get; set; } // Última sincronización
}

public class CategoriaRead
{
    public long Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
}

public class ProveedorRead
{
    public long Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
}
```

```json
{
    "_id": ObjectId("..."),
    "id": 1,
    "nombre": "Teclado Mecánico",
    "precio": 89.99,
    "categoria": {
        "id": 1,
        "nombre": "Electrónica"
    },
    "proveedor": {
        "id": 5,
        "nombre": "Logitech"
    },
    "syncAt": "2026-09-22T10:00:00Z"
}
```

### 30.4.3. Otras opciones para lecturas

MongoDB es solo **una opción**. También puedes usar:

| Opción | Ventaja | Cuándo usar |
|--------|---------|-------------|
| **MongoDB** | Documentos flexibles, búsquedas rápidas | Datos anidados, catálogos |
| **Elasticsearch** | Búsquedas full-text avanzadas | Búsqueda de texto libre |
| **Redis** | Ultra-rápido, caché | Datos que cambian poco |
| **Vistas materializadas SQL** | Simple, misma BD | Consultas precalculadas |
| **Apache Solr** | Búsquedas complejas | E-commerce grande |

📌 Ejemplo real: **Netflix** usa MongoDB para el catálogo de contenido (documentos anidados con temporadas, episodios, actores) pero Elasticsearch para las búsquedas de texto.

## 30.5. Consistencia Eventual

### 30.5.1. Qué es la consistencia eventual

La **consistencia eventual** significa que después de una escritura, las lecturas no reflejan el cambio **inmediatamente**. Hay una **ventana de inconsistencia** donde los datos están desincronizados.

📌 Ejemplo real: Cuando publicas una foto en **Instagram**, tus seguidores no la ven al instante. Hay un pequeño retraso (segundos o minutos) mientras el sistema sincroniza los datos entre servidores. Eso es consistencia eventual.

### 30.5.2. Ventana de inconsistencia

El **SyncService** es el componente que se encarga de sincronizar datos entre PostgreSQL y MongoDB. Puede funcionar de diferentes maneras:

| Tipo de SyncService | Cómo funciona | Ejemplo |
|---------------------|---------------|---------|
| **Polling** | Consulta cada X tiempo | `Task.Delay(60s)` |
| **Domain Events** | Escucha cada evento de cambio | `OnProductoCreado` |
| **CDC** | Lee el log de la BD | Debezium + Kafka |
| **RX.NET** | Observable reactivo | `Subject<T>.Throttle(30s)` |

```mermaid
sequenceDiagram
    participant Admin as Admin
    participant SQL as PostgreSQL
    participant Sync as SyncService
    participant Mongo as MongoDB
    participant Cliente as Cliente

    Note over Admin: t0: Crea producto
    Admin->>SQL: INSERT INTO Productos
    SQL-->>SQL: Confirmado

    Note over Cliente: t1: Consulta producto
    Cliente->>Mongo: SELECT (no existe aun)
    Mongo-->>Cliente: null

    Note over Sync: t2: SyncService detecta cambio
    Sync->>SQL: SELECT producto
    SQL-->>Sync: Producto nuevo
    Sync->>Mongo: INSERT INTO productos_read
    Mongo-->>Sync: OK

    Note over Cliente: t3: Consulta producto
    Cliente->>Mongo: SELECT (ya existe)
    Mongo-->>Cliente: Producto completo

    Note over Cliente: Ventana de inconsistencia: t0 → t3
```

### 30.5.3. Cómo manejarla

| Estrategia | Descripción |
|------------|-------------|
| **Mostrar timestamp** | Indicar "datos actualizados hace X min" |
| **Forzar refresh** | Botón para recargar datos manualmente |
| **Sync más rápido** | Usar Domain Events en vez de polling |
| **Cache corto** | TTL bajo en la capa de lectura |
| **Aceptar la latencia** | Si la latencia es aceptable, no hacer nada |

❌ **MALO**: Mentir al usuario diciendo que los datos están actualizados:

```csharp
// Si el sync tarda, el usuario cree que el cambio fue inmediato
return Ok(new { message = "Producto creado correctamente" });
```

✅ **BUENO**: Informar sobre la sincronización:

```csharp
return Ok(new { 
    message = "Producto creado correctamente",
    syncNote = "Los cambios serán visibles en breve"
});
```

### 30.5.4. Diagrama: Ciclo completo con timestamps

Este diagrama muestra el **ciclo de vida completo** de una operación CQRS con consistencia eventual, marcando cada instante de tiempo para que veas exactamente **dónde** ocurre la latencia:

```mermaid
sequenceDiagram
    autonumber
    participant Admin as Administrador
    participant API as API (Controller)
    participant WriteBD as PostgreSQL (Escritura)
    participant Sync as SyncService
    participant ReadBD as MongoDB (Lectura)
    participant Cliente as Cliente

    Note over Admin,Cliente: t=0s — El admin crea un producto

    Admin->>API: POST /api/productos
    API->>WriteBD: INSERT INTO Productos
    WriteBD-->>API: OK (id=42)
    API-->>Admin: 201 Created {id: 42}

    Note over Admin,Cliente: t=0.5s — Un cliente consulta el producto nuevo

    Cliente->>ReadBD: GET /api/productos/42
    ReadBD-->>Cliente: 404 Not Found ❌ (aún no existe)

    Note over Admin,Cliente: t=2s — El SyncService detecta el cambio

    Sync->>WriteBD: SELECT producto 42 con JOINs
    WriteBD-->>Sync: Producto + Categoria + Proveedor
    Sync->>ReadBD: ReplaceOne (upsert)
    ReadBD-->>Sync: OK

    Note over Admin,Cliente: t=2.5s — El cliente vuelve a consultar

    Cliente->>ReadBD: GET /api/productos/42
    ReadBD-->>Cliente: 200 OK ✅ (producto completo)

    Note over Cliente: Ventana de inconsistencia: t=0s a t=2s (2 segundos)
```

📌 Ejemplo real: **Instagram** tiene esta misma ventana. Cuando publicas una foto, tus seguidores no la ven al instante. En 2-5 segundos, los servidores de lectura se actualizan y la foto aparece.

**¿Qué Determina el Tamaño de la Ventana?**

| Factor | Ventana corta (< 1s) | Ventana larga (> 5min) |
|--------|----------------------|------------------------|
| **Mecanismo** | Domain Events o CDC | Polling cada 5 min |
| **Carga del sistema** | Baja | Alta (muchos registros) |
| **Latencia de red** | Baja (mismos servidores) | Alta (servidores lejanos) |
| ** Complejidad** | Media-Alta | Baja |

### 30.5.5. Diagrama: Lectura durante la ventana de inconsistencia

Este diagrama muestra **qué pasa exactamente** cuando un cliente lee datos **dentro** de la ventana de inconsistencia, y cómo el sistema maneja esa situación:

```mermaid
sequenceDiagram
    autonumber
    participant Admin as Administrador
    participant WriteBD as PostgreSQL
    participant Sync as SyncService
    participant ReadBD as MongoDB
    participant Cliente1 as Cliente A (lento)
    participant Cliente2 as Cliente B (rápido)

    Note over Admin,Cliente2: Escenario: Dos clientes consultan el mismo producto

    Admin->>WriteBD: UPDATE precio: 89.99 → 99.99
    WriteBD-->>Admin: OK (precio actualizado)

    Note over Cliente1: Cliente A consulta INMEDIATAMENTE

    Cliente1->>ReadBD: GET /api/productos/42
    ReadBD-->>Cliente1: 200 OK {precio: 89.99} ⚠️ (precio viejo)

    Note over Sync: SyncService detecta cambio (2s después)

    Sync->>WriteBD: SELECT producto 42
    WriteBD-->>Sync: {precio: 99.99}
    Sync->>ReadBD: ReplaceOne {precio: 99.99}
    ReadBD-->>Sync: OK

    Note over Cliente2: Cliente B consulta DESPUÉS del sync

    Cliente2->>ReadBD: GET /api/productos/42
    ReadBD-->>Cliente2: 200 OK {precio: 99.99} ✅ (precio nuevo)

    Note over Cliente1,Cliente2: Cliente A vio precio viejo, Cliente B vio precio nuevo
    Note over Cliente1,Cliente2: Ambos son válidos: consistencia eventual
```

📌 Ejemplo real: **Amazon** maneja esto mostrando "precio puede variar según el vendedor". Cuando un vendedor baja un precio, algunos usuarios ven el precio viejo unos segundos. No es un error: es consistencia eventual.

**¿Qué Hace la API con Datos Potencialmente Obsoletos?**

```csharp
// Opción 1: Mostrar timestamp de última sync
public record ProductoReadDto(
    long Id,
    string Nombre,
    decimal Precio,
    DateTime SyncAt  // "Última actualización: hace 30 segundos"
);

// Opción 2: Aceptar la ventana y documentarla
// En la documentación de la API:
// "Los datos de lectura pueden tener una latencia de hasta 5 segundos
//  respecto a las escrituras"
```

### 30.5.6. Diagrama: Polling vs Domain Events

Compara visualmente **cuánto tiempo** tarda cada enfoque en sincronizar. La diferencia es abismal:

```mermaid
sequenceDiagram
    autonumber
    participant Admin as Administrador
    participant WriteBD as PostgreSQL
    participant Polling as Polling (BackgroundService)
    participant Events as Domain Events (MediatR)
    participant ReadBD as MongoDB
    participant Cliente as Cliente

    Note over Admin,Cliente: PARTE 1: Polling cada 60 segundos

    Admin->>WriteBD: INSERT producto (t=0s)
    WriteBD-->>Admin: OK

    Note over Polling: Esperando... 58 segundos más

    Polling->>WriteBD: SELECT * WHERE UpdatedAt > lastSync
    WriteBD-->>Polling: 3 productos cambiados
    Polling->>ReadBD: ReplaceOne × 3
    ReadBD-->>Polling: OK

    Note over Cliente: t=60s: Cliente finalmente ve el producto

    Note over Admin,Cliente: PARTE 2: Domain Events (instantáneo)

    Admin->>WriteBD: INSERT producto (t=0s)
    WriteBD-->>Admin: OK

    Note over Events: ¡Evento publicado INMEDIATAMENTE!

    Events->>ReadBD: ReplaceOne (upsert)
    ReadBD-->>Events: OK

    Note over Cliente: t=0.5s: Cliente ve el producto casi al instante
```

| Métrica | Polling (60s) | Domain Events |
|---------|---------------|---------------|
| **Latencia máxima** | 60 segundos | < 1 segundo |
| **Latencia promedio** | 30 segundos | ~0.5 segundos |
| **CPU extra en SQL** | Consulta cada 60s | Solo cuando hay cambio |
| **Complejidad** | Baja | Media |
| **¿Cuándo usarlo?** | Datos que cambian poco, poca latencia aceptable | Datos en tiempo real, UX crítica |

📌 Ejemplo real: **Twitter/X** usa Domain Events. Cuando publicas un tweet, aparece en el timeline de tus seguidores en menos de 1 segundo. Con polling, tardaría minutos.

### 30.5.7. Diagrama: Manejo de errores en sincronización

¿Qué pasa si la sincronización **falla**? Este diagrama muestra el flujo de reintentos y cómo se recupera el sistema:

```mermaid
sequenceDiagram
    autonumber
    participant Admin as Administrador
    participant WriteBD as PostgreSQL
    participant Sync as SyncService
    participant ReadBD as MongoDB
    participant Log as Logs

    Admin->>WriteBD: INSERT producto
    WriteBD-->>Admin: OK

    Note over Sync: Intento 1: Sync falla (MongoDB caído)

    Sync->>ReadBD: ReplaceOne
    ReadBD-->>Sync: ❌ TimeoutException

    Sync->>Log: Registrar error: "Sync falló para producto 42"
    Note over Sync: Esperar 5 segundos (backoff)

    Note over Sync: Intento 2: Reintentar

    Sync->>ReadBD: ReplaceOne
    ReadBD-->>Sync: ❌ TimeoutException (sigue caído)

    Sync->>Log: Registrar error: "Reintento 2 falló para producto 42"
    Note over Sync: Esperar 15 segundos (backoff exponencial)

    Note over Sync: Intento 3: MongoDB se recupera

    Sync->>ReadBD: ReplaceOne
    ReadBD-->>Sync: ✅ OK

    Sync->>Log: Registrar éxito: "Sync completado para producto 42"
```

📌 Ejemplo real: **Netflix** usa reintentos con backoff exponencial para sincronizar datos entre centros de datos. Si un centro está caído, reintenta automáticamente sin perder datos.

**Estrategias de Recuperación:**

| Estrategia | Descripción | Cuándo usar |
|------------|-------------|-------------|
| **Reintento simple** | Reintentar N veces con delay fijo | Errores transitorios (red) |
| **Backoff exponencial** | Delay crece: 5s, 15s, 45s... | Sobrecarga temporal |
| **Dead Letter Queue** | Eventos fallidos van a una cola para revisar después | Errores persistentes |
| **Idempotencia** | Reintentar sin crear duplicados | Siempre (buena práctica) |

```csharp
// Ejemplo de reintento con backoff exponencial
public class SyncProductoHandler(
    MongoDbContext mongoContext,
    ILogger<SyncProductoHandler> logger) : INotificationHandler<ProductoCreadoEvent>
{
    private const int MaxRetries = 3;

    public async Task Handle(ProductoCreadoEvent notification, CancellationToken ct)
    {
        for (int intento = 1; intento <= MaxRetries; intento++)
        {
            try
            {
                var read = MapToReadModel(notification.Producto);
                await mongoContext.ProductosRead.ReplaceOneAsync(
                    p => p.Id == notification.Producto.Id,
                    read,
                    new ReplaceOptions { IsUpsert = true }, ct);
                return; // Éxito
            }
            catch (Exception ex) when (intento < MaxRetries)
            {
                var delay = TimeSpan.FromSeconds(5 * intento); // 5s, 10s, 15s
                logger.LogWarning(ex,
                    "Sync falló (intento {Intento}/{Max}), retry en {Delay}s",
                    intento, MaxRetries, delay.TotalSeconds);
                await Task.Delay(delay, ct);
            }
        }
        // Si agota reintentos: log error crítico (no perder el dato)
        logger.LogError("Sync agotó reintentos para producto {Id}",
            notification.Producto.Id);
    }
}
```

## 30.6. Sincronización SQL a MongoDB

El objetivo de la sincronización es **reducir la latencia** de la consistencia eventual. Cuanto más rápido sincronicemos, menor será la ventana de inconsistencia.

La sincronización es el corazón de CQRS. Sin ella, las lecturas mostrarían datos obsoletos.

### 30.6.1. El problema: ¿Qué pasa si sincronizamos TODO?

Si cada minuto leemos **todos** los productos de PostgreSQL, hacemos JOINs con Categorías y Proveedores, y reescribimos todo en MongoDB...

```mermaid
flowchart TD
    subgraph PROBLEMA["Problema: Sync completo cada minuto"]
        A["10 millones de productos"] --> B["SELECT * FROM Productos\n+ JOIN Categorias\n+ JOIN Proveedores"]
        B --> C["10 millones de operaciones"]
        C --> D["ReplaceOne en MongoDB\npor cada producto"]
    end

    subgraph CONSECUENCIAS["Consecuencias"]
        E["PostgreSQL: Carga extrema\nde CPU y memoria"]
        F["MongoDB: Sobrecarga\nde escrituras"]
        G["Red: Tráfico masivo\nentre servidores"]
    end

    PROBLEMA --> CONSECUENCIAS

    style PROBLEMA fill:#f44336,color:#fff
    style CONSECUENCIAS fill:#f44336,color:#fff
```

📌 Ejemplo real: Si **Amazon** sincronizara todos sus millones de productos cada minuto, sus servidores de BD colapsarían en segundos.

| Consecuencia | Impacto |
|--------------|---------|
| **CPU SQL** | 100% durante la sync |
| **Memoria SQL** | Carga millones de registros en RAM |
| **CPU MongoDB** | Sobrecarga de escrituras |
| **Red** | Tráfico masivo entre servidores |
| **Latencia** | La API se ralentiza durante la sync |

### 30.6.2. Enfoque 1: Sync Completo (NO recomendado)

Lee **todos** los registros, los transforma y los reescribe en MongoDB.

**Algoritmo:**
```
1. Cada 60 segundos:
   a. SELECT * FROM Productos (todos)
   b. JOIN con Categorias (para cada producto)
   c. JOIN con Proveedores (para cada producto)
   d. Para CADA producto (10 millones):
      - Crear documento denormalizado
      - ReplaceOne en MongoDB (upsert)
   e. Guardar timestamp de sync
```

```mermaid
flowchart LR
    A["PostgreSQL\n10M registros"] -->|"Lee TODO"| B["Transformar"]
    B -->|"10M operaciones"| C["MongoDB"]

    style A fill:#f44336,color:#fff
    style C fill:#f44336,color:#fff
```

| Pros | Contras |
|------|---------|
| Simple de implementar | Destroza SQL y MongoDB |
| Siempre consistente | No escala con millones de registros |
| Sin código adicional | Latencia de 1-5 minutos |

### 30.6.3. Enfoque 2: Sync Incremental

Solo sincroniza los registros que **cambiaron** desde la última sync. Usa el campo `UpdatedAt` como marca de agua.

**Algoritmo:**
```
1. Leer LastSyncTime de MongoDB
2. SELECT * FROM Productos WHERE UpdatedAt > LastSyncTime
3. Para CADA producto cambiado (ej: 3 de 10M):
   a. Crear documento denormalizado
   b. ReplaceOne en MongoDB (upsert)
4. Actualizar LastSyncTime en MongoDB
```

```mermaid
flowchart LR
    A["PostgreSQL\n10M registros"] -->|"Lee SOLO los que\ncambiaron"| B["3 registros"]
    B -->|"3 operaciones"| C["MongoDB"]

    style A fill:#4CAF50,color:#fff
    style B fill:#4CAF50,color:#fff
    style C fill:#4CAF50,color:#fff
```

| Pros | Contras |
|------|---------|
| Rápido (solo 3 ops en vez de 10M) | Sigue siendo polling |
| Eficiente en recursos | Latencia de 1 minuto |
| Escala con millones de registros | Si nadie cambia nada, desperdicia una consulta |

### 30.6.4. Enfoque 3: Domain Events (Ideal)

Cuando se crea/modifica/borra, se publica un evento. No hay polling.

**Algoritmo:**
```
1. Admin crea producto → Handler ejecuta Create
2. Handler publica evento "ProductoCreado" con el ID
3. SyncHandler escucha el evento
4. SyncHandler lee SOLO ese producto de PostgreSQL (con JOINs)
5. SyncHandler crea documento denormalizado
6. SyncHandler escribe en MongoDB (1 operación)
```

```mermaid
flowchart LR
    A["Admin crea producto"] -->|"Publica evento"| B["Evento\nProductoCreado"]
    B -->|"Escucha"| C["SyncHandler"]
    C -->|"1 operación"| D["MongoDB"]

    style A fill:#2196F3,color:#fff
    style B fill:#FF9800,color:#fff
    style C fill:#9C27B0,color:#fff
    style D fill:#4CAF50,color:#fff
```

| Pros | Contras |
|------|---------|
| Tiempo real (segundos) | Más código |
| No desperdicia recursos | Necesita patrón de eventos |
| Escala perfectamente | Complejidad adicional |

### 30.6.5. Enfoque 4: CDC (Profesional)

Change Data Capture captura cambios a nivel de base de datos con Kafka + Debezium.

**Algoritmo:**
```
1. Admin crea producto → PostgreSQL escribe en WAL (Write-Ahead Log)
2. Debezium lee el WAL en tiempo real
3. Debezium publica evento a Kafka
4. Consumer escucha Kafka
5. Consumer lee el producto de PostgreSQL (con JOINs)
6. Consumer escribe en MongoDB (1 operación)
```

```mermaid
sequenceDiagram
    participant Admin as Admin
    participant SQL as PostgreSQL
    participant WAL as WAL (Log)
    participant CDC as Debezium
    participant Kafka as Kafka
    participant Consumer as Sync Service
    participant Mongo as MongoDB

    Admin->>SQL: INSERT INTO Productos
    SQL->>WAL: Escribir cambio
    WAL-->>CDC: Notificar cambio
    CDC->>Kafka: Publicar evento
    Kafka-->>Consumer: Consumir evento
    Consumer->>SQL: SELECT producto con JOINs
    SQL-->>Consumer: Producto denormalizado
    Consumer->>Mongo: ReplaceOne (upsert)
```

| Pros | Contras |
|------|---------|
| Latencia < 1s | Necesita Kafka + Debezium |
| Escalable | Complejo de configurar |
| No necesita código | Coste de infraestructura |

### 30.6.6. Enfoque 5: RX.NET con Observables

Usa programación reactiva para escuchar cambios en tiempo real. Cuando se produce un cambio, se notifica inmediatamente.

**Algoritmo:**
```
1. ProductoService crea/modifica/borra producto
2. ProductoService publica cambio en Subject<Producto>
3. ReactiveSyncService escucha el Subject
4. Throttle de 30 segundos (esperar 30s de calma)
5. Cuando hay cambio, lee ese producto de PostgreSQL (con JOINs)
6. Crea documento denormalizado
7. Escribe en MongoDB (1 operación)
```

```mermaid
flowchart LR
    A["ProductoService"] -->|"OnNext(producto)"| B["Subject<Producto>"]
    B -->|"Throttle 30s"| C["ReactiveSyncService"]
    C -->|"Lee 1 producto"| D["PostgreSQL"]
    D -->|"1 operación"| E["MongoDB"]

    style A fill:#2196F3,color:#fff
    style B fill:#FF9800,color:#fff
    style C fill:#9C27B0,color:#fff
    style D fill:#f44336,color:#fff
    style E fill:#4CAF50,color:#fff
```

| Pros | Contras |
|------|---------|
| Reactivo (no polling) | Más complejo de entender |
| Latencia baja | Necesita programación reactiva |
| Throttle agrupa cambios | Más código que Domain Events |

### 30.6.7. Comparativa

| Criterio | Sync Completo | Sync Incremental | Domain Events | CDC | RX.NET |
|----------|---------------|------------------|---------------|-----|--------|
| **Eficiencia** | Mala | Buena | Excelente | Excelente | Excelente |
| **Latencia** | 1-5 min | 1 min | Segundos | < 1s | Segundos |
| **Escalabilidad** | No escala | Escala | Escala | Escala mucho | Escala |
| **Complejidad** | Baja | Baja | Media | Alta | Media |
| **Coste** | Alto (recursos) | Bajo | Bajo | Medio | Bajo |

> 📝 **Nota:** Cada enfoque tiene sus ventajas y desventajas. Elige según la escala y complejidad de tu aplicación.

## 30.7. Ventajas y Desventajas

| Ventaja | Desventaja |
|---------|------------|
| Lecturas ultra-rápidas | Complejidad de dos BDs |
| Escrituras optimizadas | Consistencia eventual |
| Cada parte escala por separado | Sincronización adicional |
| Modelos optimizados para cada caso | Más código y mantenimiento |

## 30.8. Kafka: la opción profesional

**Apache Kafka** es un sistema de streaming de eventos usado en producción para sincronizar datos entre sistemas. En lugar de polling, Kafka recibe eventos de cambios y los propaga a los consumidores.

### 30.8.1. ¿Qué es Kafka?

Kafka es como una **cola de mensajes distribuida**. Cuando algo cambia en PostgreSQL, Kafka recibe un mensaje y lo propaga a todos los sistemas que estén escuchando.

```mermaid
flowchart LR
    subgraph PRODUCIDOR["Productor"]
        A[PostgreSQL] -->|"Cambio detectado"| B[Kafka Producer]
    end

    subgraph KAFKA["Kafka (Broker)"]
        B --> C[Topic: productos-changes]
        C --> D[Partición 1]
        C --> E[Partición 2]
        C --> F[Partición N]
    end

    subgraph CONSUMIDORES["Consumidores"]
        D --> G[MongoDB Sync]
        D --> H[ElasticSearch]
        D --> I[Cache Redis]
    end

    style PRODUCIDOR fill:#2196F3,color:#fff
    style KAFKA fill:#FF9800,color:#fff
    style CONSUMIDORES fill:#4CAF50,color:#fff
```

📌 Ejemplo real: **LinkedIn** usa Kafka para sincronizar datos entre cientos de microservicios. Cuando actualizas tu perfil, el evento viaja por Kafka y actualiza ElasticSearch, caches y sistemas de recomendación.

### 30.8.2. ¿Qué es Debezium (CDC)?

**Debezium** es una herramienta de **Change Data Capture** que lee el **WAL** (Write-Ahead Log) de PostgreSQL en tiempo real.

**¿Qué es el WAL?** Es el log de transacciones de PostgreSQL. Cada vez que modificas datos, PostgreSQL escribe primero el cambio en el WAL (para seguridad) y luego lo aplica a las tablas. Debezium lee ese WAL.

```mermaid
sequenceDiagram
    participant Admin as Admin
    participant SQL as PostgreSQL
    participant WAL as WAL (Log)
    participant CDC as Debezium
    participant Kafka as Kafka

    Admin->>SQL: UPDATE Productos SET precio = 90
    SQL->>WAL: Registrar cambio
    SQL->>SQL: Aplicar cambio en tabla
    WAL-->>CDC: Notificar: "precio cambió de 89 a 90"
    CDC->>Kafka: Publicar evento
```

📌 Ejemplo real: **Amazon** usa Debezium para capturar cambios en sus bases de datos de productos. Cada vez que un vendedor actualiza un precio, Debezium lo detecta y propaga el cambio a sistemas de búsqueda y caché.

### 30.8.3. Flujo completo: PostgreSQL a Kafka a MongoDB

```mermaid
flowchart LR
    subgraph ORIGEN["Origen"]
        A[Admin] -->|CREATE/UPDATE/DELETE| B[(PostgreSQL)]
        B -->|Escribe en WAL| C[WAL]
    end

    subgraph CAPTURA["Captura"]
        C -->|Lee en tiempo real| D[Debezium]
        D -->|Publica evento| E[Kafka]
    end

    subgraph DESTINO["Destino"]
        E -->|Consume evento| F[Sync Service]
        F -->|Transforma| G[(MongoDB)]
    end

    style ORIGEN fill:#2196F3,color:#fff
    style CAPTURA fill:#FF9800,color:#fff
    style DESTINO fill:#4CAF50,color:#fff
```

### 30.8.4. Ventajas y desventajas de Kafka

| Ventaja | Desventaja |
|---------|------------|
| Latencia menor a 1 segundo | Complejo de configurar |
| Escalable: maneja millones de eventos | Necesita infraestructura adicional |
| No necesita código de sincronización | Coste de servidores Kafka + Debezium |
| Desacopla productores de consumidores | Curva de aprendizaje pronunciada |
| Persiste eventos (no se pierden) | Más difícil de debuggear |

> 📝 **Nota:** Kafka es una herramienta profesional. Para aprender, primero domina los conceptos de CQRS con las opciones más simples.

## 30.9. MediatR: Implementando CQRS

### 30.9.1. ¿Qué es MediatR?

**MediatR** es una librería de .NET que implementa el patrón **Mediator**. En CQRS, MediatR separa quién envía un command/query (el controller) de quién lo procesa (el handler). Esto desacopla completamente las capas.

📌 Ejemplo real: **eBay** usa un patrón similar. Cuando un vendedor actualiza un producto, el controller solo envía un `UpdateProductCommand`. MediatR se encarga de buscar el handler correcto, ejecutar la lógica y devolver el resultado. El controller no sabe nada de la implementación.

```mermaid
flowchart LR
    subgraph TRADICIONAL["Sin MediatR"]
        A[Controller] -->|llama directamente| B[Service]
        B -->|accede| C[Repository]
    end

    subgraph CON_MEDIATR["Con MediatR"]
        D[Controller] -->|envía Command| E[MediatR]
        E -->|despacha| F[Handler]
        F -->|accede| G[Repository]
    end

    style TRADICIONAL fill:#f44336,color:#fff
    style CON_MEDIATR fill:#4CAF50,color:#fff
```

**Instalación:**

```bash
dotnet add package MediatR
```

```csharp
// Program.cs — desde MediatR 12 el registro va en el propio paquete MediatR.
// El antiguo paquete MediatR.Extensions.Microsoft.DependencyInjection está obsoleto.
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
```

### 30.9.2. Commands con MediatR

Un **Command** es un objeto que representa una intención de modificar datos. MediatR lo envía al **Handler** correspondiente.

```csharp
// Command: representa la intención de crear un producto
public record CreateProductoCommand(
    string Nombre,
    decimal Precio,
    long CategoriaId,
    long ProveedorId
) : IRequest<Producto>;
```

```csharp
// Handler: ejecuta la lógica del command
public class CreateProductoHandler(
    IProductoRepository repository,
    IMediator mediator) : IRequestHandler<CreateProductoCommand, Producto>
{
    public async Task<Producto> Handle(
        CreateProductoCommand request,
        CancellationToken cancellationToken)
    {
        var producto = new Producto
        {
            Nombre = request.Nombre,
            Precio = request.Precio,
            CategoriaId = request.CategoriaId,
            ProveedorId = request.ProveedorId
        };

        var creado = repository.Add(producto);

        // 1. Persistir PRIMERO: si la escritura falla, salta la excepción
        //    y NUNCA se publica el evento
        await repository.SaveChangesAsync(cancellationToken);

        // 2. Solo después, publicar evento CON el objeto (consistencia)
        await mediator.Publish(new ProductoCreadoEvent(creado), cancellationToken);

        return creado;
    }
}
```

> 📝 **Nota (producción):** Si el proceso muere justo entre `SaveChangesAsync` y `Publish`, el evento se pierde. En producción se usa el patrón **Outbox**: guardar el evento en la misma transacción que el dato y publicarlo desde un Background Service. Para el ejemplo del módulo, persistir-primero-y-publicar-después es suficiente.

```csharp
// Commands adicionales (mismo estilo: IRequest<T>)
public record UpdateProductoCommand(
    long Id,
    string Nombre,
    decimal Precio
) : IRequest<Producto>;

public record DeleteProductoCommand(long Id) : IRequest<bool>;
```

```csharp
// Update: persistir primero, publicar después
public class UpdateProductoHandler(
    IProductoRepository repository,
    IMediator mediator) : IRequestHandler<UpdateProductoCommand, Producto>
{
    public async Task<Producto> Handle(
        UpdateProductoCommand request,
        CancellationToken cancellationToken)
    {
        var producto = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Producto {request.Id} no encontrado");

        producto.Nombre = request.Nombre;
        producto.Precio = request.Precio;

        repository.Update(producto);
        await repository.SaveChangesAsync(cancellationToken);

        await mediator.Publish(new ProductoActualizadoEvent(producto), cancellationToken);
        return producto;
    }
}

// Delete: persistir primero, publicar después
public class DeleteProductoHandler(
    IProductoRepository repository,
    IMediator mediator) : IRequestHandler<DeleteProductoCommand, bool>
{
    public async Task<bool> Handle(
        DeleteProductoCommand request,
        CancellationToken cancellationToken)
    {
        var producto = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (producto is null)
            return false;

        repository.Remove(producto);
        await repository.SaveChangesAsync(cancellationToken);

        await mediator.Publish(new ProductoEliminadoEvent(producto), cancellationToken);
        return true;
    }
}
```

```csharp
// En el Controller
[HttpPost]
public async Task<IActionResult> Create([FromBody] CreateProductoInput input)
{
    var command = new CreateProductoCommand(
        input.Nombre, input.Precio, input.CategoriaId, input.ProveedorId);
    
    var producto = await _mediator.Send(command);
    return CreatedAtAction(nameof(GetById), new { id = producto.Id }, producto);
}
```

### 30.9.3. Queries con MediatR

Una **Query** es un objeto que representa una intención de leer datos.

```csharp
// Query: representa la intención de buscar productos
public record SearchProductosQuery(string Termino) : IRequest<List<Producto>>;
```

```csharp
// Handler
public class SearchProductosHandler(
    IProductoReadRepository repository) : IRequestHandler<SearchProductosQuery, List<Producto>>
{
    public Task<List<Producto>> Handle(
        SearchProductosQuery request,
        CancellationToken cancellationToken)
    {
        return repository.SearchAsync(request.Termino, cancellationToken);
    }
}
```

### 30.9.4. Pipeline Behaviors

Los **Behaviors** son middleware que se ejecutan antes/después de cada handler. Útiles para logging, validación, caching, etc.

```csharp
// Logging Behavior
public class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling {RequestType}", typeof(TRequest).Name);
        var response = await next();
        logger.LogInformation("Handled {ResponseType}", typeof(TResponse).Name);
        return response;
    }
}
```

📌 Ejemplo real: **Uber** usa behaviors para validar que el conductor tenga licencia antes de procesar un viaje. El behavior se ejecuta antes del handler y verifica los permisos.

### 30.9.5. Diagrama: Flujo completo de Commands y Queries

Este diagrama muestra **todos los dominios** fluyendo a través de MediatR: Commands (Create, Update, Delete), Queries (GetAll, GetById) y la sincronización a MongoDB vía Domain Events. Observa cómo cada uno recorre el **pipeline completo** con Behaviors intercalados:

```mermaid
sequenceDiagram
    autonumber
    participant Ctrl as Controller
    participant MediatR as MediatR
    participant Log as LoggingBehavior
    participant Valid as ValidationBehavior
    participant Cache as CachingBehavior
    participant Handler as Command/Query Handler
    participant Repo as Repository (PostgreSQL)
    participant Domain as Domain Events
    participant Sync as SyncHandler
    participant Mongo as MongoDB

    Note over Ctrl,Mongo: ═══ COMMAND: Crear Producto ═══

    Ctrl->>MediatR: Send(CreateProductoCommand)
    MediatR->>Log: Handle(CreateProductoCommand)
    Log->>Valid: next()
    Valid->>Cache: next()
    Cache->>Handler: next() → Handle()
    Handler->>Repo: Add(producto)
    Handler->>Repo: SaveChangesAsync()
    Repo-->>Handler: OK (id=42)
    Handler->>MediatR: Publish(ProductoCreadoEvent)
    MediatR->>Sync: Handle(ProductoCreadoEvent)
    Sync->>Sync: Mapear a formato documento
    Sync->>Mongo: ReplaceOne (upsert)
    Mongo-->>Sync: OK
    Handler-->>Ctrl: Producto creado
    Ctrl-->>Ctrl: 201 Created

    Note over Ctrl,Mongo: ═══ COMMAND: Actualizar Producto ═══

    Ctrl->>MediatR: Send(UpdateProductoCommand)
    MediatR->>Log: Handle(UpdateProductoCommand)
    Log->>Valid: next()
    Valid->>Cache: next()
    Cache->>Handler: next() → Handle()
    Handler->>Repo: GetByIdAsync(42)
    Repo-->>Handler: Producto existente
    Handler->>Repo: Update(producto)
    Handler->>Repo: SaveChangesAsync()
    Repo-->>Handler: OK
    Handler->>MediatR: Publish(ProductoActualizadoEvent)
    MediatR->>Sync: Handle(ProductoActualizadoEvent)
    Sync->>Mongo: ReplaceOne (upsert)
    Mongo-->>Sync: OK
    Handler-->>Ctrl: Producto actualizado
    Ctrl-->>Ctrl: 200 OK

    Note over Ctrl,Mongo: ═══ COMMAND: Eliminar Producto ═══

    Ctrl->>MediatR: Send(DeleteProductoCommand)
    MediatR->>Log: Handle(DeleteProductoCommand)
    Log->>Valid: next()
    Valid->>Handler: next() → Handle()
    Handler->>Repo: GetByIdAsync(42)
    Repo-->>Handler: Producto existente
    Handler->>Repo: Remove(producto)
    Handler->>Repo: SaveChangesAsync()
    Repo-->>Handler: OK
    Handler->>MediatR: Publish(ProductoEliminadoEvent)
    MediatR->>Sync: Handle(ProductoEliminadoEvent)
    Sync->>Mongo: DeleteOne
    Mongo-->>Sync: OK
    Handler-->>Ctrl: true
    Ctrl-->>Ctrl: 200 OK

    Note over Ctrl,Mongo: ═══ QUERY: Buscar Productos ═══

    Ctrl->>MediatR: Send(SearchProductosQuery)
    MediatR->>Log: Handle(SearchProductosQuery)
    Log->>Cache: next()
    Cache->>Handler: next() → Handle()
    Handler->>Repo: SearchAsync("teclado")
    Repo-->>Handler: List<Producto>
    Handler-->>Ctrl: List<Producto>
    Ctrl-->>Ctrl: 200 OK
```

📌 Ejemplo real: **Amazon** usa este patrón completo. Cuando un vendedor crea un producto (Command), pasa por validación, logging y sincronización. Cuando un cliente busca (Query), pasa por cache y logging. Todo desacoplado vía MediatR.

**Resumen del Flujo por Tipo de Operación:**

| Operación | Tipo | Pipeline | Destino |
|-----------|------|----------|---------|
| `CreateProductoCommand` | Command | Log → Valid → Handler → Event → Sync | PostgreSQL → MongoDB |
| `UpdateProductoCommand` | Command | Log → Valid → Handler → Event → Sync | PostgreSQL → MongoDB |
| `DeleteProductoCommand` | Command | Log → Valid → Handler → Event → Sync | PostgreSQL → MongoDB |
| `GetAllProductosQuery` | Query | Log → Cache → Handler | PostgreSQL |
| `GetProductoByIdQuery` | Query | Log → Cache → Handler | PostgreSQL |
| `SearchProductosQuery` | Query | Log → Cache → Handler | PostgreSQL |

**¿Por qué es importante este diagrama?**

1. **Desacoplamiento**: El Controller no sabe qué Handler procesa el Command. Solo envía a MediatR.
2. **Pipeline configurable**: Los Behaviors (Log, Valid, Cache) se ejecutan en orden y se pueden añadir/quitar sin tocar Handlers.
3. **Domain Events**: Después de cada Command, se publica un evento que sincroniza MongoDB. La sincronización es **parte del mismo flujo**.
4. **Consistencia**: El evento se publica **después** de `SaveChangesAsync`. Si la escritura falla, nunca se publica el evento.

### 30.9.6. GraphQL también pasa por MediatR

Todo lo que hemos visto hasta ahora entra por **REST**. Pero CQRS no es cosa de HTTP: si tu API también expone GraphQL, las queries **deben pasar por el mismo `IMediator`**. Si no, acabas con dos caminos distintos hacia los mismos datos, y tarde o temprano divergen.

```csharp
/// <summary>
/// Consultas GraphQL de la tienda.
///
/// 🎓 CQRS consistente: GraphQL pasa por MediatR igual que REST.
/// Las queries usan los mismos Query Handlers que los controladores REST.
///
/// 🎓 Seguridad: GraphQL solo expone DTOs, nunca entidades del modelo de escritura.
/// </summary>
public class TiendaQuery
{
    /// <summary>Obtiene todos los productos.</summary>
    /// <param name="mediator">Mediator para enviar queries CQRS.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Lista de productos (DTOs).</returns>
    public async Task<IReadOnlyList<ProductoDto>> GetProductos(
        [Service] IMediator mediator,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetAllProductosListQuery(), ct);
        if (result.IsFailure)
            throw new Exception(result.Error.Message);

        return result.Value;
    }

    // El resto de campos siguen el mismo patrón: resolver → Send(...) → DTO
}
```

**Qué se gana con esto:**

| GraphQL llamando a los servicios directamente | GraphQL pasando por MediatR |
|---|---|
| Dos implementaciones de «traer todos los productos» | Un solo Query Handler para REST y GraphQL |
| Los Behaviors (Logging, Validación, Caché) solo corren en REST | Los Behaviors corren **también** en GraphQL |
| Cambias la regla de negocio en un sitio y GraphQL no se entera | Un único sitio que cambiar |
| Los tests de GraphQL prueban otra cosa | Los tests de GraphQL prueban el mismo handler |

> 📝 **Nota:** Fíjate en el `[Service] IMediator mediator` de la firma: HotChocolate inyecta el mediator **por resolver**, sin necesidad de constructor. Y el patrón de errores —`if (result.IsFailure) throw ...`— es el puente entre el `Result` funcional que devuelven los Handlers y el modelo de excepciones que GraphQL entiende.

📌 **Ejemplo real:** en TiendaAPI (repo CQRS/MediatR) el commit *feat: GraphQL queries pasan por MediatR (CQRS consistente con REST)* creó `GetAllProductosListQuery` y `GetAllCategoriasListQuery` y reescribió `TiendaQuery` para que todo pasara por `IMediator.Send`; los tests se adaptaron en el commit siguiente (*fix: tests GraphQL actualizados para usar IMediator*). CQRS es una **disciplina, no un detalle de transporte**: REST y GraphQL son dos ventanas a la misma caja.

> 💡 **Consejo:** Si tu GraphQL llama directamente a los servicios (`[Service] IProductoService service`), tienes **dos arquitecturas viviendo en el mismo proyecto**. Pasa las queries por MediatR **antes** de que el esquema crezca: una vez que los resolvers cuelgan de los Handlers, no hay vuelta atrás sin reescribir.

## 30.10. Sincronización con Domain Events y MediatR

En la sección 30.6 vimos que los Domain Events son una opción para sincronizar. Ahora veamos cómo implementarlos con MediatR, que ya usamos para CQRS.

### 30.10.1. La idea clave: ya tienes los datos en memoria

Cuando el handler de un Command crea/modifica un producto, **ya tiene el objeto en memoria**. No necesitas hacer `SELECT` otra vez. Solo mapeas el objeto al formato documento y lo escribes en MongoDB.

> 📝 **Nota sobre tiempos:** El evento se publica **inmediatamente** después de la escritura. Pero la consistencia eventual sigue existiendo: el tiempo que tarda el SyncHandler en **procesar** el evento y **escribir** en MongoDB. Con Domain Events, esa ventana se reduce a **segundos** en vez de minutos.

**Algoritmo:**
```
1. Admin crea producto → CreateProductoCommand
2. Handler ejecuta: producto = repository.Add(...)
3. Handler persiste: await repository.SaveChangesAsync()   ← PRIMERO la escritura
4. Handler TIENE el objeto producto en memoria (ya lo creó)
5. Handler publica evento: mediator.Publish(new ProductoCreadoEvent(producto))
6. SyncHandler recibe el evento CON el objeto
7. SyncHandler transforma a formato documento (sin JOINs)
8. SyncHandler escribe en MongoDB (1 operación)
```

**Comparación con polling:**

| | Polling (BackgroundService) | Domain Events |
|--|----------------------------|---------------|
| **¿Cuándo consulta SQL?** | Cada X tiempo | Solo cuando hay cambio |
| **¿Cuántas consultas?** | Todas las tablas (JOINs) | Ya tiene los datos en memoria |
| **¿Cuándo se ejecuta?** | Cada 60 segundos | Instantáneamente |
| **Recursos** | Consume CPU SQL periódicamente | No consume nada extra |

```mermaid
sequenceDiagram
    participant Admin as Admin
    participant Handler as CreateHandler
    participant SQL as PostgreSQL
    participant MediatR as MediatR
    participant Sync as SyncHandler
    participant Mongo as MongoDB

    Admin->>Handler: CreateProductoCommand
    Handler->>SQL: INSERT INTO Productos
    SQL-->>Handler: Producto creado (en memoria)
    Handler->>MediatR: Publish(ProductoCreadoEvent)
    MediatR->>Sync: Handle(evento)
    Note over Sync: Ya tiene el objeto producto en memoria
    Sync->>Sync: Mapear a formato documento
    Sync->>Mongo: ReplaceOne (upsert)
    Mongo-->>Sync: OK
```

### 30.10.2. Código de ejemplo

**El Command Handler publica el evento** (mismo estilo `IRequest<T>` que en 30.9.2 — un solo estilo en todo el documento):

```csharp
public class CreateProductoHandler(
    IProductoRepository repository,
    IMediator mediator) : IRequestHandler<CreateProductoCommand, Producto>
{
    public async Task<Producto> Handle(
        CreateProductoCommand request, CancellationToken cancellationToken)
    {
        // 1. Crear producto en PostgreSQL
        var producto = new Producto
        {
            Nombre = request.Nombre,
            Precio = request.Precio,
            CategoriaId = request.CategoriaId,
            CreatedAt = DateTime.UtcNow
        };

        var creado = repository.Add(producto);

        // 2. Persistir ANTES de publicar (si falla, no se publica el evento)
        await repository.SaveChangesAsync(cancellationToken);

        // 3. Publicar evento CON el objeto (ya está en memoria)
        await mediator.Publish(new ProductoCreadoEvent(creado), cancellationToken);

        return creado;
    }
}
```

**El SyncHandler escucha y sincroniza:**

```csharp
public class SyncProductoHandler(
    MongoDbContext mongoContext) : INotificationHandler<ProductoCreadoEvent>
{
    public async Task Handle(ProductoCreadoEvent notification, CancellationToken cancellationToken)
    {
        // 1. Recibe el objeto directamente (sin SELECT)
        var producto = notification.Producto;

        // 2. Mapear a formato documento
        var read = new ProductoRead
        {
            Id = producto.Id,
            Nombre = producto.Nombre,
            Precio = producto.Precio,
            Categoria = new CategoriaRead
            {
                Id = producto.CategoriaId,
                Nombre = producto.Categoria?.Nombre ?? ""
            },
            SyncAt = DateTime.UtcNow
        };

        // 3. Escribir en MongoDB (1 operación).
        //    OJO: el proveedor EF de MongoDB NO implementa ExecuteDeleteAsync:
        //    usamos el driver de MongoDB con ReplaceOneAsync + IsUpsert (upsert).
        var collection = mongoContext.ProductosRead; // IMongoCollection<ProductoRead>
        await collection.ReplaceOneAsync(
            p => p.Id == producto.Id,
            read,
            new ReplaceOptions { IsUpsert = true },
            cancellationToken);
    }
}
```

### 30.10.3. Ventajas de este enfoque

- **0 consultas SQL adicionales**: El handler ya tiene el objeto en memoria
- **Tiempo real**: Sincronización instantánea
- **Simple**: Solo handler + evento, sin configurar intervalos
- **Integrado**: Usa MediatR que ya tenemos para CQRS

> 📝 **Nota:** Este enfoque requiere que el handler tenga acceso a los datos completos del objeto (con relaciones). Si el handler solo tiene el ID, necesitaría hacer un SELECT.

## 30.11. Testing de CQRS

Estos tests cubren la lección real de los ejemplos: **Create → visible en el read model**, **Update → read model actualizado**, **Delete → GET 404**.

```csharp
[TestFixture]
public class ProductoCqrsTests
{
    [Test]
    public async Task Sync_ProductoCreado_SeSincronizaMongoDB()
    {
        // Arrange
        var producto = new Producto { Nombre = "Teclado", Precio = 89.99m, CategoriaId = 1 };
        await sqlContext.Productos.AddAsync(producto);
        await sqlContext.SaveChangesAsync();

        // Act
        await syncService.SyncProductosAsync();

        // Assert
        var read = await mongoContext.Productos.FirstOrDefaultAsync(p => p.Id == producto.Id);
        read.Should().NotBeNull();
        read!.Nombre.Should().Be("Teclado");
        read.Categoria.Should().NotBeNull();
    }

    [Test]
    public async Task Update_Producto_ReadModelActualizado()
    {
        // Arrange: crear y sincronizar el producto
        var producto = new Producto { Nombre = "Teclado", Precio = 89.99m, CategoriaId = 1 };
        await sqlContext.Productos.AddAsync(producto);
        await sqlContext.SaveChangesAsync();
        await syncService.SyncProductosAsync();

        // Act: actualizar en el modelo de escritura y resincronizar
        producto.Precio = 99.99m;
        sqlContext.Productos.Update(producto);
        await sqlContext.SaveChangesAsync();
        await syncService.SyncProductosAsync();

        // Assert: el read model refleja el nuevo precio
        var read = await mongoContext.Productos.FirstOrDefaultAsync(p => p.Id == producto.Id);
        read.Should().NotBeNull();
        read!.Precio.Should().Be(99.99m);
    }

    [Test]
    public async Task Delete_Producto_GetDevuelve404()
    {
        // Arrange: crear, sincronizar y verificar que existe
        var producto = new Producto { Nombre = "Ratón", Precio = 19.99m, CategoriaId = 1 };
        await sqlContext.Productos.AddAsync(producto);
        await sqlContext.SaveChangesAsync();
        await syncService.SyncProductosAsync();

        var okResponse = await client.GetAsync($"/api/productos/{producto.Id}");
        okResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Act: borrar en el modelo de escritura y resincronizar
        await mediator.Send(new DeleteProductoCommand(producto.Id));
        await syncService.SyncProductosAsync();

        // Assert: la lectura ya no encuentra el producto → 404
        var response = await client.GetAsync($"/api/productos/{producto.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
```

## 30.12. Buenas Prácticas

> ⚠️ **Advertencia — CQRS no es la solución para todo**

CQRS añade complejidad (dos BDs, sincronización, consistencia eventual). Solo tiene sentido cuando las **ventajas superan la complejidad**.

**¿Cuándo SÍ usar CQRS?**
- Miles de lecturas por cada escritura
- Consultas complejas con muchos JOINs que lentan la BD
- Necesitas escalar las lecturas independientemente de las escrituras
- Los DTOs "montan" datos de 3+ tablas relacionadas
- La latencia de las consultas es un problema real

**¿Cuándo NO usar CQRS?**
- API simple con pocos usuarios (100-500)
- Consultas simples sin muchos JOINs
- Cache ya resuelve el problema de rendimiento
- No tienes experiencia con sincronización de datos
- La complejidad no compensa el beneficio

📌 Ejemplo real: **Netflix** usa CQRS porque tiene millones de usuarios buscando contenido. **Una tienda online pequeña** con 100 productos no necesita CQRS: un solo PostgreSQL con cache es suficiente.

- **Sincronización periódica**: Usa el enfoque que mejor se adapte a tu caso
- **Upsert en MongoDB**: `ReplaceOne` con `IsUpsert = true` para crear o actualizar
- **Timestamps de sync**: Guarda `SyncAt` para saber cuándo se sincronizó cada documento
- **Manejo de errores**: Si falla la sync, reintenta pero no bloquee el sistema
- **Logs detallados**: Registra cada sync para debugging

## 30.13. Reto

> Implementa CQRS para FunkoApp: PostgreSQL para escrituras, MongoDB para lecturas.

**Añade a tu API:**

1. **Modelo SQL:** Funko con CategoriaId y ProveedorId (claves foráneas)
2. **Modelo MongoDB:** FunkoRead con Categoría y Proveedor embebidos
3. **Mecanismo de sincronización** entre PostgreSQL y MongoDB
4. **Queries en MongoDB** (lecturas rápidas con documentos denormalizados)
5. **Commands en PostgreSQL** (escrituras transaccionales)
6. **Tests:** Verificar que la sincronización funciona correctamente

**Puntos extra:**

- Mostrar timestamp de última sincronización en la API
- Manejar errores de sincromización con reintentos
- Documentar la ventana de inconsistencia

---

**Resumen del punto:**

| Concepto | Descripción |
|----------|-------------|
| **CQRS** | Separar Commands (escrituras) de Queries (lecturas) |
| **Command** | Operación que modifica el estado (Create, Update, Delete) |
| **Query** | Operación que lee datos sin modificarlos |
| **PostgreSQL normalizado** | BD relacional para escrituras (integridad referencial) |
| **MongoDB denormalizado** | BD NoSQL para lecturas (documentos anidados) |
| **BackgroundService** | Tarea programada que sincroniza PostgreSQL → MongoDB |
| **Consistencia eventual** | Retraso entre escritura y disponibilidad en lectura |
| **Ventana de inconsistencia** | Tiempo que tarda la sincronización (ej: 5 min) |
| **Kafka** | Sistema de streaming para sincronización profesional |
| **Upsert** | Crear o actualizar en una sola operación |
| **Timestamp de sync** | Saber cuándo se sincronizó cada documento |

**¿Qué viene después?**

En el siguiente punto veremos **API Gateway**: cómo crear un punto de entrada único que enrute peticiones a diferentes microservicios, gestione autenticación, rate limiting y agregación de respuestas.
