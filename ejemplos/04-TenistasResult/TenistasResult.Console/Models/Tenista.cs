namespace TenistasResult.Console.Models;

public class Tenista
{
    public long Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int Ranking { get; set; }
    public string Pais { get; set; } = string.Empty;
    public int Titulos { get; set; }

    public override string ToString() =>
        $"Tenista{{Id={Id}, Nombre='{Nombre}', Ranking={Ranking}, País='{Pais}', Títulos={Titulos}}}";
}
