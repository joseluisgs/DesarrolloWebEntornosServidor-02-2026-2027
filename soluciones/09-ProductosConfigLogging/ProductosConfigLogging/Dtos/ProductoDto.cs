namespace ProductosConfigLogging.Dtos;

public record ProductoDto(
    long Id,
    string Nombre,
    decimal Precio,
    string Categoria,
    string Imagen,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    bool IsActivo)
{
    public ProductoDto() : this(0, string.Empty, 0, string.Empty, string.Empty, DateTime.MinValue, null, false) { }
}
