using Npgsql;
using TestContainers.Tests.Models;

namespace TestContainers.Tests.Services;

/// <summary>
/// Repositorio de usuarios usando PostgreSQL
/// Similar a Spring Data JPA Repository en Java
/// </summary>
public class UsuarioRepository
{
    private readonly string _connectionString;

    public UsuarioRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task InicializarBaseDatosAsync()
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var createTableSql = @"
            CREATE TABLE IF NOT EXISTS usuarios (
                id SERIAL PRIMARY KEY,
                nombre VARCHAR(100) NOT NULL,
                email VARCHAR(100) NOT NULL UNIQUE,
                fecha_registro TIMESTAMP NOT NULL DEFAULT NOW()
            )";

        await using var command = new NpgsqlCommand(createTableSql, connection);
        await command.ExecuteNonQueryAsync();
    }

    public async Task<Usuario> CrearUsuarioAsync(Usuario usuario)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var sql = @"
            INSERT INTO usuarios (nombre, email, fecha_registro) 
            VALUES (@nombre, @email, @fecha_registro)
            RETURNING id";

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("nombre", usuario.Nombre);
        command.Parameters.AddWithValue("email", usuario.Email);
        command.Parameters.AddWithValue("fecha_registro", usuario.FechaRegistro);

        var id = (int)(await command.ExecuteScalarAsync() ?? 0);
        usuario.Id = id;
        return usuario;
    }

    public async Task<Usuario?> ObtenerUsuarioPorIdAsync(int id)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var sql = "SELECT id, nombre, email, fecha_registro FROM usuarios WHERE id = @id";
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("id", id);

        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Usuario
            {
                Id = reader.GetInt32(0),
                Nombre = reader.GetString(1),
                Email = reader.GetString(2),
                FechaRegistro = reader.GetDateTime(3)
            };
        }

        return null;
    }

    public async Task<List<Usuario>> ObtenerTodosUsuariosAsync()
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var sql = "SELECT id, nombre, email, fecha_registro FROM usuarios ORDER BY id";
        await using var command = new NpgsqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync();

        var usuarios = new List<Usuario>();
        while (await reader.ReadAsync())
        {
            usuarios.Add(new Usuario
            {
                Id = reader.GetInt32(0),
                Nombre = reader.GetString(1),
                Email = reader.GetString(2),
                FechaRegistro = reader.GetDateTime(3)
            });
        }

        return usuarios;
    }

    public async Task<bool> EliminarUsuarioAsync(int id)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var sql = "DELETE FROM usuarios WHERE id = @id";
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("id", id);

        var rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public async Task LimpiarBaseDatosAsync()
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var sql = "DELETE FROM usuarios";
        await using var command = new NpgsqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync();
    }
}
