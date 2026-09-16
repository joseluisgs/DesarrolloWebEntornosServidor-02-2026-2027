namespace AccesoAdoNet.Console.Models;

/// <summary>
/// Modelo de Tenista - Equivalente a una entidad Java
/// Representa un jugador de tenis con sus datos básicos
/// 
/// En Java sería:
/// public class Tenista {
///     private Long id;
///     private String nombre;
///     private int ranking;
///     // getters y setters
/// }
/// </summary>
public class Tenista
{
    /// <summary>
    /// ID único del tenista (autoincremental en base de datos)
    /// En Java: private Long id;
    /// En C#: Propiedades con { get; set; } automáticas
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Nombre completo del tenista
    /// En Java: private String nombre;
    /// En C#: string es alias de System.String
    /// </summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Ranking ATP del tenista
    /// En Java: private int ranking;
    /// En C#: int es alias de System.Int32
    /// </summary>
    public int Ranking { get; set; }

    /// <summary>
    /// País de origen del tenista
    /// </summary>
    public string Pais { get; set; } = string.Empty;

    /// <summary>
    /// Altura en centímetros
    /// </summary>
    public int Altura { get; set; }

    /// <summary>
    /// Peso en kilogramos
    /// </summary>
    public int Peso { get; set; }

    /// <summary>
    /// Número de títulos ganados
    /// </summary>
    public int Titulos { get; set; }

    /// <summary>
    /// Fecha de nacimiento
    /// En Java: LocalDate
    /// En C#: DateTime o DateOnly (C# 10+)
    /// </summary>
    public DateTime FechaNacimiento { get; set; }

    /// <summary>
    /// Override de ToString() para representación textual
    /// En Java: @Override public String toString()
    /// En C#: override solo para métodos virtuales de Object
    /// </summary>
    public override string ToString()
    {
        return $"Tenista{{Id={Id}, Nombre='{Nombre}', Ranking={Ranking}, País='{Pais}', " +
               $"Altura={Altura}cm, Peso={Peso}kg, Títulos={Titulos}, FechaNacimiento={FechaNacimiento:yyyy-MM-dd}}}";
    }
}
