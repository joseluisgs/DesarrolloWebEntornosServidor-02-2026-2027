using AccesoAdoNet.Console.Models;

namespace AccesoAdoNet.Console.Repositories;

/// <summary>
/// Interfaz genérica de repositorio
/// En Java: public interface Repository<T, ID>
/// Patrón Repository para abstraer el acceso a datos
/// </summary>
public interface ITenistaRepository
{
    Task<Tenista> CreateAsync(Tenista tenista);
    Task<Tenista?> FindByIdAsync(long id);
    Task<List<Tenista>> FindAllAsync();
    Task<Tenista> UpdateAsync(Tenista tenista);
    Task<bool> DeleteAsync(long id);
    Task<List<Tenista>> FindByPaisAsync(string pais);
    Task<int> CountAsync();
}
