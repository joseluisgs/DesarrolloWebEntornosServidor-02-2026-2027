using Cassandra.Mapping.Attributes;

namespace ProductosCassandra.Models;

/// <summary>
/// Categoria del catalogo. Tabla simple de lectura directa (consulta Q3).
/// </summary>
[Table("categorias")]
public class Categoria
{
    /// <summary>Identificador de la categoria.</summary>
    [Column("id")]
    [PartitionKey]
    public long Id { get; set; }

    /// <summary>Nombre visible de la categoria.</summary>
    [Column("nombre")]
    public string Nombre { get; set; } = string.Empty;

    /// <summary>Descripcion corta.</summary>
    [Column("descripcion")]
    public string Descripcion { get; set; } = string.Empty;
}
