using ProductosMicroService.Models;

namespace ProductosMicroService.Repositories;

/// <summary>
/// Interfaz del repositorio remoto de Proveedores (Refit).
/// Consulta al microservicio de Proveedores via HTTP.
/// </summary>
public interface IProveedorRemoteRepository
{
    Task<ProveedorReadDto?> GetByIdAsync(string id);
}
