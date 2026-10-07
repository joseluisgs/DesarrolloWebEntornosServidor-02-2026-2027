using Microsoft.Extensions.DependencyInjection;
using ProductosTest.Services;

namespace ProductosTest.Infrastructure;

public static class ServicesConfig
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IProductoService, ProductoService>();
        return services;
    }
}
