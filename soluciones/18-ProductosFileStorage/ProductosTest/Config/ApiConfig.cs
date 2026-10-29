namespace ProductosEFCore.Config;

/// <summary>
/// Configuración de la API.
/// Se lee de la sección "Api" de appsettings.json.
/// </summary>
public class ApiConfig
{
    public string Nombre { get; set; } = "Productos EF Core API";
    public string Mensaje { get; set; } = "CRUD con Entity Framework Core + PostgreSQL";
}
