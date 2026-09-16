using System.Data;
using AccesoAdoNet.Console.Database;
using AccesoAdoNet.Console.Models;
using Npgsql;

namespace AccesoAdoNet.Console.Repositories;

/// <summary>
/// Implementación del repositorio con ADO.NET puro
/// Equivalente a usar JDBC directamente en Java (sin JPA)
/// 
/// En Java con JDBC sería:
/// public class TenistaRepositoryImpl implements TenistaRepository {
///     private Connection connection;
///     
///     public Tenista save(Tenista tenista) {
///         String sql = "INSERT INTO tenistas ...";
///         PreparedStatement ps = connection.prepareStatement(sql);
///         ps.setString(1, tenista.getNombre());
///         ...
///     }
/// }
/// 
/// ADO.NET proporciona:
/// - NpgsqlCommand = PreparedStatement en Java
/// - NpgsqlDataReader = ResultSet en Java
/// - NpgsqlParameter = ? placeholders en Java
/// </summary>
public class TenistaRepository : ITenistaRepository
{
    private readonly DatabaseManager _databaseManager;

    public TenistaRepository(DatabaseManager databaseManager)
    {
        _databaseManager = databaseManager;
    }

    /// <summary>
    /// Crea un nuevo tenista en la base de datos
    /// En Java/JDBC: PreparedStatement ps = conn.prepareStatement("INSERT INTO ...", Statement.RETURN_GENERATED_KEYS);
    /// </summary>
    public async Task<Tenista> CreateAsync(Tenista tenista)
    {
        var connection = await _databaseManager.GetConnectionAsync();

        // SQL con RETURNING para obtener el ID generado (específico de PostgreSQL)
        // En Java/MySQL usarías Statement.RETURN_GENERATED_KEYS
        var sql = @"
            INSERT INTO tenistas (nombre, ranking, pais, altura, peso, titulos, fecha_nacimiento)
            VALUES (@nombre, @ranking, @pais, @altura, @peso, @titulos, @fechaNacimiento)
            RETURNING id";

        await using var command = new NpgsqlCommand(sql, connection);

        // Agregar parámetros para prevenir SQL Injection
        // En Java: ps.setString(1, tenista.getNombre());
        // En C#: command.Parameters.AddWithValue("@nombre", tenista.Nombre);
        command.Parameters.AddWithValue("@nombre", tenista.Nombre);
        command.Parameters.AddWithValue("@ranking", tenista.Ranking);
        command.Parameters.AddWithValue("@pais", tenista.Pais);
        command.Parameters.AddWithValue("@altura", tenista.Altura);
        command.Parameters.AddWithValue("@peso", tenista.Peso);
        command.Parameters.AddWithValue("@titulos", tenista.Titulos);
        command.Parameters.AddWithValue("@fechaNacimiento", tenista.FechaNacimiento);

        // Ejecutar y obtener el ID generado
        // En Java: ResultSet rs = ps.getGeneratedKeys(); if(rs.next()) id = rs.getLong(1);
        var id = await command.ExecuteScalarAsync();
        tenista.Id = Convert.ToInt64(id);

        System.Console.WriteLine($"✅ Tenista creado: {tenista.Nombre} con ID {tenista.Id}");
        return tenista;
    }

    /// <summary>
    /// Busca un tenista por ID
    /// En Java/JDBC: PreparedStatement ps = conn.prepareStatement("SELECT * FROM tenistas WHERE id = ?");
    ///               ps.setLong(1, id);
    ///               ResultSet rs = ps.executeQuery();
    /// </summary>
    public async Task<Tenista?> FindByIdAsync(long id)
    {
        var connection = await _databaseManager.GetConnectionAsync();

        var sql = "SELECT * FROM tenistas WHERE id = @id";
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);

