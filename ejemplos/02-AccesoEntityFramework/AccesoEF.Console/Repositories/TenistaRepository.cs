using AccesoEF.Console.Data;
using AccesoEF.Console.Models;
using Microsoft.EntityFrameworkCore;

namespace AccesoEF.Console.Repositories;

/// <summary>
/// Implementación del repositorio con Entity Framework Core
/// Equivalente a un @Repository en Spring Data JPA
/// 
/// En Java/Spring Data JPA:
/// @Repository
/// public interface TenistaRepository extends JpaRepository<Tenista, Long> {
///     // Spring Data genera automáticamente la implementación
///     List<Tenista> findByPais(String pais);
/// }
/// 
/// En C#/EF Core:
/// - DbContext.Set<T>() proporciona operaciones CRUD básicas
/// - LINQ reemplaza JPQL/HQL (lenguajes de consulta de JPA/Hibernate)
/// - Métodos async/await para todas las operaciones de BD
/// 
/// COMPARACIÓN LINQ vs JPQL:
/// 
/// JPQL (Java):
/// entityManager.createQuery("SELECT t FROM Tenista t WHERE t.pais = :pais")
///              .setParameter("pais", pais)
///              .getResultList();
/// 
/// LINQ (C#):
/// _context.Tenistas
///         .Where(t => t.Pais == pais)
///         .ToListAsync();
/// </summary>
public class TenistaRepository : ITenistaRepository
{
    private readonly TenistasDbContext _context;

    public TenistaRepository(TenistasDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Crear nuevo tenista
    /// En Java/Spring Data: tenistaRepository.save(tenista);
    /// En C#/EF Core: context.Add(tenista); context.SaveChanges();
    /// </summary>
    public async Task<Tenista> CreateAsync(Tenista tenista)
    {
        // En Java/JPA: entityManager.persist(tenista);
        // En C#/EF Core: context.Add marca la entidad para inserción
        _context.Tenistas.Add(tenista);
        
        // En Java/JPA: entityManager.flush(); o @Transactional
        // En C#/EF Core: SaveChangesAsync ejecuta las operaciones pendientes
        await _context.SaveChangesAsync();
        
        System.Console.WriteLine($"✅ Tenista creado: {tenista.Nombre} con ID {tenista.Id}");
        return tenista;
    }

    /// <summary>
    /// Buscar por ID
    /// En Java/Spring Data: Optional<Tenista> tenista = tenistaRepository.findById(id);
    /// En C#/EF Core: var tenista = await context.Tenistas.FindAsync(id);
    /// </summary>
    public async Task<Tenista?> FindByIdAsync(long id)
    {
        // FindAsync es el método más eficiente para buscar por clave primaria
        // En Java/JPA: entityManager.find(Tenista.class, id);
        return await _context.Tenistas.FindAsync(id);
    }

    /// <summary>
    /// Obtener todos los tenistas
    /// En Java/Spring Data: List<Tenista> tenistas = tenistaRepository.findAll();
    /// En C#/EF Core: var tenistas = await context.Tenistas.ToListAsync();
    /// </summary>
    public async Task<List<Tenista>> FindAllAsync()
    {
        // ToListAsync ejecuta la consulta y retorna una lista
        // En Java/JPQL: entityManager.createQuery("FROM Tenista").getResultList();
        // En C#/LINQ: Similar a Stream API pero más integrado con el ORM
        return await _context.Tenistas
            .OrderBy(t => t.Ranking)
            .ToListAsync();
    }

    /// <summary>
    /// Actualizar tenista existente
    /// En Java/Spring Data: tenistaRepository.save(tenista); // save actualiza si existe ID
    /// En C#/EF Core: context.Update(tenista); context.SaveChanges();
    /// </summary>
    public async Task<Tenista> UpdateAsync(Tenista tenista)
    {
        // En Java/JPA: entityManager.merge(tenista);
        // En C#/EF Core: Update marca la entidad como modificada
        _context.Tenistas.Update(tenista);
        
        await _context.SaveChangesAsync();
        
        System.Console.WriteLine($"✅ Tenista actualizado: {tenista.Nombre}");
        return tenista;
    }

    /// <summary>
    /// Eliminar tenista por ID
    /// En Java/Spring Data: tenistaRepository.deleteById(id);
    /// En C#/EF Core: context.Remove(tenista); context.SaveChanges();
    /// </summary>
    public async Task<bool> DeleteAsync(long id)
    {
        var tenista = await FindByIdAsync(id);
        if (tenista == null)
        {
            return false;
        }

        // En Java/JPA: entityManager.remove(tenista);
        // En C#/EF Core: Remove marca la entidad para eliminación
        _context.Tenistas.Remove(tenista);
        await _context.SaveChangesAsync();
        
        System.Console.WriteLine($"✅ Tenista con ID {id} eliminado");
        return true;
    }

    /// <summary>
    /// Buscar por país usando LINQ
    /// En Java/Spring Data: List<Tenista> tenistas = tenistaRepository.findByPais(pais);
    /// En C#/EF Core: LINQ proporciona consultas type-safe
    /// 
    /// COMPARACIÓN:
    /// 
    /// Java/JPQL:
    /// @Query("SELECT t FROM Tenista t WHERE t.pais = :pais ORDER BY t.ranking")
    /// List<Tenista> findByPaisOrderByRanking(@Param("pais") String pais);
    /// 
    /// C#/LINQ:
    /// context.Tenistas
    ///        .Where(t => t.Pais == pais)
    ///        .OrderBy(t => t.Ranking)
    ///        .ToListAsync();
    /// </summary>
    public async Task<List<Tenista>> FindByPaisAsync(string pais)
    {
        // LINQ (Language Integrated Query) - similar a Stream API pero más potente
        // Where = filter en Java Streams
        // OrderBy = sorted en Java Streams
        // ToListAsync = collect(Collectors.toList()) en Java Streams
        return await _context.Tenistas
            .Where(t => t.Pais == pais)
            .OrderBy(t => t.Ranking)
            .ToListAsync();
    }

    /// <summary>
    /// Contar total de registros
    /// En Java/Spring Data: long count = tenistaRepository.count();
    /// En C#/EF Core: int count = await context.Tenistas.CountAsync();
    /// </summary>
    public async Task<int> CountAsync()
    {
        // En Java/JPQL: (Long) entityManager.createQuery("SELECT COUNT(t) FROM Tenista t").getSingleResult();
        // En C#/LINQ: Aggregate functions son métodos simples
        return await _context.Tenistas.CountAsync();
    }

    /// <summary>
    /// Ejemplo de consulta con filtro numérico
    /// Demuestra LINQ con expresiones lambda
    /// 
    /// En Java/Spring Data:
    /// @Query("SELECT t FROM Tenista t WHERE t.ranking < :ranking")
    /// List<Tenista> findByRankingLessThan(@Param("ranking") int ranking);
    /// 
    /// En C#/EF Core con LINQ:
    /// Más natural y type-safe que JPQL
    /// </summary>
    public async Task<List<Tenista>> FindByRankingLessThanAsync(int ranking)
    {
        // LINQ con expresiones lambda (más expresivo que JPQL)
        // En Java/Stream: tenistas.stream().filter(t -> t.getRanking() < ranking).collect(...)
        // En C#/LINQ: Se traduce directamente a SQL eficiente
        return await _context.Tenistas
            .Where(t => t.Ranking < ranking)
            .OrderBy(t => t.Ranking)
            .ToListAsync();
    }
}
