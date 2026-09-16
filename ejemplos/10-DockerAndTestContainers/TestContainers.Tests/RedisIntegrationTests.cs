using Testcontainers.Redis;
using TestContainers.Tests.Services;

namespace TestContainers.Tests;

/// <summary>
/// Tests de integración con Redis usando Testcontainers
/// Equivalente a @Testcontainers y @Container en Java
/// 
/// En Java (Testcontainers):
/// @Testcontainers
/// class RedisTests {
///     @Container
///     GenericContainer redis = new GenericContainer("redis:7-alpine")
///         .withExposedPorts(6379);
/// }
/// 
/// En C# (Testcontainers.NET):
/// [TestFixture]
/// class RedisTests {
///     RedisContainer _redis = new RedisBuilder().Build();
/// }
/// </summary>
[TestFixture]
public class RedisIntegrationTests
{
    private RedisContainer _redisContainer = null!;
    private CacheService _cacheService = null!;

    [SetUp]
    public async Task Setup()
    {
        // Crear y arrancar contenedor Redis
        _redisContainer = new RedisBuilder()
            .WithImage("redis:7-alpine")
            .Build();

        await _redisContainer.StartAsync();

        // Crear servicio de caché con la cadena de conexión del contenedor
        _cacheService = new CacheService(_redisContainer.GetConnectionString());
    }

    [TearDown]
    public async Task TearDown()
    {
        _cacheService?.Dispose();
        await _redisContainer.DisposeAsync();
    }

    [Test]
    public async Task GuardarYObtener_DeberiaRetornarValorGuardado()
    {
        // Arrange
        const string key = "test-key";
        const string value = "test-value";

        // Act
        await _cacheService.GuardarAsync(key, value);
        var resultado = await _cacheService.ObtenerAsync(key);

        // Assert
        Assert.That(resultado, Is.EqualTo(value));
    }

    [Test]
    public async Task Obtener_ClaveNoExiste_DeberiaRetornarNull()
    {
        // Act
        var resultado = await _cacheService.ObtenerAsync("clave-inexistente");

        // Assert
        Assert.That(resultado, Is.Null);
    }

    [Test]
    public async Task Existe_ClaveExiste_DeberiaRetornarTrue()
    {
        // Arrange
        const string key = "existing-key";
        await _cacheService.GuardarAsync(key, "value");

        // Act
        var existe = await _cacheService.ExisteAsync(key);

        // Assert
        Assert.That(existe, Is.True);
    }

    [Test]
    public async Task Existe_ClaveNoExiste_DeberiaRetornarFalse()
    {
        // Act
        var existe = await _cacheService.ExisteAsync("non-existing-key");

        // Assert
        Assert.That(existe, Is.False);
    }

    [Test]
    public async Task Eliminar_ClaveExiste_DeberiaEliminarYRetornarTrue()
    {
        // Arrange
        const string key = "key-to-delete";
        await _cacheService.GuardarAsync(key, "value");

        // Act
        var resultado = await _cacheService.EliminarAsync(key);
        var existe = await _cacheService.ExisteAsync(key);

        // Assert
        Assert.That(resultado, Is.True);
        Assert.That(existe, Is.False);
    }

    [Test]
    public async Task Eliminar_ClaveNoExiste_DeberiaRetornarFalse()
    {
        // Act
        var resultado = await _cacheService.EliminarAsync("non-existing-key");

        // Assert
        Assert.That(resultado, Is.False);
    }

    [Test]
    public async Task GuardarConExpiracion_DeberiaCaducar()
    {
        // Arrange
        const string key = "expiring-key";
        const string value = "expiring-value";

        // Act
        await _cacheService.GuardarAsync(key, value, TimeSpan.FromMilliseconds(100));
        var valorInicial = await _cacheService.ObtenerAsync(key);
        
        await Task.Delay(TimeSpan.FromMilliseconds(200));
        var valorDespuesExpiracion = await _cacheService.ObtenerAsync(key);

        // Assert
        Assert.That(valorInicial, Is.EqualTo(value));
        Assert.That(valorDespuesExpiracion, Is.Null);
    }

    [Test]
    public async Task Incrementar_DeberiaSumarValor()
    {
        // Arrange
        const string key = "counter";

        // Act
        var valor1 = await _cacheService.IncrementarAsync(key);
        var valor2 = await _cacheService.IncrementarAsync(key);
        var valor3 = await _cacheService.IncrementarAsync(key);

        // Assert
        Assert.That(valor1, Is.EqualTo(1));
        Assert.That(valor2, Is.EqualTo(2));
        Assert.That(valor3, Is.EqualTo(3));
    }

    [Test]
    public async Task GuardarHash_DeberiaAlmacenarMultiplesValores()
    {
        // Arrange
        const string key = "user:1";
        var datos = new Dictionary<string, string>
        {
            { "nombre", "Juan Pérez" },
            { "email", "juan@example.com" },
            { "edad", "30" }
        };

        // Act
        await _cacheService.GuardarHashAsync(key, datos);
        var resultado = await _cacheService.ObtenerHashAsync(key);

        // Assert
        Assert.That(resultado, Has.Count.EqualTo(3));
        Assert.That(resultado["nombre"], Is.EqualTo("Juan Pérez"));
        Assert.That(resultado["email"], Is.EqualTo("juan@example.com"));
        Assert.That(resultado["edad"], Is.EqualTo("30"));
    }

    [Test]
    public async Task GuardarMultiplesClaves_DeberiaAlmacenarTodas()
    {
        // Arrange & Act
        await _cacheService.GuardarAsync("key1", "value1");
        await _cacheService.GuardarAsync("key2", "value2");
        await _cacheService.GuardarAsync("key3", "value3");

        var valor1 = await _cacheService.ObtenerAsync("key1");
        var valor2 = await _cacheService.ObtenerAsync("key2");
        var valor3 = await _cacheService.ObtenerAsync("key3");

        // Assert
        Assert.That(valor1, Is.EqualTo("value1"));
        Assert.That(valor2, Is.EqualTo("value2"));
        Assert.That(valor3, Is.EqualTo("value3"));
    }

    [Test]
    public async Task Limpiar_DeberiaEliminarTodosLosDatos()
    {
        // Arrange
        await _cacheService.GuardarAsync("key1", "value1");
        await _cacheService.GuardarAsync("key2", "value2");
        await _cacheService.GuardarAsync("key3", "value3");

        // Act
        await _cacheService.LimpiarAsync();
        
        var existe1 = await _cacheService.ExisteAsync("key1");
        var existe2 = await _cacheService.ExisteAsync("key2");
        var existe3 = await _cacheService.ExisteAsync("key3");

        // Assert
        Assert.That(existe1, Is.False);
        Assert.That(existe2, Is.False);
        Assert.That(existe3, Is.False);
    }

    [Test]
    public async Task ContenedorRedis_DeberiaTenerConfiguracionCorrecta()
    {
        // Assert - Verificar que el contenedor está configurado correctamente
        Assert.That(_redisContainer.Name, Does.Contain("redis"));
        Assert.That(_redisContainer.GetConnectionString(), Is.Not.Empty);
    }
}
