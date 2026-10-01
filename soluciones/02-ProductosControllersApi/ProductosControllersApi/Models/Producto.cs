namespace ProductosControllersApi.Models;

/// <summary>
/// Representa un producto en el sistema.
/// </summary>
/// <remarks>
/// Es un <see langword="record"/> inmutable (<c>init</c>): para modificarlo se usa <c>with</c>.
/// El servidor asigna <see cref="Id"/> y <see cref="CreatedAt"/> en el repositorio, e ignora
/// esos valores si llegan en la petición.
/// </remarks>
public record Producto
{
    /// <summary>Identificador asignado por el servidor.</summary>
    public long Id { get; init; }

    /// <summary>Nombre del producto.</summary>
    public string Nombre { get; init; } = string.Empty;

    /// <summary>Precio en euros.</summary>
    public decimal Precio { get; init; }

    /// <summary>Categoría a la que pertenece.</summary>
    public string Categoria { get; init; } = string.Empty;

    /// <summary>URL de la imagen del producto, si tiene.</summary>
    public string? Imagen { get; init; }

    /// <summary>Fecha de creación (UTC).</summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>Última modificación (UTC), si la ha habido.</summary>
    public DateTime? UpdatedAt { get; init; }

    /// <summary>Fecha de borrado lógico (UTC); <c>null</c> mientras esté activo.</summary>
    public DateTime? DeletedAt { get; init; }

    /// <summary><c>true</c> si el producto no ha sido borrado lógicamente.</summary>
    public bool IsActivo => DeletedAt is null;
}
