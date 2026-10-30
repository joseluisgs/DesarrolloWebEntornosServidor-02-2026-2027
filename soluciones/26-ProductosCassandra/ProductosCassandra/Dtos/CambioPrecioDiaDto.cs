namespace ProductosCassandra.Dtos;

/// <summary>
/// Una fila del informe de precios cambiados en un día.
/// </summary>
public record CambioPrecioDiaDto(
    long ProductoId,
    double Precio)
{
    /// <summary>Ctor por defecto para la serialización.</summary>
    public CambioPrecioDiaDto() : this(0, 0) { }
}
