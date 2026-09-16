using Npgsql;

namespace AccesoAdoNet.Console.Database;

/// <summary>
/// DatabaseManager - Gestión de conexiones a base de datos con ADO.NET
/// Equivalente a JDBC en Java (Connection, DriverManager)
/// 
/// En Java sería similar a:
/// public class DatabaseManager {
///     private Connection connection;
///     public Connection getConnection() throws SQLException { ... }
/// }
/// 
/// ADO.NET es el equivalente de bajo nivel a JDBC en Java.
/// Proporciona control total sobre la conexión y ejecución de SQL.
/// </summary>
public class DatabaseManager : IDisposable
{
    private readonly string _connectionString;
    private NpgsqlConnection? _connection;

    /// <summary>
    /// Constructor que recibe la cadena de conexión
    /// En Java: public DatabaseManager(String connectionString)
    /// </summary>
    public DatabaseManager(string connectionString)
    {
        _connectionString = connectionString;
    }

    /// <summary>
    /// Obtiene o crea una conexión a la base de datos
    /// Equivalente a Java: Connection connection = DriverManager.getConnection(url, user, password);
    /// </summary>
    /// <returns>Conexión activa a PostgreSQL</returns>
    public async Task<NpgsqlConnection> GetConnectionAsync()
    {
        // Si ya existe una conexión abierta, la reutilizamos
        if (_connection != null && _connection.State == System.Data.ConnectionState.Open)
        {
            return _connection;
        }

        // Crear nueva conexión
        _connection = new NpgsqlConnection(_connectionString);
        
        // En Java: connection.open();
        // En C#: await connection.OpenAsync();
        await _connection.OpenAsync();
        
        return _connection;
    }

    /// <summary>
    /// Crea las tablas necesarias en la base de datos
    /// Equivalente a ejecutar DDL (Data Definition Language) en Java/JDBC
    /// </summary>
    public async Task InitializeDatabaseAsync()
    {
        var connection = await GetConnectionAsync();

        // SQL DDL para crear tabla
        // En Java: Statement statement = connection.createStatement();
        // En C#: NpgsqlCommand command = new NpgsqlCommand(sql, connection);
        var createTableSql = @"
            CREATE TABLE IF NOT EXISTS tenistas (
                id SERIAL PRIMARY KEY,
                nombre VARCHAR(100) NOT NULL,
                ranking INTEGER NOT NULL,
                pais VARCHAR(50) NOT NULL,
                altura INTEGER NOT NULL,
                peso INTEGER NOT NULL,
                titulos INTEGER NOT NULL,
                fecha_nacimiento DATE NOT NULL
            )";

        await using var command = new NpgsqlCommand(createTableSql, connection);
        
        // En Java: statement.execute();
        // En C#: await command.ExecuteNonQueryAsync();
        await command.ExecuteNonQueryAsync();
        
        System.Console.WriteLine("✅ Base de datos inicializada correctamente");
    }

    /// <summary>
    /// Limpia todas las tablas (útil para testing)
    /// En Java: statement.executeUpdate("TRUNCATE TABLE tenistas");
    /// </summary>
    public async Task ClearDatabaseAsync()
    {
        var connection = await GetConnectionAsync();
        
        var truncateSql = "TRUNCATE TABLE tenistas RESTART IDENTITY CASCADE";
        await using var command = new NpgsqlCommand(truncateSql, connection);
        await command.ExecuteNonQueryAsync();
        
        System.Console.WriteLine("🗑️  Base de datos limpiada");
    }

    /// <summary>
    /// Inicia una transacción
    /// En Java: connection.setAutoCommit(false);
    /// En C#: var transaction = await connection.BeginTransactionAsync();
    /// </summary>
    public async Task<NpgsqlTransaction> BeginTransactionAsync()
    {
        var connection = await GetConnectionAsync();
        return await connection.BeginTransactionAsync();
    }

    /// <summary>
    /// Implementación de IDisposable para liberar recursos
    /// En Java: @Override public void close() throws Exception
    /// En C#: public void Dispose() - patrón estándar
    /// </summary>
    public void Dispose()
    {
        // En Java: if (connection != null) connection.close();
        // En C#: using y Dispose se encargan automáticamente
        _connection?.Dispose();
    }
}
