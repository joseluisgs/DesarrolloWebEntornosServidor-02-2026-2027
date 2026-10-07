using Microsoft.Extensions.DependencyInjection;
using ProductosConfigLogging.Services;

namespace ProductosConfigLogging.Infrastructure;

public static class ServicesConfig
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IProductoService, ProductoService>();
        return services;
    }
}
