using Microsoft.Extensions.DependencyInjection;
using ProductosAvanzados.Repositories;

namespace ProductosAvanzados.Infrastructure;

public static class RepositoriesConfig
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddSingleton<IProductoRepository, ProductoRepository>();
        return services;
    }
}
