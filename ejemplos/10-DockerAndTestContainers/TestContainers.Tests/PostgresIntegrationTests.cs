using Testcontainers.PostgreSql;
using TestContainers.Tests.Models;
using TestContainers.Tests.Services;

namespace TestContainers.Tests;

/// <summary>
/// Tests de integración con PostgreSQL usando Testcontainers
/// Equivalente a @Testcontainers y @Container en Java
/// 
/// En Java (Testcontainers):
/// @Testcontainers
/// class PostgresTests {
///     @Container
///     PostgreSQLContainer postgres = new PostgreSQLContainer("postgres:15");
/// }
/// 
/// En C# (Testcontainers.NET):
/// [TestFixture]
/// class PostgresTests {
///     PostgreSqlContainer _postgres = new PostgreSqlBuilder().Build();
/// }
/// </summary>
[TestFixture]
public class PostgresIntegrationTests
{
    private PostgreSqlContainer _postgresContainer = null!;
    private UsuarioRepository _repository = null!;

    /// <summary>
    /// Configuración antes de cada test
    /// En Java: @BeforeEach
    /// En C#: [SetUp]
    /// </summary>
    [SetUp]
    public async Task Setup()
    {
        // Crear y arrancar contenedor PostgreSQL
        _postgresContainer = new PostgreSqlBuilder()
            .WithImage("postgres:15-alpine")
            .WithDatabase("testdb")
            .WithUsername("testuser")
            .WithPassword("testpass")
            .Build();

        await _postgresContainer.StartAsync();

        // Crear repositorio con la cadena de conexión del contenedor
        _repository = new UsuarioRepository(_postgresContainer.GetConnectionString());
        await _repository.InicializarBaseDatosAsync();
    }

    /// <summary>
    /// Limpieza después de cada test
    /// En Java: @AfterEach
    /// En C#: [TearDown]
    /// </summary>
    [TearDown]
    public async Task TearDown()
    {
        await _postgresContainer.DisposeAsync();
    }

    [Test]
    public async Task CrearUsuario_DeberiaGuardarEnBaseDatos()
    {
        // Arrange
        var usuario = new Usuario
        {
            Nombre = "Juan Pérez",
            Email = "juan@example.com",
            FechaRegistro = DateTime.UtcNow
        };

        // Act
        var usuarioCreado = await _repository.CrearUsuarioAsync(usuario);

        // Assert
        Assert.That(usuarioCreado.Id, Is.GreaterThan(0));
        Assert.That(usuarioCreado.Nombre, Is.EqualTo("Juan Pérez"));
        Assert.That(usuarioCreado.Email, Is.EqualTo("juan@example.com"));
    }

    [Test]
    public async Task ObtenerUsuarioPorId_UsuarioExiste_DeberiaRetornarUsuario()
    {
        // Arrange
        var usuario = new Usuario
        {
            Nombre = "María García",
            Email = "maria@example.com",
            FechaRegistro = DateTime.UtcNow
        };
        var usuarioCreado = await _repository.CrearUsuarioAsync(usuario);

        // Act
        var usuarioObtenido = await _repository.ObtenerUsuarioPorIdAsync(usuarioCreado.Id);

        // Assert
        Assert.That(usuarioObtenido, Is.Not.Null);
        Assert.That(usuarioObtenido!.Id, Is.EqualTo(usuarioCreado.Id));
        Assert.That(usuarioObtenido.Nombre, Is.EqualTo("María García"));
        Assert.That(usuarioObtenido.Email, Is.EqualTo("maria@example.com"));
    }

    [Test]
    public async Task ObtenerUsuarioPorId_UsuarioNoExiste_DeberiaRetornarNull()
    {
        // Act
        var usuario = await _repository.ObtenerUsuarioPorIdAsync(999);

        // Assert
        Assert.That(usuario, Is.Null);
    }

    [Test]
    public async Task ObtenerTodosUsuarios_DeberiaRetornarListaCompleta()
    {
        // Arrange
        await _repository.CrearUsuarioAsync(new Usuario
        {
            Nombre = "Usuario 1",
            Email = "user1@example.com",
            FechaRegistro = DateTime.UtcNow
        });

        await _repository.CrearUsuarioAsync(new Usuario
        {
            Nombre = "Usuario 2",
            Email = "user2@example.com",
            FechaRegistro = DateTime.UtcNow
        });

        await _repository.CrearUsuarioAsync(new Usuario
        {
            Nombre = "Usuario 3",
            Email = "user3@example.com",
            FechaRegistro = DateTime.UtcNow
        });

        // Act
        var usuarios = await _repository.ObtenerTodosUsuariosAsync();

        // Assert
        Assert.That(usuarios, Has.Count.EqualTo(3));
        Assert.That(usuarios[0].Nombre, Is.EqualTo("Usuario 1"));
        Assert.That(usuarios[1].Nombre, Is.EqualTo("Usuario 2"));
        Assert.That(usuarios[2].Nombre, Is.EqualTo("Usuario 3"));
    }

    [Test]
    public async Task EliminarUsuario_UsuarioExiste_DeberiaEliminarYRetornarTrue()
    {
        // Arrange
        var usuario = await _repository.CrearUsuarioAsync(new Usuario
        {
            Nombre = "Usuario a Eliminar",
            Email = "eliminar@example.com",
            FechaRegistro = DateTime.UtcNow
        });

        // Act
        var resultado = await _repository.EliminarUsuarioAsync(usuario.Id);
        var usuarioEliminado = await _repository.ObtenerUsuarioPorIdAsync(usuario.Id);

        // Assert
        Assert.That(resultado, Is.True);
        Assert.That(usuarioEliminado, Is.Null);
    }

    [Test]
    public async Task EliminarUsuario_UsuarioNoExiste_DeberiaRetornarFalse()
    {
        // Act
        var resultado = await _repository.EliminarUsuarioAsync(999);

        // Assert
        Assert.That(resultado, Is.False);
    }

    [Test]
    public async Task CrearMultiplesUsuarios_DeberiaGuardarTodos()
    {
        // Arrange & Act
        var usuario1 = await _repository.CrearUsuarioAsync(new Usuario
        {
            Nombre = "Test User 1",
            Email = "test1@example.com",
            FechaRegistro = DateTime.UtcNow
        });

        var usuario2 = await _repository.CrearUsuarioAsync(new Usuario
        {
            Nombre = "Test User 2",
            Email = "test2@example.com",
            FechaRegistro = DateTime.UtcNow
        });

        var usuarios = await _repository.ObtenerTodosUsuariosAsync();

        // Assert
        Assert.That(usuario1.Id, Is.GreaterThan(0));
        Assert.That(usuario2.Id, Is.GreaterThan(0));
        Assert.That(usuario2.Id, Is.GreaterThan(usuario1.Id));
        Assert.That(usuarios, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task ContenedorPostgres_DeberiaTenerConfiguracionCorrecta()
    {
        // Assert - Verificar que el contenedor está configurado correctamente
        Assert.That(_postgresContainer.Name, Does.Contain("postgres"));
        Assert.That(_postgresContainer.GetConnectionString(), Does.Contain("testdb"));
        Assert.That(_postgresContainer.GetConnectionString(), Does.Contain("testuser"));
    }
}
