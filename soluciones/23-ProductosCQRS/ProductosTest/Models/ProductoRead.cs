namespace ProductosCQRS.Models;

public class ProductoRead
{
    public long Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public CategoriaRead? Categoria { get; set; }
    public ProveedorRead? Proveedor { get; set; }
    public DateTime SyncAt { get; set; }
}

public class CategoriaRead
{
    public long Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
}

public class ProveedorRead
{
    public long Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
}
