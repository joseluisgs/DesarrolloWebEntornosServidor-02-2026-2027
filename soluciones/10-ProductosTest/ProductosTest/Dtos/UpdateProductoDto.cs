using System.ComponentModel.DataAnnotations;

namespace ProductosTest.Dtos;

public record UpdateProductoDto
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [MaxLength(100)]
    public string Nombre { get; init; } = string.Empty;

    [Range(0.01, 999999.99, ErrorMessage = "El precio debe estar entre 0.01 y 999999.99")]
    public decimal Precio { get; init; }

    [Required(ErrorMessage = "La categoría es obligatoria")]
    public string Categoria { get; init; } = string.Empty;

    public string? Imagen { get; init; }
}
