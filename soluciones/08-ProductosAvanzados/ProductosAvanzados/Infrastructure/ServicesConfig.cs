using Microsoft.Extensions.DependencyInjection;
using ProductosAvanzados.Services;

namespace ProductosAvanzados.Infrastructure;

public static class ServicesConfig
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IProductoService, ProductoService>();
        return services;
    }
}
