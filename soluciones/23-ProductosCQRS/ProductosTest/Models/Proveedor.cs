using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductosCQRS.Models;

public class Proveedor
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(200)]
    public string Direccion { get; set; } = string.Empty;

    [StringLength(20)]
    public string Telefono { get; set; } = string.Empty;

    public ICollection<Producto> Productos { get; set; } = [];
}
