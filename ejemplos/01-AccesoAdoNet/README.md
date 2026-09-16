# 📊 Ejemplo 01: Acceso a Datos con ADO.NET

## Tabla de Contenidos
- [Descripción](#descripción)
- [Conceptos Java → C#](#conceptos-java--c)
- [Estructura del Proyecto](#estructura-del-proyecto)
- [Comparación JDBC vs ADO.NET](#comparación-jdbc-vs-adonet)
- [Instalación y Ejecución](#instalación-y-ejecución)
- [Operaciones CRUD](#operaciones-crud)
- [Cuándo Usar ADO.NET](#cuándo-usar-adonet)

## Descripción

Este ejemplo demuestra el **acceso manual a bases de datos** usando **ADO.NET**, que es el equivalente de bajo nivel a **JDBC en Java**. Proporciona control total sobre SQL, conexiones y mapeo de datos.

### ¿Qué es ADO.NET?

**ADO.NET** (ActiveX Data Objects .NET) es la tecnología de acceso a datos de bajo nivel en .NET, comparable a **JDBC** en Java. Permite ejecutar SQL directamente y manejar conexiones manualmente.

### Comparación con Java

| Concepto | Java (JDBC) | C# (ADO.NET) |
|----------|------------|--------------|
| **API de bajo nivel** | JDBC | ADO.NET |
| **ORM (alto nivel)** | JPA/Hibernate | Entity Framework Core |

## Conceptos Java → C#

### Clases Principales

```java
// JAVA (JDBC)
Connection conn = DriverManager.getConnection(url, user, pass);
PreparedStatement ps = conn.prepareStatement(sql);
ResultSet rs = ps.executeQuery();
```

```csharp
// C# (ADO.NET)
var conn = new NpgsqlConnection(connectionString);
var cmd = new NpgsqlCommand(sql, conn);
var reader = await cmd.ExecuteReaderAsync();
```

### Tabla de Equivalencias

| Java (JDBC) | C# (ADO.NET) | Descripción |
|-------------|--------------|-------------|
| `Connection` | `NpgsqlConnection` | Conexión a la base de datos |
| `PreparedStatement` | `NpgsqlCommand` | Comando SQL con parámetros |
| `ResultSet` | `NpgsqlDataReader` | Cursor para leer resultados |
| `DriverManager.getConnection()` | `new NpgsqlConnection()` | Crear conexión |
| `ps.executeQuery()` | `cmd.ExecuteReaderAsync()` | Ejecutar SELECT |
| `ps.executeUpdate()` | `cmd.ExecuteNonQueryAsync()` | Ejecutar INSERT/UPDATE/DELETE |
| `rs.next()` | `await reader.ReadAsync()` | Leer siguiente fila |
| `rs.getString("col")` | `reader.GetString("col")` | Obtener valor de columna |
| `try-with-resources` | `using` / `await using` | Gestión automática de recursos |

## Estructura del Proyecto

```
AccesoAdoNet.Console/
├── Models/
│   └── Tenista.cs                    # Modelo de datos (POJO en Java)
├── Database/
│   └── DatabaseManager.cs            # Gestión de conexiones (DriverManager)
├── Repositories/
│   ├── ITenistaRepository.cs         # Interfaz del repositorio
│   └── TenistaRepository.cs          # Implementación con ADO.NET
└── Program.cs                        # Programa principal con ejemplos
```

## Comparación JDBC vs ADO.NET

### Conexión a Base de Datos

#### Java (JDBC)
```java
String url = "jdbc:postgresql://localhost:5432/tenistas_db";
String user = "admin";
String password = "admin123";

try (Connection conn = DriverManager.getConnection(url, user, password)) {
    // Usar conexión
}
```

#### C# (ADO.NET)
```csharp
var connectionString = "Host=localhost;Port=5432;Database=tenistas_db;Username=admin;Password=admin123";

await using var conn = new NpgsqlConnection(connectionString);
await conn.OpenAsync();
// Usar conexión
```

### INSERT con ID Generado

#### Java (JDBC)
```java
String sql = "INSERT INTO tenistas (nombre, ranking) VALUES (?, ?)";
PreparedStatement ps = conn.prepareStatement(sql, Statement.RETURN_GENERATED_KEYS);
ps.setString(1, "Rafael Nadal");
ps.setInt(2, 1);
ps.executeUpdate();

ResultSet rs = ps.getGeneratedKeys();
if (rs.next()) {
    long id = rs.getLong(1);
}
```

#### C# (ADO.NET)
```csharp
var sql = "INSERT INTO tenistas (nombre, ranking) VALUES (@nombre, @ranking) RETURNING id";
await using var cmd = new NpgsqlCommand(sql, conn);
cmd.Parameters.AddWithValue("@nombre", "Rafael Nadal");
cmd.Parameters.AddWithValue("@ranking", 1);

var id = await cmd.ExecuteScalarAsync();
long tenistaId = Convert.ToInt64(id);
```

### SELECT y Mapeo de Resultados

#### Java (JDBC)
```java
String sql = "SELECT * FROM tenistas WHERE id = ?";
PreparedStatement ps = conn.prepareStatement(sql);
ps.setLong(1, id);
ResultSet rs = ps.executeQuery();

if (rs.next()) {
    Tenista tenista = new Tenista();
    tenista.setId(rs.getLong("id"));
    tenista.setNombre(rs.getString("nombre"));
    tenista.setRanking(rs.getInt("ranking"));
    // ...
}
```

#### C# (ADO.NET)
```csharp
var sql = "SELECT * FROM tenistas WHERE id = @id";
await using var cmd = new NpgsqlCommand(sql, conn);
cmd.Parameters.AddWithValue("@id", id);

await using var reader = await cmd.ExecuteReaderAsync();
if (await reader.ReadAsync())
{
    var tenista = new Tenista
    {
        Id = reader.GetInt64("id"),
        Nombre = reader.GetString("nombre"),
        Ranking = reader.GetInt32("ranking")
        // ...
    };
}
```

### UPDATE

#### Java (JDBC)
```java
String sql = "UPDATE tenistas SET ranking = ? WHERE id = ?";
PreparedStatement ps = conn.prepareStatement(sql);
ps.setInt(1, newRanking);
ps.setLong(2, id);
int rowsAffected = ps.executeUpdate();
```

#### C# (ADO.NET)
```csharp
var sql = "UPDATE tenistas SET ranking = @ranking WHERE id = @id";
await using var cmd = new NpgsqlCommand(sql, conn);
cmd.Parameters.AddWithValue("@ranking", newRanking);
cmd.Parameters.AddWithValue("@id", id);
var rowsAffected = await cmd.ExecuteNonQueryAsync();
```

### DELETE

#### Java (JDBC)
```java
String sql = "DELETE FROM tenistas WHERE id = ?";
PreparedStatement ps = conn.prepareStatement(sql);
ps.setLong(1, id);
int rowsAffected = ps.executeUpdate();
```

#### C# (ADO.NET)
```csharp
var sql = "DELETE FROM tenistas WHERE id = @id";
await using var cmd = new NpgsqlCommand(sql, conn);
cmd.Parameters.AddWithValue("@id", id);
var rowsAffected = await cmd.ExecuteNonQueryAsync();
```

## Instalación y Ejecución

### Prerrequisitos

1. **.NET 8 SDK**
2. **PostgreSQL** (se puede usar Docker)

### Iniciar PostgreSQL con Docker

```bash
docker run -d \
  --name postgres-tenistas \
  -p 5432:5432 \
  -e POSTGRES_DB=tenistas_db \
  -e POSTGRES_USER=admin \
  -e POSTGRES_PASSWORD=admin123 \
  postgres:15
```

### Compilar y Ejecutar

```bash
cd ejemplos/01-AccesoAdoNet/AccesoAdoNet.Console
dotnet restore
dotnet run
```

### Salida Esperada

```
╔══════════════════════════════════════════════════════════════════════╗
║  📊 EJEMPLO ADO.NET - Acceso Manual a Base de Datos (JDBC → C#)    ║
╚══════════════════════════════════════════════════════════════════════╝

🔌 Conectando a PostgreSQL...
✅ Base de datos inicializada correctamente
🗑️  Base de datos limpiada

═══════════════════════════════════════════════════════════════
📝 OPERACIÓN 1: CREATE - Insertar tenistas
═══════════════════════════════════════════════════════════════
✅ Tenista creado: Rafael Nadal con ID 1
✅ Tenista creado: Novak Djokovic con ID 2
...
```

## Operaciones CRUD

El ejemplo demuestra las siguientes operaciones:

### 1. **CREATE** (INSERT)
```csharp
var tenista = new Tenista { Nombre = "Rafael Nadal", Ranking = 1, ... };
await repository.CreateAsync(tenista);
```

### 2. **READ ALL** (SELECT *)
```csharp
var tenistas = await repository.FindAllAsync();
```

### 3. **READ BY ID** (SELECT WHERE id = ?)
```csharp
var tenista = await repository.FindByIdAsync(1);
```

### 4. **READ BY FILTER** (SELECT WHERE campo = ?)
```csharp
var tenistasEspañoles = await repository.FindByPaisAsync("España");
```

### 5. **UPDATE**
```csharp
tenista.Titulos = 23;
await repository.UpdateAsync(tenista);
```

### 6. **COUNT** (Aggregate)
```csharp
var count = await repository.CountAsync();
```

### 7. **DELETE**
```csharp
var deleted = await repository.DeleteAsync(4);
```

## Cuándo Usar ADO.NET

### ✅ Usa ADO.NET cuando:
- Necesitas **máximo control** sobre SQL y conexiones
- Trabajas con **consultas complejas** o **stored procedures**
- Requieres **máximo rendimiento** (sin overhead de ORM)
- Migras código **JDBC existente** y quieres mantener el estilo
- Trabajas con bases de datos **legacy** con esquemas complejos

### ❌ Evita ADO.NET cuando:
- Quieres **desarrollo rápido** con menos código boilerplate
- Prefieres un **mapeo automático** de objetos (ORM)
- Necesitas **migraciones de base de datos** automáticas
- Trabajas con **relaciones complejas** entre entidades
- Buscas **portabilidad** entre diferentes bases de datos

### 💡 Alternativa Recomendada
Para desarrollo moderno con ORM (equivalente a JPA/Hibernate), usa **Entity Framework Core**:
```bash
cd ../02-AccesoEntityFramework
```

## Ventajas y Desventajas

### Ventajas de ADO.NET

| Ventaja | Descripción |
|---------|-------------|
| **Control Total** | Control completo sobre SQL y ejecución |
| **Alto Rendimiento** | Sin overhead de ORM, ideal para queries complejas |
| **Flexibilidad** | Perfecto para stored procedures y SQL avanzado |
| **Familiaridad** | Similar a JDBC, fácil para desarrolladores Java |
| **Depuración** | SQL visible y debuggeable directamente |

### Desventajas de ADO.NET

| Desventaja | Descripción |
|------------|-------------|
| **Boilerplate** | Más código que un ORM |
| **Mapeo Manual** | Mapeo manual de ResultSet a objetos |
| **Mantenimiento** | Cambios en schema requieren actualizar múltiples lugares |
| **SQL Injection** | Mayor riesgo si no se usan parámetros correctamente |
| **Portabilidad** | SQL específico de base de datos |

## Recursos Adicionales

### Documentación Oficial
- [ADO.NET Documentation](https://learn.microsoft.com/en-us/dotnet/framework/data/adonet/)
- [Npgsql (PostgreSQL Provider)](https://www.npgsql.org/)
- [System.Data.Common](https://learn.microsoft.com/en-us/dotnet/api/system.data.common)

### Comparaciones
- [JDBC vs ADO.NET](https://stackoverflow.com/questions/tagged/adonet+jdbc)
- [When to use ADO.NET vs EF Core](https://stackoverflow.com/questions/154497/entity-framework-vs-adonet)

### Siguientes Pasos
1. **[Ejemplo 02](../02-AccesoEntityFramework/)** - Entity Framework Core (ORM como JPA)
2. **[Documentación C#](../../csharp/08-data-access/)** - Conceptos de acceso a datos
3. **[Documentación .NET](../../netcore/03-data-access/)** - JPA → EF Core

---

## Autor

Codificado con ❤️ por **José Luis González Sánchez**

[![Twitter](https://img.shields.io/twitter/follow/JoseLuisGS_?style=social)](https://twitter.com/JoseLuisGS_)
[![GitHub](https://img.shields.io/github/followers/joseluisgs?style=social)](https://github.com/joseluisgs)

### Contacto
- 🌐 Web: [https://joseluisgs.dev](https://joseluisgs.dev)
- 📧 Email: joseluis.gonzalez@profesor.com
- 🐦 Twitter: [@JoseLuisGS_](https://twitter.com/JoseLuisGS_)
- 💼 LinkedIn: [joseluisgonsan](https://www.linkedin.com/in/joseluisgonsan)

## Licencia

Este proyecto está licenciado bajo **Creative Commons BY-NC-SA 4.0**

<a rel="license" href="http://creativecommons.org/licenses/by-nc-sa/4.0/">
  <img alt="Licencia de Creative Commons" style="border-width:0" src="https://i.creativecommons.org/l/by-nc-sa/4.0/88x31.png" />
</a>
