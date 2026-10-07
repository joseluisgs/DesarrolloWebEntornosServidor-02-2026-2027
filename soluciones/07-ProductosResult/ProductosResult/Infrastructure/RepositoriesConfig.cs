using Microsoft.Extensions.DependencyInjection;
using ProductosResult.Repositories;

namespace ProductosResult.Infrastructure;

public static class RepositoriesConfig
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddSingleton<IProductoRepository, ProductoRepository>();
        return services;
    }
}
