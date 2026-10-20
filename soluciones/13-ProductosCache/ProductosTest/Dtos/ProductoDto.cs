namespace ProductosEFCore.Dtos;

public record ProductoDto(
    long Id,
    string Nombre,
    decimal Precio,
    string CategoriaNombre,
    string Imagen,
    bool IsDeleted,
    DateTime CreatedAt,
    DateTime? UpdatedAt)
{
    public ProductoDto() : this(0, string.Empty, 0, string.Empty, string.Empty, false, DateTime.MinValue, null) { }
}

public record CategoriaDto(
    long Id,
    string Nombre,
    string Descripcion)
{
    public CategoriaDto() : this(0, string.Empty, string.Empty) { }
}
