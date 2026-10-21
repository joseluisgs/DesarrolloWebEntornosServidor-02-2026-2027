namespace ProductosEFCore.Dtos;

public record ProductoDto(
    long Id,
    string Nombre,
    decimal Precio,
    string CategoriaNombre,
    string Imagen,
    int Stock,
    bool IsDeleted,
    DateTime CreatedAt,
    DateTime? UpdatedAt)
{
    public ProductoDto() : this(0, string.Empty, 0, string.Empty, string.Empty, 0, false, DateTime.MinValue, null) { }
}

public record CategoriaDto(
    Guid Id,
    string Nombre,
    string Descripcion)
{
    public CategoriaDto() : this(Guid.Empty, string.Empty, string.Empty) { }
}
