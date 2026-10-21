using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProductosEFCore.IdGenerators;

namespace ProductosEFCore.Models;

public class Proveedor
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [YouTubeId]
    public string Id { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Contacto { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
