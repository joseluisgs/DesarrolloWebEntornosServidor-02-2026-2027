using AccesoAdoNet.Console.Database;
using AccesoAdoNet.Console.Models;
using AccesoAdoNet.Console.Repositories;
using FluentAssertions;
using NUnit.Framework;
using Testcontainers.PostgreSql;

namespace AccesoAdoNet.Tests;

/// <summary>
/// Tests de integración para TenistaRepository
/// Usa TestContainers para PostgreSQL real
/// 
/// En Java/Spring Boot sería:
/// @SpringBootTest
/// @Testcontainers
/// public class TenistaRepositoryTest { ... }
/// 
/// En C#/NUnit:
/// [TestFixture]
/// public class TenistaRepositoryTests { ... }
/// </summary>
[TestFixture]
public class TenistaRepositoryTests
{
    private PostgreSqlContainer? _postgresContainer;
    private DatabaseManager? _dbManager;
    private TenistaRepository? _repository;

    [SetUp]
    public async Task SetUp()
    {
        // Iniciar PostgreSQL con TestContainers
        _postgresContainer = new PostgreSqlBuilder()
            .WithImage("postgres:15")
            .WithDatabase("tenistas_db")
            .WithUsername("admin")
            .WithPassword("admin123")
            .Build();

        await _postgresContainer.StartAsync();
        
        var connectionString = _postgresContainer.GetConnectionString();
        _dbManager = new DatabaseManager(connectionString);
        
        // Inicializar base de datos
        await _dbManager.InitializeDatabaseAsync();
        await _dbManager.ClearDatabaseAsync();
        
        _repository = new TenistaRepository(_dbManager);
    }

    [TearDown]
    public async Task TearDown()
    {
        _dbManager?.Dispose();
        
        if (_postgresContainer != null)
        {
            await _postgresContainer.DisposeAsync();
        }
    }

    #region CREATE Tests

