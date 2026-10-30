using Cassandra.Mapping.Attributes;

namespace ProductosCassandra.Models;

/// <summary>
/// Fila de la tabla por consulta: una fila por producto DENTRO de su categoria.
/// Es el modelo de la consulta Q1 ("los productos de la categoria 3").
/// </summary>
[Table("productos_por_categoria")]
public class ProductoPorCategoria
{
    /// <summary>
    /// Partition key: decide donde vive el dato y que lectura es barata.
    /// Leer una categoria entera es leer una particion: una sola llamada, un solo nodo.
    /// </summary>
    [Column("categoria_id")]
    [PartitionKey]
    public long CategoriaId { get; set; }

    /// <summary>
    /// Clustering key: el orden dentro de la particion.
    /// Los productos salen ordenados por ella de serie, sin ningun ORDER BY.
    /// </summary>
    [Column("producto_id")]
    [ClusteringKey]
    public long ProductoId { get; set; }

    /// <summary>Nombre del producto.</summary>
    [Column("nombre")]
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Precio. CQL "double" se mapa a double; el "decimal" de CQL se mapa a
    /// Cassandra.BigDecimal del driver, que no es System.Decimal.
    /// </summary>
    [Column("precio")]
    public double Precio { get; set; }

    /// <summary>
    /// Denormalizacion: el nombre de la categoria se COPIA en cada fila.
    /// No hay joins en Cassandra; quien escribe paga el cruce.
    /// </summary>
    [Column("categoria_nombre")]
    public string CategoriaNombre { get; set; } = string.Empty;
}
