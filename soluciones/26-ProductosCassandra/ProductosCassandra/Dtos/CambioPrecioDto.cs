using System.ComponentModel.DataAnnotations;

namespace ProductosCassandra.Dtos;

/// <summary>
/// Datos de entrada para registrar un cambio de precio.
/// </summary>
public record CambioPrecioDto
{
    /// <summary>Precio nuevo del producto.</summary>
    [Range(0.01, 999999.99, ErrorMessage = "El precio debe estar entre 0.01 y 999999.99")]
    public double Precio { get; init; }
}
