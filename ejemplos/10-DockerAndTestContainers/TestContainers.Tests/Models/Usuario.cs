namespace TestContainers.Tests.Models;

public class Usuario
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime FechaRegistro { get; set; }

    public override string ToString() => $"{Id}. {Nombre} - {Email}";
}
