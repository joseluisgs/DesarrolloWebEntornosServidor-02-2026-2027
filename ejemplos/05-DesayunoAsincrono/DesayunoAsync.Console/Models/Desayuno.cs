namespace DesayunoAsync.Console.Models;

public class Ingrediente
{
    public string Nombre { get; set; } = string.Empty;
    public int TiempoPreparacion { get; set; } // milisegundos

    public override string ToString() => $"{Nombre} ({TiempoPreparacion}ms)";
}

public class Desayuno
{
    public List<string> Componentes { get; set; } = new();

    public override string ToString() => $"Desayuno: {string.Join(", ", Componentes)}";
}
