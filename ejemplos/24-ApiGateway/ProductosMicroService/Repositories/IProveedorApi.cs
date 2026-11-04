using Refit;
using ProductosMicroService.Repositories;

namespace ProductosMicroService.Repositories;

/// <summary>
/// Interfaz Refit para comunicacion con el microservicio de Proveedores.
/// </summary>
public interface IProveedorApi
{
    [Get("/api/proveedores/{id}")]
    Task<ProveedorReadDto?> GetById(string id);
}
