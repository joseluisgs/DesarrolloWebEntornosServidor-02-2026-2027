namespace ProductosMicroService.Models;

/// <summary>
/// Modelo de dominio: Categoria de productos.
/// </summary>
public class Categoria
{
    public long Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Descripcion { get; set; } = string.Empty;
}

/// <summary>
/// Modelo de dominio: Producto.
/// Contiene una Categoria y referencia a un Proveedor (remoto).
/// </summary>
public class Producto
{
    public long Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public decimal Precio { get; set; }

    public string? Imagen { get; set; }

    public long CategoriaId { get; set; }

    public Categoria? Categoria { get; set; }

    /// <summary>
    /// ObjectId del proveedor en MongoDB (no es numérico).
    /// </summary>
    public string ProveedorId { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
