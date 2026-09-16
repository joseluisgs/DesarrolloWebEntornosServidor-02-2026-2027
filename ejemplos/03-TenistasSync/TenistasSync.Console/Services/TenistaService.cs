using TenistasSync.Console.Models;
using TenistasSync.Console.Utils;

namespace TenistasSync.Console.Services;

/// <summary>
/// Servicio para operaciones síncronas sobre tenistas
/// Demuestra operaciones comunes en programación síncrona
/// similar a servicios en Java/Spring
/// 
/// En Java sería:
/// @Service
/// public class TenistaService {
///     public List<Tenista> getAllTenistas() { ... }
/// }
/// </summary>
public class TenistaService
{
    private readonly List<Tenista> _tenistas;

    public TenistaService()
    {
        _tenistas = InicializarDatos();
    }

    /// <summary>
    /// Inicializa datos de ejemplo
    /// En Java: private List<Tenista> inicializarDatos() { ... }
    /// </summary>
    private List<Tenista> InicializarDatos()
    {
        return new List<Tenista>
        {
            new() { Id = 1, Nombre = "Rafael Nadal", Ranking = 1, Pais = "España", Altura = 185, Peso = 85, Titulos = 22, FechaNacimiento = new DateTime(1986, 6, 3) },
            new() { Id = 2, Nombre = "Novak Djokovic", Ranking = 2, Pais = "Serbia", Altura = 188, Peso = 77, Titulos = 24, FechaNacimiento = new DateTime(1987, 5, 22) },
            new() { Id = 3, Nombre = "Carlos Alcaraz", Ranking = 3, Pais = "España", Altura = 183, Peso = 74, Titulos = 5, FechaNacimiento = new DateTime(2003, 5, 5) },
            new() { Id = 4, Nombre = "Roger Federer", Ranking = 4, Pais = "Suiza", Altura = 185, Peso = 85, Titulos = 20, FechaNacimiento = new DateTime(1981, 8, 8) },
            new() { Id = 5, Nombre = "Andy Murray", Ranking = 5, Pais = "Reino Unido", Altura = 190, Peso = 84, Titulos = 3, FechaNacimiento = new DateTime(1987, 5, 15) },
            new() { Id = 6, Nombre = "Juan Martín del Potro", Ranking = 6, Pais = "Argentina", Altura = 198, Peso = 97, Titulos = 1, FechaNacimiento = new DateTime(1988, 9, 23) },
            new() { Id = 7, Nombre = "Dominic Thiem", Ranking = 7, Pais = "Austria", Altura = 185, Peso = 79, Titulos = 1, FechaNacimiento = new DateTime(1993, 9, 3) },
            new() { Id = 8, Nombre = "Stefanos Tsitsipas", Ranking = 8, Pais = "Grecia", Altura = 193, Peso = 90, Titulos = 2, FechaNacimiento = new DateTime(1998, 8, 12) }
        };
    }

    /// <summary>
    /// Obtiene todos los tenistas
    /// En Java: public List<Tenista> findAll() { return new ArrayList<>(tenistas); }
    /// </summary>
    public List<Tenista> ObtenerTodos()
    {
        return new List<Tenista>(_tenistas);
    }

    /// <summary>
    /// Busca un tenista por ID
    /// En Java: public Optional<Tenista> findById(Long id)
    /// En C#: Retorna null si no encuentra (o usar Nullable<Tenista>)
    /// </summary>
    public Tenista? BuscarPorId(long id)
    {
        return _tenistas.FirstOrDefault(t => t.Id == id);
    }

    /// <summary>
    /// Obtiene tenistas por país
    /// En Java: public List<Tenista> findByPais(String pais)
    /// </summary>
    public List<Tenista> ObtenerPorPais(string pais)
    {
        return CollectionUtils.FiltrarPorPais(_tenistas, pais);
    }

    /// <summary>
    /// Obtiene el top N de tenistas por ranking
    /// En Java: public List<Tenista> getTopN(int n)
    /// </summary>
    public List<Tenista> ObtenerTopN(int n)
    {
        return CollectionUtils.TopN(_tenistas, n);
    }

    /// <summary>
    /// Obtiene tenistas con más de X títulos
    /// En Java: public List<Tenista> findByTitulosGreaterThan(int titulos)
    /// </summary>
    public List<Tenista> ObtenerConMasDeTitulos(int titulos)
    {
        return _tenistas.Where(t => t.Titulos > titulos).ToList();
    }

    /// <summary>
    /// Agrupa tenistas por país
    /// En Java: public Map<String, List<Tenista>> groupByPais()
    /// </summary>
    public Dictionary<string, List<Tenista>> AgruparPorPais()
    {
        return CollectionUtils.AgruparPorPais(_tenistas);
    }

    /// <summary>
    /// Calcula estadísticas de títulos
    /// En Java: IntSummaryStatistics getEstadisticasTitulos()
    /// </summary>
    public (int Total, double Promedio, int Minimo, int Maximo) ObtenerEstadisticasTitulos()
    {
        return CollectionUtils.EstadisticasTitulos(_tenistas);
    }

    /// <summary>
    /// Obtiene la edad promedio de los tenistas
    /// En Java: public double getEdadPromedio()
    /// </summary>
    public double ObtenerEdadPromedio()
    {
        return _tenistas.Average(t => t.Edad);
    }

    /// <summary>
    /// Busca tenistas jóvenes (menores de 25 años)
    /// En Java: public List<Tenista> findJovenes()
    /// </summary>
    public List<Tenista> ObtenerJovenes()
    {
        return _tenistas.Where(t => t.Edad < 25).ToList();
    }

    /// <summary>
    /// Obtiene el tenista con más títulos
    /// En Java: public Optional<Tenista> getTenistaConMasTitulos()
    /// </summary>
    public Tenista? ObtenerTenistaConMasTitulos()
    {
        return _tenistas.OrderByDescending(t => t.Titulos).FirstOrDefault();
    }

    /// <summary>
    /// Cuenta tenistas por condición
    /// En Java: public long countByCondition(Predicate<Tenista> predicate)
    /// </summary>
    public int ContarPorCondicion(Func<Tenista, bool> predicado)
    {
        return _tenistas.Count(predicado);
    }

    /// <summary>
    /// Verifica si existe algún tenista que cumple una condición
    /// En Java: public boolean exists(Predicate<Tenista> predicate)
    /// </summary>
    public bool ExisteConCondicion(Func<Tenista, bool> predicado)
    {
        return _tenistas.Any(predicado);
    }

    /// <summary>
    /// Obtiene nombres de todos los tenistas
    /// En Java: public List<String> getAllNombres()
    /// </summary>
    public List<string> ObtenerNombres()
    {
        return CollectionUtils.MapearANombres(_tenistas);
    }

    /// <summary>
    /// Agrupa tenistas por rango de edad
    /// </summary>
    public Dictionary<string, List<Tenista>> AgruparPorRangoEdad()
    {
        return _tenistas
            .GroupBy(t => t.Edad switch
            {
                < 25 => "Jóvenes (< 25)",
                >= 25 and < 35 => "Adultos (25-35)",
                _ => "Veteranos (35+)"
            })
            .ToDictionary(g => g.Key, g => g.ToList());
    }
}
