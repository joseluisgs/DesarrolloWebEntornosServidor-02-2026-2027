using Microsoft.Extensions.DependencyInjection;
using ProductosCassandra.Repositories;
using ProductosCassandra.Repositories.Cassandra;
using Serilog;

namespace ProductosCassandra.Infrastructures;

/// <summary>
/// Configuración de repositorios.
/// </summary>
public static class RepositoriesConfig
{
    /// <summary>
    /// Registra los repositorios en el contenedor de dependencias.
    /// </summary>
    /// <param name="services">Colección de servicios.</param>
    /// <returns>IServiceCollection para encadenar.</returns>
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        // Sin ORM: no hay proveedor de EF Core mantenido para Cassandra.
        // El driver de DataStax es la vía oficial, como el driver nativo de MongoDB en el tema 13.
        Log.Information("Registrando repositorios: Categoria y Producto sobre Cassandra");

        services.AddScoped<ICategoriaRepository, CategoriaCassandraRepository>();
        services.AddScoped<IProductoRepository, ProductoCassandraRepository>();

        Log.Information("Repositorios registrados correctamente.");
        return services;
    }
}
