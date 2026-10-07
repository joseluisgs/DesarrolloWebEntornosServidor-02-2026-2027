using Microsoft.Extensions.DependencyInjection;
using ProductosResult.Services;

namespace ProductosResult.Infrastructure;

public static class ServicesConfig
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IProductoService, ProductoService>();
        return services;
    }
}
