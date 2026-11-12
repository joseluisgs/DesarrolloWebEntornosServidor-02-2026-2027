namespace ProductosCQRS.Dtos;

public record ProductoReadDto
{
    public long Id { get; init; }
    public string Nombre { get; init; } = string.Empty;
    public decimal Precio { get; init; }
    public string CategoriaNombre { get; init; } = string.Empty;
    public string ProveedorNombre { get; init; } = string.Empty;
    public DateTime SyncAt { get; init; }
}
