namespace ProductosCassandra.Dtos;

/// <summary>
/// Un cambio del histórico de precios: cuándo, a cuánto y desde cuánto.
/// </summary>
public record HistorialPrecioDto(
    DateTime Fecha,
    double Precio,
    double PrecioAnterior)
{
    /// <summary>Ctor por defecto para la serialización.</summary>
    public HistorialPrecioDto() : this(DateTime.MinValue, 0, 0) { }
}
