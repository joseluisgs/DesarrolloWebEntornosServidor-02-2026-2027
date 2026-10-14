using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductosEFCore.Models;

public class Producto
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    // MinimumLength = 1 alimenta el CHECK del paquete: CHECK (LENGTH("Nombre") >= 1).
    // Sin [Required]: los dos juntos generan el mismo nombre de CHECK
    // (CK_Producto_Nombre_MinLength) y el paquete revienta al construir el modelo.
    // Sobre un string no nullable la columna ya sale NOT NULL sin el atributo.
    [StringLength(100, MinimumLength = 1)]
    public string Nombre { get; set; } = string.Empty;

    // Sin [Required]: en un decimal no aporta nada (la columna ya es NOT NULL) y con
    // EFCore.CheckConstraints provocaría un CHECK de longitud sobre columna numérica.
    // El precio no negativo lo garantiza el CHECK de AppDbContext.
    public decimal Precio { get; set; }

    // La longitud máxima vive en la fluent API (AppDbContext), no con [StringLength]:
    // el paquete solo traduce el mínimo, y sin mínimo generaría un CHECK >= 0 inútil.
    public string Imagen { get; set; } = string.Empty;

    public long CategoriaId { get; set; }
    public Categoria Categoria { get; set; } = null!;

    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
