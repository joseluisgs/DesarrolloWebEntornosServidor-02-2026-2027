using TenistasAsync.Console.Models;

namespace TenistasAsync.Console.Services;

/// <summary>
/// Servicio que demuestra IAsyncEnumerable
/// Equivalente a Java Streams con operaciones asíncronas
/// 
/// En Java:
/// Stream<Tenista> getTenistas() { ... }
/// 
/// En C#:
/// IAsyncEnumerable<Tenista> GetTenistasAsync() { ... }
/// </summary>
public class TenistaAsyncService
{
    /// <summary>
    /// Genera tenistas de forma asíncrona (streaming)
    /// En Java: Stream<T> con operaciones bloqueantes
    /// En C#: IAsyncEnumerable<T> con yield return asíncrono
    /// </summary>
    public async IAsyncEnumerable<Tenista> GetTenistasStreamAsync()
    {
        var tenistas = new[]
        {
            new Tenista { Id = 1, Nombre = "Rafael Nadal", Ranking = 1, Pais = "España" },
            new Tenista { Id = 2, Nombre = "Novak Djokovic", Ranking = 2, Pais = "Serbia" },
            new Tenista { Id = 3, Nombre = "Carlos Alcaraz", Ranking = 3, Pais = "España" },
            new Tenista { Id = 4, Nombre = "Roger Federer", Ranking = 4, Pais = "Suiza" },
            new Tenista { Id = 5, Nombre = "Andy Murray", Ranking = 5, Pais = "Reino Unido" }
        };

        foreach (var tenista in tenistas)
        {
            // Simular operación I/O asíncrona (como consulta a BD)
            await Task.Delay(200);
            yield return tenista;
        }
    }

    /// <summary>
    /// Filtra tenistas de forma asíncrona
    /// </summary>
    public async IAsyncEnumerable<Tenista> FiltrarPorPaisAsync(string pais)
    {
        await foreach (var tenista in GetTenistasStreamAsync())
        {
            if (tenista.Pais == pais)
            {
                yield return tenista;
            }
        }
    }

    /// <summary>
    /// Mapea tenistas a sus nombres de forma asíncrona
    /// </summary>
    public async IAsyncEnumerable<string> MapearANombresAsync()
    {
        await foreach (var tenista in GetTenistasStreamAsync())
        {
            yield return tenista.Nombre;
        }
    }
}
