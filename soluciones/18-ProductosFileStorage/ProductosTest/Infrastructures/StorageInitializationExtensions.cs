using Microsoft.AspNetCore.Builder;
using Serilog;

namespace ProductosEFCore.Infrastructures;

public static class StorageInitializationExtensions
{
    public static void InitializeStorage(this WebApplication app, bool isDevelopment)
    {
        var rootPath = app.Environment.WebRootPath ?? app.Environment.ContentRootPath;
        var storagePath = System.IO.Path.Combine(rootPath, "uploads");
        var storageDirectory = new System.IO.DirectoryInfo(storagePath);

        if (isDevelopment)
        {
            Log.Information("[DESARROLLO] Preparando directorio de almacenamiento: {Path}", storagePath);
            try
            {
                if (storageDirectory.Exists)
                {
                    foreach (var file in storageDirectory.GetFiles()) file.Delete();
                    foreach (var dir in storageDirectory.GetDirectories()) dir.Delete(true);
                    Log.Information("Contenido del directorio borrado");
                }
                if (!storageDirectory.Exists) storageDirectory.Create();
            }
            catch (Exception ex) { Log.Error(ex, "Error al preparar directorio de almacenamiento"); }
        }
        else
        {
            Log.Information("[PRODUCCION] Verificando directorio de almacenamiento: {Path}", storagePath);
            try { if (!storageDirectory.Exists) storageDirectory.Create(); }
            catch (Exception ex) { Log.Error(ex, "Error al verificar directorio de almacenamiento"); }
        }
    }
}
