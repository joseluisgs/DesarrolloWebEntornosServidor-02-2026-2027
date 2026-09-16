using AccesoEF.Console.Data;
using AccesoEF.Console.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace AccesoEF.Tests;

/// <summary>
/// Tests para TenistasDbContext
/// 
/// En Java/JPA sería similar a tests de EntityManager o JpaRepository
/// @DataJpaTest en Spring Boot
/// 
/// Estos tests verifican la configuración del DbContext y operaciones básicas
/// </summary>
[TestFixture]
public class TenistasDbContextTests
{
    private TenistasDbContext? _context;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<TenistasDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TenistasDbContext(options);
    }

    [TearDown]
    public void TearDown()
    {
        _context?.Database.EnsureDeleted();
        _context?.Dispose();
    }

    [Test]
    public void DbContext_ShouldHaveTenistasDbSet()
    {
        // Assert
        _context!.Tenistas.Should().NotBeNull();
    }

    [Test]
    public async Task DbContext_Add_ShouldAddEntityToContext()
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
        _context!.Tenistas.Add(tenista);
        await _context.SaveChangesAsync();

        // Assert
        var saved = await _context.Tenistas.FirstOrDefaultAsync();
        saved.Should().NotBeNull();
        saved!.Nombre.Should().Be("Rafael Nadal");
    }

    [Test]
    public async Task DbContext_ChangeTracking_ShouldDetectModifications()
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

        _context!.Tenistas.Add(tenista);
        await _context.SaveChangesAsync();

        // Act - Modificar entidad rastreada
        tenista.Titulos = 23;
        await _context.SaveChangesAsync();

        // Assert
        var updated = await _context.Tenistas.FindAsync(tenista.Id);
        updated!.Titulos.Should().Be(23);
    }

    [Test]
    public async Task DbContext_Remove_ShouldDeleteEntity()
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

        _context!.Tenistas.Add(tenista);
        await _context.SaveChangesAsync();
        var id = tenista.Id;

        // Act
        _context.Tenistas.Remove(tenista);
        await _context.SaveChangesAsync();

        // Assert
        var deleted = await _context.Tenistas.FindAsync(id);
        deleted.Should().BeNull();
    }

    [Test]
    public async Task DbContext_AsNoTracking_ShouldNotTrackEntities()
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

        _context!.Tenistas.Add(tenista);
        await _context.SaveChangesAsync();

        // Act - Query con AsNoTracking (similar a detached en JPA)
        var queriedTenista = await _context.Tenistas
            .AsNoTracking()
            .FirstAsync();

        queriedTenista.Titulos = 99;
        await _context.SaveChangesAsync();

        // Assert - El cambio NO debería guardarse
        var fromDb = await _context.Tenistas.FindAsync(tenista.Id);
        fromDb!.Titulos.Should().Be(22); // Valor original
    }

    [Test]
    public async Task DbContext_Transaction_ShouldRollbackOnError()
    {
        // Arrange & Act
        await using var transaction = await _context!.Database.BeginTransactionAsync();

        var tenista = new Tenista
        {
            Nombre = "Test Rollback",
            Ranking = 999,
            Pais = "Test",
            Altura = 185,
            Peso = 80,
            Titulos = 0,
            FechaNacimiento = DateTime.Now
        };

        _context.Tenistas.Add(tenista);
        await _context.SaveChangesAsync();

        // Rollback
        await transaction.RollbackAsync();

        // Assert
        var count = await _context.Tenistas.CountAsync();
        count.Should().Be(0);
    }

    [Test]
    public async Task DbContext_BulkInsert_ShouldAddMultipleEntities()
    {
        // Arrange
        var tenistas = new List<Tenista>
        {
            new() { Nombre = "Rafael Nadal", Ranking = 1, Pais = "España", Altura = 185, Peso = 85, Titulos = 22, FechaNacimiento = new DateTime(1986, 6, 3) },
            new() { Nombre = "Novak Djokovic", Ranking = 2, Pais = "Serbia", Altura = 188, Peso = 77, Titulos = 24, FechaNacimiento = new DateTime(1987, 5, 22) },
            new() { Nombre = "Carlos Alcaraz", Ranking = 3, Pais = "España", Altura = 183, Peso = 74, Titulos = 5, FechaNacimiento = new DateTime(2003, 5, 5) }
        };

        // Act
        _context!.Tenistas.AddRange(tenistas);
        await _context.SaveChangesAsync();

        // Assert
        var count = await _context.Tenistas.CountAsync();
        count.Should().Be(3);
    }

    [Test]
    public async Task DbContext_Find_ShouldUseCachedEntity()
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

        _context!.Tenistas.Add(tenista);
        await _context.SaveChangesAsync();

        // Act - Find busca primero en el caché
        var found1 = await _context.Tenistas.FindAsync(tenista.Id);
        var found2 = await _context.Tenistas.FindAsync(tenista.Id);

        // Assert - Deberían ser la misma instancia (del caché)
        found1.Should().BeSameAs(found2);
    }
}
