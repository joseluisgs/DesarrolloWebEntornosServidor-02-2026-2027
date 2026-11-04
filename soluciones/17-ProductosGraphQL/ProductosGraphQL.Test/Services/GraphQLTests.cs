using System.Security.Claims;
using FluentAssertions;
using HotChocolate;
using HotChocolate.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using ProductosGraphQL.Entity;
using ProductosGraphQL.GraphQL;
using ProductosGraphQL.GraphQL.Resolvers;
using ProductosGraphQL.GraphQL.Types;
using ProductosGraphQL.Repositories;
using ProductosGraphQL.Services;

namespace ProductosGraphQL.Test.Services;

[TestFixture]
public class GraphQLTests : IntegrationTestBase
{
    private IRequestExecutor _executor = null!;
    private ServiceProvider _serviceProvider = null!;

    [SetUp]
    public async Task SetUpGraphQL()
    {
        // Insertar categorias base
        await CreateTestCategoria("Electronica", "Dispositivos electronicos");
        await CreateTestCategoria("Mobiliario", "Muebles");
        await CreateTestCategoria("Instrumentos", "Instrumentos musicales");

        // Configurar HotChocolate via DI para tests
        var services = new ServiceCollection();

        services.AddLogging(builder => builder.AddConsole());
        services.AddDbContextFactory<AppDbContext>(options =>
            options.UseNpgsql(Postgres.GetConnectionString()));

        // Repos y servicios (como en Tienda)
        services.AddScoped<IProductoRepository, ProductoRepository>();
        services.AddScoped<ICategoriaRepository, CategoriaRepository>();
        services.AddScoped<IProductoService, ProductoService>();

        // Politicas de autorizacion (como en Tienda)
        services.AddAuthorizationBuilder()
            .AddPolicy("AdminOnly", p => p.RequireRole("ADMIN"))
            .AddPolicy("RequireAdminRole", p => p.RequireRole("ADMIN"))
            .AddPolicy("RequireUserRole", p => p.RequireRole("USER", "ADMIN"));

        services
            .AddGraphQLServer()
            .AddQueryType<ProductosQuery>()
            .AddMutationType<ProductoMutation>()
            .AddSubscriptionType<Subscription>()
            .AddType<ProductoType>()
            .AddType<CategoriaType>()
            .AddTypeExtension<ProductoResolvers>()
            .AddAuthorization()
            .AddFiltering()
            .AddSorting()
            .AddProjections()
            .AddInMemorySubscriptions();

        _serviceProvider = services.BuildServiceProvider();
        _executor = await _serviceProvider
            .GetRequiredService<IRequestExecutorProvider>()
            .GetExecutorAsync();
    }

    [TearDown]
    public async Task TearDownGraphQL()
    {
        if (_serviceProvider is IAsyncDisposable asyncDisposable)
            await asyncDisposable.DisposeAsync();
        else
            _serviceProvider?.Dispose();
    }

