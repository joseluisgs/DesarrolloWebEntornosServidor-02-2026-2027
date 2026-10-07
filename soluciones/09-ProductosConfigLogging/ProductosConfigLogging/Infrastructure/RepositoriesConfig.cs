using Microsoft.Extensions.DependencyInjection;
using ProductosConfigLogging.Repositories;

namespace ProductosConfigLogging.Infrastructure;

public static class RepositoriesConfig
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddSingleton<IProductoRepository, ProductoRepository>();
        return services;
    }
}
