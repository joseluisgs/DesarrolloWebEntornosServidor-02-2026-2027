using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductosGraphQL.Models;

public class Producto
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    public decimal Precio { get; set; }

    [StringLength(500)]
    public string Imagen { get; set; } = string.Empty;

    public long CategoriaId { get; set; }
    public Categoria Categoria { get; set; } = null!;

    /// <summary>
    /// ID del usuario que creo el producto (propietario).
    /// Se usa para notificar al propietario cuando se actualiza stock.
    /// </summary>
    public long? UsuarioId { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
