using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductosEFCore.Models;

public class Categoria
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    // MinimumLength = 1 → CHECK (LENGTH("Nombre") >= 1) vía EFCore.CheckConstraints.
    // Sin [Required] para no duplicar el nombre del CHECK (ver Producto.Nombre).
    [StringLength(50, MinimumLength = 1)]
    public string Nombre { get; set; } = string.Empty;

    // La longitud máxima vive en la fluent API (AppDbContext).
    public string Descripcion { get; set; } = string.Empty;

    public ICollection<Producto> Productos { get; set; } = [];
}
