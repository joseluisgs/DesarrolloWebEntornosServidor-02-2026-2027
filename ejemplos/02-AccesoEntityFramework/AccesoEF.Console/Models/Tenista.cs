using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccesoEF.Console.Models;

/// <summary>
/// Entidad Tenista con Entity Framework Core
/// Equivalente a una @Entity de JPA en Java
/// 
/// En Java (JPA):
/// @Entity
/// @Table(name = "tenistas")
/// public class Tenista {
///     @Id
///     @GeneratedValue(strategy = GenerationType.IDENTITY)
///     private Long id;
///     
///     @Column(name = "nombre", nullable = false, length = 100)
///     private String nombre;
///     ...
/// }
/// 
/// En C# (EF Core):
/// - [Table] → @Table
/// - [Key] → @Id
/// - [DatabaseGenerated(DatabaseGeneratedOption.Identity)] → @GeneratedValue
/// - [Column] → @Column
/// - [Required] → @Column(nullable = false)
/// - [MaxLength] → @Column(length = ...)
/// </summary>
[Table("tenistas")]
public class Tenista
{
    /// <summary>
    /// ID autoincremental
    /// En Java/JPA: @Id @GeneratedValue(strategy = GenerationType.IDENTITY)
    /// En C#/EF: [Key] [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public long Id { get; set; }

    /// <summary>
    /// Nombre del tenista (obligatorio, máximo 100 caracteres)
    /// En Java/JPA: @Column(name = "nombre", nullable = false, length = 100)
    /// En C#/EF: [Required] [MaxLength(100)] [Column("nombre")]
    /// </summary>
    [Required]
    [MaxLength(100)]
    [Column("nombre")]
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Ranking ATP
    /// En Java/JPA: @Column(name = "ranking", nullable = false)
    /// </summary>
    [Required]
    [Column("ranking")]
    public int Ranking { get; set; }

    /// <summary>
    /// País de origen
    /// En Java/JPA: @Column(name = "pais", nullable = false, length = 50)
    /// </summary>
    [Required]
    [MaxLength(50)]
    [Column("pais")]
    public string Pais { get; set; } = string.Empty;

    /// <summary>
    /// Altura en centímetros
    /// </summary>
    [Required]
    [Column("altura")]
    public int Altura { get; set; }

    /// <summary>
    /// Peso en kilogramos
    /// </summary>
    [Required]
    [Column("peso")]
    public int Peso { get; set; }

    /// <summary>
    /// Número de títulos ganados
    /// </summary>
    [Required]
    [Column("titulos")]
    public int Titulos { get; set; }

    /// <summary>
    /// Fecha de nacimiento
    /// En Java/JPA: @Column(name = "fecha_nacimiento")
    ///               private LocalDate fechaNacimiento;
    /// En C#/EF: DateTime (compatible con DATE de PostgreSQL)
    /// </summary>
    [Required]
    [Column("fecha_nacimiento")]
    public DateTime FechaNacimiento { get; set; }

    /// <summary>
    /// Representación textual
    /// </summary>
    public override string ToString()
    {
        return $"Tenista{{Id={Id}, Nombre='{Nombre}', Ranking={Ranking}, País='{Pais}', " +
               $"Altura={Altura}cm, Peso={Peso}kg, Títulos={Titulos}, FechaNacimiento={FechaNacimiento:yyyy-MM-dd}}}";
    }
}
