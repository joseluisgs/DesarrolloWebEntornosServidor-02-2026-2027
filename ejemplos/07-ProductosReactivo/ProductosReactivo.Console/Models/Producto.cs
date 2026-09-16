namespace ProductosReactivo.Console.Models;

public class Producto
{
    public long Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public string Categoria { get; set; } = string.Empty;
    public int Stock { get; set; }

    public override string ToString() => $"{Nombre} - ${Precio:F2} ({Categoria}) - Stock: {Stock}";
}
