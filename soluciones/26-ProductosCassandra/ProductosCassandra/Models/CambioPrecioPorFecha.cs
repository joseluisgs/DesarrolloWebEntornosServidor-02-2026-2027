using Cassandra.Mapping.Attributes;

namespace ProductosCassandra.Models;

/// <summary>
/// Fila del informe de cambios por día (consulta Q5).
/// La partition key es el DÍA: la partición tiene techo por diseño (bucketing),
/// y "¿qué precios cambiaron el lunes?" es una sola lectura.
/// </summary>
[Table("cambios_precio_por_fecha")]
public class CambioPrecioPorFecha
{
    /// <summary>Partition key: el día completo, con crecimiento acotado.
    /// Se guarda como timestamp a medianoche: CQL date pediría DateOnly y el
    /// driver 3.23 no lo mapea ("Unknown Cassandra target type").
    /// </summary>
    [Column("fecha")]
    [PartitionKey]
    public DateTime Fecha { get; set; }

    /// <summary>Clustering key: los cambios del día, ordenados por producto.</summary>
    [Column("producto_id")]
    [ClusteringKey]
    public long ProductoId { get; set; }

    /// <summary>Precio nuevo del producto.</summary>
    [Column("precio")]
    public double Precio { get; set; }
}