    [Test]
    public async Task CreateAsync_ShouldInsertTenistaAndReturnWithId()
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
        result.Ranking.Should().Be(1);
    }

    [Test]
    public async Task CreateAsync_MultipleInserts_ShouldGenerateDifferentIds()
    {
        // Arrange
        var tenista1 = new Tenista
        {
            Nombre = "Rafael Nadal",
            Ranking = 1,
            Pais = "España",
            Altura = 185,
            Peso = 85,
            Titulos = 22,
            FechaNacimiento = new DateTime(1986, 6, 3)
        };

        var tenista2 = new Tenista
        {
            Nombre = "Novak Djokovic",
            Ranking = 2,
            Pais = "Serbia",
            Altura = 188,
            Peso = 77,
            Titulos = 24,
            FechaNacimiento = new DateTime(1987, 5, 22)
        };

        // Act
        var result1 = await _repository!.CreateAsync(tenista1);
        var result2 = await _repository!.CreateAsync(tenista2);

        // Assert
        result1.Id.Should().NotBe(result2.Id);
        result2.Id.Should().BeGreaterThan(result1.Id);
    }

    #endregion

    #region READ Tests

    [Test]
    public async Task FindByIdAsync_ExistingId_ShouldReturnTenista()
    {
        // Arrange
        var tenista = await CreateTestTenista("Carlos Alcaraz", 3);

        // Act
        var result = await _repository!.FindByIdAsync(tenista.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(tenista.Id);
        result.Nombre.Should().Be("Carlos Alcaraz");
        result.Ranking.Should().Be(3);
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
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Test]
    public async Task FindAllAsync_WithData_ShouldReturnAllTenistas()
    {
        // Arrange
        await CreateTestTenista("Rafael Nadal", 1);
        await CreateTestTenista("Novak Djokovic", 2);
        await CreateTestTenista("Carlos Alcaraz", 3);

        // Act
        var result = await _repository!.FindAllAsync();

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(t => t.Nombre == "Rafael Nadal");
        result.Should().Contain(t => t.Nombre == "Novak Djokovic");
        result.Should().Contain(t => t.Nombre == "Carlos Alcaraz");
    }

    [Test]
    public async Task FindAllAsync_ShouldReturnOrderedByRanking()
    {
        // Arrange - Insertar en orden aleatorio
        await CreateTestTenista("Carlos Alcaraz", 3);
        await CreateTestTenista("Rafael Nadal", 1);
        await CreateTestTenista("Novak Djokovic", 2);

        // Act
        var result = await _repository!.FindAllAsync();

        // Assert
        result.Should().HaveCount(3);
        result[0].Ranking.Should().Be(1);
        result[1].Ranking.Should().Be(2);
        result[2].Ranking.Should().Be(3);
    }

    [Test]
    public async Task FindByPaisAsync_ExistingCountry_ShouldReturnMatchingTenistas()
    {
        // Arrange
        await CreateTestTenista("Rafael Nadal", 1, "España");
        await CreateTestTenista("Novak Djokovic", 2, "Serbia");
        await CreateTestTenista("Carlos Alcaraz", 3, "España");

        // Act
        var result = await _repository!.FindByPaisAsync("España");

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(t => t.Pais.Should().Be("España"));
    }

    [Test]
    public async Task FindByPaisAsync_NonExistingCountry_ShouldReturnEmptyList()
    {
        // Arrange
        await CreateTestTenista("Rafael Nadal", 1, "España");

        // Act
        var result = await _repository!.FindByPaisAsync("Francia");

        // Assert
        result.Should().BeEmpty();
    }

    #endregion

    #region UPDATE Tests

    [Test]
    public async Task UpdateAsync_ExistingTenista_ShouldModifyData()
    {
        // Arrange
        var tenista = await CreateTestTenista("Rafael Nadal", 1);
        tenista.Titulos = 23;
        tenista.Ranking = 2;

        // Act
        var result = await _repository!.UpdateAsync(tenista);

        // Assert
        result.Titulos.Should().Be(23);
        result.Ranking.Should().Be(2);

        // Verificar en BD
        var fromDb = await _repository!.FindByIdAsync(tenista.Id);
        fromDb!.Titulos.Should().Be(23);
        fromDb.Ranking.Should().Be(2);
    }

    [Test]
    public void UpdateAsync_NonExistingTenista_ShouldThrowException()
    {
        // Arrange
        var tenista = new Tenista
        {
            Id = 999,
            Nombre = "Non Existing",
            Ranking = 1,
            Pais = "Test",
            Altura = 180,
            Peso = 75,
            Titulos = 0,
            FechaNacimiento = DateTime.Now
        };

        // Act & Assert
        var act = async () => await _repository!.UpdateAsync(tenista);
        act.Should().ThrowAsync<InvalidOperationException>();
    }

    #endregion

    #region DELETE Tests

    [Test]
    public async Task DeleteAsync_ExistingId_ShouldReturnTrueAndRemoveTenista()
    {
        // Arrange
        var tenista = await CreateTestTenista("Roger Federer", 4);

        // Act
        var result = await _repository!.DeleteAsync(tenista.Id);

        // Assert
        result.Should().BeTrue();

        // Verificar que ya no existe
        var fromDb = await _repository!.FindByIdAsync(tenista.Id);
        fromDb.Should().BeNull();
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
        await CreateTestTenista("Rafael Nadal", 1);
        await CreateTestTenista("Novak Djokovic", 2);
        await CreateTestTenista("Carlos Alcaraz", 3);

        // Act
        var result = await _repository!.CountAsync();

        // Assert
        result.Should().Be(3);
    }

    #endregion

    #region Helper Methods

    private async Task<Tenista> CreateTestTenista(string nombre, int ranking, string pais = "España")
    {
        var tenista = new Tenista
        {
            Nombre = nombre,
            Ranking = ranking,
            Pais = pais,
            Altura = 185,
            Peso = 80,
            Titulos = 10,
            FechaNacimiento = new DateTime(1990, 1, 1)
        };

        return await _repository!.CreateAsync(tenista);
    }

    #endregion
}
