using ProductosEFCore.Storage;
using Serilog;

namespace ProductosEFCore.Infrastructures;

public static class StorageConfig
{
    public static IServiceCollection AddStorage(this IServiceCollection services)
    {
        Log.Information("Configurando servicio de almacenamiento...");
        return services.AddScoped<IStorageService, FileSystemStorageService>();
    }
}
