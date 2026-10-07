using Microsoft.Extensions.DependencyInjection;
using ProductosTest.Repositories;

namespace ProductosTest.Infrastructure;

public static class RepositoriesConfig
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddSingleton<IProductoRepository, ProductoRepository>();
        return services;
    }
}
