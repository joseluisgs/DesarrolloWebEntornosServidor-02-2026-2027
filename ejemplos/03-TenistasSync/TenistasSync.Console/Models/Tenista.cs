namespace TenistasSync.Console.Models;

/// <summary>
/// Modelo de Tenista - Simple POCO (Plain Old CLR Object)
/// Equivalente a POJO (Plain Old Java Object) en Java
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
    public long Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int Ranking { get; set; }
    public string Pais { get; set; } = string.Empty;
    public int Altura { get; set; }
    public int Peso { get; set; }
    public int Titulos { get; set; }
    public DateTime FechaNacimiento { get; set; }

    /// <summary>
    /// Calcula la edad del tenista
    /// En Java: public int getEdad() { ... }
    /// En C#: property calculada o método
    /// </summary>
    public int Edad => DateTime.Now.Year - FechaNacimiento.Year;

    /// <summary>
    /// ToString para representación textual
    /// En Java: @Override public String toString()
    /// En C#: override string ToString()
    /// </summary>
    public override string ToString()
    {
        return $"Tenista{{Id={Id}, Nombre='{Nombre}', Ranking={Ranking}, País='{Pais}', " +
               $"Edad={Edad}, Altura={Altura}cm, Peso={Peso}kg, Títulos={Titulos}}}";
    }

    /// <summary>
    /// Equals para comparación de objetos
    /// En Java: @Override public boolean equals(Object obj)
    /// En C#: override bool Equals(object? obj)
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is not Tenista other) return false;
        return Id == other.Id;
    }

    /// <summary>
    /// GetHashCode para uso en colecciones hash
    /// En Java: @Override public int hashCode()
    /// En C#: override int GetHashCode()
    /// </summary>
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}
