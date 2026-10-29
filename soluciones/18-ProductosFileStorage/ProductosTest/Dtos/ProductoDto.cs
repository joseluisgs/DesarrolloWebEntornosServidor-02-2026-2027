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

    /// <summary>
    /// Identificador de la categoría del producto. Es un miembro interno: no viaja en la
    /// respuesta (ni en JSON ni en XML) y no aparece en Swagger. Solo lo usan la capa de
    /// servicio y el controlador para reconstruir el <see cref="UpdateProductoDto"/> que
    /// necesita el PATCH de imagen, que conserva la categoría original.
    /// </summary>
    internal long CategoriaId { get; init; }
}

public record CategoriaDto(
    long Id,
    string Nombre,
    string Descripcion)
{
    public CategoriaDto() : this(0, string.Empty, string.Empty) { }
}
