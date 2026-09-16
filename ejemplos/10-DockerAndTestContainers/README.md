# 🐳 Ejemplo 10: TestContainers - Testing con Docker

## Descripción

Este ejemplo demuestra cómo usar **Testcontainers.NET** para realizar tests de integración con bases de datos y servicios reales usando contenedores Docker. Es el equivalente de **TestContainers** en Java.

## Comparación Java vs C#

### Java (TestContainers)

```java
@Testcontainers
class PostgresTests {
    @Container
    static PostgreSQLContainer<?> postgres = 
        new PostgreSQLContainer<>("postgres:15");
    
    @Test
    void testDatabase() {
        String jdbcUrl = postgres.getJdbcUrl();
        // ... usar la base de datos
    }
}
```

### C# (Testcontainers.NET)

```csharp
[TestFixture]
public class PostgresTests {
    private PostgreSqlContainer _postgres;
    
    [SetUp]
    public async Task Setup() {
        _postgres = new PostgreSqlBuilder()
            .WithImage("postgres:15-alpine")
            .Build();
        await _postgres.StartAsync();
    }
    
    [Test]
    public async Task TestDatabase() {
        var connectionString = _postgres.GetConnectionString();
        // ... usar la base de datos
    }
    
    [TearDown]
    public async Task TearDown() {
        await _postgres.DisposeAsync();
    }
}
```

## Tecnologías Utilizadas

| Componente | Paquete NuGet | Descripción |
|------------|---------------|-------------|
| Framework de Tests | NUnit 3.14.0 | Similar a JUnit 5 |
| TestContainers Core | Testcontainers 3.10.0 | Librería base |
| PostgreSQL Container | Testcontainers.PostgreSql 3.10.0 | Contenedor PostgreSQL |
| Redis Container | Testcontainers.Redis 3.10.0 | Contenedor Redis |
| Cliente PostgreSQL | Npgsql 8.0.5 | Driver ADO.NET para PostgreSQL |
| Cliente Redis | StackExchange.Redis 2.8.16 | Cliente Redis oficial |

## Tests Incluidos

### PostgreSQL Integration Tests

- ✅ Crear usuarios en base de datos
- ✅ Obtener usuario por ID
- ✅ Listar todos los usuarios
- ✅ Eliminar usuarios
- ✅ Operaciones CRUD completas

### Redis Integration Tests

- ✅ Guardar y obtener valores en caché
- ✅ Verificar existencia de claves
- ✅ Eliminar claves
- ✅ Valores con expiración (TTL)
- ✅ Operaciones de contador (INCR)
- ✅ Hashes (estructuras complejas)
- ✅ Limpiar toda la base de datos

## Ejecución de Tests

### Prerrequisitos

- Docker Desktop instalado y ejecutándose
- .NET 8 SDK

### Ejecutar Tests

```bash
# Navegar al directorio del proyecto
cd ejemplos/10-DockerAndTestContainers/TestContainers.Tests

# Restaurar dependencias
dotnet restore

# Ejecutar todos los tests
dotnet test

# Ejecutar con información detallada
dotnet test --logger "console;verbosity=detailed"

# Ejecutar tests específicos
dotnet test --filter "FullyQualifiedName~PostgresIntegrationTests"
dotnet test --filter "FullyQualifiedName~RedisIntegrationTests"
```

## Ventajas de Testcontainers

### ✅ Tests con Dependencias Reales

- No necesitas mocks para bases de datos
- Tests con el mismo comportamiento que producción
- Detecta problemas de compatibilidad tempranamente

### ✅ Aislamiento de Tests

- Cada test puede tener su propio contenedor
- No hay interferencia entre tests
- Base de datos limpia para cada test

### ✅ Portabilidad

- Funciona en cualquier máquina con Docker
- CI/CD sin configuración adicional
- Sin necesidad de instalar bases de datos localmente

### ✅ Reproducibilidad

- Versión exacta de la base de datos definida
- Comportamiento consistente en todos los entornos
- Fácil de actualizar versiones

## Estructura del Proyecto

