namespace ProductosMongo.Config;

/// <summary>
/// Configuración de conexión a MongoDB.
/// Se lee de la sección "MongoDbSettings" de appsettings.json.
/// </summary>
public class MongoDbSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
}
