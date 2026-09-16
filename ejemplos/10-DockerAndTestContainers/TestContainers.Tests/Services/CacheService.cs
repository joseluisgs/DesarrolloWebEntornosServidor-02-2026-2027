using StackExchange.Redis;

namespace TestContainers.Tests.Services;

/// <summary>
/// Servicio de caché usando Redis
/// Similar a Spring Cache en Java
/// </summary>
public class CacheService : IDisposable
{
    private readonly ConnectionMultiplexer _connection;
    private readonly IDatabase _database;
    private bool _disposed;

    public CacheService(string connectionString)
    {
        _connection = ConnectionMultiplexer.Connect(connectionString);
        _database = _connection.GetDatabase();
    }

    public async Task<bool> GuardarAsync(string key, string value, TimeSpan? expiration = null)
    {
        return await _database.StringSetAsync(key, value, expiration);
    }

    public async Task<string?> ObtenerAsync(string key)
    {
        var value = await _database.StringGetAsync(key);
        return value.HasValue ? value.ToString() : null;
    }

    public async Task<bool> ExisteAsync(string key)
    {
        return await _database.KeyExistsAsync(key);
    }

    public async Task<bool> EliminarAsync(string key)
    {
        return await _database.KeyDeleteAsync(key);
    }

    public async Task<long> IncrementarAsync(string key)
    {
        return await _database.StringIncrementAsync(key);
    }

    public async Task<bool> GuardarHashAsync(string key, Dictionary<string, string> valores)
    {
        var entries = valores.Select(kvp => new HashEntry(kvp.Key, kvp.Value)).ToArray();
        await _database.HashSetAsync(key, entries);
        return true;
    }

    public async Task<Dictionary<string, string>> ObtenerHashAsync(string key)
    {
        var entries = await _database.HashGetAllAsync(key);
        return entries.ToDictionary(
            entry => entry.Name.ToString(),
            entry => entry.Value.ToString()
        );
    }

    public async Task LimpiarAsync()
    {
        try
        {
            var endpoints = _connection.GetEndPoints();
            foreach (var endpoint in endpoints)
            {
                var server = _connection.GetServer(endpoint);
                await server.FlushDatabaseAsync();
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error al limpiar la base de datos Redis", ex);
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _connection?.Dispose();
            _disposed = true;
        }
    }
}
