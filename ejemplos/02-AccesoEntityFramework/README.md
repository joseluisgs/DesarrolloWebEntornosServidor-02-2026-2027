# 🗄️ Ejemplo 02: Entity Framework Core - ORM Completo

## Tabla de Contenidos
- [Descripción](#descripción)
- [JPA/Hibernate → Entity Framework Core](#jpahibernate--entity-framework-core)
- [Estructura del Proyecto](#estructura-del-proyecto)
- [Conceptos Clave](#conceptos-clave)
- [LINQ vs JPQL/HQL](#linq-vs-jpqlhql)
- [Instalación y Ejecución](#instalación-y-ejecución)
- [Comparación Detallada](#comparación-detallada)

## Descripción

Este ejemplo demuestra **Entity Framework Core**, el **ORM (Object-Relational Mapping)** oficial de .NET, equivalente a **JPA (Java Persistence API)** y **Hibernate** en Java.

### ¿Qué es Entity Framework Core?

**Entity Framework Core (EF Core)** es un ORM moderno, ligero y extensible que permite a los desarrolladores trabajar con bases de datos usando objetos .NET, eliminando la necesidad de escribir la mayoría del código de acceso a datos.

### Comparación con Java

| Concepto | Java | C# |
|----------|------|-----|
| **ORM** | JPA (spec) + Hibernate (impl) | Entity Framework Core |
| **Acceso bajo nivel** | JDBC | ADO.NET |
| **Lenguaje de consulta** | JPQL/HQL (Strings) | LINQ (Code) |

## JPA/Hibernate → Entity Framework Core

### Tabla de Equivalencias Principales

| Java (JPA/Hibernate) | C# (EF Core) | Función |
|---------------------|--------------|---------|
| `EntityManager` | `DbContext` | Gestor de contexto de persistencia |
| `@Entity` | `[Table]` o Fluent API | Mapeo de entidad a tabla |
| `@Id` | `[Key]` | Clave primaria |
| `@GeneratedValue` | `[DatabaseGenerated]` | Valor autogenerado |
| `@Column` | `[Column]` o Fluent API | Mapeo de columna |
| `@Table` | `[Table]` o Fluent API | Nombre de tabla |
| `@OneToMany` | `HasMany()` | Relación uno a muchos |
| `@ManyToOne` | `HasOne()` | Relación muchos a uno |
| `JpaRepository<T, ID>` | `DbSet<T>` | Repositorio de entidades |
| `JPQL` | `LINQ` | Lenguaje de consulta |
| `entityManager.persist()` | `context.Add()` | Insertar entidad |
| `entityManager.find()` | `context.Find()` | Buscar por ID |
| `entityManager.merge()` | `context.Update()` | Actualizar entidad |
| `entityManager.remove()` | `context.Remove()` | Eliminar entidad |
| `@Query("SELECT...")` | LINQ expressions | Consultas personalizadas |
| `repository.save()` | `context.SaveChanges()` | Guardar cambios |

## Estructura del Proyecto

```
AccesoEF.Console/
├── Models/
│   └── Tenista.cs                    # Entidad (equivalente a @Entity)
├── Data/
│   └── TenistasDbContext.cs          # DbContext (equivalente a EntityManager)
├── Repositories/
│   ├── ITenistaRepository.cs         # Interfaz del repositorio
│   └── TenistaRepository.cs          # Implementación con LINQ
└── Program.cs                        # Programa principal con ejemplos
```

## Conceptos Clave

### 1. DbContext (EntityManager en JPA)

**DbContext** es el equivalente a **EntityManager** en JPA. Gestiona el ciclo de vida de las entidades y proporciona change tracking.

#### Java (JPA)
```java
@Configuration
public class DatabaseConfig {
    @Bean
    public LocalContainerEntityManagerFactoryBean entityManagerFactory() {
        // Configuración de EntityManager
    }
}

@PersistenceContext
private EntityManager entityManager;
```

#### C# (EF Core)
```csharp
public class TenistasDbContext : DbContext
{
    public DbSet<Tenista> Tenistas { get; set; }
    
    public TenistasDbContext(DbContextOptions<TenistasDbContext> options) 
        : base(options) { }
}
```

### 2. Entidades (Entity Mapping)

#### Java (JPA)
```java
@Entity
@Table(name = "tenistas")
public class Tenista {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;
    
    @Column(name = "nombre", nullable = false, length = 100)
    private String nombre;
    
    @Column(name = "ranking")
    private int ranking;
    
    // getters y setters
}
```

#### C# (EF Core) - Data Annotations
```csharp
[Table("tenistas")]
public class Tenista
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public long Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    [Column("nombre")]
    public string Nombre { get; set; }
    
    [Column("ranking")]
    public int Ranking { get; set; }
}
```

#### C# (EF Core) - Fluent API
```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Tenista>(entity =>
    {
        entity.ToTable("tenistas");
        entity.HasKey(t => t.Id);
        entity.Property(t => t.Nombre)
              .IsRequired()
              .HasMaxLength(100)
              .HasColumnName("nombre");
    });
}
```

### 3. Repositorios

#### Java (Spring Data JPA)
```java
@Repository
public interface TenistaRepository extends JpaRepository<Tenista, Long> {
    // Spring Data genera automáticamente la implementación
    List<Tenista> findByPais(String pais);
    
    @Query("SELECT t FROM Tenista t WHERE t.ranking < :ranking")
    List<Tenista> findByRankingLessThan(@Param("ranking") int ranking);
}
```

#### C# (EF Core con LINQ)
```csharp
public class TenistaRepository : ITenistaRepository
{
    private readonly TenistasDbContext _context;
    
    public async Task<List<Tenista>> FindByPaisAsync(string pais)
    {
        return await _context.Tenistas
            .Where(t => t.Pais == pais)
            .ToListAsync();
    }
    
    public async Task<List<Tenista>> FindByRankingLessThanAsync(int ranking)
    {
        return await _context.Tenistas
            .Where(t => t.Ranking < ranking)
            .OrderBy(t => t.Ranking)
            .ToListAsync();
    }
}
```

## LINQ vs JPQL/HQL

### Diferencias Fundamentales

| Aspecto | JPQL/HQL (Java) | LINQ (C#) |
|---------|----------------|-----------|
| **Tipo** | Strings (runtime) | Code (compile-time) |
| **Type Safety** | ❌ No (errores en runtime) | ✅ Sí (errores en compile-time) |
| **IntelliSense** | ❌ Limitado | ✅ Completo |
| **Refactoring** | ❌ Difícil | ✅ Automático |
| **Sintaxis** | Similar a SQL | Lambda expressions |

### Ejemplos Comparativos

#### Consulta Simple

**Java (JPQL)**
```java
String jpql = "SELECT t FROM Tenista t WHERE t.pais = :pais";
TypedQuery<Tenista> query = entityManager.createQuery(jpql, Tenista.class);
query.setParameter("pais", "España");
List<Tenista> result = query.getResultList();
```

**C# (LINQ)**
```csharp
var result = await _context.Tenistas
    .Where(t => t.Pais == "España")
    .ToListAsync();
```

#### Consulta con Ordenamiento

**Java (JPQL)**
```java
String jpql = "SELECT t FROM Tenista t WHERE t.pais = :pais ORDER BY t.ranking";
TypedQuery<Tenista> query = entityManager.createQuery(jpql, Tenista.class);
query.setParameter("pais", "España");
List<Tenista> result = query.getResultList();
```

**C# (LINQ)**
```csharp
var result = await _context.Tenistas
    .Where(t => t.Pais == "España")
    .OrderBy(t => t.Ranking)
    .ToListAsync();
```

#### Proyecciones (SELECT específico)

**Java (JPQL)**
```java
String jpql = "SELECT NEW com.example.TenistaDTO(t.nombre, t.ranking) " +
              "FROM Tenista t WHERE t.titulos > :min";
TypedQuery<TenistaDTO> query = entityManager.createQuery(jpql, TenistaDTO.class);
query.setParameter("min", 10);
List<TenistaDTO> result = query.getResultList();
```

**C# (LINQ)**
```csharp
var result = await _context.Tenistas
    .Where(t => t.Titulos > 10)
    .Select(t => new TenistaDTO 
    { 
        Nombre = t.Nombre, 
        Ranking = t.Ranking 
    })
    .ToListAsync();
```

#### Agregaciones

**Java (JPQL)**
```java
String jpql = "SELECT COUNT(t) FROM Tenista t WHERE t.pais = :pais";
Long count = entityManager.createQuery(jpql, Long.class)
                          .setParameter("pais", "España")
                          .getSingleResult();
```

**C# (LINQ)**
```csharp
var count = await _context.Tenistas
    .Where(t => t.Pais == "España")
    .CountAsync();
```

#### Joins

**Java (JPQL)**
```java
String jpql = "SELECT t FROM Tenista t " +
              "JOIN t.torneos to " +
              "WHERE to.nombre = :torneo";
List<Tenista> result = entityManager
    .createQuery(jpql, Tenista.class)
    .setParameter("torneo", "Wimbledon")
    .getResultList();
```

**C# (LINQ)**
```csharp
var result = await _context.Tenistas
    .Include(t => t.Torneos)
    .Where(t => t.Torneos.Any(to => to.Nombre == "Wimbledon"))
    .ToListAsync();
```

## Instalación y Ejecución

### Prerrequisitos

1. **.NET 8 SDK**
2. **PostgreSQL** (Docker recomendado)

### Iniciar PostgreSQL

```bash
docker run -d \
  --name postgres-ef \
  -p 5432:5432 \
  -e POSTGRES_DB=tenistas_db \
  -e POSTGRES_USER=admin \
  -e POSTGRES_PASSWORD=admin123 \
  postgres:15
```

### Ejecutar el Ejemplo

```bash
cd ejemplos/02-AccesoEntityFramework/AccesoEF.Console
dotnet restore
dotnet run
```

### Salida Esperada

```
╔══════════════════════════════════════════════════════════════════════╗
║  🗄️  EJEMPLO ENTITY FRAMEWORK CORE - ORM (JPA/Hibernate → C#)      ║
╚══════════════════════════════════════════════════════════════════════╝

🔌 Conectando a PostgreSQL con Entity Framework Core...
✅ Base de datos creada con Entity Framework Core

═══════════════════════════════════════════════════════════════
📝 OPERACIÓN 1: CREATE - Insertar tenistas con EF Core
═══════════════════════════════════════════════════════════════
✅ Tenista creado: Rafael Nadal con ID 1
...
```

## Comparación Detallada

### Operaciones CRUD

| Operación | Java (JPA) | C# (EF Core) |
|-----------|------------|--------------|
| **INSERT** | `entityManager.persist(entity)` | `context.Add(entity); context.SaveChanges()` |
| **SELECT BY ID** | `entityManager.find(Tenista.class, id)` | `context.Tenistas.Find(id)` |
| **SELECT ALL** | `entityManager.createQuery("FROM Tenista")` | `context.Tenistas.ToList()` |
| **UPDATE** | `entityManager.merge(entity)` | `context.Update(entity); context.SaveChanges()` |
| **DELETE** | `entityManager.remove(entity)` | `context.Remove(entity); context.SaveChanges()` |

### Ventajas de EF Core sobre JPA/Hibernate

| Ventaja | Descripción |
|---------|-------------|
| **LINQ** | Consultas type-safe compiladas (vs strings JPQL) |
| **Async/Await** | Soporte nativo para operaciones asíncronas |
| **Migrations** | Sistema de migraciones integrado (como Flyway) |
| **Performance** | Generalmente más rápido que Hibernate |
| **Change Tracking** | Sistema de seguimiento de cambios optimizado |

### Ventajas de JPA/Hibernate sobre EF Core

| Ventaja | Descripción |
|---------|-------------|
| **Madurez** | Más años en el mercado, ecosistema más grande |
| **Lazy Loading** | Más flexible por defecto |
| **Caché L2** | Sistema de caché de segundo nivel más avanzado |

## Migraciones (Migrations)

### Java (Flyway/Liquibase)

```sql
-- V1__Create_tenistas_table.sql
CREATE TABLE tenistas (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    ranking INTEGER NOT NULL
);
```

### C# (EF Core Migrations)

```bash
# Crear migración
dotnet ef migrations add InitialCreate

# Aplicar migración
dotnet ef database update
```

**Migración generada automáticamente:**
```csharp
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "tenistas",
            columns: table => new
            {
                id = table.Column<long>(nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", 
                        NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                nombre = table.Column<string>(maxLength: 100, nullable: false),
                ranking = table.Column<int>(nullable: false)
            });
    }
}
```

## Recursos Adicionales

### Documentación Oficial
- [Entity Framework Core Docs](https://docs.microsoft.com/en-us/ef/core/)
- [LINQ Documentation](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/)
- [EF Core vs EF6](https://docs.microsoft.com/en-us/ef/efcore-and-ef6/)

### Comparaciones
- [JPA vs EF Core](https://stackoverflow.com/questions/tagged/entity-framework-core+jpa)
- [LINQ vs JPQL](https://stackoverflow.com/questions/tagged/linq+jpql)

### Siguientes Pasos
1. **[Ejemplo 03](../03-TenistasSync/)** - Operaciones síncronas avanzadas
2. **[Documentación C#](../../csharp/04-streams-linq/)** - LINQ en profundidad
3. **[Documentación .NET](../../netcore/03-data-access/)** - JPA → EF Core completo

---

## Autor

Codificado con ❤️ por **José Luis González Sánchez**

[![Twitter](https://img.shields.io/twitter/follow/JoseLuisGS_?style=social)](https://twitter.com/JoseLuisGS_)
[![GitHub](https://img.shields.io/github/followers/joseluisgs?style=social)](https://github.com/joseluisgs)

### Contacto
- 🌐 Web: [https://joseluisgs.dev](https://joseluisgs.dev)
- 📧 Email: joseluis.gonzalez@profesor.com

## Licencia

Este proyecto está licenciado bajo **Creative Commons BY-NC-SA 4.0**

<a rel="license" href="http://creativecommons.org/licenses/by-nc-sa/4.0/">
  <img alt="Licencia de Creative Commons" style="border-width:0" src="https://i.creativecommons.org/l/by-nc-sa/4.0/88x31.png" />
</a>
