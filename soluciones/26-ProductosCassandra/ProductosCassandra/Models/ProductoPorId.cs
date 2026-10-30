using Cassandra.Mapping.Attributes;

namespace ProductosCassandra.Models;

/// <summary>
/// Fila de la tabla de apoyo: el producto suelto por su id (consulta Q2).
/// Si el id no tuviera su propia tabla, la pregunta "¿de qué categoría es?"
/// obligaría a barrer todas las particiones.
/// </summary>
[Table("productos_por_id")]
public class ProductoPorId
{
    /// <summary>Identificador unico del producto (partition key unica).</summary>
    [Column("id")]
    [PartitionKey]
    public long Id { get; set; }

    /// <summary>Nombre del producto.</summary>
    [Column("nombre")]
    public string Nombre { get; set; } = string.Empty;

    /// <summary>Precio del producto.</summary>
    [Column("precio")]
    public double Precio { get; set; }

    /// <summary> Categoria a la que pertenece (solo el id: el nombre ya vive en la otra tabla).</summary>
    [Column("categoria_id")]
    public long CategoriaId { get; set; }

    /// <summary>Copia del nombre de la categoria, para responder sin cruzar tablas.</summary>
    [Column("categoria_nombre")]
    public string CategoriaNombre { get; set; } = string.Empty;
}
