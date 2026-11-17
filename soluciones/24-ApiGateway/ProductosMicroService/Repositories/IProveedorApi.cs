using Refit;
using ProductosMicroService.Repositories;

namespace ProductosMicroService.Repositories;

/// <summary>
/// Interfaz Refit para comunicacion con el microservicio de Proveedores.
/// </summary>
public interface IProveedorApi
{
    /// <summary>
    /// Obtiene todos los proveedores activos.
    /// </summary>
    [Get("/api/proveedores")]
    Task<List<ProveedorReadDto>> GetAll();

    /// <summary>
    /// Obtiene un proveedor por su identificador.
    /// </summary>
    [Get("/api/proveedores/{id}")]
    Task<ProveedorReadDto?> GetById(string id);
}
