using AccesoEF.Console.Data;
using AccesoEF.Console.Models;
using AccesoEF.Console.Repositories;
using Microsoft.EntityFrameworkCore;

// ==================================================================================
// 🚀 EJEMPLO 02: ENTITY FRAMEWORK CORE (equivalente a JPA/Hibernate en Java)
// ==================================================================================
// 
// Este ejemplo demuestra Entity Framework Core, el ORM oficial de .NET
// Es el equivalente directo a JPA (Java Persistence API) y Hibernate en Java.
//
// CONCEPTOS JAVA → C#:
// - JPA/Hibernate → Entity Framework Core
// - @Entity → [Table] o Fluent API
// - EntityManager → DbContext
// - JpaRepository → DbSet<T>
// - JPQL/HQL → LINQ (Language Integrated Query)
// - @Query → LINQ expressions
//
// EF Core proporciona un ORM completo con tracking de cambios, migraciones,
// y consultas type-safe usando LINQ.
// ==================================================================================

Console.WriteLine("╔══════════════════════════════════════════════════════════════════════╗");
Console.WriteLine("║  🗄️  EJEMPLO ENTITY FRAMEWORK CORE - ORM (JPA/Hibernate → C#)      ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════════════════╝");
Console.WriteLine();

// Configuración del DbContext (equivalente a EntityManager en JPA)
// En Java/Spring Boot esto estaría en application.properties:
// spring.datasource.url=jdbc:postgresql://localhost:5432/tenistas_db
// spring.jpa.hibernate.ddl-auto=update
var connectionString = "Host=localhost;Port=5432;Database=tenistas_db;Username=admin;Password=admin123";

var optionsBuilder = new DbContextOptionsBuilder<TenistasDbContext>();
optionsBuilder.UseNpgsql(connectionString);

using var context = new TenistasDbContext(optionsBuilder.Options);

