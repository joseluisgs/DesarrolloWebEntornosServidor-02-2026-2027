using System.ComponentModel.DataAnnotations;

namespace ProductosCassandra.Dtos;

/// <summary>
/// Datos de entrada para dar de alta un producto.
/// El id lo pone quien llama: en Cassandra no hay autoincremento, el id se genera
/// en la aplicación (por eso es obligatorio en el DTO y no lo es en un relacional).
/// </summary>
public record CreateProductoDto
{
    /// <summary>Identificador del producto nuevo.</summary>
    [Range(1, long.MaxValue, ErrorMessage = "El id debe ser positivo")]
    public long Id { get; init; }

    /// <summary>Nombre del producto.</summary>
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [MaxLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres")]
    public string Nombre { get; init; } = string.Empty;

    /// <summary>Precio del producto.</summary>
    [Range(0.01, 999999.99, ErrorMessage = "El precio debe estar entre 0.01 y 999999.99")]
    public double Precio { get; init; }

    /// <summary>Categoría a la que pertenece: de ahí se copia el nombre en cada fila.</summary>
    [Range(1, long.MaxValue, ErrorMessage = "La categoría es obligatoria")]
    public long CategoriaId { get; init; }
}
