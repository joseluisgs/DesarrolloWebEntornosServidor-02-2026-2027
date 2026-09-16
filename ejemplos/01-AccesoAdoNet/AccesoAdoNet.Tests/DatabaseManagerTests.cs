using AccesoAdoNet.Console.Database;
using FluentAssertions;
using NUnit.Framework;
using Testcontainers.PostgreSql;

namespace AccesoAdoNet.Tests;

/// <summary>
/// Tests para DatabaseManager - Gestión de conexiones ADO.NET
/// 
/// En Java/JUnit sería:
/// @Test
/// public void testConnection() { ... }
/// 
/// En C#/NUnit:
/// [Test]
/// public async Task TestConnection() { ... }
/// 
/// Usa TestContainers para PostgreSQL real en tests de integración
/// </summary>
[TestFixture]
public class DatabaseManagerTests
{
    private PostgreSqlContainer? _postgresContainer;
    private string _connectionString = string.Empty;

    /// <summary>
    /// Setup ejecutado antes de cada test
    /// Equivalente a @BeforeEach en JUnit 5
    /// </summary>
    [SetUp]
    public async Task SetUp()
    {
        // Crear contenedor PostgreSQL con TestContainers
        // En Java: PostgreSQLContainer<?> postgres = new PostgreSQLContainer<>("postgres:15")
        _postgresContainer = new PostgreSqlBuilder()
            .WithImage("postgres:15")
            .WithDatabase("tenistas_db")
            .WithUsername("admin")
            .WithPassword("admin123")
            .Build();

        await _postgresContainer.StartAsync();
        _connectionString = _postgresContainer.GetConnectionString();
    }

    /// <summary>
    /// Cleanup ejecutado después de cada test
    /// Equivalente a @AfterEach en JUnit 5
    /// </summary>
    [TearDown]
    public async Task TearDown()
    {
        if (_postgresContainer != null)
        {
            await _postgresContainer.DisposeAsync();
        }
    }

    [Test]
    public async Task GetConnectionAsync_ShouldReturnOpenConnection()
    {
        // Arrange
        using var dbManager = new DatabaseManager(_connectionString);

        // Act
        var connection = await dbManager.GetConnectionAsync();

        // Assert
        connection.Should().NotBeNull();
        connection.State.Should().Be(System.Data.ConnectionState.Open);
    }

    [Test]
    public async Task GetConnectionAsync_ShouldReuseExistingConnection()
    {
        // Arrange
        using var dbManager = new DatabaseManager(_connectionString);

        // Act
        var connection1 = await dbManager.GetConnectionAsync();
        var connection2 = await dbManager.GetConnectionAsync();

        // Assert
        connection1.Should().BeSameAs(connection2);
    }

    [Test]
    public async Task InitializeDatabaseAsync_ShouldCreateTenistasTable()
    {
        // Arrange
        using var dbManager = new DatabaseManager(_connectionString);

        // Act
        await dbManager.InitializeDatabaseAsync();
        var connection = await dbManager.GetConnectionAsync();

        // Assert - Verificar que la tabla existe
        await using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT EXISTS (
                SELECT FROM information_schema.tables 
                WHERE table_schema = 'public' 
                AND table_name = 'tenistas'
            )";
        
        var exists = await command.ExecuteScalarAsync();
        exists.Should().Be(true);
    }

    [Test]
    public async Task ClearDatabaseAsync_ShouldTruncateTable()
    {
        // Arrange
        using var dbManager = new DatabaseManager(_connectionString);
        await dbManager.InitializeDatabaseAsync();
        
        // Insertar datos de prueba
        var connection = await dbManager.GetConnectionAsync();
        await using var insertCmd = connection.CreateCommand();
        insertCmd.CommandText = @"
            INSERT INTO tenistas (nombre, ranking, pais, altura, peso, titulos, fecha_nacimiento)
            VALUES ('Test Player', 1, 'Test', 180, 75, 10, '1990-01-01')";
        await insertCmd.ExecuteNonQueryAsync();

        // Act
        await dbManager.ClearDatabaseAsync();

        // Assert - Verificar que la tabla está vacía
        await using var countCmd = connection.CreateCommand();
        countCmd.CommandText = "SELECT COUNT(*) FROM tenistas";
        var count = await countCmd.ExecuteScalarAsync();
        Convert.ToInt32(count).Should().Be(0);
    }

    [Test]
    public async Task BeginTransactionAsync_ShouldReturnTransaction()
    {
        // Arrange
        using var dbManager = new DatabaseManager(_connectionString);
        await dbManager.InitializeDatabaseAsync();

        // Act
        var transaction = await dbManager.BeginTransactionAsync();

        // Assert
        transaction.Should().NotBeNull();
        await transaction.RollbackAsync(); // Cleanup
    }

    [Test]
    public async Task Transaction_ShouldRollbackOnError()
    {
        // Arrange
        using var dbManager = new DatabaseManager(_connectionString);
        await dbManager.InitializeDatabaseAsync();
        var connection = await dbManager.GetConnectionAsync();

        // Act & Assert
        await using var transaction = await dbManager.BeginTransactionAsync();
        
        // Insertar registro
        await using var insertCmd = connection.CreateCommand();
        insertCmd.Transaction = transaction;
        insertCmd.CommandText = @"
            INSERT INTO tenistas (nombre, ranking, pais, altura, peso, titulos, fecha_nacimiento)
            VALUES ('Rollback Test', 99, 'Test', 180, 75, 0, '1990-01-01')";
        await insertCmd.ExecuteNonQueryAsync();

        // Rollback
        await transaction.RollbackAsync();

        // Verificar que no se guardó
        await using var countCmd = connection.CreateCommand();
        countCmd.CommandText = "SELECT COUNT(*) FROM tenistas WHERE nombre = 'Rollback Test'";
        var count = await countCmd.ExecuteScalarAsync();
        Convert.ToInt32(count).Should().Be(0);
    }

    [Test]
    public void Dispose_ShouldCloseConnection()
    {
        // Arrange
        var dbManager = new DatabaseManager(_connectionString);
        var connectionTask = dbManager.GetConnectionAsync();
        connectionTask.Wait();
        var connection = connectionTask.Result;
        
        connection.State.Should().Be(System.Data.ConnectionState.Open);

        // Act
        dbManager.Dispose();

        // Assert
        connection.State.Should().Be(System.Data.ConnectionState.Closed);
    }
}
