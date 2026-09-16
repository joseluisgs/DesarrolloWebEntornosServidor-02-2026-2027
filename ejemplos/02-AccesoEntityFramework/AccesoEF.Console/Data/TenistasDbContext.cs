using AccesoEF.Console.Models;
using Microsoft.EntityFrameworkCore;

namespace AccesoEF.Console.Data;

/// <summary>
/// DbContext de Entity Framework Core
/// Equivalente a EntityManager en JPA/Hibernate
/// 
/// En Java (Spring Data JPA):
/// @Configuration
/// public class DatabaseConfig {
///     @Bean
///     public LocalContainerEntityManagerFactoryBean entityManagerFactory() { ... }
/// }
/// 
/// En C# (EF Core):
/// - DbContext = EntityManager (JPA)
/// - DbSet<T> = Repository<T> (Spring Data)
/// - OnModelCreating = @Entity mappings
/// 
/// El DbContext es el punto central de EF Core, similar a cómo el EntityManager
/// es el punto central de JPA en Java.
/// </summary>
public class TenistasDbContext : DbContext
{
    /// <summary>
    /// DbSet representa una tabla en la base de datos
    /// Equivalente a un Repository en Spring Data JPA
    /// 
    /// En Java/Spring Data:
    /// public interface TenistaRepository extends JpaRepository<Tenista, Long> { }
    /// 
    /// En C#/EF Core:
    /// public DbSet<Tenista> Tenistas { get; set; }
    /// 
    /// Ambos proporcionan operaciones CRUD automáticas
    /// </summary>
    public DbSet<Tenista> Tenistas { get; set; } = null!;

    /// <summary>
    /// Constructor que recibe opciones de configuración
    /// En Java/JPA esto se configura en application.properties o persistence.xml
    /// </summary>
    public TenistasDbContext(DbContextOptions<TenistasDbContext> options) 
        : base(options)
    {
    }

    /// <summary>
    /// Configuración del modelo y relaciones
    /// Equivalente a las anotaciones @Entity, @Table, @Column en Java/JPA
    /// 
    /// En Java/JPA:
    /// @Entity
    /// @Table(name = "tenistas")
    /// public class Tenista { ... }
    /// 
    /// En C#/EF Core puedes usar:
    /// 1. Data Annotations: [Table("tenistas")] en la clase
    /// 2. Fluent API: modelBuilder.Entity<Tenista>().ToTable("tenistas") aquí
    /// 
    /// Fluent API es más potente y similar a Hibernate's Configuration en Java
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración de la entidad Tenista con Fluent API
        // Equivalente a @Entity, @Table, etc. en Java/JPA
        modelBuilder.Entity<Tenista>(entity =>
        {
            // Nombre de la tabla
            // En Java/JPA: @Table(name = "tenistas")
            entity.ToTable("tenistas");

            // Clave primaria
            // En Java/JPA: @Id
            entity.HasKey(t => t.Id);

            // Configuración de columnas
            // En Java/JPA: @Column(name = "nombre", nullable = false, length = 100)
            entity.Property(t => t.Nombre)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("nombre");

            entity.Property(t => t.Ranking)
                .IsRequired()
                .HasColumnName("ranking");

            entity.Property(t => t.Pais)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("pais");

            entity.Property(t => t.Altura)
                .IsRequired()
                .HasColumnName("altura");

            entity.Property(t => t.Peso)
                .IsRequired()
                .HasColumnName("peso");

            entity.Property(t => t.Titulos)
                .IsRequired()
                .HasColumnName("titulos");

            entity.Property(t => t.FechaNacimiento)
                .IsRequired()
                .HasColumnName("fecha_nacimiento")
                .HasColumnType("date");

            // Índices (opcional pero recomendado para búsquedas frecuentes)
            // En Java/JPA: @Table(indexes = {@Index(name = "idx_ranking", columnList = "ranking")})
            entity.HasIndex(t => t.Ranking)
                .HasDatabaseName("idx_ranking");

            entity.HasIndex(t => t.Pais)
                .HasDatabaseName("idx_pais");
        });
    }

    /// <summary>
    /// Configuración de la conexión (solo si no se pasa en el constructor)
    /// En Java/JPA esto estaría en application.properties:
    /// 
    /// spring.datasource.url=jdbc:postgresql://localhost:5432/tenistas_db
    /// spring.datasource.username=admin
    /// spring.datasource.password=admin123
    /// spring.jpa.hibernate.ddl-auto=update
    /// 
    /// En C#/EF Core se configura aquí o al crear el DbContext
    /// </summary>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql(
                "Host=localhost;Port=5432;Database=tenistas_db;Username=admin;Password=admin123");
        }
    }
}
