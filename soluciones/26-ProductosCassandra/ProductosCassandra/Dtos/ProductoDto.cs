namespace ProductosCassandra.Dtos;

/// <summary>
/// Producto proyectado para la respuesta HTTP.
/// El nombre de la categoría viene copiado en la fila de Cassandra (denormalización),
/// así que el DTO lo expone tal cual: el controlador no cruza nada.
/// </summary>
public record ProductoDto(
    long Id,
    string Nombre,
    double Precio,
    long CategoriaId,
    string CategoriaNombre)
{
    /// <summary>Ctor por defecto para la serialización.</summary>
    public ProductoDto() : this(0, string.Empty, 0, 0, string.Empty) { }
}
