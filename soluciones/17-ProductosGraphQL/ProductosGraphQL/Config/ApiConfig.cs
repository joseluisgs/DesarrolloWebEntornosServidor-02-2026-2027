namespace ProductosGraphQL.Config;

/// <summary>
/// Configuración de la API.
/// Se lee de la sección "Api" de appsettings.json.
/// </summary>
public class ApiConfig
{
    public string Nombre { get; set; } = "Productos GraphQL API";
    public string Mensaje { get; set; } = "CRUD con HotChocolate + Entity Framework Core + PostgreSQL";
    public int Puerto { get; set; } = 5017;
}
