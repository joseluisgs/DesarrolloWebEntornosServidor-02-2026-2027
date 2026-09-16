using AccesoEF.Console.Models;

namespace AccesoEF.Console.Repositories;

/// <summary>
/// Interfaz de repositorio
/// En Spring Data JPA, esto sería:
/// public interface TenistaRepository extends JpaRepository<Tenista, Long> {
///     List<Tenista> findByPais(String pais);
/// }
/// 
/// En EF Core, creamos la interfaz manualmente pero la implementación
/// es muy similar gracias a DbContext y LINQ
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
    Task<List<Tenista>> FindByRankingLessThanAsync(int ranking);
}
