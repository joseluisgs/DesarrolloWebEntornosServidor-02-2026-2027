namespace ProductosMongo.Config;

/// <summary>
/// Configuración de la API.
/// Se lee de la sección "Api" de appsettings.json.
/// </summary>
public class ApiConfig
{
    public string Nombre { get; set; } = "Productos MongoDB API";
    public string Mensaje { get; set; } = "CRUD con MongoDB";
}
