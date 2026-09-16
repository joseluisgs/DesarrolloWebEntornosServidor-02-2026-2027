using AccesoEF.Console.Data;
using AccesoEF.Console.Models;
using AccesoEF.Console.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace AccesoEF.Tests;

/// <summary>
/// Tests para TenistaRepository usando InMemory database
/// 
/// En Java/Spring Boot sería:
/// @SpringBootTest
/// @DataJpaTest
/// public class TenistaRepositoryTest { ... }
/// 
/// En C#/EF Core con InMemory:
/// [TestFixture]
/// public class TenistaRepositoryTests { ... }
/// 
/// InMemory database es similar a H2 in-memory en Java
/// </summary>
[TestFixture]
public class TenistaRepositoryTests
{
    private TenistasDbContext? _context;
    private TenistaRepository? _repository;

    /// <summary>
    /// Setup antes de cada test
    /// Crea un nuevo DbContext con InMemory database
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        // Crear DbContext con InMemory database
        // En Java/Spring Boot: @DataJpaTest automáticamente usa H2
        var options = new DbContextOptionsBuilder<TenistasDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TenistasDbContext(options);
        _repository = new TenistaRepository(_context);
    }

    /// <summary>
    /// Cleanup después de cada test
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        _context?.Database.EnsureDeleted();
        _context?.Dispose();
    }

    #region CREATE Tests

    [Test]
    public async Task CreateAsync_ShouldAddTenistaAndGenerateId()
    {
        // Arrange
        var tenista = new Tenista
        {
            Nombre = "Rafael Nadal",
            Ranking = 1,
            Pais = "España",
            Altura = 185,
            Peso = 85,
            Titulos = 22,
            FechaNacimiento = new DateTime(1986, 6, 3)
        };

        // Act
        var result = await _repository!.CreateAsync(tenista);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Nombre.Should().Be("Rafael Nadal");
    }

    [Test]
    public async Task CreateAsync_MultipleInserts_ShouldTrackAllEntities()
    {
        // Arrange
        var tenista1 = CreateTestTenista("Rafael Nadal", 1);
        var tenista2 = CreateTestTenista("Novak Djokovic", 2);

        // Act
        await _repository!.CreateAsync(tenista1);
        await _repository!.CreateAsync(tenista2);

        // Assert
        var all = await _repository!.FindAllAsync();
        all.Should().HaveCount(2);
    }

    #endregion

    #region READ Tests

    [Test]
    public async Task FindByIdAsync_ExistingId_ShouldReturnTenista()
    {
        // Arrange
        var tenista = await _repository!.CreateAsync(CreateTestTenista("Carlos Alcaraz", 3));

        // Act
        var result = await _repository!.FindByIdAsync(tenista.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Nombre.Should().Be("Carlos Alcaraz");
    }

    [Test]
    public async Task FindByIdAsync_NonExistingId_ShouldReturnNull()
    {
        // Act
        var result = await _repository!.FindByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task FindAllAsync_EmptyDatabase_ShouldReturnEmptyList()
    {
        // Act
        var result = await _repository!.FindAllAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Test]
    public async Task FindAllAsync_WithData_ShouldReturnAllTenistas()
    {
        // Arrange
        await _repository!.CreateAsync(CreateTestTenista("Rafael Nadal", 1));
        await _repository!.CreateAsync(CreateTestTenista("Novak Djokovic", 2));
        await _repository!.CreateAsync(CreateTestTenista("Carlos Alcaraz", 3));

        // Act
        var result = await _repository!.FindAllAsync();

        // Assert
        result.Should().HaveCount(3);
    }

    [Test]
    public async Task FindByPaisAsync_ExistingCountry_ShouldReturnMatchingTenistas()
    {
        // Arrange
        await _repository!.CreateAsync(CreateTestTenista("Rafael Nadal", 1, "España"));
        await _repository!.CreateAsync(CreateTestTenista("Novak Djokovic", 2, "Serbia"));
        await _repository!.CreateAsync(CreateTestTenista("Carlos Alcaraz", 3, "España"));

        // Act
        var result = await _repository!.FindByPaisAsync("España");

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(t => t.Pais.Should().Be("España"));
    }

    [Test]
    public async Task FindByRankingLessThanAsync_ShouldReturnMatchingTenistas()
    {
        // Arrange
        await _repository!.CreateAsync(CreateTestTenista("Rafael Nadal", 1));
        await _repository!.CreateAsync(CreateTestTenista("Novak Djokovic", 2));
        await _repository!.CreateAsync(CreateTestTenista("Carlos Alcaraz", 3));
        await _repository!.CreateAsync(CreateTestTenista("Roger Federer", 5));

        // Act
        var result = await _repository!.FindByRankingLessThanAsync(4);

        // Assert
        result.Should().HaveCount(3);
        result.Should().AllSatisfy(t => t.Ranking.Should().BeLessThan(4));
    }

    #endregion

    #region UPDATE Tests

    [Test]
    public async Task UpdateAsync_ExistingTenista_ShouldModifyData()
    {
        // Arrange
        var tenista = await _repository!.CreateAsync(CreateTestTenista("Rafael Nadal", 1));
        
        // Detach para simular cambio en nueva sesión
        _context!.Entry(tenista).State = EntityState.Detached;
        
        tenista.Titulos = 23;
        tenista.Ranking = 2;

        // Act
        var result = await _repository!.UpdateAsync(tenista);

        // Assert
        result.Titulos.Should().Be(23);
        result.Ranking.Should().Be(2);
    }

    [Test]
    public async Task UpdateAsync_ChangeTracking_ShouldDetectModifications()
    {
        // Arrange
        var tenista = await _repository!.CreateAsync(CreateTestTenista("Rafael Nadal", 1));
        
        // No detachamos - EF Core detectará cambios automáticamente
        tenista.Titulos = 23;

        // Act
        await _context!.SaveChangesAsync();

        // Assert
        var updated = await _repository!.FindByIdAsync(tenista.Id);
        updated!.Titulos.Should().Be(23);
    }

    #endregion

    #region DELETE Tests

    [Test]
    public async Task DeleteAsync_ExistingId_ShouldReturnTrueAndRemove()
    {
        // Arrange
        var tenista = await _repository!.CreateAsync(CreateTestTenista("Roger Federer", 4));

        // Act
        var result = await _repository!.DeleteAsync(tenista.Id);

        // Assert
        result.Should().BeTrue();
        var deleted = await _repository!.FindByIdAsync(tenista.Id);
        deleted.Should().BeNull();
    }

    [Test]
    public async Task DeleteAsync_NonExistingId_ShouldReturnFalse()
    {
        // Act
        var result = await _repository!.DeleteAsync(999);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region COUNT Tests

    [Test]
    public async Task CountAsync_EmptyDatabase_ShouldReturnZero()
    {
        // Act
        var result = await _repository!.CountAsync();

        // Assert
        result.Should().Be(0);
    }

    [Test]
    public async Task CountAsync_WithData_ShouldReturnCorrectCount()
    {
        // Arrange
        await _repository!.CreateAsync(CreateTestTenista("Rafael Nadal", 1));
        await _repository!.CreateAsync(CreateTestTenista("Novak Djokovic", 2));
        await _repository!.CreateAsync(CreateTestTenista("Carlos Alcaraz", 3));

        // Act
        var result = await _repository!.CountAsync();

        // Assert
        result.Should().Be(3);
    }

    #endregion

    #region LINQ Query Tests

    [Test]
    public async Task LinqQuery_ComplexFilter_ShouldWork()
    {
        // Arrange
        await _repository!.CreateAsync(CreateTestTenistaWithTitles("Rafael Nadal", 1, 22));
        await _repository!.CreateAsync(CreateTestTenistaWithTitles("Novak Djokovic", 2, 24));
        await _repository!.CreateAsync(CreateTestTenistaWithTitles("Carlos Alcaraz", 3, 2));

        // Act - LINQ query directo en el DbContext
        var topChampions = await _context!.Tenistas
            .Where(t => t.Titulos > 20)
            .OrderByDescending(t => t.Titulos)
            .ToListAsync();

        // Assert
        topChampions.Should().HaveCount(2);
        topChampions[0].Nombre.Should().Be("Novak Djokovic");
        topChampions[1].Nombre.Should().Be("Rafael Nadal");
    }

    [Test]
    public async Task LinqQuery_Projection_ShouldWork()
    {
        // Arrange
        await _repository!.CreateAsync(CreateTestTenista("Rafael Nadal", 1, "España"));
        await _repository!.CreateAsync(CreateTestTenista("Novak Djokovic", 2, "Serbia"));

        // Act - Proyección LINQ (similar a DTO en Java)
        var nombres = await _context!.Tenistas
            .Select(t => t.Nombre)
            .ToListAsync();

        // Assert
        nombres.Should().HaveCount(2);
        nombres.Should().Contain("Rafael Nadal");
        nombres.Should().Contain("Novak Djokovic");
    }

    [Test]
    public async Task LinqQuery_GroupBy_ShouldWork()
    {
        // Arrange
        await _repository!.CreateAsync(CreateTestTenista("Rafael Nadal", 1, "España"));
        await _repository!.CreateAsync(CreateTestTenista("Carlos Alcaraz", 3, "España"));
        await _repository!.CreateAsync(CreateTestTenista("Novak Djokovic", 2, "Serbia"));

        // Act - GroupBy en LINQ
        var porPais = await _context!.Tenistas
            .GroupBy(t => t.Pais)
            .Select(g => new { Pais = g.Key, Count = g.Count() })
            .ToListAsync();

        // Assert
        porPais.Should().HaveCount(2);
        porPais.First(p => p.Pais == "España").Count.Should().Be(2);
        porPais.First(p => p.Pais == "Serbia").Count.Should().Be(1);
    }

    #endregion

    #region Helper Methods

    private Tenista CreateTestTenista(string nombre, int ranking, string pais = "España")
    {
        return new Tenista
        {
            Nombre = nombre,
            Ranking = ranking,
            Pais = pais,
            Altura = 185,
            Peso = 80,
            Titulos = 10,
            FechaNacimiento = new DateTime(1990, 1, 1)
        };
    }

    private Tenista CreateTestTenistaWithTitles(string nombre, int ranking, int titulos)
    {
        return new Tenista
        {
            Nombre = nombre,
            Ranking = ranking,
            Pais = "España",
            Altura = 185,
            Peso = 80,
            Titulos = titulos,
            FechaNacimiento = new DateTime(1990, 1, 1)
        };
    }

    #endregion
}
