# 13. MongoDB

- [13. MongoDB](#13-mongodb)
  - [13.1. Fundamentos NoSQL](#131-fundamentos-nosql)
    - [13.1.1. ¿Qué es MongoDB?](#1311-qué-es-mongodb)
    - [13.1.2. Modelo de documentos](#1312-modelo-de-documentos)
    - [13.1.3. BSON vs JSON](#1313-bson-vs-json)
    - [13.1.4. Colecciones y documentos](#1314-colecciones-y-documentos)
  - [13.2. Diseño NoSQL: de SQL a MongoDB](#132-diseño-nosql-de-sql-a-mongodb)
    - [13.2.1. Tablas → Colecciones, Filas → Documentos](#1321-tablas--colecciones-filas--documentos)
    - [13.2.2. Claves foráneas → Referencias o Embebido](#1322-claves-foráneas--referencias-o-embebido)
    - [13.2.3. Cuándo embeber vs cuándo referenciar](#1323-cuándo-embeber-vs-cuándo-referenciar)
    - [13.2.4. El límite de 16 MB](#1324-el-límite-de-16-mb)
    - [13.2.5. Patrones de diseño habituales](#1325-patrones-de-diseño-habituales)
  - [13.3. MongoDB con Driver Nativo](#133-mongodb-con-driver-nativo)
    - [13.3.1. Paquete NuGet](#1331-paquete-nuget)
    - [13.3.2. Conexión: MongoClient](#1332-conexión-mongoclient)
    - [13.3.3. Database y Collection](#1333-database-y-collection)
    - [13.3.4. Documentos POCO y BsonDocument](#1334-documentos-poco-y-bsondocument)
    - [13.3.5. Insertar documentos](#1335-insertar-documentos)
    - [13.3.6. Consultar documentos](#1336-consultar-documentos)
    - [13.3.7. Actualizar documentos](#1337-actualizar-documentos)
    - [13.3.8. Borrar documentos](#1338-borrar-documentos)
    - [13.3.9. Builders de filtros y actualizaciones](#1339-builders-de-filtros-y-actualizaciones)
    - [13.3.10. Índices](#13310-índices)
    - [13.4.2. DbContext con UseMongoDB](#1342-dbcontext-con-usemongodb)
    - [13.4.3. Data Annotations en MongoDB](#1343-data-annotations-en-mongodb)
    - [13.4.4. Fluent API en MongoDB](#1344-fluent-api-en-mongodb)
    - [13.4.5. Entidades Owned (documentos embebidos)](#1345-entidades-owned-documentos-embebidos)
    - [13.4.6. Referencias manuales](#1346-referencias-manuales)
    - [13.4.7. Consultas LINQ](#1347-consultas-linq)
    - [13.4.8. Limitaciones de EF Core con MongoDB](#1348-limitaciones-de-ef-core-con-mongodb)
  - [13.5. Driver Nativo vs EF Core: Comparativa](#135-driver-nativo-vs-ef-core-comparativa)
  - [13.6. Repositorio CRUD con MongoDB](#136-repositorio-crud-con-mongodb)
    - [13.6.1. Modelo y configuración](#1361-modelo-y-configuración)
    - [13.6.2. Interfaz del repositorio](#1362-interfaz-del-repositorio)
    - [13.6.3. Implementación con Driver Nativo](#1363-implementación-con-driver-nativo)
    - [13.6.4. Implementación con EF Core](#1364-implementación-con-ef-core)
  - [13.7. Testing con MongoDB](#137-testing-con-mongodb)
    - [13.7.1. Compatibilidad de versiones: EF Core + MongoDB](#1371-compatibilidad-de-versiones-ef-core--mongodb)
    - [13.7.2. TestContainers](#1372-testcontainers)
    - [13.7.3. Tests con Driver Nativo](#1373-tests-con-driver-nativo)
    - [13.7.4. Tests con EF Core](#1374-tests-con-ef-core)
  - [13.8. Semilla de datos (Seed)](#138-semilla-de-datos-seed)
    - [Con Driver Nativo](#con-driver-nativo)
    - [Con EF Core](#con-ef-core)
  - [13.9. Buenas prácticas](#139-buenas-prácticas)
  - [13.10. Reto](#1310-reto)


---

> 💡 **Punto de partida:** ¿Alguna vez has tenido que diseñar una base de datos con 15 tablas y 20 joins solo para guardar una factura? ¿O has sufrido con un ORM relacional que no encaja con tu modelo de datos? MongoDB te propone otra forma: guarda lo que piensas como un documento, sin tablas, sin joins, sin ceremony.

**Objetivos de aprendizaje:**
- Comprender el modelo NoSQL de documentos y cuándo usarlo
- Diseñar esquemas NoSQL a partir de un modelo relacional
- Trabajar con MongoDB usando el Driver Nativo en C#
- Trabajar con MongoDB usando EF Core para MongoDB
- Entender documentos embebidos vs referencias
- Comparar ambas aproximaciones (Driver vs EF Core)
- Testear con TestContainers

13.1. Fundamentos NoSQL

### 13.1.1. ¿Qué es MongoDB?

MongoDB es una base de datos **NoSQL orientada a documentos**. A diferencia de MySQL o PostgreSQL, no guarda datos en tablas con filas y columnas, sino en **documentos** dentro de **colecciones**. Cada documento es un JSON (o BSON) flexible que puede tener estructuras diferentes dentro de la misma colección.

> 💡 **Analogía:** MongoDB es como un cajón de carpetas donde cada carpeta puede tener un formato diferente. En SQL, todos los documentos deben seguir la misma plantilla (como formularios oficiales). En MongoDB, cada carpeta es libre de incluir las secciones que quieras.

📌 Ejemplo real: **Instagram** usa MongoDB para almacenar perfiles de usuario, donde cada usuario puede tener campos diferentes (unos tienen blog, otros tienda, otros solo fotos). No tendría sentido forzar a todos a tener las mismas columnas.

| Concepto SQL | MongoDB |
|-------------|---------|
| Base de datos | Database |
| Tabla | Colección |
| Fila | Documento |
| Columna | Campo |
| JOIN | Embebido o referencia |
| PRIMARY KEY | `_id` |

```mermaid
graph TD
    subgraph SQL["SQL (Relacional)"]
        S1[Base de Datos] --> S2[Tabla Productos]
        S1 --> S3[Tabla Categorías]
        S2 -->|FK| S3
        S2 --> S4[Fila: 1 producto = 1 fila]
    end

    subgraph NOSQL["MongoDB (Documentos)"]
        M1[Database] --> M2[Colección Productos]
        M2 --> M3[Documento: 1 producto = 1 JSON completo]
    end

    style SQL fill:#2196F3,color:#fff
    style NOSQL fill:#4CAF50,color:#fff
    style S1 fill:#2196F3,color:#fff
    style S2 fill:#2196F3,color:#fff
    style S3 fill:#2196F3,color:#fff
    style S4 fill:#42A5F5,color:#fff
    style M1 fill:#4CAF50,color:#fff
    style M2 fill:#4CAF50,color:#fff
    style M3 fill:#66BB6A,color:#fff
```

### 13.1.2. Modelo de documentos

Un documento MongoDB es una estructura de pares clave-valor. Puede anidar objetos y arrays de forma natural:

```json
{
  "_id": ObjectId("..."),
  "nombre": "Teclado Mecánico",
  "precio": 89.99,
  "categorias": ["periféricos", "gaming"],
  "especificaciones": {
    "teclas": 104,
    "conexion": "USB-C",
    "retroiluminado": true
  },
  "createdAt": "2026-09-21T10:00:00Z"
}
```

En SQL, esto requeriría al menos 3 tablas (productos, categorías, producto_categorías). En MongoDB, **es un solo documento**.

### 13.1.3. BSON vs JSON

MongoDB guarda los documentos en formato **BSON** (Binary JSON), que es una representación binaria de JSON con tipos de datos adicionales (como `ObjectId`, `DateTime`, `Int32`, `Int64`).

```csharp
// JSON: texto plano, tipos limitados
{ "nombre": "Teclado", "precio": 89.99 }

// BSON: binario, con ObjectId y tipos nativos
{ "_id": ObjectId("507f1f77bcf86cd799439011"), "nombre": "Teclado", "precio": Decimal128(89.99) }
```

> 📝 **Nota:** Cuando trabajas con el driver de C#, el driver se encarga de la serialización BSON automáticamente. Tú trabajas con objetos POCO normales.

### 13.1.4. Colecciones y documentos

Una **colección** es un conjunto de documentos (equivalente a una tabla). Las colecciones son **flexibles**: no tienen esquema fijo. Dos documentos en la misma colección pueden tener campos diferentes.

```json
// Misma colección "productos"
{ "_id": 1, "nombre": "Teclado", "precio": 89.99 }
{ "_id": 2, "nombre": "Silla", "precio": 249.99, "color": "negro", "peso": 15.5 }
```

> ⚠️ **Advertencia:** La flexibilidad es una navaja de dos cortes. Sin un esquema definido, puedes terminar con documentos inconsistentes que son difíciles de mantener. Siempre define un modelo claro en tu aplicación.

13.2. Diseño NoSQL: de SQL a MongoDB

### 13.2.1. Tablas → Colecciones, Filas → Documentos

El equivalente directo:

| SQL | MongoDB | Ejemplo |
|-----|---------|---------|
| `CREATE TABLE productos` | `db.createCollection('productos')` | Crear colección |
| `INSERT INTO productos (...)` | `db.productos.insertOne({...})` | Insertar documento |
| `SELECT * FROM productos WHERE id = 1` | `db.productos.findOne({_id: 1})` | Buscar por ID |
| `UPDATE productos SET precio = 90 WHERE id = 1` | `db.productos.updateOne({_id: 1}, {$set: {precio: 90}})` | Actualizar |
| `DELETE FROM productos WHERE id = 1` | `db.productos.deleteOne({_id: 1})` | Borrar |

### 13.2.2. Claves foráneas → Referencias o Embebido

En SQL, las relaciones se hacen con claves foráneas y JOINs. En MongoDB, hay **dos formas**:

**Referencia** (estilo SQL):
```json
// Colección "productos"
{ "_id": 1, "nombre": "Teclado", "categoriaId": 10 }

// Colección "categorias"
{ "_id": 10, "nombre": "Periféricos" }
```

**Documento embebido** (estilo NoSQL):
```json
// Colección "productos" - la categoría está DENTRO del producto
{
  "_id": 1,
  "nombre": "Teclado",
  "categoria": {
    "_id": 10,
    "nombre": "Periféricos"
  }
}
```

### 13.2.3. Cuándo embeber vs cuándo referenciar

Esta es **la decisión más importante** al diseñar en NoSQL:

```mermaid
graph TD
    START[Necesito guardar una relación] --> Q1{Los datos se leen juntos?}
    Q1 -->|Sí| Q2{Cambia con frecuencia?}
    Q1 -->|No| REF[Referenciar]

    Q2 -->|No| Q3{Crece sin límite?}
    Q2 -->|Sí| REF

    Q3 -->|No| EMB[Embeber]
    Q3 -->|Sí| Q4{Consultas independientes?}

    Q4 -->|Sí| REF
    Q4 -->|No| SUBSET[Subset Pattern]

    EMB --> OK[Documento completo en una consulta]
    REF --> OK2[Dos consultas, datos normalizados]
    SUBSET --> OK3[Lo mejor de ambos mundos]

    style START fill:#FF9800,color:#fff
    style EMB fill:#4CAF50,color:#fff
    style REF fill:#2196F3,color:#fff
    style SUBSET fill:#9C27B0,color:#fff
    style OK fill:#4CAF50,color:#fff
    style OK2 fill:#2196F3,color:#fff
    style OK3 fill:#7B1FA2,color:#fff
```

| Criterio | Embeber | Referenciar |
|----------|---------|-------------|
| **Datos que se leen juntos** | ✅ Sí | ❌ No |
| **Datos que cambian a menudo** | ❌ No | ✅ Sí |
| **Colección que crece sin límite** | ❌ No | ✅ Sí |
| **Datos que se consultan independiente** | ❌ No | ✅ Sí |
| **Relación 1:N con pocos elementos** | ✅ Sí | ⚠️ Opcional |
| **Relación N:M** | ❌ No | ✅ Sí |
| **Datos duplicados aceptables** | ✅ Sí | ❌ No |

```csharp
// ❌ MALO: Pensar en SQL y separar todo en colecciones distintas
// Esto fuerza múltiples consultas y pierde la ventaja de MongoDB
var producto = collection<Producto>.Find(p => p.Id == id).First();
var categoria = collection<Categoria>.Find(c => c.Id == producto.CategoriaId).First(); // 2ª consulta
var reviews = collection<Review>.Find(r => r.ProductoId == id).ToList(); // 3ª consulta

// ✅ BUENO: Embeber datos que siempre se leen juntos en un solo documento
var producto = collection<Producto>.Find(p => p.Id == id).First();
// Producto ya contiene: Categoria { ... } y Reviews [ ... ]
// Una sola consulta, datos consistentes, mejor rendimiento
```

```csharp
// ❌ MALO: Embeber colecciones que crecen sin límite (llegarás a 16 MB)
var logs = collection<Log>.Find(_ => true).ToList();
// Si un producto tiene millones de logs embebidos →.DocumentExceedsSizeLimitException

// ✅ BUENO: Usar Subset Pattern — embeber solo los más recientes
varproducto = collection<Producto>.Find(p => p.Id == id).First();
// Producto tiene: ReviewsRecientes [últimos 5] + ReviewCount
// El resto de reviews se obtiene de una colección separada si hace falta
```

> 💡 **Analogía:** Piensa en MongoDB como una carpeta de expediente. Puedes pegar notas, fotos y documentos dentro de la misma carpeta (embeber). Pero si la carpeta crece demasiado, necesitarás un archivador separado (referenciar) y solo dejar un Post-It con la referencia.

📌 Ejemplo real: **Netflix** embebe la lista de "episodios" dentro de cada "serie" porque siempre se ven juntos. Pero usa referencias para los "actores" porque un actor aparece en múltiples series.

### 13.2.4. El límite de 16 MB

MongoDB tiene un **límite de 16 MB por documento**. Si embebes un array que puede crecer indefinidamente (ej: millones de logs), llegarás al límite.

Solución: **Subset Pattern** — guarda solo los últimos N elementos embebidos, el resto en una colección separada.

### 13.2.5. Patrones de diseño habituales

| Patrón | Descripción | Ejemplo |
|--------|-------------|---------|
| **Subset** | Embebe solo los datos más recientes/frecuentes | Últimos 10 mensajes de un chat |
| **Outlier** | Separa documentos inusuales en otra colección | Producto con 1000 fotos vs los normales con 3 |
| **Bucket** | Agrupa documentos por tiempo/tamaño | Sensor que guarda lecturas por hora |
| **Schema Versioning** | Versiona el esquema para migraciones | v1: nombre, v2: nombre + email |
| **Computed** | Pre-calcula datos que se leen mucho | Total de ventas pre-calculado |

13.3. MongoDB con Driver Nativo

### 13.3.1. Paquete NuGet

```bash
dotnet add package MongoDB.Driver
```

> 📝 **Nota:** El driver nativo es el paquete oficial de MongoDB para .NET. Es la forma más directa y completa de trabajar con MongoDB.

### 13.3.2. Conexión: MongoClient

```csharp
using MongoDB.Driver;

// Conexión local (Docker)
var client = new MongoClient("mongodb://localhost:27017");

// Conexión a Atlas (nube)
var client = new MongoClient("mongodb+srv://usuario:password@cluster0.abc.mongodb.net/?retryWrites=true&w=majority");
```

```mermaid
graph LR
    APP[Tu App C#] -->|1 conexión| MC[MongoClient]
    MC -->|Pool| P1[Conexión 1]
    MC -->|Pool| P2[Conexión 2]
    MC -->|Pool| PN[Conexión N]
    P1 --> DB[(MongoDB Server)]
    P2 --> DB
    PN --> DB

    style APP fill:#FF9800,color:#fff
    style MC fill:#4CAF50,color:#fff
    style P1 fill:#2196F3,color:#fff
    style P2 fill:#2196F3,color:#fff
    style PN fill:#2196F3,color:#fff
    style DB fill:#f44336,color:#fff
```

`MongoClient` es **thread-safe** y gestiona un pool de conexiones. Debes crear **una sola instancia** y reutilizarla (como `HttpClient`).

### 13.3.3. Database y Collection

```csharp
// Obtener la base de datos
var database = client.GetDatabase("tienda");

// Obtener una colección tipada
var productos = database.GetCollection<Producto>("productos");

// Obtener como BsonDocument (sin modelo tipado)
var collection = database.GetCollection<BsonDocument>("productos");
```

### 13.3.4. Documentos POCO y BsonDocument

**BsonDocument** — documentos dinámicos, sin modelo fijo:
```csharp
using MongoDB.Bson;

var doc = new BsonDocument
{
    { "nombre", "Teclado" },
    { "precio", 89.99 },
    { "categorias", new BsonArray { "periféricos", "gaming" } }
};

collection.InsertOne(doc);
```

**POCO** — documentos tipados con clases C# (recomendado):
```csharp
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Producto
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [BsonElement("precio")]
    public decimal Precio { get; set; }

    [BsonElement("categorias")]
    public List<string> Categorias { get; set; } = [];
}
```

> 💡 **Consejo:** Usa siempre POCO en proyectos reales. BsonDocument es útil para prototipos o consultas dinámicas, pero no tiene IntelliSense ni compile-time checking.

```csharp
// ❌ MALO: Usar BsonDocument en producción — sin tipado, errores en runtime
var doc = collection.Find(new BsonDocument { { "nombre", "Teclado" } }).First();
var precio = doc["precio"].AsDecimal; // Error si el campo no existe o cambia de tipo
string nombre = doc["nombre"].AsString; // Sin IntelliSense, sin validación

// ✅ BUENO: Usar POCO con atributos — tipado, IntelliSense, compile-time checking
var producto = collection.Find(p => p.Nombre == "Teclado").First();
var precio = producto.Precio; // decimal, seguro
string nombre = producto.Nombre; // string, con autocompletado
```

Atributos Bson más habituales:

| Atributo | Uso |
|----------|-----|
| `[BsonId]` | Marca la propiedad como `_id` |
| `[BsonElement("nombre")]` | Mapea a un nombre de campo concreto |
| `[BsonIgnore]` | Ignora la propiedad (no se serializa) |
| `[BsonDateTimeKind(DateTimeKind.Utc)]` | Fuerza la zona horaria |
| `[BsonRepresentation(BsonType.ObjectId)]` | Representa como ObjectId en BSON |

### 13.3.5. Insertar documentos

```csharp
var collection = database.GetCollection<Producto>("productos");

// Insertar uno
var producto = new Producto { Nombre = "Teclado", Precio = 89.99m };
collection.InsertOne(producto);

// Insertar varios
var productos = new List<Producto>
{
    new() { Nombre = "Ratón", Precio = 35.50m },
    new() { Nombre = "Monitor", Precio = 299.00m }
};
collection.InsertMany(productos);
```

> 📝 **Nota:** Después de `InsertOne`, el driver rellena automáticamente el campo `Id` con un `ObjectId` generado.

### 13.3.6. Consultar documentos

```csharp
var collection = database.GetCollection<Producto>("productos");

// Todos
var todos = collection.Find(_ => true).ToList();

// Por ID
var producto = collection.Find(p => p.Id == id).FirstOrDefault();

// Con filtro
var baratos = collection.Find(p => p.Precio < 50).ToList();

// Paginado
var pagina = collection.Find(_ => true)
    .Skip((page - 1) * pageSize)
    .Limit(pageSize)
    .ToList();

// Ordenado
var ordenados = collection.Find(_ => true)
    .SortByDescending(p => p.Precio)
    .ToList();

// Contar
var total = collection.CountDocuments(_ => true);
```

### 13.3.7. Actualizar documentos

```csharp
// Actualizar uno
var filtro = Builders<Producto>.Filter.Eq(p => p.Id, id);
var actualizacion = Builders<Producto>.Update
    .Set(p => p.Precio, 95.00m)
    .Set(p => p.Nombre, "Teclado Mecánico Pro");

collection.UpdateOne(filtro, actualizacion);

// Actualizar varios
var filtroBaratos = Builders<Producto>.Filter.Lt(p => p.Precio, 50);
var actualizacionIncremento = Builders<Producto>.Update
    .Inc(p => p.Precio, 5);  // Incrementar en 5

collection.UpdateMany(filtroBaratos, actualizacionIncremento);

// Upsert: actualizar si existe, insertar si no
collection.UpdateOne(filtro, actualizacion, new UpdateOptions { IsUpsert = true });
```

### 13.3.8. Borrar documentos

```csharp
// Borrar uno
collection.DeleteOne(p => p.Id == id);

// Borrar varios
collection.DeleteMany(p => p.Precio < 10);
```

### 13.3.9. Builders de filtros y actualizaciones

Los `Builders<T>` son la forma tipada de crear filtros y actualizaciones:

```csharp
// Filtros
var filtro1 = Builders<Producto>.Filter.Eq(p => p.Nombre, "Teclado");
var filtro2 = Builders<Producto>.Filter.Gt(p => p.Precio, 50);
var filtro3 = Builders<Producto>.Filter.And(filtro1, filtro2);
var filtro4 = Builders<Producto>.Filter.Or(
    Builders<Producto>.Filter.Eq(p => p.Categoria, "Electrónica"),
    Builders<Producto>.Filter.Eq(p => p.Categoria, "Gaming")
);

// Proyecciones (seleccionar campos)
var proyeccion = Builders<Producto>.Projection
    .Include(p => p.Nombre)
    .Include(p => p.Precio)
    .Exclude(p => p._id);

var resultados = collection.Find(filtro1).Project(proyeccion).ToList();
```

### 13.3.10. Índices

```csharp
// ❌ MALO: No crear índices y hacer full table scan en colecciones grandes
var productos = collection.Find(p => p.Nombre == "Teclado").ToList();
// MongoDB escanea TODOS los documentos → lentitud extrema con millones de registros

// ✅ BUENO: Crear índices en campos de consulta frecuentes
collection.Indexes.CreateOne(new CreateIndexModel<Producto>(
    Builders<Producto>.IndexKeys.Ascending(p => p.Nombre)));

// Ahora la búsqueda por nombre usa el índice → respuesta en milisegundos
var productos = collection.Find(p => p.Nombre == "Teclado").ToList();
```

```csharp
// ❌ MALO: Crear índices en campos que nunca se consultan
// Esto desperdicia memoria y ralentiza las escrituras
collection.Indexes.CreateOne(new CreateIndexModel<Producto>(
    Builders<Producto>.IndexKeys.Ascending(p => p.DescripcionLarga)));
// Nadie busca por descripción larga → inútil

// ✅ BUENO: Índice compuesto para consultas que filtran y ordenan
collection.Indexes.CreateOne(new CreateIndexModel<Producto>(
    Builders<Producto>.IndexKeys
        .Ascending(p => p.Categoria)
        .Descending(p => p.Precio)));
// Optimiza: WHERE categoria = 'X' ORDER BY precio DESC
```

// Índice único
indices.CreateOne(new CreateIndexModel<Producto>(
    Builders<Producto>.IndexKeys.Ascending(p => p.Nombre),
    new CreateIndexOptions { Unique = true }));

// Índice compuesto
indices.CreateOne(new CreateIndexModel<Producto>(
    Builders<Producto>.IndexKeys
        .Ascending(p => p.Categoria)
        .Descending(p => p.Precio)));
```

13.4. MongoDB con EF Core

### 13.4.1. Paquete NuGet

```bash
dotnet add package MongoDB.EntityFrameworkCore
```

> 📝 **Nota:** El proveedor EF Core para MongoDB es oficial (desarrollado por MongoDB, Inc). Permite usar la misma sintaxis de EF Core que usas con PostgreSQL o SQLite, pero guardando en MongoDB.

### 13.4.2. DbContext con UseMongoDB

```csharp
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

public class TiendaDbContext(DbContextOptions<TiendaDbContext> options) : DbContext(options)
{
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Categoria> Categorias => Set<Categoria>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // MongoDB no necesita migraciones
        modelBuilder.Entity<Producto>().ToCollection("productos");
        modelBuilder.Entity<Categoria>().ToCollection("categorias");
    }
}

// Registro en Program.cs / DI
var client = new MongoClient("mongodb://localhost:27017");

services.AddDbContext<TiendaDbContext>(options =>
    options.UseMongoDB(client, "tienda"));
```

### 13.4.3. Data Annotations en MongoDB

```csharp
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Producto
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("nombre")]
    [Required]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [BsonElement("precio")]
    public decimal Precio { get; set; }

    [BsonElement("imagen")]
    public string Imagen { get; set; } = string.Empty;

    [BsonElement("categoria_id")]
    public long CategoriaId { get; set; }
}
```

> 📝 **Nota:** En MongoDB, los Data Annotations de EF Core (`[Required]`, `[StringLength]`) funcionan para validación en C#, pero **no se aplican en la BD**. MongoDB no tiene constraints como SQL. La `[BsonId]` y `[BsonElement]` son las que controlan el mapeo real.

### 13.4.4. Fluent API en MongoDB

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Producto>(entity =>
    {
        entity.ToCollection("productos");

        // MongoDB no soporta HasPrecision, HasIndex a través de EF Core
        // Los índices se crean con el driver o desde la BD
    });

    modelBuilder.Entity<Categoria>(entity =>
    {
        entity.ToCollection("categorias");
    });
}
```

### 13.4.5. Entidades Owned (documentos embebidos)

Las entidades **Owned** son el equivalente a los documentos embebidos en MongoDB. Es la forma natural de modelar datos que van siempre juntos.

```csharp
// ❌ MALO: Guardar dirección como referencia (estilo SQL) en MongoDB
public class Cliente
{
    [BsonId]
    public ObjectId Id { get; set; }
    public long DireccionId { get; set; } // Referencia a otra colección
}
// Requiere 2 consultas para obtener cliente + dirección. En MongoDB esto es innecesario.

// ✅ BUENO: Embebir dirección como Owned Type dentro del mismo documento
[Owned]
public class Direccion
{
    public string Calle { get; set; } = string.Empty;
    public string Ciudad { get; set; } = string.Empty;
    public string CodigoPostal { get; set; } = string.Empty;
}

public class Cliente
{
    [BsonId]
    public ObjectId Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public Direccion Direccion { get; set; } = null!; // Todo en un solo documento
}
// Una sola consulta, datos siempre consistentes, mejor rendimiento
```

```mermaid
graph TD
    subgraph CLASES["Clases C#"]
        C[Cliente] -->|propiedad| D[Direccion]
    end

    subgraph MONGO["Documento MongoDB"]
        M["_id: ObjectId(...)"]
        M --> N["Nombre: Juan"]
        M --> DIR["Direccion: { Calle, Ciudad, CodigoPostal }"]
    end

    CLASES -->|serializa| MONGO

    style CLASES fill:#2196F3,color:#fff
    style MONGO fill:#4CAF50,color:#fff
    style C fill:#2196F3,color:#fff
    style D fill:#2196F3,color:#fff
    style M fill:#4CAF50,color:#fff
    style N fill:#4CAF50,color:#fff
    style DIR fill:#4CAF50,color:#fff
```

```csharp
// Modelo embebido: Dirección dentro de Cliente
[Owned]
public class Direccion
{
    public string Calle { get; set; } = string.Empty;
    public string Ciudad { get; set; } = string.Empty;
    public string CodigoPostal { get; set; } = string.Empty;
}

public class Cliente
{
    [BsonId]
    public ObjectId Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public Direccion Direccion { get; set; } = null!;  // Se embebe en el documento
}
```

En MongoDB, esto se guarda como:

```json
{
  "_id": ObjectId("..."),
  "Nombre": "Juan",
  "Direccion": {
    "Calle": "Calle Mayor 1",
    "Ciudad": "Madrid",
    "CodigoPostal": "28001"
  }
}
```

También puedes embeber colecciones con `OwnsMany`:

```csharp
public class Pedido
{
    [BsonId]
    public ObjectId Id { get; set; }
    public string ClienteNombre { get; set; } = string.Empty;
    public List<LineaPedido> Lineas { get; set; } = [];  // Se embebe como array
}

[Owned]
public class LineaPedido
{
    public string Producto { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}
```

```json
{
  "_id": ObjectId("..."),
  "ClienteNombre": "Juan",
  "Lineas": [
    { "Producto": "Teclado", "Cantidad": 1, "PrecioUnitario": 89.99 },
    { "Producto": "Ratón", "Cantidad": 2, "PrecioUnitario": 35.50 }
  ]
}
```

### 13.4.6. Referencias manuales

Cuando los datos cambian a menudo o se consultan independientemente, usa **referencias** en vez de embeber:

```csharp
public class Producto
{
    [BsonId]
    public ObjectId Id { get; set; }
    public string Nombre { get; set; } = string.Empty;

    // Referencia a Categoría (no se embebe)
    public long CategoriaId { get; set; }
}

public class Categoria
{
    [BsonId]
    public long Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
}
```

Para obtener la categoría, debes hacer **dos consultas**:

```csharp
var producto = await db.Productos.FindAsync(productoId);
var categoria = await db.Categorias.FindAsync(producto.CategoriaId);
```

> ⚠️ **Advertencia:** A diferencia de SQL, MongoDB **no tiene JOINs** ni `Include()` como en EF Core relacional. Cada consulta es independiente. Si necesitas los datos juntos, embebe.

### 13.4.7. Consultas LINQ

EF Core con MongoDB soporta la mayoría de consultas LINQ habituales:

```csharp
// Filtrar
var baratos = await db.Productos
    .Where(p => p.Precio < 50)
    .ToListAsync();

// Ordenar
var ordenados = await db.Productos
    .OrderByDescending(p => p.Precio)
    .ToListAsync();

// Primero o por ID
var producto = await db.Productos
    .FirstOrDefaultAsync(p => p.Id == id);

// Contar
var total = await db.Productos.CountAsync();

// Any
var existe = await db.Productos.AnyAsync(p => p.Nombre == "Teclado");
```

### 13.4.8. Limitaciones de EF Core con MongoDB

| Feature | Soporte | Notas |
|---------|---------|-------|
| **Migraciones** | ❌ No | MongoDB es schema-flexible, no necesita migraciones |
| **Database-First** | ❌ No | No tiene esquema que inferir |
| **Foreign Keys** | ❌ No | No existen en MongoDB |
| **Select Projections** | ❌ No | No soporta `Select()` a tipos anónimos |
| **Table Splitting** | ❌ No | No hay tablas |
| **Temporal Tables** | ❌ No | Feature exclusiva de SQL Server |
| **Include (Eager Loading)** | ❌ No | No hay JOINs, usa OwnsMany/Owned |
| **LINQ básico** | ✅ Sí | Where, OrderBy, Count, Any |
| **Owned Types** | ✅ Sí | Documentos embebidos |
| **SaveChanges** | ✅ Sí | Insert/Update/Delete |

> 📝 **Nota:** El proveedor MongoDB EF Core está en desarrollo activo. Algunas features pueden añadirse en el futuro. Consulta siempre la [documentación oficial](https://www.mongodb.com/es/docs/entity-framework/current/limitations/) para ver el estado actual.

13.5. Driver Nativo vs EF Core: Comparativa

```mermaid
graph TD
    subgraph DRIVER["Driver Nativo"]
        D1[MongoClient] --> D2[IMongoCollection T]
        D2 --> D3[Builders T]
        D3 --> D4[(MongoDB)]
    end

    subgraph EF["EF Core"]
        E1[DbContext] --> E2[DbSet T]
        E2 --> E3[LINQ]
        E3 --> E4[(MongoDB)]
    end

    DRIVER -->|"Control total, más código"| DEC{¿Qué prefieres?}
    EF -->|"Abstracción, menos código"| DEC

    DEC -->|"MongoDB puro, agregaciones"| DRIVER
    DEC -->|"CRUD simple, cambiable"| EF

    style DRIVER fill:#FF9800,color:#fff
    style EF fill:#9C27B0,color:#fff
    style D1 fill:#FF9800,color:#fff
    style D2 fill:#EF6C00,color:#fff
    style D3 fill:#F57C00,color:#fff
    style D4 fill:#f44336,color:#fff
    style E1 fill:#9C27B0,color:#fff
    style E2 fill:#7B1FA2,color:#fff
    style E3 fill:#8E24AA,color:#fff
    style E4 fill:#f44336,color:#fff
    style DEC fill:#607D8B,color:#fff
```

| Criterio | Driver Nativo | EF Core |
|----------|--------------|---------|
| **Control total** | ✅ Sí, acceso directo a la API de MongoDB | ⚠️ Limitado por el ORM |
| **Funciones MongoDB** | ✅ Todas (agregaciones, geo, text search) | ❌ Solo las que el proveedor soporta |
| **Sintaxis familiar** | ❌ Builders y métodos específicos de MongoDB | ✅ LINQ, como con SQL |
| **Cambio de BD** | ❌ Acoplado a MongoDB | ✅ Puedes cambiar a SQL fácilmente |
| **Migraciones** | ❌ No existen | ❌ No soportadas (pero el ORM sí las gestiona en SQL) |
| **Owned Types** | ❌ A mano | ✅ `[Owned]` y `OwnsOne/OwnsMany` |
| **Performance** | ✅ Máxima (sin overhead de ORM) | ⚠️ Alguna pérdida por abstracción |
| **Documentos embebidos** | ✅ Total control | ✅ Con Owned Types |
| **Complejidad** | Mayor, más código | Menos código, más declarativo |
| **Testing** | TestContainers | TestContainers + InMemory |
| **Curva aprendizaje** | Baja si conoces MongoDB | Baja si conoces EF Core |

> 💡 **Consejo:** Si tu aplicación es **100% MongoDB** y necesitas funcionalidades avanzadas (agregaciones, geo-queries, Atlas Search), usa el **Driver Nativo**. Si vienes de EF Core relacional y quieres una capa de abstracción más uniforme (o piensas cambiar de BD a futuro), usa **EF Core**.

📌 Ejemplo real: **TiendaAPI** usa el Driver Nativo para MongoDB porque necesita agregaciones y consultas complejas que EF Core no soporta. Pero para PostgreSQL usa EF Core porque las migraciones y el LINQ son muy útiles.

13.6. Repositorio CRUD con MongoDB

### 13.6.1. Modelo y configuración

```csharp
// Modelo
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Funko
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [BsonElement("precio")]
    public decimal Precio { get; set; }

    [BsonElement("imagen")]
    public string Imagen { get; set; } = string.Empty;

    [BsonElement("is_deleted")]
    public bool IsDeleted { get; set; }

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}
```

### 13.6.2. Interfaz del repositorio

```csharp
public interface IFunkoRepository
{
    IEnumerable<Funko> GetAll();
    Funko? GetById(ObjectId id);
    Funko Add(Funko funko);
    Funko? Update(Funko funko);
    bool Delete(ObjectId id);
    bool IsDuplicated(string nombre, ObjectId excludeId = default);
}
```

### 13.6.3. Implementación con Driver Nativo

```csharp
using MongoDB.Driver;

public class FunkoMongoRepository(IMongoDatabase database) : IFunkoRepository
{
    private readonly IMongoCollection<Funko> _collection = database.GetCollection<Funko>("funkos");

    public IEnumerable<Funko> GetAll() =>
        _collection.Find(f => !f.IsDeleted).ToList();

    public Funko? GetById(ObjectId id) =>
        _collection.Find(f => f.Id == id).FirstOrDefault();

    public Funko Add(Funko funko)
    {
        funko.CreatedAt = DateTime.UtcNow;
        _collection.InsertOne(funko);
        return funko;
    }

    public Funko? Update(Funko funko)
    {
        funko.UpdatedAt = DateTime.UtcNow;
        var result = _collection.ReplaceOne(f => f.Id == funko.Id, funko);
        return result.IsAcknowledged ? funko : null;
    }

    public bool Delete(ObjectId id)
    {
        var update = Builders<Funko>.Update
            .Set(f => f.IsDeleted, true);
        var result = _collection.UpdateOne(f => f.Id == id, update);
        return result.IsAcknowledged && result.ModifiedCount > 0;
    }

    public bool IsDuplicated(string nombre, ObjectId excludeId = default) =>
        _collection.Find(f => f.Nombre == nombre && f.Id != excludeId).Any();
}
```

### 13.6.4. Implementación con EF Core

```csharp
using Microsoft.EntityFrameworkCore;

public class FunkoEfCoreRepository(TiendaDbContext db) : IFunkoRepository
{
    public IEnumerable<Funko> GetAll() =>
        db.Funkos.Where(f => !f.IsDeleted).ToList();

    public Funko? GetById(ObjectId id) =>
        db.Funkos.FirstOrDefault(f => f.Id == id);

    public Funko Add(Funko funko)
    {
        funko.CreatedAt = DateTime.UtcNow;
        db.Funkos.Add(funko);
        db.SaveChanges();
        return funko;
    }

    public Funko? Update(Funko funko)
    {
        var existente = db.Funkos.Find(funko.Id);
        if (existente is null) return null;

        existente.Nombre = funko.Nombre;
        existente.Precio = funko.Precio;
        existente.Imagen = funko.Imagen;
        existente.UpdatedAt = DateTime.UtcNow;

        db.SaveChanges();
        return existente;
    }

    public bool Delete(ObjectId id)
    {
        var funko = db.Funkos.Find(id);
        if (funko is null) return false;

        funko.IsDeleted = true;
        db.SaveChanges();
        return true;
    }

    public bool IsDuplicated(string nombre, ObjectId excludeId = default) =>
        db.Funkos.Any(f => f.Nombre == nombre && f.Id != excludeId);
}
```

13.7. Testing con MongoDB

### 13.7.1. Compatibilidad de versiones: EF Core + MongoDB

> ⚠️ **Advertencia — ¡Ojo con las versiones!**
>
> `MongoDB.EntityFrameworkCore` depende de una versión **exacta** de `Microsoft.EntityFrameworkCore`. Si usas versiones incompatibles, obtendrás errores en runtime como `MissingMethodException` o `FileNotFoundException`.
>
> **Tabla de compatibilidad verificada:**
>
> | MongoDB.EntityFrameworkCore | MongoDB.Driver | Microsoft.EntityFrameworkCore | Estado |
> |------------------------------|----------------|-------------------------------|--------|
> | **10.0.3** | **3.11.0** | **10.0.11** | ✅ Funciona |
> | 9.0.0 | 3.3.0 | 10.0.0 | ❌ `MissingMethodException` |
> | 9.0.0 | 3.3.0 | 9.0.x | ✅ Funciona |
>
> ```csharp
> // ❌ MALO: Mezclar versiones incompatibles
> <PackageReference Include="MongoDB.EntityFrameworkCore" Version="9.0.0" />
> <PackageReference Include="Microsoft.EntityFrameworkCore" Version="10.0.0" />  // ¡INCOMPATIBLE!
> // Resultado: MissingMethodException en runtime
>
> // ✅ BUENO: Todas las versiones alineadas
> <PackageReference Include="MongoDB.EntityFrameworkCore" Version="10.0.3" />
> <PackageReference Include="MongoDB.Driver" Version="3.11.0" />
> <PackageReference Include="Microsoft.EntityFrameworkCore" Version="10.0.11" />  // Compatible
> ```
>
> 🔧 **Truco:** Si cambias la versión de EF Core en un proyecto relacional (PostgreSQL, SQLite), acuérdate de actualizar también `MongoDB.EntityFrameworkCore` en el proyecto Mongo. Mantén siempre las versiones alineadas.

### 13.7.2. TestContainers

TestContainers levanta un contenedor Docker de MongoDB real para los tests, sin depender de una instalación local.

```mermaid
sequenceDiagram
    participant T as Test
    participant TC as TestContainers
    participant D as Docker
    participant M as MongoDB

    T->+TC: Build contenedor
    TC->+D: docker run mongo:7
    D->+M: Iniciar MongoDB
    M-->>-D: Listo
    D-->>-TC: Puerto asignado
    TC-->>-T: Connection string

    T->+M: Insertar datos de test
    M-->>-T: OK

    T->+M: Ejecutar tests
    M-->>-T: Resultados

    T->+TC: Dispose
    TC->+D: docker stop + rm
    D-->>-TC: Contenedor eliminado
    TC-->>-T: Limpieza completada
```

```bash
dotnet add package Testcontainers.MongoDb
```

```csharp
using Testcontainers.MongoDb;

public abstract class MongoTestBase : IAsyncLifetime
{
    private readonly MongoDbContainer _mongo = new MongoDbBuilder()
        .WithImage("mongo:7")
        .Build();

    protected IMongoDatabase Database { get; private set; } = null!;
    protected MongoClient Client { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await _mongo.StartAsync();
        Client = new MongoClient(_mongo.GetConnectionString());
        Database = Client.GetDatabase("test_db");
    }

    public async Task DisposeAsync()
    {
        await _mongo.DisposeAsync();
    }
}
```

> ⚠️ **Advertencia — Errores habituales con TestContainers**
>
> **Principio fundamental: cada test debe ser aislado**
>
> Un test no debe depender del estado que haya dejado otro test anterior. Si el test A inserta 3 productos y el test B espera encontrar exactamente 2 productos, el test B falla... ¡aunque el código sea correcto! Por eso, cada test debe empezar con una **BD limpia y con los mismos datos base**. Así todos los tests se ejecutan en las mismas condiciones, sin importar el orden.
>
> ```mermaid
> flowchart LR
>     T1["Test A: inserta 3 productos"] --> T2["Test B: espera 2 productos"]
>     T2 --> FAIL["❌ FALLA: encuentra 5"]
>
>     T1B["Test A: inserta 3 productos"] --> CLEAN["🧹 Limpieza"]
>     CLEAN --> T2B["Test B: BD limpia, inserta 2"]
>     T2B --> OK["✅ PASA: encuentra solo 2"]
>
>     style FAIL fill:#f44336,color:#fff
>     style OK fill:#4CAF50,color:#fff
>     style CLEAN fill:#FF9800,color:#fff
> ```
>
> **1. Container como campo instance, nunca `static`**
>
> Si el contenedor es `static readonly`, se comparte entre todos los `[TestFixture]` de la solución. Pero NUnit ejecuta cada `[TestFixture]` en un ensamblado diferente, y el contenedor se destruye al terminar el primero. El siguiente fixture intenta usar un contenedor muerto → errores raros.
>
> ```csharp
> // ❌ MALO: static readonly — compartido entre fixtures, se destruye antes de tiempo
> public abstract class MongoTestBase : IAsyncLifetime
> {
>     private static readonly MongoDbContainer _mongo = new MongoDbBuilder()
>         .WithImage("mongo:7").Build();
> }
>
> // ✅ BUENO: instance field — cada fixture obtiene su propio contenedor
> public abstract class MongoTestBase : IAsyncLifetime
> {
>     private readonly MongoDbContainer _mongo = new MongoDbBuilder()
>         .WithImage("mongo:7").Build();
> }
> ```
>
> **2. `[OneTimeTearDown]` para dispose del contenedor**
>
> Usa `[OneTimeTearDown]` (no `[TearDown]`) para destruir el contenedor. Así se ejecuta una sola vez al final de todos los tests del fixture, no después de cada test.
>
> ```csharp
> [OneTimeTearDown]
> public void OneTimeTearDown()
> {
>     _mongo?.Dispose();
> }
> ```
>
> **3. Limpieza de datos entre tests**
>
> TestContainers no recrea la BD entre tests. Si no limpias los datos, los tests se contaminan entre sí. Cada test debe empezar con la BD limpia y con los mismos datos base. Para **MongoDB**, usa `DeleteMany` en `[SetUp]`:
>
> ```csharp
> [SetUp]
> public void SetUp()
> {
>     // Cada test empieza con la BD limpia y los mismos datos base
>     Database.GetCollection<BsonDocument>("productos").DeleteMany(FilterDefinition<BsonDocument>.Empty);
>     Database.GetCollection<BsonDocument>("categorias").DeleteMany(FilterDefinition<BsonDocument>.Empty);
> }
> ```
>
> 🔧 **Truco:** MongoDB no tiene `TRUNCATE ... RESTART IDENTITY` porque los IDs son `ObjectId` (generados por el driver, no por la BD). La limpieza con `DeleteMany` es suficiente.

### 13.7.3. Tests con Driver Nativo

```csharp
[TestFixture]
public class FunkoRepositoryTests : MongoTestBase
{
    private FunkoMongoRepository _repository = null!;

    [SetUp]
    public void SetUp()
    {
        _repository = new FunkoMongoRepository(Database);
    }

    [Test]
    public void Add_FunkoValido_RetornaConId()
    {
        // Arrange
        var funko = new Funko { Nombre = "Spider-Man", Precio = 29.99m };

        // Act
        var resultado = _repository.Add(funko);

        // Assert
        resultado.Id.Should().NotBe(ObjectId.Empty);
        resultado.Nombre.Should().Be("Spider-Man");
    }

    [Test]
    public void GetById_FunkoExiste_RetornaFunko()
    {
        // Arrange
        var funko = _repository.Add(new Funko { Nombre = "Batman", Precio = 35.00m });

        // Act
        var resultado = _repository.GetById(funko.Id);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Nombre.Should().Be("Batman");
    }

    [Test]
    public void Delete_FunkoExiste_MarcaComoBorrado()
    {
        // Arrange
        var funko = _repository.Add(new Funko { Nombre = "Para Borrar", Precio = 10m });

        // Act
        var resultado = _repository.Delete(funko.Id);

        // Assert
        resultado.Should().BeTrue();
        _repository.GetById(funko.Id).Should().BeNull();
    }
}
```

### 13.7.4. Tests con EF Core

```csharp
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;

public class TiendaMongoDbContext : DbContext
{
    public DbSet<Funko> Funkos => Set<Funko>();

    public TiendaMongoDbContext(DbContextOptions<TiendaMongoDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Funko>().ToCollection("funkos");
    }
}

[TestFixture]
public class FunkoEfCoreTests : MongoTestBase
{
    private TiendaMongoDbContext _context = null!;
    private FunkoEfCoreRepository _repository = null!;

    [SetUp]
    public void SetUp()
    {
        _context = new TiendaMongoDbContext(
            new DbContextOptionsBuilder<TiendaMongoDbContext>()
                .UseMongoDB(Client, "test_db")
                .Options);

        _repository = new FunkoEfCoreRepository(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    [Test]
    public void Add_FunkoValido_RetornaConId()
    {
        // Arrange
        var funko = new Funko { Nombre = "Iron Man", Precio = 25.00m };

        // Act
        var resultado = _repository.Add(funko);

        // Assert
        resultado.Id.Should().NotBe(ObjectId.Empty);
    }
}
```

13.8. Semilla de datos (Seed)

### Con Driver Nativo

```csharp
public class MongoDbSeeder(IMongoDatabase database)
{
    public async Task SeedAsync()
    {
        var collection = database.GetCollection<Funko>("funkos");

        if (await collection.CountDocumentsAsync(FilterDefinition<Funko>.Empty) > 0)
            return;  // Ya hay datos, no sembrar

        var funkos = new List<Funko>
        {
            new() { Nombre = "Spider-Man", Precio = 29.99m, CreatedAt = DateTime.UtcNow },
            new() { Nombre = "Batman", Precio = 35.00m, CreatedAt = DateTime.UtcNow },
            new() { Nombre = "Iron Man", Precio = 25.00m, CreatedAt = DateTime.UtcNow }
        };

        await collection.InsertManyAsync(funkos);
    }
}
```

### Con EF Core

```csharp
// En Program.cs (solo desarrollo)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var seeder = scope.ServiceProvider.GetRequiredService<MongoDbSeeder>();
    await seeder.SeedAsync();
}
```

> 💡 **Consejo:** El seeder siempre debe comprobar si ya existes datos (`CountDocumentsAsync` o `AnyAsync`) antes de insertar. Así es seguro ejecutarlo múltiples veces sin duplicar datos.

13.9. Buenas prácticas

```csharp
// ❌ MALO: Crear un MongoClient por petición — agota el pool de conexiones
public Producto GetProducto(int id)
{
    var client = new MongoClient("mongodb://localhost:27017"); // ¡MAL! Cada petición crea un cliente
    var db = client.GetDatabase("tienda");
    return db.GetCollection<Producto>("productos").Find(p => p.Id == id).First();
}

// ✅ BUENO: MongoClient como singleton — thread-safe, reutilizable
// Registrar en DI como singleton:
builder.Services.AddSingleton<IMongoClient>(new MongoClient("mongodb://localhost:27017"));
// Linyectarlo en repositorios/servicios — UN solo cliente para toda la app
```

```csharp
// ❌ MALO: Usar ObjectId como string — pierde rendimiento en búsquedas
[BsonElement("_id")]
public string Id { get; set; } // No se puede buscar eficientemente

// ✅ BUENO: Usar ObjectId tipado — búsqueda directa por el _id nativo
[BsonId]
public ObjectId Id { get; set; } // Indexado automáticamente, búsqueda óptima
```

1. **Embebe cuando los datos van juntos** — Categoría en Producto, Dirección en Cliente
2. **Referencia cuando cambian a menudo** — No embebas un contador de visitas dentro de un Producto
3. **Usa POCO, no BsonDocument** — Tipado, IntelliSense, maintainability
4. **`MongoClient` es singleton** — Crear uno por aplicación, reutilizar
5. **El `_id` es obligatorio** — Cada documento debe tener un campo `_id`
6. **No embebas colecciones ilimitadas** — El límite es 16 MB por documento
7. **Diseña para tus consultas** — Piensa qué datos necesitas juntos antes de embeber
8. **Driver Nativo para consultas complejas** — Agregaciones, geo-queries
9. **EF Core para CRUD simple** — Si vienes de EF Core relacional, es más fácil
10. **TestContainers para tests** — Un MongoDB real en Docker, sin depender de instalación local

13.10. Reto

Implementa un repositorio CRUD de **Funkos con Categorías embebidas** usando MongoDB.

**Especificación:**

1. **Modelo Funko:** `Nombre`, `Precio`, `Imagen`, `IsDeleted`, `CreatedAt`, `UpdatedAt`
2. **Categoría embebida:** `Categoria` como propiedad dentro de Funko (no como colección separada)
3. **Implementa DOS versiones:**
   - Una con Driver Nativo (`MongoDB.Driver`)
   - Otra con EF Core (`MongoDB.EntityFrameworkCore`)
4. **Repositorio:** Create, Read (GetAll, GetById), Update, Delete (borrado lógico), IsDuplicated
5. **Tests con TestContainers:** Al menos 5 tests: Create, GetById, GetAll, Delete lógico, IsDuplicated
6. **Seed data:** 5 Funkos con categorías embebidas que se insertan al arrancar en desarrollo

**Ejemplo de documento Funko con categoría embebida:**

```json
{
  "_id": ObjectId("..."),
  "nombre": "Spider-Man",
  "precio": 29.99,
  "imagen": "https://picsum.photos/seed/spiderman/300",
  "categoria": {
    "nombre": "Cómics",
    "descripcion": "Personajes de cómic y superhéroes"
  },
  "is_deleted": false,
  "created_at": "2026-09-21T10:00:00Z",
  "updated_at": null
}
```

**Arquitectura:**

```
ProductosMongo/
├── ProductosMongo.slnx
├── docker-compose.yml          # Solo MongoDB
├── docker-compose.api.yml      # API + MongoDB
├── ProductosMongo/
│   ├── Program.cs
│   ├── Models/
│   │   └── Funko.cs
│   ├── Repositories/
│   │   ├── IFunkoRepository.cs
│   │   ├── FunkoMongoRepository.cs      # Driver nativo
│   │   └── FunkoEfCoreRepository.cs     # EF Core
│   ├── Entity/
│   │   └── TiendaMongoDbContext.cs
│   ├── Infrastructures/
│   │   ├── DatabaseConfig.cs
│   │   ├── RepositoriesConfig.cs
│   │   └── ServicesConfig.cs
│   └── Seed/
│       └── MongoDbSeeder.cs
└── ProductosMongo.Test/
    ├── Driver/
    │   └── FunkoMongoRepositoryTests.cs
    └── EfCore/
        └── FunkoEfCoreRepositoryTests.cs
```

---

**Resumen del punto:**

```mermaid
graph TD
    MONGO[MongoDB] --> NOSQL[NoSQL Documentos]
    MONGO --> DESIGN[Diseño]
    MONGO --> TOOLS[Herramientas C#]
    MONGO --> TEST[Testing]

    NOSQL --> BSON[BSON / JSON]
    NOSQL --> COL[Colecciones]
    NOSQL --> DOC[Documentos flexibles]

    DESIGN --> EMB[Embeber Owned]
    DESIGN --> REF[Referenciar]
    DESIGN --> PAT[Patrones de diseño]

    TOOLS --> DRIVER[Driver Nativo]
    TOOLS --> EF[MongoDB EF Core]

    DRIVER --> CRUD1[CRUD Builders]
    EF --> CRUD2[LINQ DbSet]

    TEST --> TC[TestContainers Docker]

    style MONGO fill:#f44336,color:#fff
    style NOSQL fill:#4CAF50,color:#fff
    style DESIGN fill:#2196F3,color:#fff
    style TOOLS fill:#FF9800,color:#fff
    style TEST fill:#9C27B0,color:#fff
    style BSON fill:#4CAF50,color:#fff
    style COL fill:#4CAF50,color:#fff
    style DOC fill:#4CAF50,color:#fff
    style EMB fill:#2196F3,color:#fff
    style REF fill:#2196F3,color:#fff
    style PAT fill:#2196F3,color:#fff
    style DRIVER fill:#FF9800,color:#fff
    style EF fill:#9C27B0,color:#fff
    style CRUD1 fill:#EF6C00,color:#fff
    style CRUD2 fill:#7B1FA2,color:#fff
    style TC fill:#7B1FA2,color:#fff
```

| Concepto | Descripción |
|----------|-------------|
| **MongoDB** | Base de datos NoSQL orientada a documentos |
| **Documento** | JSON binario (BSON) con estructura flexible |
| **Colección** | Equivalente a una tabla, pero sin esquema fijo |
| **Embebido** | Datos dentro del mismo documento (Owned) |
| **Referencia** | ID de otro documento en un campo |
| **Driver Nativo** | `MongoDB.Driver` — acceso directo, control total |
| **EF Core** | `MongoDB.EntityFrameworkCore` — ORM, LINQ, abstracción |
| **Owned Types** | `[Owned]`, `OwnsOne()`, `OwnsMany()` — documentos embebidos |
| **BsonId** | Atributo para marcar el `_id` del documento |
| **TestContainers** | MongoDB real en Docker para tests |
| **16 MB** | Límite de tamaño por documento MongoDB |

**¿Qué viene después?**

En el siguiente punto veremos **Redis**: un almacén de datos en memoria ultra-rápido que se usa como caché, cola de mensajes y sesión. Verás cómo el `IDatabase` de StackExchange.Redis se registra como singleton, cómo guardianes datos con TTL (Time-To-Live) y cómo Redis complementa a MongoDB: MongoDB para persistencia, Redis para velocidad.
