using CSharpFunctionalExtensions;
using TenistasResult.Console.Models;

namespace TenistasResult.Console.Services;

/// <summary>
/// Servicio que usa Result Pattern para manejo de errores
/// 
/// En Java sería:
/// - Optional<T> para valores que pueden no existir
/// - Either<L, R> con Vavr o Arrow
/// - Result types con bibliotecas funcionales
/// 
/// En C#:
/// - Result<T> de CSharpFunctionalExtensions
/// - Railway Oriented Programming
/// </summary>
public class TenistaService
{
    private readonly List<Tenista> _tenistas;

    public TenistaService()
    {
        _tenistas = new List<Tenista>
        {
            new() { Id = 1, Nombre = "Rafael Nadal", Ranking = 1, Pais = "España", Titulos = 22 },
            new() { Id = 2, Nombre = "Novak Djokovic", Ranking = 2, Pais = "Serbia", Titulos = 24 },
            new() { Id = 3, Nombre = "Carlos Alcaraz", Ranking = 3, Pais = "España", Titulos = 5 }
        };
    }

    /// <summary>
    /// Busca un tenista por ID retornando Result
    /// En Java: Optional<Tenista> findById(Long id)
    /// En C#: Result<Tenista> FindById(long id)
    /// </summary>
    public Result<Tenista> FindById(long id)
    {
        var tenista = _tenistas.FirstOrDefault(t => t.Id == id);
        
        return tenista != null
            ? Result.Success(tenista)
            : Result.Failure<Tenista>($"Tenista con ID {id} no encontrado");
    }

    /// <summary>
    /// Valida y crea un tenista
    /// Demuestra Railway Oriented Programming
    /// </summary>
    public Result<Tenista> CreateTenista(string nombre, int ranking, string pais, int titulos)
    {
        return ValidarNombre(nombre)
            .Bind(() => ValidarRanking(ranking))
            .Bind(() => ValidarPais(pais))
            .Bind(() => ValidarTitulos(titulos))
            .Map(() => new Tenista
            {
                Id = _tenistas.Max(t => t.Id) + 1,
                Nombre = nombre,
                Ranking = ranking,
                Pais = pais,
                Titulos = titulos
            })
            .Tap(tenista => _tenistas.Add(tenista));
    }

    /// <summary>
    /// Actualiza ranking con validación
    /// </summary>
    public Result<Tenista> UpdateRanking(long id, int nuevoRanking)
    {
        return FindById(id)
            .Ensure(t => nuevoRanking > 0, "El ranking debe ser mayor que 0")
            .Tap(t => t.Ranking = nuevoRanking);
    }

    /// <summary>
    /// Obtiene el top N con validación
    /// </summary>
    public Result<List<Tenista>> GetTopN(int n)
    {
        if (n <= 0)
            return Result.Failure<List<Tenista>>("N debe ser mayor que 0");
        
        if (n > _tenistas.Count)
            return Result.Failure<List<Tenista>>($"Solo hay {_tenistas.Count} tenistas");

        var top = _tenistas.OrderBy(t => t.Ranking).Take(n).ToList();
        return Result.Success(top);
    }

    // Métodos de validación
    private Result ValidarNombre(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            return Result.Failure("El nombre no puede estar vacío");
        
        if (nombre.Length < 3)
            return Result.Failure("El nombre debe tener al menos 3 caracteres");

        return Result.Success();
    }

    private Result ValidarRanking(int ranking)
    {
        if (ranking <= 0)
            return Result.Failure("El ranking debe ser mayor que 0");
        
        if (_tenistas.Any(t => t.Ranking == ranking))
            return Result.Failure($"Ya existe un tenista con ranking {ranking}");

        return Result.Success();
    }

    private Result ValidarPais(string pais)
    {
        if (string.IsNullOrWhiteSpace(pais))
            return Result.Failure("El país no puede estar vacío");

        return Result.Success();
    }

    private Result ValidarTitulos(int titulos)
    {
        if (titulos < 0)
            return Result.Failure("Los títulos no pueden ser negativos");

        return Result.Success();
    }

    public Result<List<Tenista>> GetAll()
    {
        return Result.Success(new List<Tenista>(_tenistas));
    }
}