try
{
    Console.WriteLine("🔌 Conectando a PostgreSQL con Entity Framework Core...");
    
    // Crear la base de datos y las tablas si no existen
    // En Java/JPA: spring.jpa.hibernate.ddl-auto=create-drop
    // En C#/EF Core: context.Database.EnsureCreated()
    await context.Database.EnsureDeletedAsync();  // Limpiar BD anterior
    await context.Database.EnsureCreatedAsync();  // Crear BD y tablas
    Console.WriteLine("✅ Base de datos creada con Entity Framework Core");
    
    // Crear repositorio
    var repository = new TenistaRepository(context);

    // ==================================================================================
    // OPERACIÓN 1: CREATE con EF Core
    // ==================================================================================
    Console.WriteLine();
    Console.WriteLine("═══════════════════════════════════════════════════════════════");
    Console.WriteLine("📝 OPERACIÓN 1: CREATE - Insertar tenistas con EF Core");
    Console.WriteLine("═══════════════════════════════════════════════════════════════");
    Console.WriteLine();
    Console.WriteLine("💡 En Java/JPA: entityManager.persist(tenista);");
    Console.WriteLine("💡 En C#/EF Core: context.Add(tenista); await context.SaveChangesAsync();");
    Console.WriteLine();
    
    var tenistas = new List<Tenista>
    {
        new() { Nombre = "Rafael Nadal", Ranking = 1, Pais = "España", Altura = 185, Peso = 85, Titulos = 22, FechaNacimiento = new DateTime(1986, 6, 3) },
        new() { Nombre = "Novak Djokovic", Ranking = 2, Pais = "Serbia", Altura = 188, Peso = 77, Titulos = 24, FechaNacimiento = new DateTime(1987, 5, 22) },
        new() { Nombre = "Carlos Alcaraz", Ranking = 3, Pais = "España", Altura = 183, Peso = 80, Titulos = 2, FechaNacimiento = new DateTime(2003, 5, 5) },
        new() { Nombre = "Roger Federer", Ranking = 4, Pais = "Suiza", Altura = 185, Peso = 85, Titulos = 20, FechaNacimiento = new DateTime(1981, 8, 8) },
        new() { Nombre = "Andy Murray", Ranking = 5, Pais = "Reino Unido", Altura = 190, Peso = 84, Titulos = 3, FechaNacimiento = new DateTime(1987, 5, 15) }
    };

    foreach (var tenista in tenistas)
    {
        await repository.CreateAsync(tenista);
    }

    // ==================================================================================
    // OPERACIÓN 2: READ ALL con LINQ
    // ==================================================================================
    Console.WriteLine();
    Console.WriteLine("═══════════════════════════════════════════════════════════════");
    Console.WriteLine("📖 OPERACIÓN 2: READ ALL - LINQ vs JPQL");
    Console.WriteLine("═══════════════════════════════════════════════════════════════");
    Console.WriteLine();
    Console.WriteLine("💡 Java/JPQL: entityManager.createQuery(\"FROM Tenista\").getResultList();");
    Console.WriteLine("💡 C#/LINQ: context.Tenistas.ToListAsync();");
    Console.WriteLine();
    
    var todosLosTenistas = await repository.FindAllAsync();
    Console.WriteLine($"📊 Total de tenistas: {todosLosTenistas.Count}");
    foreach (var t in todosLosTenistas)
    {
        Console.WriteLine($"  → {t}");
    }

    // ==================================================================================
    // OPERACIÓN 3: READ BY ID
    // ==================================================================================
    Console.WriteLine();
    Console.WriteLine("═══════════════════════════════════════════════════════════════");
    Console.WriteLine("🔍 OPERACIÓN 3: READ BY ID - FindAsync");
    Console.WriteLine("═══════════════════════════════════════════════════════════════");
    Console.WriteLine();
    Console.WriteLine("💡 Java/JPA: Optional<Tenista> t = repository.findById(1L);");
    Console.WriteLine("💡 C#/EF Core: var t = await context.Tenistas.FindAsync(1);");
    Console.WriteLine();
    
    var tenista1 = await repository.FindByIdAsync(1);
    if (tenista1 != null)
    {
        Console.WriteLine($"✅ Encontrado: {tenista1}");
    }

    // ==================================================================================
    // OPERACIÓN 4: LINQ QUERIES (equivalente a @Query en Spring Data)
    // ==================================================================================
    Console.WriteLine();
    Console.WriteLine("═══════════════════════════════════════════════════════════════");
    Console.WriteLine("🔎 OPERACIÓN 4: LINQ QUERIES - Consultas type-safe");
    Console.WriteLine("═══════════════════════════════════════════════════════════════");
    Console.WriteLine();
    Console.WriteLine("💡 Java/JPQL:");
    Console.WriteLine("   @Query(\"SELECT t FROM Tenista t WHERE t.pais = :pais\")");
    Console.WriteLine("   List<Tenista> findByPais(@Param(\"pais\") String pais);");
    Console.WriteLine();
    Console.WriteLine("💡 C#/LINQ:");
    Console.WriteLine("   context.Tenistas.Where(t => t.Pais == pais).ToListAsync();");
    Console.WriteLine();
    
    var tenistasEspañoles = await repository.FindByPaisAsync("España");
    Console.WriteLine($"🇪🇸 Tenistas españoles: {tenistasEspañoles.Count}");
    foreach (var t in tenistasEspañoles)
    {
        Console.WriteLine($"  → {t.Nombre} (Ranking: {t.Ranking})");
    }

    // ==================================================================================
    // OPERACIÓN 5: LINQ con filtros complejos
    // ==================================================================================
    Console.WriteLine();
    Console.WriteLine("═══════════════════════════════════════════════════════════════");
    Console.WriteLine("🎯 OPERACIÓN 5: FILTROS COMPLEJOS con LINQ");
    Console.WriteLine("═══════════════════════════════════════════════════════════════");
    Console.WriteLine();
    
    var top3 = await repository.FindByRankingLessThanAsync(4);
    Console.WriteLine($"🏆 Top 3 tenistas (ranking < 4): {top3.Count}");
    foreach (var t in top3)
    {
        Console.WriteLine($"  {t.Ranking}. {t.Nombre} - {t.Titulos} títulos");
    }

    // ==================================================================================
    // OPERACIÓN 6: UPDATE con change tracking
    // ==================================================================================
    Console.WriteLine();
    Console.WriteLine("═══════════════════════════════════════════════════════════════");
    Console.WriteLine("✏️  OPERACIÓN 6: UPDATE - Change Tracking de EF Core");
    Console.WriteLine("═══════════════════════════════════════════════════════════════");
    Console.WriteLine();
    Console.WriteLine("💡 EF Core automáticamente detecta cambios en entidades rastreadas");
    Console.WriteLine("💡 Similar a Hibernate's Session tracking en Java");
    Console.WriteLine();
    
    if (tenista1 != null)
    {
        Console.WriteLine($"Antes: Títulos = {tenista1.Titulos}");
        tenista1.Titulos = 23;
        await repository.UpdateAsync(tenista1);
        
        var tenistaActualizado = await repository.FindByIdAsync(tenista1.Id);
        Console.WriteLine($"Después: Títulos = {tenistaActualizado?.Titulos}");
    }

    // ==================================================================================
    // OPERACIÓN 7: COUNT (Aggregate)
    // ==================================================================================
    Console.WriteLine();
    Console.WriteLine("═══════════════════════════════════════════════════════════════");
    Console.WriteLine("🔢 OPERACIÓN 7: COUNT - Funciones agregadas");
    Console.WriteLine("═══════════════════════════════════════════════════════════════");
    Console.WriteLine();
    Console.WriteLine("💡 Java/JPA: long count = repository.count();");
    Console.WriteLine("💡 C#/LINQ: int count = await context.Tenistas.CountAsync();");
    Console.WriteLine();
    
    var totalTenistas = await repository.CountAsync();
    Console.WriteLine($"📊 Total de tenistas: {totalTenistas}");

    // ==================================================================================
    // OPERACIÓN 8: DELETE
    // ==================================================================================
    Console.WriteLine();
    Console.WriteLine("═══════════════════════════════════════════════════════════════");
    Console.WriteLine("🗑️  OPERACIÓN 8: DELETE");
    Console.WriteLine("═══════════════════════════════════════════════════════════════");
    Console.WriteLine();
    Console.WriteLine("💡 Java/JPA: repository.deleteById(5L);");
    Console.WriteLine("💡 C#/EF Core: context.Remove(tenista); await context.SaveChangesAsync();");
    Console.WriteLine();
    
    var deleted = await repository.DeleteAsync(5);
    Console.WriteLine(deleted ? "✅ Tenista eliminado" : "❌ Tenista no encontrado");
    
    var tenistasFinal = await repository.FindAllAsync();
    Console.WriteLine($"📊 Tenistas restantes: {tenistasFinal.Count}");

    // ==================================================================================
    // DEMOSTRACIÓN LINQ AVANZADO
    // ==================================================================================
    Console.WriteLine();
    Console.WriteLine("═══════════════════════════════════════════════════════════════");
    Console.WriteLine("🚀 DEMOSTRACIÓN: LINQ Avanzado (similar a Stream API)");
    Console.WriteLine("═══════════════════════════════════════════════════════════════");
    Console.WriteLine();
    
    // LINQ es como Stream API de Java pero más integrado con el ORM
    // Consulta compleja con múltiples operaciones
    var consultaCompleja = await context.Tenistas
        .Where(t => t.Titulos > 5)           // filter
        .OrderByDescending(t => t.Titulos)    // sorted
        .Select(t => new                      // map
        {
            t.Nombre,
            t.Titulos,
            t.Ranking
        })
        .Take(3)                              // limit
        .ToListAsync();

    Console.WriteLine("🏆 Top 3 tenistas con más de 5 títulos:");
    foreach (var t in consultaCompleja)
    {
        Console.WriteLine($"  → {t.Nombre}: {t.Titulos} títulos (Ranking: {t.Ranking})");
    }

    // ==================================================================================
    // RESUMEN Y COMPARACIÓN
    // ==================================================================================
    Console.WriteLine();
    Console.WriteLine("╔══════════════════════════════════════════════════════════════════════╗");
    Console.WriteLine("║  📚 COMPARACIÓN JPA/Hibernate vs Entity Framework Core              ║");
    Console.WriteLine("╚══════════════════════════════════════════════════════════════════════╝");
    Console.WriteLine();
    Console.WriteLine("┌───────────────────────────────┬─────────────────────────────────────┐");
    Console.WriteLine("│ JAVA (JPA/Hibernate)          │ C# (Entity Framework Core)          │");
    Console.WriteLine("├───────────────────────────────┼─────────────────────────────────────┤");
    Console.WriteLine("│ EntityManager                 │ DbContext                           │");
    Console.WriteLine("│ @Entity                       │ [Table] o Fluent API                │");
    Console.WriteLine("│ @Id                           │ [Key]                               │");
    Console.WriteLine("│ @GeneratedValue               │ [DatabaseGenerated]                 │");
    Console.WriteLine("│ @Column                       │ [Column] o Fluent API               │");
    Console.WriteLine("│ JpaRepository<T, ID>          │ DbSet<T>                            │");
    Console.WriteLine("│ JPQL/HQL                      │ LINQ                                │");
    Console.WriteLine("│ entityManager.persist()       │ context.Add()                       │");
    Console.WriteLine("│ entityManager.find()          │ context.Find() / FindAsync()        │");
    Console.WriteLine("│ entityManager.merge()         │ context.Update()                    │");
    Console.WriteLine("│ entityManager.remove()        │ context.Remove()                    │");
    Console.WriteLine("│ @Query(\"SELECT...\")           │ LINQ queries                        │");
    Console.WriteLine("│ repository.save()             │ context.SaveChanges()               │");
    Console.WriteLine("│ @Transactional                │ using transactions or SaveChanges   │");
    Console.WriteLine("└───────────────────────────────┴─────────────────────────────────────┘");
    Console.WriteLine();
    Console.WriteLine("💡 VENTAJAS de Entity Framework Core:");
    Console.WriteLine("  ✅ LINQ: consultas type-safe y expresivas");
    Console.WriteLine("  ✅ Change Tracking automático (como Hibernate)");
    Console.WriteLine("  ✅ Migraciones integradas (como Flyway/Liquibase)");
    Console.WriteLine("  ✅ Menos código boilerplate que ADO.NET");
    Console.WriteLine("  ✅ Soporte para múltiples bases de datos");
    Console.WriteLine();
    Console.WriteLine("💡 LINQ vs JPQL:");
    Console.WriteLine("  📌 JPQL (String): \"SELECT t FROM Tenista t WHERE t.pais = :pais\"");
    Console.WriteLine("  📌 LINQ (Code): context.Tenistas.Where(t => t.Pais == pais)");
    Console.WriteLine("  ✅ LINQ es type-safe, refactorable y compilado");
    Console.WriteLine();
    Console.WriteLine("👉 Para acceso manual (como JDBC), ver ejemplo 01-AccesoAdoNet");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error: {ex.Message}");
    Console.WriteLine($"   Asegúrate de que PostgreSQL esté ejecutándose:");
    Console.WriteLine($"   docker run -d -p 5432:5432 -e POSTGRES_DB=tenistas_db -e POSTGRES_USER=admin -e POSTGRES_PASSWORD=admin123 postgres:15");
}

Console.WriteLine();
Console.WriteLine("╔══════════════════════════════════════════════════════════════════════╗");
Console.WriteLine("║  ✨ Ejemplo completado - Entity Framework Core vs JPA               ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════════════════╝");

// ==================================================================================
// 👨‍💻 Autor: José Luis González Sánchez
// 📧 Email: joseluis.gonzalez@profesor.com
// 🌐 Web: https://joseluisgs.dev
// 📅 Fecha: Octubre 2025
// 📝 Licencia: Creative Commons BY-NC-SA 4.0
// ==================================================================================
