namespace ProductosEFCore.Dtos;

public record ProveedorDto(
    string Id,
    string Nombre,
    string Contacto,
    bool IsDeleted,
    DateTime CreatedAt,
    DateTime? UpdatedAt)
{
    public ProveedorDto() : this(string.Empty, string.Empty, string.Empty, false, DateTime.MinValue, null) { }
}

public record CreateProveedorDto
{
    public string Nombre { get; init; } = string.Empty;
    public string Contacto { get; init; } = string.Empty;
}

public record UpdateProveedorDto
{
    public string Nombre { get; init; } = string.Empty;
    public string Contacto { get; init; } = string.Empty;
}
