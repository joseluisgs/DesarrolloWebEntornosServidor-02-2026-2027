- [13. MongoDB](#13-mongodb)
  - [13.1. Fundamentos NoSQL](#131-fundamentos-nosql)
    - [13.1.1. ¿Qué es MongoDB?](#1311-qué-es-mongodb)
    - [13.1.2. Modelo de documentos](#1312-modelo-de-documentos)
    - [13.1.3. BSON vs JSON](#1313-bson-vs-json)
    - [13.1.4. Colecciones y documentos](#1314-colecciones-y-documentos)
  - [13.2. Diseño NoSQL: de SQL a MongoDB](#132-diseño-nosql-de-sql-a-mongodb)
    - [13.2.1. Tablas a colecciones, filas a documentos](#1321-tablas-a-colecciones-filas-a-documentos)
    - [13.2.2. Claves foráneas: referencias o embebido](#1322-claves-foráneas-referencias-o-embebido)
    - [13.2.3. Cuándo embeber vs cuándo referenciar](#1323-cuándo-embeber-vs-cuándo-referenciar)
    - [13.2.4. El límite de 16 MB](#1324-el-límite-de-16-mb)
    - [13.2.5. Patrones de diseño habituales](#1325-patrones-de-diseño-habituales)
  - [13.3. MongoDB con driver nativo](#133-mongodb-con-driver-nativo)
    - [13.3.1. Paquete NuGet](#1331-paquete-nuget)
    - [13.3.2. Conexión: MongoClient](#1332-conexión-mongoclient)
    - [13.3.3. Database y collection](#1333-database-y-collection)
    - [13.3.4. Documentos POCO y BsonDocument](#1334-documentos-poco-y-bsondocument)
    - [13.3.5. Insertar documentos](#1335-insertar-documentos)
    - [13.3.6. Consultar documentos](#1336-consultar-documentos)
    - [13.3.7. Actualizar documentos](#1337-actualizar-documentos)
    - [13.3.8. Borrar documentos](#1338-borrar-documentos)
    - [13.3.9. Builders de filtros y actualizaciones](#1339-builders-de-filtros-y-actualizaciones)
    - [13.3.10. Índices](#13310-índices)
    - [13.3.11. Composición y referencias con el driver](#13311-composición-y-referencias-con-el-driver)
  - [13.4. EF Core con MongoDB](#134-ef-core-con-mongodb)
    - [13.4.1. Paquete NuGet](#1341-paquete-nuget)
    - [13.4.2. DbContext con UseMongoDB](#1342-dbcontext-con-usemongodb)
    - [13.4.3. Data annotations en MongoDB](#1343-data-annotations-en-mongodb)
    - [13.4.4. Fluent API en MongoDB](#1344-fluent-api-en-mongodb)
    - [13.4.5. Entidades owned (documentos embebidos)](#1345-entidades-owned-documentos-embebidos)
    - [13.4.6. Referencias manuales](#1346-referencias-manuales)
    - [13.4.7. Consultas LINQ](#1347-consultas-linq)
    - [13.4.8. Limitaciones de EF Core con MongoDB](#1348-limitaciones-de-ef-core-con-mongodb)
  - [13.5. Driver nativo vs EF Core: comparativa](#135-driver-nativo-vs-ef-core-comparativa)
  - [13.6. Repositorio CRUD con MongoDB](#136-repositorio-crud-con-mongodb)
    - [13.6.1. Modelo y configuración](#1361-modelo-y-configuración)
    - [13.6.2. Interfaz del repositorio](#1362-interfaz-del-repositorio)
    - [13.6.3. Implementación con driver nativo](#1363-implementación-con-driver-nativo)
    - [13.6.4. Implementación con EF Core](#1364-implementación-con-ef-core)
  - [13.7. Testing con MongoDB](#137-testing-con-mongodb)
    - [13.7.1. Compatibilidad de versiones: EF Core + MongoDB](#1371-compatibilidad-de-versiones-ef-core--mongodb)
    - [13.7.2. TestContainers](#1372-testcontainers)
    - [13.7.3. Tests con driver nativo](#1373-tests-con-driver-nativo)
    - [13.7.4. Tests con EF Core](#1374-tests-con-ef-core)
  - [13.8. Semilla de datos (seed)](#138-semilla-de-datos-seed)
    - [Con driver nativo](#con-driver-nativo)
    - [Con EF Core](#con-ef-core)
  - [13.9. Buenas prácticas](#139-buenas-prácticas)
  - [13.10. Reto](#1310-reto)



# 13. MongoDB

> 💡 **Punto de partida:** ¿Alguna vez has tenido que diseñar una base de datos con 15 tablas y 20 joins solo para guardar una factura? ¿O has sufrido con un ORM relacional que no encaja con tu modelo de datos? MongoDB te propone otra forma: guarda lo que piensas como un documento, sin tablas, sin joins, sin ceremonia.

**Objetivos de aprendizaje:**
- Comprender el modelo NoSQL de documentos y cuándo usarlo
- Diseñar esquemas NoSQL a partir de un modelo relacional
- Trabajar con MongoDB usando el Driver Nativo en C#
- Trabajar con MongoDB usando EF Core para MongoDB
- Entender documentos embebidos vs referencias
- Comparar ambas aproximaciones (Driver vs EF Core)
- Escribir tests con TestContainers

## 13.1. Fundamentos NoSQL

### 13.1.1. ¿Qué es MongoDB?

MongoDB es una base de datos **NoSQL orientada a documentos**. A diferencia de MySQL o PostgreSQL, no guarda datos en tablas con filas y columnas, sino en **documentos** dentro de **colecciones**. Cada documento es un JSON (o BSON) flexible que puede tener estructuras diferentes dentro de la misma colección.

> 💡 **Analogía:** MongoDB es como un cajón de carpetas donde cada carpeta puede tener un formato diferente. En SQL, todos los documentos deben seguir la misma plantilla (como formularios oficiales). En MongoDB, cada carpeta es libre de incluir las secciones que quieras.

📌 **Ejemplo real:** Instagram usa MongoDB para almacenar perfiles de usuario, donde cada usuario puede tener campos diferentes (unos tienen blog, otros tienda, otros solo fotos). No tendría sentido forzar a todos a tener las mismas columnas.

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

## 13.2. Diseño NoSQL: de SQL a MongoDB

### 13.2.1. Tablas a colecciones, filas a documentos

El equivalente directo:

| SQL | MongoDB | Ejemplo |
|-----|---------|---------|
| `CREATE TABLE productos` | `db.createCollection('productos')` | Crear colección |
| `INSERT INTO productos (...)` | `db.productos.insertOne({...})` | Insertar documento |
| `SELECT * FROM productos WHERE id = 1` | `db.productos.findOne({_id: 1})` | Buscar por ID |
| `UPDATE productos SET precio = 90 WHERE id = 1` | `db.productos.updateOne({_id: 1}, {$set: {precio: 90}})` | Actualizar |
| `DELETE FROM productos WHERE id = 1` | `db.productos.deleteOne({_id: 1})` | Borrar |

### 13.2.2. Claves foráneas: referencias o embebido

En SQL, las relaciones se hacen con claves foráneas y JOINs. En MongoDB, hay **dos formas**. Para ver la diferencia, mira el mismo dominio dibujado de las tres maneras:

```mermaid
graph TD
    subgraph REL["SQL: 3 tablas + FK"]
        T1[productos] -->|categoria_id| T2[categorias]
        T1 -->|proveedor_id| T3[proveedores]
    end

    subgraph EMB["MongoDB embebido: 1 documento"]
        D1["producto: _id, nombre, categoria: { ... }"]
    end

    subgraph REF["MongoDB referenciado: 2 colecciones"]
        D2["producto: _id, nombre, proveedor_id"] -->|ObjectId| P1[proveedor]
    end

    style REL fill:#2196F3,color:#fff
    style EMB fill:#4CAF50,color:#fff
    style REF fill:#FF9800,color:#fff
    style T1 fill:#2196F3,color:#fff
    style T2 fill:#2196F3,color:#fff
    style T3 fill:#2196F3,color:#fff
    style D1 fill:#4CAF50,color:#fff
    style D2 fill:#FF9800,color:#fff
    style P1 fill:#FF9800,color:#fff
```

**Referencia** (estilo SQL):
```json
// Colección "productos"
{ "_id": ObjectId("..."), "nombre": "Teclado", "proveedor_id": ObjectId("64b000000000000000000001") }

// Colección "proveedores"
{ "_id": ObjectId("64b000000000000000000001"), "nombre": "Logitech" }
```

**Documento embebido** (estilo NoSQL):
```json
// Colección "productos" - la categoría está DENTRO del producto
{
  "_id": ObjectId("..."),
  "nombre": "Teclado",
  "categoria": {
    "nombre": "Electrónica",
    "descripcion": "Componentes electrónicos"
  }
}
```

En C# la diferencia se ve en **el tipo de la propiedad**:

**Referencia** — la propiedad es un `ObjectId`:

```csharp
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Producto
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("nombre")]
    public string Nombre { get; set; } = string.Empty;

    // REFERENCIA: solo el _id del proveedor viaja dentro del producto
    [BsonElement("proveedor_id")]
    public ObjectId ProveedorId { get; set; }
}

public class Proveedor
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [BsonElement("contacto")]
    public string Contacto { get; set; } = string.Empty;
}
```

El producto ocupa **exactamente lo mismo** con o sin proveedor: solo cambia un campo de 12 bytes.

**Composición** — la propiedad es **otro objeto**:

```csharp
public class Producto
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("nombre")]
    public string Nombre { get; set; } = string.Empty;

    // COMPOSICIÓN: la categoría vive DENTRO del producto
    [BsonElement("categoria")]
    public Categoria Categoria { get; set; } = null!;
}

public class Categoria
{
    [BsonElement("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [BsonElement("descripcion")]
    public string Descripcion { get; set; } = string.Empty;
}
```

> 💡 **Regla rápida:** mira la propiedad. Si es `ObjectId` → **referencia**. Si es una clase → **composición**. El `[BsonElement(...)]` solo dice cómo se llama el campo en BSON; no cambia el comportamiento.

| | Composición (`Categoria`) | Referencia (`ProveedorId`) |
|---|---|---|
| **Tipo en C#** | clase `Categoria` | `ObjectId` |
| **En BSON** | subdocumento `{ ... }` | `ObjectId("...")` |
| **Colecciones necesarias** | 1 | 2 |
| **Consultas para leerlo** | 1 | 2 |

📌 **Ejemplo real:** Amazon guarda la categoría dentro del producto (se lee siempre junto), pero el vendedor va en otra colección: millones de productos comparten vendedor y este cambia nombre, email y dirección constantemente.

### 13.2.3. Cuándo embeber vs cuándo referenciar

Esta es **la decisión más importante** al diseñar en NoSQL, y no se toma mirando el modelo: se toma mirando las consultas. El método tiene cuatro pasos:

1. **Lista las lecturas de la app**: qué pide cada vista o endpoint (ficha de producto, listado, detalle de pedido).
2. **Anota qué datos viajan juntos** en cada respuesta. Lo que se pide casi siempre en el mismo rato es candidato a ir embebido.
3. **Cuenta la cardinalidad**: cuántos elementos hay en el lado de "muchos" y cuántos documentos comparten el dato del lado de "uno". Si no sabes ponerle techo a un array, es que no es "pocos".
4. **Comprueba consistencia y coste**: si los datos deben verse siempre como un bloque, embebe; si un dato compartido cambia a menudo, reférencia.

Con los cuatro pasos en la mano, el diagrama de decisión hace el resto:

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

La primera pata del criterio es la **cardinalidad**: el tamaño del lado "muchos" de la relación, que es la que manda sobre el tamaño final:

| Cardinalidad | Ejemplo | Decisión |
|--------------|---------|----------|
| **1:pocos** (decenas, con techo claro) | Producto → su categoría | Embeber |
| **1:limitado** (cientos, crece con freno) | Chat → últimos 100 mensajes | Subset |
| **1:ilimitado** (miles o más, sin freno) | Producto → millones de logs | Referenciar |
| **N:1** (muchos comparten uno) | 10.000 productos → 1 proveedor | Referenciar |
| **N:M** (cruce de los dos lados) | Producto → varias categorías | Referenciar |

Regla práctica: si no eres capaz de decir "como mucho X elementos", no es "pocos": no lo embebas.

La segunda es la **consistencia**. Un documento es la unidad atómica de MongoDB: leerlo entero es una foto coherente de todos sus campos, y un `updateOne` sobre él es un todo o nada. Si el precio y el descuento van dentro del mismo documento, nadie lee un precio actualizado con un descuento viejo. Con dos colecciones son dos consultas en dos instantes: entre medias, el dato referenciado puede cambiar. Esa es la consistencia eventual que te juegas al referenciar.

Y la fila de "cambian a menudo" esconde una cuenta sencilla: si 10.000 productos llevan embebido el email del proveedor y este lo cambia, hay que reescribir 10.000 documentos; referenciado, se toca uno solo. Con muchas lecturas juntas gana lo embebido; con escrituras frecuentes sobre un dato compartido, gana lo referenciado.

| Criterio | Embeber | Referenciar |
|----------|---------|-------------|
| **Se leen siempre juntos** | ✅ Una sola consulta | ❌ Varias consultas |
| **Lectura coherente** | ✅ Foto de un solo instante | ❌ Dos consultas, dos instantes |
| **Dato compartido que cambia a menudo** | ❌ Reescribir N documentos | ✅ Una sola escritura |
| **Colección que crece sin límite** | ❌ No | ✅ Sí |
| **Se consulta por su cuenta** | ❌ No | ✅ Sí |
| **Relación 1:N con pocos elementos** | ✅ Sí | ⚠️ Opcional |
| **Relación N:M** | ❌ No | ✅ Sí |
| **Datos duplicados aceptables** | ✅ Sí | ❌ No |

```csharp
// ❌ MALO: modelo pensado en SQL — ids por todas partes y tres consultas por lectura
var producto = productos.Find(p => p.Id == id).First();
var categoria = categorias.Find(c => c.Id == producto.CategoriaId).First();     // 2.ª consulta
var proveedor = proveedores.Find(pr => pr.Id == producto.ProveedorId).First();  // 3.ª consulta

// ✅ BUENO: embeber lo que la ficha enseña siempre junto
var producto = productos.Find(p => p.Id == id).First();
// El producto ya trae Categoria { ... } dentro: la ficha sale en una sola consulta
// y todos sus campos son del mismo instante. El proveedor, que decidió la tabla
// de arriba, se queda referenciado y se resuelve solo si la vista lo muestra.
```

```csharp
// ❌ MALO: embeber una colección que crece sin límite
producto.Resenas = resenasDeTodaLaVida;  // array sin freno
productos.InsertOne(producto);           // con 16 MB, el servidor responde con
                                         // el error 10334 (BSONObjectTooLarge)

// ✅ BUENO: Subset Pattern — embeber solo los más recientes
var producto = productos.Find(p => p.Id == id).First();
// El producto ya trae ResenasRecientes [las últimas 5] y ResenaCount;
// el resto vive en una colección aparte y se consulta si hace falta
```

> 💡 **Analogía:** Piensa en MongoDB como una carpeta de expediente. Puedes pegar notas, fotos y documentos dentro de la misma carpeta (embeber). Pero si la carpeta crece demasiado, necesitarás un archivador separado (referenciar) y solo dejar un Post-It con la referencia.

📌 **Ejemplo real:** Netflix embebe la lista de "episodios" dentro de cada "serie" porque siempre se ven juntos. Pero usa referencias para los "actores" porque un actor aparece en múltiples series.

Aplicado a nuestro modelo de productos, la decisión queda así:

```mermaid
graph LR
    P[Producto] -->|"embebido · 1 consulta"| C[Categoria]
    P -->|"referencia · 2 consultas"| PR[Proveedor]

    style P fill:#2196F3,color:#fff
    style C fill:#4CAF50,color:#fff
    style PR fill:#FF9800,color:#fff
```

| Relación | Decisión | ¿Por qué? |
|----------|----------|-----------|
| **Producto → Categoría** | ✅ **Embeber** | Se lee siempre junto, es pequeña y casi nunca cambia |
| **Producto → Proveedor** | ✅ **Referenciar** | Miles de productos comparten proveedor, y este cambia nombre, contacto y país |

> 📝 **Nota:** ¿Y si ya tienes referencias y de pronto necesitas los datos juntos? MongoDB ofrece `$lookup`, el equivalente a un LEFT JOIN dentro de un pipeline de agregación. Funciona, pero se evalúa con menos potencia que un JOIN relacional, y cada lectura paga así una decisión de diseño pospuesta. Úsalo como salida puntual, no como plan de diseño.

> ⚠️ **Advertencia:** la pregunta no es "¿es una relación?", sino **"¿cuándo y cómo se leen estos datos?"**. Si decides por la forma de los datos en SQL, acabarás recreando tablas y JOINs dentro de un documento, y perderás lo que MongoDB aporta.

### 13.2.4. El límite de 16 MB

MongoDB tiene un **límite de 16 MB por documento**. No es un capricho: el documento es lo que viaja en una sola lectura, y sin techo una consulta podría cargar medio catálogo en memoria. Si embebes un array que crece sin freno (logs, mensajes, lecturas de sensor), el día que pase de 16 MB el servidor rechaza la escritura con el error `10334` (`BSONObjectTooLarge`), que el driver .NET devuelve como `MongoCommandException`.

El tamaño no es el único problema: un array con millones de elementos hace lenta cualquier operación que lo toque, porque MongoDB reescribe el documento entero para modificar un solo campo. Antes de embeber un array, pregúntate qué pasará con ese mismo array dentro de dos años.

Para medir dónde estás, el propio servidor te devuelve el tamaño de cada documento:

```javascript
// Tamaño en bytes de cada documento de la colección
db.productos.aggregate([
  { $project: { nombre: 1, tamano: { $bsonSize: "$$ROOT" } } }
])
```

Cuando un array se acerque a su techo (o simplemente ya no quepa), la respuesta es el **Subset Pattern**: guarda dentro del documento solo los últimos N elementos, los que se leen siempre, y deja el resto en una colección aparte. Es uno de los patrones del apartado siguiente.

### 13.2.5. Patrones de diseño habituales

| Patrón | Descripción | Ejemplo |
|--------|-------------|---------|
| **Subset** | Embebe solo los datos más recientes/frecuentes | Últimos 10 mensajes de un chat |
| **Outlier** | Separa documentos inusuales en otra colección | Producto con 1000 fotos vs los normales con 3 |
| **Bucket** | Agrupa documentos por tiempo/tamaño | Sensor que guarda lecturas por hora |
| **Schema Versioning** | Versiona el esquema para migraciones | v1: nombre, v2: nombre + email |
| **Computed** | Pre-calcula datos que se leen mucho | Total de ventas pre-calculado |

## 13.3. MongoDB con driver nativo

### 13.3.1. Paquete NuGet

Para usar el driver de MongoDB necesitas instalar el paquete NuGet oficial. Es el punto de partida para trabajar con MongoDB en .NET.

```bash
dotnet add package MongoDB.Driver
```

> 📝 **Nota:** El driver nativo es el paquete oficial de MongoDB para .NET. Es la forma más directa y completa de trabajar con MongoDB.

### 13.3.2. Conexión: MongoClient

`MongoClient` es la clase que gestiona la conexión con el servidor MongoDB. Se crea una sola vez y se reutiliza en toda la aplicación.

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

### 13.3.3. Database y collection

La conexión con MongoDB se organiza en database y collections. Una database contiene varias collections, y cada collection guarda documentos JSON.

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
// ❌ MALO: Usar BsonDocument en producción — sin tipado, errores en ejecución
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

Insertar documentos en MongoDB es similar a añadir registros en SQL, pero trabajas con objetos JSON directamente.

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

Las consultas en MongoDB usan filtros con la sintaxis del driver, que traduce a JSON Query.

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

Actualizar documentos permite modificar campos específicos sin reemplazar el documento completo.

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

Borrar documentos elimina registros de la collection según un filtro.

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
    // Ruta de campo: se puede filtrar dentro del subdocumento embebido
    Builders<Producto>.Filter.Eq("categoria.nombre", "Electrónica"),
    Builders<Producto>.Filter.Eq("categoria.nombre", "Gaming")
);

// Proyecciones (seleccionar campos)
var proyeccion = Builders<Producto>.Projection
    .Include(p => p.Nombre)
    .Include(p => p.Precio)
    .Exclude(p => p.Id);

var resultados = collection.Find(filtro1).Project(proyeccion).ToList();
```

### 13.3.10. Índices

Los índices en MongoDB aceleran las consultas igual que en SQL. Sin ellos, cada consulta recorre todos los documentos de la collection.

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

// Índice único
collection.Indexes.CreateOne(new CreateIndexModel<Producto>(
    Builders<Producto>.IndexKeys.Ascending(p => p.Nombre),
    new CreateIndexOptions { Unique = true }));
```

### 13.3.11. Composición y referencias con el driver

Las dos formas conviven en el mismo modelo. La regla de oro: **primero lo referenciado, después lo compuesto**.

```csharp
using MongoDB.Bson;
using MongoDB.Driver;

var productos = database.GetCollection<Producto>("productos");
var proveedores = database.GetCollection<Proveedor>("proveedores");

// 1) Primero el referenciado: si no existe, el producto queda colgado
var proveedor = new Proveedor
{
    Id = ObjectId.Parse("64b000000000000000000001"),
    Nombre = "Logitech",
    Contacto = "exportaciones@logitech.com"
};
proveedores.InsertOne(proveedor);

// 2) Después el compuesto, apuntando al proveedor ya creado
var producto = new Producto
{
    Nombre = "Teclado Mecánico",
    Precio = 89.99m,
    Categoria = new Categoria { Nombre = "Electrónica", Descripcion = "Componentes" },
    ProveedorId = proveedor.Id
};
productos.InsertOne(producto);
```

Lo que queda en MongoDB:

```json
{
  "_id": ObjectId("6abf69e28f1db051c52c96d9"),
  "nombre": "Teclado Mecánico",
  "precio": NumberDecimal("89.99"),
  "categoria": { "nombre": "Electrónica", "descripcion": "Componentes" },
  "proveedor_id": ObjectId("64b000000000000000000001"),
  "is_deleted": false
}
```

Fíjate en la diferencia: `categoria` es un **subdocumento completo** y `proveedor_id` es **solo un `ObjectId`**.

**Resolver la referencia** — dos consultas, porque no hay JOINs:

```csharp
var producto = productos.Find(p => p.Id == productoId).FirstOrDefault();
var proveedor = producto is null
    ? null
    : proveedores.Find(p => p.Id == producto.ProveedorId).FirstOrDefault();
```

**Consultar y actualizar por la referencia:**

```csharp
// Productos de un proveedor concreto
var delProveedor = productos.Find(p => p.ProveedorId == proveedorId).ToList();

// Cambiar de proveedor: solo se toca un campo, no el documento entero
productos.UpdateOne(
    p => p.Id == productoId,
    Builders<Producto>.Update.Set(p => p.ProveedorId, otroProveedorId));
```

> 💡 **Consejo:** si cada página de listado necesita el nombre del proveedor, estás pagando **2 consultas por página**. Ese es exactamente el motivo de decidir bien en el apartado 13.2.3: lo que se lee siempre junto, se embebe.

📌 **Ejemplo real:** Glovo embebe dentro del restaurante sus horarios y su carta (se leen juntos en cada pantalla), pero el repartidor va referenciado: se asigna y cambia en cada pedido sin tocar el documento del restaurante.

## 13.4. MongoDB con EF Core

### 13.4.1. Paquete NuGet

Para usar EF Core con MongoDB necesitas el proveedor oficial, que permite trabajar con el mismo patrón que con SQL.

```bash
dotnet add package MongoDB.EntityFrameworkCore
```

> 📝 **Nota:** El proveedor EF Core para MongoDB es oficial (desarrollado por MongoDB, Inc). Permite usar la misma sintaxis de EF Core que usas con PostgreSQL o SQLite, pero guardando en MongoDB.

### 13.4.2. DbContext con UseMongoDB

El DbContext con UseMongoDB configura la conexión a MongoDB siguiendo el mismo patrón que con Entity Framework y SQL.

```csharp
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

public class ProductosMongoDbContext(DbContextOptions<ProductosMongoDbContext> options)
    : DbContext(options)
{
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Proveedor> Proveedores => Set<Proveedor>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // MongoDB no necesita migraciones: una colección por tipo de documento.
        // La categoría no aparece: va embebida dentro de Producto (apartado 13.4.5)
        modelBuilder.Entity<Producto>().ToCollection("productos");
        modelBuilder.Entity<Proveedor>().ToCollection("proveedores");
    }
}

// Registro en Program.cs / DI
var client = new MongoClient("mongodb://localhost:27017");

services.AddDbContext<ProductosMongoDbContext>(options =>
    options.UseMongoDB(client, "tienda"));
```

### 13.4.3. Data annotations en MongoDB

Las data annotations funcionan igual con MongoDB que con SQL: configuran el modelo directamente en las entidades.

```csharp
using System.ComponentModel.DataAnnotations;
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

    // Composición: el objeto entero se serializa como subdocumento
    [BsonElement("categoria")]
    public Categoria Categoria { get; set; } = null!;

    // Referencia: solo el ObjectId viaja en el campo indicado
    [BsonElement("proveedor_id")]
    public ObjectId ProveedorId { get; set; }
}
```

> 📝 **Nota:** En MongoDB, los Data Annotations de EF Core (`[Required]`, `[StringLength]`) funcionan para validación en C#, pero **no se aplican en la BD**. MongoDB no tiene constraints como SQL. La `[BsonId]` y `[BsonElement]` son las que controlan el mapeo real.

### 13.4.4. Fluent API en MongoDB

La fluent API en MongoDB permite configurar colecciones, índices y serialización con el mismo patrón que en SQL.

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

### 13.4.5. Entidades owned (documentos embebidos)

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

> ⚠️ **Advertencia:** los tipos Owned **no se pueden compartir**. Si dos productos apuntan al **mismo objeto** `Categoria`, EF Core solo la guarda en uno de ellos y los demás se quedan con `"categoria": null`. Cada producto necesita **su propia instancia**:

```csharp
// ❌ MALO: la misma instancia compartida — solo UN producto guardará la categoría
var categorias = new[]
{
    new Categoria { Nombre = "Electrónica", Descripcion = "Componentes" },
    new Categoria { Nombre = "Mobiliario", Descripcion = "Muebles" }
};

var productos = new List<Producto>
{
    new() { Nombre = "Teclado", Categoria = categorias[0] },
    new() { Nombre = "Ratón", Categoria = categorias[0] }   // ¡misma instancia!
};
// Resultado en MongoDB: el Ratón queda con "categoria": null

// ✅ BUENO: cada producto con su propia instancia
var productosOk = new List<Producto>
{
    new()
    {
        Nombre = "Teclado",
        Categoria = new Categoria { Nombre = "Electrónica", Descripcion = "Componentes" }
    },
    new()
    {
        Nombre = "Ratón",
        Categoria = new Categoria { Nombre = "Electrónica", Descripcion = "Componentes" }
    }
};
```

> 📝 **Nota:** el **Driver Nativo** no tiene este problema: serializa el objeto tal cual esté. La restricción es de EF Core, que lleva la cuenta de las entidades en su *change tracker* y asume que cada instancia Owned pertenece a una sola entidad propietaria.

📌 **Ejemplo real:** en `12-ProductosMongo`, el seeder construye la categoría con `NuevaCategoria(indice)` en lugar de reutilizar un array de objetos, precisamente por esto.

### 13.4.6. Referencias manuales

Cuando los datos cambian a menudo o se consultan independientemente, usa **referencias** en vez de embeber. En EF Core se hace con **dos `DbSet`** y una propiedad escalar `ObjectId`:

```csharp
using Microsoft.EntityFrameworkCore;

public class ProductosMongoDbContext(DbContextOptions<ProductosMongoDbContext> options)
    : DbContext(options)
{
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Proveedor> Proveedores => Set<Proveedor>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Producto>().ToCollection("productos");
        modelBuilder.Entity<Proveedor>().ToCollection("proveedores");
    }
}
```

La propiedad de referencia es un simple `ObjectId`. **No hay navegación, ni `[ForeignKey]`, ni `[InverseProperty]`**: en MongoDB eso no existe.

```csharp
public class Producto
{
    [BsonId]
    public ObjectId Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    // REFERENCIA: solo el _id. Sin objeto anidado y sin [ForeignKey]
    [BsonElement("proveedor_id")]
    public ObjectId ProveedorId { get; set; }

    // COMPOSICIÓN: objeto con [Owned] (apartado 13.4.5)
    public Categoria Categoria { get; set; } = null!;
}
```

Para obtener el proveedor, **dos consultas a mano**:

```csharp
var producto = await db.Productos.FirstOrDefaultAsync(p => p.Id == id);
var proveedor = producto is null
    ? null
    : await db.Proveedores.FirstOrDefaultAsync(p => p.Id == producto.ProveedorId);
```

Y para filtrar por la referencia, LINQ normal:

```csharp
var delProveedor = await db.Productos
    .Where(p => p.ProveedorId == proveedorId && !p.IsDeleted)
    .ToListAsync();
```

> ⚠️ **Advertencia:** a diferencia de SQL, MongoDB **no tiene JOINs** ni `Include()` como en EF Core relacional. Cada consulta es independiente. Si necesitas los datos juntos, embebe.

> 💡 **Truco:** si la referencia puede no existir, haz la segunda consulta solo cuando la primera haya devuelto algo:

```csharp
if (producto is not null)
{
    var proveedor = await db.Proveedores
        .FirstOrDefaultAsync(p => p.Id == producto.ProveedorId);
}
```

📌 **Ejemplo real:** en el ejemplo `12-ProductosMongo`, `GET /api/productos` devuelve la categoría embebida en la misma consulta y solo el `proveedorId`: la API no resuelve el proveedor porque no lo necesita.

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
| **Transacciones** | ⚠️ Solo con réplica set | En un servidor **standalone** salta `NotSupportedException` |
| **Instancias Owned compartidas** | ⚠️ No | Dos entidades no pueden apuntar al mismo objeto (13.4.5) |

> 📝 **Nota:** El proveedor MongoDB EF Core está en desarrollo activo y algunas funciones llegarán con el tiempo. La página [Limitaciones de MongoDB.EntityFrameworkCore](https://www.mongodb.com/es/docs/entity-framework/current/limitations/) mantiene al día el estado de la versión que estés usando.

> ⚠️ **Advertencia — transacciones en standalone:** el provider EF Core envuelve cada `SaveChanges` con varios documentos en una **transacción**, y un MongoDB standalone (el de nuestro `docker-compose.yml`) no las soporta. Si necesitas sembrar o escribir varios documentos de golpe sin montar un replica set, puedes desactivarlas:

```csharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    // Standalone: sin transacciones. En producción con réplica set, quita esta línea.
    Database.AutoTransactionBehavior = AutoTransactionBehavior.Never;
}
```

> 💡 **Consejo:** una única escritura (`Add` + `SaveChanges` de UN producto) funciona igualmente sin tocar nada. Este ajuste solo hace falta cuando agrupas varios documentos en la misma operación.

## 13.5. Driver nativo vs EF Core: comparativa

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
    style E3 fill:#8E24A0,color:#fff
    style E4 fill:#f44336,color:#fff
    style DEC fill:#607D8B,color:#fff
```

| Criterio | Driver Nativo | EF Core |
|----------|--------------|---------|
| **Control total** | ✅ Sí, acceso directo a la API de MongoDB | ⚠️ Limitado por el ORM |
| **Funciones MongoDB** | ✅ Todas (agregaciones, geo, text search) | ❌ Solo las que el proveedor soporta |
| **Sintaxis familiar** | ❌ Builders y métodos específicos de MongoDB | ✅ LINQ, como con SQL |
| **Cambio de BD** | ❌ Acoplado a MongoDB | ⚠️ LINQ y repositorio se reutilizan, el modelo no se traslada solo |
| **Migraciones** | ❌ No existen | ❌ No soportadas (pero el ORM sí las gestiona en SQL) |
| **Owned Types** | ❌ A mano | ✅ `[Owned]` y `OwnsOne/OwnsMany` |
| **Performance** | ✅ Máxima (sin overhead de ORM) | ⚠️ Alguna pérdida por abstracción |
| **Documentos embebidos** | ✅ Total control | ✅ Con Owned Types |
| **Complejidad** | Mayor, más código | Menos código, más declarativo |
| **Testing** | TestContainers | TestContainers + InMemory |
| **Curva aprendizaje** | Baja si conoces MongoDB | Baja si conoces EF Core |

> 💡 **Consejo:** Si tu aplicación es **100% MongoDB** y necesitas funcionalidades avanzadas (agregaciones, geo-queries, Atlas Search), usa el **Driver Nativo**. Si vienes de EF Core relacional y quieres una capa de abstracción más uniforme (o piensas cambiar de BD a futuro), usa **EF Core**.

📌 **Ejemplo real:** eBay usa MongoDB para su catálogo de millones de artículos, donde cada categoría tiene atributos propios (talla, color, estado). Las búsquedas del catálogo combinan filtros, facetas y agregaciones, justo el tipo de consulta que se escribe directamente con el driver.

## 13.6. Repositorio CRUD con MongoDB

### 13.6.1. Modelo y configuración

El repositorio trabaja sobre el mismo `Producto` que diseñó el apartado 13.2.2, con sus dos formas de relación más los campos de control que pide el CRUD:

```csharp
// Modelo
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

    [BsonElement("imagen")]
    public string Imagen { get; set; } = string.Empty;

    // COMPOSICIÓN: la categoría vive DENTRO del producto (apartado 13.4.5)
    [BsonElement("categoria")]
    public Categoria Categoria { get; set; } = null!;

    // REFERENCIA: solo el _id del proveedor viaja en el producto (apartado 13.4.6)
    [BsonElement("proveedor_id")]
    public ObjectId ProveedorId { get; set; }

    [BsonElement("is_deleted")]
    public bool IsDeleted { get; set; }

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}
```

### 13.6.2. Interfaz del repositorio

La interfaz del repositorio define las operaciones CRUD que el almacén de MongoDB ofrece.

```csharp
public interface IProductoRepository
{
    IEnumerable<Producto> GetAll();
    Producto? GetById(ObjectId id);
    Producto Add(Producto producto);
    Producto? Update(Producto producto);
    bool Delete(ObjectId id);
    bool IsDuplicated(string nombre, ObjectId excludeId = default);
}
```

### 13.6.3. Implementación con driver nativo

La implementación con el driver nativo de MongoDB usa las clases del driver directamente, sin EF Core.

```csharp
using MongoDB.Driver;

public class ProductoMongoRepository(IMongoDatabase database) : IProductoRepository
{
    private readonly IMongoCollection<Producto> _collection = database.GetCollection<Producto>("productos");

    public IEnumerable<Producto> GetAll() =>
        _collection.Find(p => !p.IsDeleted).ToList();

    public Producto? GetById(ObjectId id) =>
        _collection.Find(p => p.Id == id && !p.IsDeleted).FirstOrDefault();

    public Producto Add(Producto producto)
    {
        producto.CreatedAt = DateTime.UtcNow;
        _collection.InsertOne(producto);
        return producto;
    }

    public Producto? Update(Producto producto)
    {
        producto.UpdatedAt = DateTime.UtcNow;
        var result = _collection.ReplaceOne(p => p.Id == producto.Id, producto);
        return result.IsAcknowledged ? producto : null;
    }

    public bool Delete(ObjectId id)
    {
        var update = Builders<Producto>.Update
            .Set(p => p.IsDeleted, true);
        var result = _collection.UpdateOne(p => p.Id == id, update);
        return result.IsAcknowledged && result.ModifiedCount > 0;
    }

    public bool IsDuplicated(string nombre, ObjectId excludeId = default) =>
        _collection.Find(p => p.Nombre == nombre && p.Id != excludeId).Any();
}
```

**Atrapar el `E11000` cuando el upsert es intencionado**

Si añades un índice único (sección 13.3.10) y haces un *upsert* con `ReplaceOne(..., IsUpsert = true)` o `UpdateOne(...)` con `IsUpsert`, MongoDB puede devolverte el error **`E11000 duplicate key error`**. Ojo: **no siempre es un bug**.

Cuando existe una *guarda de versión* ("solo sobrescribe si el documento guardado no es más reciente que el entrante"), el filtro no coincide con el documento existente, así que el driver intenta **insertar** y se choca contra el `_id` ya presente:

```csharp
// Filtro con guarda de versión: solo entra si el guardado NO es más reciente
var filter = Builders<ProductoRead>.Filter.Eq(p => p.Id, producto.Id)
           & Builders<ProductoRead>.Filter.Lte(p => p.UpdatedAt, producto.UpdatedAt);

try
{
    var result = await Collection.ReplaceOneAsync(
        filter,
        producto,
        new ReplaceOptions { IsUpsert = true });

    if (result.MatchedCount == 0 && result.UpsertedId is null)
        logger.LogDebug("Upsert omitido (dato más reciente ya presente): {Id}", producto.Id);
    else
        logger.LogDebug("Producto sincronizado en MongoDB (upsert): {Id}", producto.Id);
}
catch (MongoWriteException ex) when (ex.WriteError?.Code == 11000)
{
    // E11000: el documento existente tiene UpdatedAt más reciente,
    // el filtro no coincide y el upsert intenta insertar con _id duplicado.
    // Es el comportamiento esperado de la guardia de versión — omitir a propósito.
    logger.LogDebug("Upsert omitido por guardia de versión (E11000): {Id}", producto.Id);
}
```

Este patrón es típico cuando MongoDB funciona como réplica de lectura de una base SQL: cada sincronización refresca el documento con un upsert, y la guarda de versión convierte el intento en una inserción que choca contra el `_id` existente. No es un fallo: es la guarda cumpliendo su función.

> 📝 **Nota:** La clave está en el **`when`** (`ex.WriteError?.Code == 11000`). Sin ese filtro, el `catch` se tragaría **todos** los errores de escritura (red caída, permisos insuficientes, documento demasiado grande) y los convertiría en "éxitos". Con el filtro, solo se ignora el `11000` y el resto sigue saltando.

> ⚠️ **Advertencia:** Antes de dar por bueno un `E11000`, comprueba **qué índice** lo ha provocado. Si **no** tienes guardia de versión y no has tocado el esquema, ese duplicado es un problema real de integridad (dos escrituras simultáneas, ID mal generado…). Ignorar un duplicado sin leer `ex.WriteError` es como cerrar el ojo al humo porque "a veces humea".

> 💡 **Truco:** `ex.WriteError?.Code` devuelve el código numérico (`11000`); si prefieres no dejar números mágicos en el código, usa `ex.WriteError?.Category == WriteErrorCategory.DuplicateKey`. Es la misma condición, pero se lee sola.

### 13.6.4. Implementación con EF Core

La implementación con EF Core permite usar el mismo patrón que con SQL, pero sobre MongoDB: el contexto `ProductosMongoDbContext` de la sección 13.4.6 y LINQ en lugar de Builders.

```csharp
using Microsoft.EntityFrameworkCore;

public class ProductoEfCoreRepository(ProductosMongoDbContext db) : IProductoRepository
{
    public IEnumerable<Producto> GetAll() =>
        db.Productos.Where(p => !p.IsDeleted).ToList();

    public Producto? GetById(ObjectId id) =>
        db.Productos.FirstOrDefault(p => p.Id == id && !p.IsDeleted);

    public Producto Add(Producto producto)
    {
        producto.CreatedAt = DateTime.UtcNow;
        db.Productos.Add(producto);
        db.SaveChanges();
        return producto;
    }

    public Producto? Update(Producto producto)
    {
        var existente = db.Productos.Find(producto.Id);
        if (existente is null) return null;

        existente.Nombre = producto.Nombre;
        existente.Precio = producto.Precio;
        existente.Imagen = producto.Imagen;
        existente.UpdatedAt = DateTime.UtcNow;

        db.SaveChanges();
        return existente;
    }

    public bool Delete(ObjectId id)
    {
        var producto = db.Productos.Find(id);
        if (producto is null) return false;

        producto.IsDeleted = true;
        db.SaveChanges();
        return true;
    }

    public bool IsDuplicated(string nombre, ObjectId excludeId = default) =>
        db.Productos.Any(p => p.Nombre == nombre && p.Id != excludeId);
}
```

## 13.7. Testing con MongoDB

### 13.7.1. Compatibilidad de versiones: EF Core + MongoDB

> ⚠️ **Advertencia — ¡Ojo con las versiones!**
>
> `MongoDB.EntityFrameworkCore` depende de una versión **exacta** de `Microsoft.EntityFrameworkCore`. Si usas versiones incompatibles, obtendrás errores en ejecución como `MissingMethodException` o `FileNotFoundException`.
>
> **Tabla de compatibilidad entre versiones:**
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
> // Resultado: MissingMethodException en ejecución
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
    TC->+D: docker run mongo:7.0
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
        .WithImage("mongo:7.0")
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
> **1. El contenedor como campo de instancia, nunca `static`**
>
> Si el contenedor es `static readonly`, se comparte entre todos los `[TestFixture]` de la solución. El problema es el ciclo de vida: el primer fixture que termina lo destruye en su `[OneTimeTearDown]`, y el siguiente arranca con un contenedor ya eliminado → errores raros.
>
> ```csharp
> // ❌ MALO: static readonly — compartido entre fixtures, se destruye antes de tiempo
> public abstract class MongoTestBase : IAsyncLifetime
> {
>     private static readonly MongoDbContainer _mongo = new MongoDbBuilder()
>         .WithImage("mongo:7.0").Build();
> }
>
> // ✅ BUENO: instance field — cada fixture obtiene su propio contenedor
> public abstract class MongoTestBase : IAsyncLifetime
> {
>     private readonly MongoDbContainer _mongo = new MongoDbBuilder()
>         .WithImage("mongo:7.0").Build();
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
>     Database.GetCollection<BsonDocument>("proveedores").DeleteMany(FilterDefinition<BsonDocument>.Empty);
> }
> ```
>
> 🔧 **Truco:** MongoDB no tiene `TRUNCATE ... RESTART IDENTITY` porque los IDs son `ObjectId` (generados por el driver, no por la BD). La limpieza con `DeleteMany` es suficiente.

### 13.7.3. Tests con driver nativo

Los tests con el driver nativo prueban las operaciones CRUD directamente contra una base de datos real en Docker.

```csharp
[TestFixture]
public class ProductoRepositoryTests : MongoTestBase
{
    private ProductoMongoRepository _repository = null!;

    [SetUp]
    public void SetUp()
    {
        // Limpieza entre tests: cada test arranca con la colección vacía
        // (si no, los documentos de tests anteriores contaminan los asserts)
        Database.GetCollection<BsonDocument>("productos")
            .DeleteMany(FilterDefinition<BsonDocument>.Empty);

        _repository = new ProductoMongoRepository(Database);
    }

    [Test]
    public void Add_ProductoValido_RetornaConId()
    {
        // Arrange
        var producto = new Producto
        {
            Nombre = "Teclado Mecánico",
            Precio = 89.99m,
            Categoria = new Categoria { Nombre = "Electrónica", Descripcion = "Componentes" }
        };

        // Act
        var resultado = _repository.Add(producto);

        // Assert
        resultado.Id.Should().NotBe(ObjectId.Empty);
        resultado.Nombre.Should().Be("Teclado Mecánico");
    }

    [Test]
    public void GetById_ProductoExiste_RetornaProducto()
    {
        // Arrange
        var producto = _repository.Add(new Producto
        {
            Nombre = "Monitor",
            Precio = 299.00m,
            Categoria = new Categoria { Nombre = "Electrónica", Descripcion = "Componentes" }
        });

        // Act
        var resultado = _repository.GetById(producto.Id);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Nombre.Should().Be("Monitor");
    }

    [Test]
    public void Delete_ProductoExiste_MarcaComoBorrado()
    {
        // Arrange
        var producto = _repository.Add(new Producto
        {
            Nombre = "Ratón de Prueba",
            Precio = 10m,
            Categoria = new Categoria { Nombre = "Electrónica", Descripcion = "Componentes" }
        });

        // Act
        var resultado = _repository.Delete(producto.Id);

        // Assert
        resultado.Should().BeTrue();
        _repository.GetById(producto.Id).Should().BeNull(); // GetById filtra IsDeleted
    }
}
```

> 💡 **Consejo:** El repositorio debe filtrar `IsDeleted` también en `GetById`, no solo en `GetAll`. Si no, un elemento borrado lógicamente seguiría siendo visible por ID — inconsistente con el test `Delete → GetById == Null`.

### 13.7.4. Tests con EF Core

Los tests con EF Core prueban el repositorio usando el mismo patrón que con SQL, pero sobre MongoDB. El contexto es el de la sección 13.4.6 con una novedad: un filtro de consulta que oculta los borrados en todas las consultas.

```csharp
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;

public class ProductosMongoDbContext(DbContextOptions<ProductosMongoDbContext> options)
    : DbContext(options)
{
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Proveedor> Proveedores => Set<Proveedor>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Producto>().ToCollection("productos");
        modelBuilder.Entity<Proveedor>().ToCollection("proveedores");

        // Query Filter: oculta los borrados lógicamente en TODAS las consultas
        modelBuilder.Entity<Producto>().HasQueryFilter(p => !p.IsDeleted);
    }
}

[TestFixture]
public class ProductoEfCoreTests : MongoTestBase
{
    private ProductosMongoDbContext _context = null!;
    private ProductoEfCoreRepository _repository = null!;

    [SetUp]
    public void SetUp()
    {
        _context = new ProductosMongoDbContext(
            new DbContextOptionsBuilder<ProductosMongoDbContext>()
                .UseMongoDB(Client, "test_db")
                .Options);

        _repository = new ProductoEfCoreRepository(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    [Test]
    public void Add_ProductoValido_RetornaConId()
    {
        // Arrange
        var producto = new Producto
        {
            Nombre = "Monitor",
            Precio = 299.00m,
            Categoria = new Categoria { Nombre = "Electrónica", Descripcion = "Componentes" }
        };

        // Act
        var resultado = _repository.Add(producto);

        // Assert
        resultado.Id.Should().NotBe(ObjectId.Empty);
    }
}
```

## 13.8. Semilla de datos (seed)

La semilla puebla la base de datos de partida: proveedores, categorías y productos de ejemplo para poder trabajar desde el primer arranque. Solo se ejecuta en desarrollo; en producción los datos se gestionan a mano.

### Con driver nativo

Cuando hay **referencias**, el orden de siembra importa: **primero lo referenciado, después lo compuesto**. Y los IDs de lo referenciado deben ser **fijos**, o los productos quedarían apuntando a proveedores que ya no existen tras cada reinicio:

```csharp
public static class MongoDbSeeder
{
    // IDs fijos: si fueran aleatorios, las referencias se romperían en cada arranque
    private static readonly ObjectId LogitechId = ObjectId.Parse("64b000000000000000000001");
    private static readonly ObjectId IkeaId = ObjectId.Parse("64b000000000000000000002");

    public static void Seed(IMongoDatabase database)
    {
        var proveedores = database.GetCollection<Proveedor>("proveedores");
        var productos = database.GetCollection<Producto>("productos");

        // 1) Referenciados PRIMERO
        proveedores.DeleteMany(FilterDefinition<Proveedor>.Empty);
        proveedores.InsertMany(new[]
        {
            new Proveedor { Id = LogitechId, Nombre = "Logitech", Contacto = "exportaciones@logitech.com" },
            new Proveedor { Id = IkeaId, Nombre = "Ikea", Contacto = "proveedores@ikea.com" }
        });

        // 2) Compuestos DESPUÉS: cada uno apunta a un proveedor ya existente
        productos.DeleteMany(FilterDefinition<Producto>.Empty);
        productos.InsertMany(new[]
        {
            new Producto
            {
                Nombre = "Teclado Mecánico",
                Precio = 89.99m,
                Categoria = new Categoria { Nombre = "Electrónica", Descripcion = "Componentes" },
                ProveedorId = LogitechId,
                CreatedAt = DateTime.UtcNow
            },
            new Producto
            {
                Nombre = "Silla Ergonómica",
                Precio = 249.99m,
                Categoria = new Categoria { Nombre = "Mobiliario", Descripcion = "Muebles" },
                ProveedorId = IkeaId,
                CreatedAt = DateTime.UtcNow
            }
        });
    }
}
```

> 📝 **Nota:** en el ejemplo `12-ProductosMongo` son **5 proveedores** y **10 productos**, y cada producto lleva **su propia instancia** de `Categoria`: si compartieran instancia, EF Core solo la guardaría en uno de ellos (apartado 13.4.5).

En `Program.cs` la invocación es de desarrollo y resuelve la base de datos registrada en DI:

```csharp
// Program.cs (solo desarrollo)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var database = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();
    MongoDbSeeder.Seed(database);
}
```

### Con EF Core

El mismo seeder con EF Core, con el mismo orden:

```csharp
public static void SeedEfCore(ProductosMongoDbContext context)
{
    // 1) Referenciados primero
    context.Proveedores.AddRange(
        new Proveedor { Id = ObjectId.Parse("64b000000000000000000001"), Nombre = "Logitech" });

    // 2) Compuestos después, con su propia instancia de Categoria
    context.Productos.AddRange(
        new Producto
        {
            Nombre = "Teclado Mecánico",
            Categoria = new Categoria { Nombre = "Electrónica", Descripcion = "Componentes" },
            ProveedorId = ObjectId.Parse("64b000000000000000000001"),
            CreatedAt = DateTime.UtcNow
        });

    context.SaveChanges();
}
```

Y la invocación en desarrollo repite el patrón con el contexto:

```csharp
// Program.cs (solo desarrollo)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ProductosMongoDbContext>();
    await context.Database.EnsureDeletedAsync();  // limpia los datos del arranque anterior
    await context.Database.EnsureCreatedAsync();  // recrea las colecciones vacías
    MongoDbSeeder.SeedEfCore(context);
}
```

> 💡 **Consejo:** Elige una estrategia de siembra y sé coherente con ella: o vacías y re siembras (`DeleteMany` antes de insertar, como en los ejemplos de arriba) o compruebas `CountDocumentsAsync` antes de insertar. Las dos evitan duplicados; lo malo es mezclarlas.

## 13.9. Buenas prácticas

Estas son las reglas que debes seguir al trabajar con MongoDB en .NET.

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
// Inyectarlo en repositorios/servicios — UN solo cliente para toda la app
```

```csharp
// ❌ MALO: Guardar el _id como string — 24 caracteres en vez de 12 bytes
[BsonElement("_id")]
public string Id { get; set; } // Sí se indexa, pero ocupa el doble y pierde el timestamp del ObjectId

// ✅ BUENO: Usar ObjectId tipado — búsqueda directa por el _id nativo
[BsonId]
public ObjectId Id { get; set; } // Indexado automáticamente, búsqueda óptima
```

1. **Embebe cuando los datos van juntos** — Categoría en Producto, Dirección en Cliente
2. **Referencia cuando cambian a menudo** — No embebas un contador de visitas dentro de un Producto
3. **Usa POCO, no BsonDocument** — Tipado, IntelliSense, mantenibilidad
4. **`MongoClient` es singleton** — Crear uno por aplicación, reutilizar
5. **El `_id` es obligatorio** — Cada documento debe tener un campo `_id`
6. **No embebas colecciones ilimitadas** — El límite es 16 MB por documento
7. **Diseña para tus consultas** — Piensa qué datos necesitas juntos antes de embeber
8. **Driver Nativo para consultas complejas** — Agregaciones, geo-queries
9. **EF Core para CRUD simple** — Si vienes de EF Core relacional, es más fácil
10. **TestContainers para tests** — Un MongoDB real en Docker, sin depender de instalación local

## 13.10. Reto

Implementa un repositorio CRUD de **Funkos con Categoría embebida y Proveedor referenciado** usando MongoDB.

**Especificación:**

1. **Modelo Funko:** `Nombre`, `Precio`, `Imagen`, `IsDeleted`, `CreatedAt`, `UpdatedAt`
2. **Categoría embebida (composición):** `Categoria` como propiedad dentro de Funko (no como colección separada)
3. **Proveedor referenciado (referencia):** colección `proveedores` aparte y solo `ProveedorId` dentro de Funko — nunca el objeto entero
4. **Implementa DOS versiones:**
   - Una con Driver Nativo (`MongoDB.Driver`)
   - Otra con EF Core (`MongoDB.EntityFrameworkCore`)
5. **Repositorio:** Create, Read (GetAll, GetById), Update, Delete (borrado lógico), IsDuplicated
6. **Tests con TestContainers:** Al menos 6 tests: Create, GetById, GetAll, Delete lógico, IsDuplicated y **que la referencia al proveedor se persista sin embeberse**
7. **Seed data:** 3 proveedores con **ID fijo** + 5 Funkos que apuntan a ellos; se insertan al arrancar en desarrollo, **primero los referenciados**
8. **Validación:** un Funko sin `ProveedorId` debe devolver `400`

**Ejemplo de documento Funko: categoría embebida + proveedor referenciado:**

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
  "proveedor_id": ObjectId("64b000000000000000000001"),
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
│   │   ├── Funko.cs
│   │   └── Proveedor.cs                 # documento referenciado
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

---

**Resumen del punto:**

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

En el siguiente punto veremos **Redis**: un almacén de datos en memoria ultra-rápido que se usa como caché, cola de mensajes y sesión. Verás cómo el `IDatabase` de StackExchange.Redis se registra como singleton, cómo guardar datos con TTL (Time-To-Live) y cómo Redis complementa a MongoDB: MongoDB para persistencia, Redis para velocidad.