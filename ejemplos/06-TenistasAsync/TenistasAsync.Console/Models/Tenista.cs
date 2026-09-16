namespace TenistasAsync.Console.Models;

public class Tenista
{
    public long Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int Ranking { get; set; }
    public string Pais { get; set; } = string.Empty;

    public override string ToString() => $"{Ranking}. {Nombre} ({Pais})";
}
