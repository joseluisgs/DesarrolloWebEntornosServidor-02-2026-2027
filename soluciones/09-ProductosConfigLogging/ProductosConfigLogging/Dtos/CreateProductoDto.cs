using System.ComponentModel.DataAnnotations;

namespace ProductosConfigLogging.Dtos;

public record CreateProductoDto
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [MaxLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres")]
    [NoAdmin]
    public string Nombre { get; init; } = string.Empty;

    [Required(ErrorMessage = "El precio es obligatorio")]
    [Range(0.01, 999999.99, ErrorMessage = "El precio debe estar entre 0.01 y 999999.99")]
    public decimal Precio { get; init; }

    [Required(ErrorMessage = "La categoría es obligatoria")]
    public string Categoria { get; init; } = string.Empty;

    public string? Imagen { get; init; }
}