    /// <summary>
    /// Crea un ClaimsPrincipal fake para tests con autorizacion.
    /// </summary>
    private static ClaimsPrincipal CreateAdminPrincipal()
    {
        var identity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Name, "admin"),
            new Claim(ClaimTypes.Role, "ADMIN"),
            new Claim(ClaimTypes.Role, "EDITOR")
        ], "test");
        return new ClaimsPrincipal(identity);
    }

    [TestFixture]
    public class GetProductos : GraphQLTests
    {
        [Test]
        public async Task Query_GetProductos_ReturnsAllProducts()
        {
            // Arrange
            await CreateTestProducto("Teclado", 89.99m, 1);
            await CreateTestProducto("Raton", 35.50m, 1);

            // Act
            var result = await _executor.ExecuteAsync("""
                query {
                    productos {
                        nodes {
                            id
                            nombre
                            precio
                        }
                    }
                }
            """);

            // Assert
            var json = result.ToJson();
            json.Should().Contain("Teclado");
            json.Should().Contain("Raton");
        }

        [Test]
        public async Task Query_GetProductos_WithFilter_ReturnsFilteredResults()
        {
            // Arrange
            await CreateTestProducto("Teclado", 89.99m, 1);
            await CreateTestProducto("Raton", 35.50m, 1);

            // Act
            var result = await _executor.ExecuteAsync("""
                query {
                    productos(where: { precio: { gt: 50 } }) {
                        nodes {
                            id
                            nombre
                            precio
                        }
                    }
                }
            """);

            // Assert
            var json = result.ToJson();
            json.Should().Contain("Teclado");
            json.Should().NotContain("Raton");
        }

        [Test]
        public async Task Query_GetProductoById_ReturnsProduct()
        {
            // Arrange
            var creado = await CreateTestProducto("Monitor", 299.00m, 1);

            // Act
            var result = await _executor.ExecuteAsync($$"""
                query {
                    producto(id: {{creado.Id}}) {
                        id
                        nombre
                        precio
                    }
                }
            """);

            // Assert
            var json = result.ToJson();
            json.Should().Contain("Monitor");
            json.Should().Contain("299");
        }
    }

    [TestFixture]
    public class GetCategorias : GraphQLTests
    {
        [Test]
        public async Task Query_GetCategorias_ReturnsAllCategories()
        {
            // Act
            var result = await _executor.ExecuteAsync("""
                query {
                    categorias {
                        nodes {
                            id
                            nombre
                        }
                    }
                }
            """);

            // Assert
            var json = result.ToJson();
            json.Should().Contain("Electronica");
            json.Should().Contain("Mobiliario");
            json.Should().Contain("Instrumentos");
        }
    }

    [TestFixture]
    public class Mutations : GraphQLTests
    {
        [Test]
        public async Task Mutation_CrearProducto_ReturnsNewProduct()
        {
            // Arrange - crear request con usuario ADMIN
            var request = OperationRequestBuilder.New()
                .SetDocument("""
                    mutation {
                        crearProducto(input: {
                            nombre: "Batman"
                            precio: 29.99
                            categoriaId: 1
                        }) {
                            id
                            nombre
                            precio
                        }
                    }
                """)
                .AddGlobalState(nameof(ClaimsPrincipal), CreateAdminPrincipal())
                .Build();

            // Act
            var result = await _executor.ExecuteAsync(request);

            // Assert
            var json = result.ToJson();
            json.Should().Contain("Batman");
            json.Should().Contain("29.99");
        }

        [Test]
        public async Task Mutation_EliminarProducto_ReturnsTrue()
        {
            // Arrange
            var creado = await CreateTestProducto("Para Eliminar", 10.00m, 1);

            var request = OperationRequestBuilder.New()
                .SetDocument($$"""
                    mutation {
                        eliminarProducto(id: {{creado.Id}})
                    }
                """)
                .AddGlobalState(nameof(ClaimsPrincipal), CreateAdminPrincipal())
                .Build();

            // Act
            var result = await _executor.ExecuteAsync(request);

            // Assert
            var json = result.ToJson();
            json.Should().Contain("true");
        }

        [Test]
        public async Task Mutation_EliminarProducto_NoExiste_ReturnsFalse()
        {
            // Arrange
            var request = OperationRequestBuilder.New()
                .SetDocument("""
                    mutation {
                        eliminarProducto(id: 999)
                    }
                """)
                .AddGlobalState(nameof(ClaimsPrincipal), CreateAdminPrincipal())
                .Build();

            // Act
            var result = await _executor.ExecuteAsync(request);

            // Assert
            var json = result.ToJson();
            json.Should().Contain("false");
        }
    }

    [TestFixture]
    public class SeedData : GraphQLTests
    {
        [Test]
        public async Task Seed_CategoriasInsertadas_Retorna3()
        {
            // Act
            var categorias = await Db.Categorias.ToListAsync();

            // Assert
            categorias.Should().HaveCount(3);
        }
    }
}
