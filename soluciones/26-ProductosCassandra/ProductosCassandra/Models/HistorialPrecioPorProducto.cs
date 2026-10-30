using Cassandra.Mapping.Attributes;

namespace ProductosCassandra.Models;

/// <summary>
/// Serie temporal de precios: una fila por cada cambio (consulta Q4).
/// El orden temporal está GUARDADO en la tabla con CLUSTERING ORDER BY (fecha DESC),
/// así que "¿a cuánto estuvo el producto hace tres semanas?" sale ordenado de serie.
/// </summary>
[Table("historial_precios_por_producto")]
public class HistorialPrecioPorProducto
{
    /// <summary>Partition key: el histórico de un producto vive entero en su partición.</summary>
    [Column("producto_id")]
    [PartitionKey]
    public long ProductoId { get; set; }

    /// <summary>Clustering key en orden descendente: los cambios recientes salen primero.</summary>
    [Column("fecha")]
    [ClusteringKey]
    public DateTime Fecha { get; set; }

    /// <summary>Precio del producto en ese momento.</summary>
    [Column("precio")]
    public double Precio { get; set; }

    /// <summary>
    /// El precio anterior: la fila ya sabe cuánto cambió, sin funciones de ventana.
    /// </summary>
    [Column("precio_anterior")]
    public double PrecioAnterior { get; set; }
}
