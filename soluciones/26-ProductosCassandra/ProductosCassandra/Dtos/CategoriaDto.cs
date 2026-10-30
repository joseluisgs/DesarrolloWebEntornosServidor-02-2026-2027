namespace ProductosCassandra.Dtos;

/// <summary>
/// Categoría del catálogo, proyectada para la respuesta HTTP.
/// </summary>
public record CategoriaDto(
    long Id,
    string Nombre,
    string? Descripcion)
{
    /// <summary>Ctor por defecto para la serialización.</summary>
    public CategoriaDto() : this(0, string.Empty, null) { }
}
