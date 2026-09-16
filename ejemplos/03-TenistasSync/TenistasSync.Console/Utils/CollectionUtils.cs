using TenistasSync.Console.Models;

namespace TenistasSync.Console.Utils;

/// <summary>
/// Utilidades para trabajar con colecciones en C#
/// Demuestra operaciones comunes sobre List<T> que son equivalentes a ArrayList en Java
/// 
/// CONCEPTOS JAVA → C#:
/// - ArrayList<T> → List<T>
/// - Collections.sort() → List.Sort() o LINQ OrderBy
/// - Collections.filter() → LINQ Where
/// - Collections.map() → LINQ Select
/// - Stream API → LINQ (Language Integrated Query)
/// </summary>
public static class CollectionUtils
{
    /// <summary>
    /// Ordena una lista de tenistas por ranking (ascendente)
    /// 
    /// En Java:
    /// tenistas.sort(Comparator.comparing(Tenista::getRanking));
    /// 
    /// En C#:
    /// tenistas.Sort((a, b) => a.Ranking.CompareTo(b.Ranking));
    /// o con LINQ:
    /// tenistas.OrderBy(t => t.Ranking).ToList();
    /// </summary>
    public static List<Tenista> OrdenarPorRanking(List<Tenista> tenistas)
    {
        var ordenados = new List<Tenista>(tenistas);
        ordenados.Sort((a, b) => a.Ranking.CompareTo(b.Ranking));
        return ordenados;
    }

    /// <summary>
    /// Filtra tenistas por país
    /// 
    /// En Java (Stream API):
    /// return tenistas.stream()
    ///     .filter(t -> t.getPais().equals(pais))
    ///     .collect(Collectors.toList());
    /// 
    /// En C# (LINQ):
    /// return tenistas.Where(t => t.Pais == pais).ToList();
    /// </summary>
    public static List<Tenista> FiltrarPorPais(List<Tenista> tenistas, string pais)
    {
        return tenistas.Where(t => t.Pais == pais).ToList();
    }

    /// <summary>
    /// Mapea tenistas a sus nombres
    /// 
    /// En Java (Stream API):
    /// return tenistas.stream()
    ///     .map(Tenista::getNombre)
    ///     .collect(Collectors.toList());
    /// 
    /// En C# (LINQ):
    /// return tenistas.Select(t => t.Nombre).ToList();
    /// </summary>
    public static List<string> MapearANombres(List<Tenista> tenistas)
    {
        return tenistas.Select(t => t.Nombre).ToList();
    }

    /// <summary>
    /// Cuenta tenistas que cumplen una condición
    /// 
    /// En Java (Stream API):
    /// return (int) tenistas.stream()
    ///     .filter(predicado)
    ///     .count();
    /// 
    /// En C# (LINQ):
    /// return tenistas.Count(predicado);
    /// </summary>
    public static int ContarConCondicion(List<Tenista> tenistas, Func<Tenista, bool> predicado)
    {
        return tenistas.Count(predicado);
    }

    /// <summary>
    /// Encuentra el primer tenista que cumple una condición
    /// 
    /// En Java (Stream API):
    /// return tenistas.stream()
    ///     .filter(predicado)
    ///     .findFirst()
    ///     .orElse(null);
    /// 
    /// En C# (LINQ):
    /// return tenistas.FirstOrDefault(predicado);
    /// </summary>
    public static Tenista? EncontrarPrimero(List<Tenista> tenistas, Func<Tenista, bool> predicado)
    {
        return tenistas.FirstOrDefault(predicado);
    }

    /// <summary>
    /// Verifica si algún tenista cumple una condición
    /// 
    /// En Java (Stream API):
    /// return tenistas.stream().anyMatch(predicado);
    /// 
    /// En C# (LINQ):
    /// return tenistas.Any(predicado);
    /// </summary>
    public static bool AlgunoCumple(List<Tenista> tenistas, Func<Tenista, bool> predicado)
    {
        return tenistas.Any(predicado);
    }

    /// <summary>
    /// Verifica si todos los tenistas cumplen una condición
    /// 
    /// En Java (Stream API):
    /// return tenistas.stream().allMatch(predicado);
    /// 
    /// En C# (LINQ):
    /// return tenistas.All(predicado);
    /// </summary>
    public static bool TodosCumplen(List<Tenista> tenistas, Func<Tenista, bool> predicado)
    {
        return tenistas.All(predicado);
    }

    /// <summary>
    /// Agrupa tenistas por país
    /// 
    /// En Java (Stream API):
    /// return tenistas.stream()
    ///     .collect(Collectors.groupingBy(Tenista::getPais));
    /// 
    /// En C# (LINQ):
    /// return tenistas.GroupBy(t => t.Pais)
    ///     .ToDictionary(g => g.Key, g => g.ToList());
    /// </summary>
    public static Dictionary<string, List<Tenista>> AgruparPorPais(List<Tenista> tenistas)
    {
        return tenistas
            .GroupBy(t => t.Pais)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    /// <summary>
    /// Calcula estadísticas sobre los títulos
    /// 
    /// En Java (Stream API):
    /// IntSummaryStatistics stats = tenistas.stream()
    ///     .collect(Collectors.summarizingInt(Tenista::getTitulos));
    /// 
    /// En C# (LINQ):
    /// Varias operaciones: Sum, Average, Min, Max
    /// </summary>
    public static (int Total, double Promedio, int Minimo, int Maximo) EstadisticasTitulos(List<Tenista> tenistas)
    {
        if (!tenistas.Any())
            return (0, 0, 0, 0);

        return (
            Total: tenistas.Sum(t => t.Titulos),
            Promedio: tenistas.Average(t => t.Titulos),
            Minimo: tenistas.Min(t => t.Titulos),
            Maximo: tenistas.Max(t => t.Titulos)
        );
    }

    /// <summary>
    /// Obtiene los top N tenistas por ranking
    /// 
    /// En Java (Stream API):
    /// return tenistas.stream()
    ///     .sorted(Comparator.comparing(Tenista::getRanking))
    ///     .limit(n)
    ///     .collect(Collectors.toList());
    /// 
    /// En C# (LINQ):
    /// return tenistas.OrderBy(t => t.Ranking).Take(n).ToList();
    /// </summary>
    public static List<Tenista> TopN(List<Tenista> tenistas, int n)
    {
        return tenistas
            .OrderBy(t => t.Ranking)
            .Take(n)
            .ToList();
    }

    /// <summary>
    /// Partición de lista en dos grupos según un predicado
    /// 
    /// En Java (Stream API):
    /// Map<Boolean, List<Tenista>> partitioned = tenistas.stream()
    ///     .collect(Collectors.partitioningBy(predicado));
    /// 
    /// En C# (LINQ):
    /// No hay método directo, pero se puede hacer con Where
    /// </summary>
    public static (List<Tenista> Cumple, List<Tenista> NoCumple) Particionar(
        List<Tenista> tenistas,
        Func<Tenista, bool> predicado)
    {
        return (
            Cumple: tenistas.Where(predicado).ToList(),
            NoCumple: tenistas.Where(t => !predicado(t)).ToList()
        );
    }
}
