namespace ProductosTest.Config;

/// <summary>
/// Configuración de la API.
/// Se lee de la sección "Api" de appsettings.json.
/// Permite cambiar nombre y mensaje según el entorno.
/// </summary>
public class ApiConfig
{
    public string Nombre { get; set; } = "Productos API";
    public string Mensaje { get; set; } = "Modo producción";
}