        // ExecuteReader = executeQuery() en Java
        // DataReader = ResultSet en Java
        await using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            // Mapeo manual de ResultSet a objeto
            // En Java: tenista.setNombre(rs.getString("nombre"));
            return MapReaderToTenista(reader);
        }

        return null;
    }

    /// <summary>
    /// Obtiene todos los tenistas
    /// En Java/JDBC: Statement stmt = conn.createStatement();
    ///               ResultSet rs = stmt.executeQuery("SELECT * FROM tenistas");
    /// </summary>
    public async Task<List<Tenista>> FindAllAsync()
    {
        var connection = await _databaseManager.GetConnectionAsync();
        var tenistas = new List<Tenista>();

        var sql = "SELECT * FROM tenistas ORDER BY ranking";
        await using var command = new NpgsqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync();

        // Iterar sobre el ResultSet
        // En Java: while(rs.next()) { ... }
        while (await reader.ReadAsync())
        {
            tenistas.Add(MapReaderToTenista(reader));
        }

        return tenistas;
    }

    /// <summary>
    /// Actualiza un tenista existente
    /// En Java/JDBC: PreparedStatement ps = conn.prepareStatement("UPDATE tenistas SET ... WHERE id = ?");
    /// </summary>
    public async Task<Tenista> UpdateAsync(Tenista tenista)
    {
        var connection = await _databaseManager.GetConnectionAsync();

        var sql = @"
            UPDATE tenistas 
            SET nombre = @nombre, 
                ranking = @ranking, 
                pais = @pais, 
                altura = @altura, 
                peso = @peso, 
                titulos = @titulos, 
                fecha_nacimiento = @fechaNacimiento
            WHERE id = @id";

        await using var command = new NpgsqlCommand(sql, connection);
        
        command.Parameters.AddWithValue("@id", tenista.Id);
        command.Parameters.AddWithValue("@nombre", tenista.Nombre);
        command.Parameters.AddWithValue("@ranking", tenista.Ranking);
        command.Parameters.AddWithValue("@pais", tenista.Pais);
        command.Parameters.AddWithValue("@altura", tenista.Altura);
        command.Parameters.AddWithValue("@peso", tenista.Peso);
        command.Parameters.AddWithValue("@titulos", tenista.Titulos);
        command.Parameters.AddWithValue("@fechaNacimiento", tenista.FechaNacimiento);

        // ExecuteNonQuery = executeUpdate() en Java (retorna filas afectadas)
        var rowsAffected = await command.ExecuteNonQueryAsync();

        if (rowsAffected == 0)
        {
            throw new InvalidOperationException($"Tenista con ID {tenista.Id} no encontrado");
        }

        System.Console.WriteLine($"✅ Tenista actualizado: {tenista.Nombre}");
        return tenista;
    }

    /// <summary>
    /// Elimina un tenista por ID
    /// En Java/JDBC: PreparedStatement ps = conn.prepareStatement("DELETE FROM tenistas WHERE id = ?");
    /// </summary>
    public async Task<bool> DeleteAsync(long id)
    {
        var connection = await _databaseManager.GetConnectionAsync();

        var sql = "DELETE FROM tenistas WHERE id = @id";
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);

        var rowsAffected = await command.ExecuteNonQueryAsync();
        
        if (rowsAffected > 0)
        {
            System.Console.WriteLine($"✅ Tenista con ID {id} eliminado");
            return true;
        }

        return false;
    }

    /// <summary>
    /// Busca tenistas por país
    /// Ejemplo de consulta con filtro
    /// </summary>
    public async Task<List<Tenista>> FindByPaisAsync(string pais)
    {
        var connection = await _databaseManager.GetConnectionAsync();
        var tenistas = new List<Tenista>();

        var sql = "SELECT * FROM tenistas WHERE pais = @pais ORDER BY ranking";
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@pais", pais);

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            tenistas.Add(MapReaderToTenista(reader));
        }

        return tenistas;
    }

    /// <summary>
    /// Cuenta el total de tenistas
    /// En Java/JDBC: ResultSet rs = stmt.executeQuery("SELECT COUNT(*) FROM tenistas");
    ///               int count = rs.getInt(1);
    /// </summary>
    public async Task<int> CountAsync()
    {
        var connection = await _databaseManager.GetConnectionAsync();

        var sql = "SELECT COUNT(*) FROM tenistas";
        await using var command = new NpgsqlCommand(sql, connection);

        // ExecuteScalar para obtener un solo valor
        // En Java: Object result = stmt.executeQuery(...).getObject(1);
        var count = await command.ExecuteScalarAsync();
        return Convert.ToInt32(count);
    }

    /// <summary>
    /// Mapea un DataReader a un objeto Tenista
    /// Equivalente al mapeo manual de ResultSet en Java:
    /// 
    /// Tenista tenista = new Tenista();
    /// tenista.setId(rs.getLong("id"));
    /// tenista.setNombre(rs.getString("nombre"));
    /// ...
    /// </summary>
    private static Tenista MapReaderToTenista(NpgsqlDataReader reader)
    {
        return new Tenista
        {
            // En Java: rs.getLong("id")
            Id = reader.GetInt64("id"),
            
            // En Java: rs.getString("nombre")
            Nombre = reader.GetString("nombre"),
            
            // En Java: rs.getInt("ranking")
            Ranking = reader.GetInt32("ranking"),
            
            Pais = reader.GetString("pais"),
            Altura = reader.GetInt32("altura"),
            Peso = reader.GetInt32("peso"),
            Titulos = reader.GetInt32("titulos"),
            
            // En Java: rs.getDate("fecha_nacimiento")
            FechaNacimiento = reader.GetDateTime("fecha_nacimiento")
        };
    }
}