```
10-DockerAndTestContainers/
└── TestContainers.Tests/
    ├── Models/
    │   └── Usuario.cs                    # Modelo de dominio
    ├── Services/
    │   ├── UsuarioRepository.cs          # Repositorio PostgreSQL
    │   └── CacheService.cs               # Servicio Redis
    ├── PostgresIntegrationTests.cs       # Tests de PostgreSQL
    ├── RedisIntegrationTests.cs          # Tests de Redis
    └── TestContainers.Tests.csproj       # Configuración del proyecto
```

## Ciclo de Vida de Tests

### 1. SetUp ([SetUp] en NUnit / @BeforeEach en JUnit)

```csharp
[SetUp]
public async Task Setup()
{
    // Crear contenedor
    _container = new PostgreSqlBuilder().Build();
    
    // Iniciar contenedor
    await _container.StartAsync();
    
    // Obtener connection string
    var connectionString = _container.GetConnectionString();
    
    // Inicializar servicio/repositorio
    _repository = new Repository(connectionString);
}
```

### 2. Test Execution

```csharp
[Test]
public async Task MiTest()
{
    // Arrange - Preparar datos
    var usuario = new Usuario { /* ... */ };
    
    // Act - Ejecutar acción
    var resultado = await _repository.Crear(usuario);
    
    // Assert - Verificar resultado
    Assert.That(resultado.Id, Is.GreaterThan(0));
}
```

### 3. TearDown ([TearDown] en NUnit / @AfterEach en JUnit)

```csharp
[TearDown]
public async Task TearDown()
{
    // Detener y eliminar contenedor
    await _container.DisposeAsync();
}
```

## Buenas Prácticas

### ✅ Usa Contenedores Alpine Cuando Sea Posible

```csharp
.WithImage("postgres:15-alpine")  // Más ligero
.WithImage("redis:7-alpine")      // Más rápido
```

### ✅ Limpia Datos Entre Tests

```csharp
[SetUp]
public async Task Setup()
{
    await _container.StartAsync();
    await _repository.InicializarBaseDatos();
    await _repository.LimpiarDatos(); // Importante
}
```

### ✅ Configura Timeouts Apropiados

```csharp
var container = new PostgreSqlBuilder()
    .WithStartupTimeout(TimeSpan.FromMinutes(2))
    .Build();
```

### ✅ Reutiliza Contenedores Para Múltiples Tests

```csharp
[OneTimeSetUp]  // Una vez para toda la clase
public async Task OneTimeSetup()
{
    await _container.StartAsync();
}

[OneTimeTearDown]  // Una vez al final
public async Task OneTimeTearDown()
{
    await _container.DisposeAsync();
}
```

## Tabla Comparativa Final

| Aspecto | Java (TestContainers) | C# (Testcontainers.NET) |
|---------|----------------------|-------------------------|
| **Anotación de Clase** | `@Testcontainers` | `[TestFixture]` |
| **Anotación de Container** | `@Container` | No necesaria (manual) |
| **Setup** | `@BeforeEach` | `[SetUp]` |
| **Cleanup** | `@AfterEach` | `[TearDown]` |
| **Test Method** | `@Test` | `[Test]` |
| **Assert** | `assertEquals()` | `Assert.That()` |
| **Postgres Container** | `PostgreSQLContainer` | `PostgreSqlContainer` |
| **Redis Container** | `GenericContainer` | `RedisContainer` |
| **Get Connection** | `getJdbcUrl()` | `GetConnectionString()` |
| **Start Container** | Automático | `await StartAsync()` |
| **Stop Container** | Automático | `await DisposeAsync()` |

## Cuándo Usar Testcontainers

### ✅ SI

- Tests de integración con bases de datos
- Tests de microservicios
- Tests de APIs que dependen de servicios externos
- Validación de queries SQL complejos
- Tests de migraciones de base de datos

### ❌ NO

- Tests unitarios puros (usa mocks)
- CI/CD sin Docker disponible
- Tests muy frecuentes (puede ser lento)
- Desarrollo sin Docker Desktop

## Recursos Adicionales

- [Documentación Testcontainers.NET](https://dotnet.testcontainers.org/)
- [Testcontainers Java (original)](https://www.testcontainers.org/)
- [Docker Hub - Imágenes Oficiales](https://hub.docker.com/)
- [NUnit Documentation](https://docs.nunit.org/)

---

**💡 Tip**: Los contenedores se crean y destruyen automáticamente para cada test, garantizando un entorno limpio y reproducible.
