using System.ComponentModel.DataAnnotations;

namespace ProductosEFCore.Dtos;

public record UpdateProductoDto
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [MaxLength(100)]
    public string Nombre { get; init; } = string.Empty;

    [Range(0.01, 999999.99, ErrorMessage = "El precio debe estar entre 0.01 y 999999.99")]
    public decimal Precio { get; init; }

    [Required(ErrorMessage = "La categoría es obligatoria")]
    public long CategoriaId { get; init; }

    public string? Imagen { get; init; }
}
