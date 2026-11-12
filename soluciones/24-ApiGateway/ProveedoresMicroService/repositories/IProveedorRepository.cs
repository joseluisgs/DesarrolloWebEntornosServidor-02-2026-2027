using ProveedoresMicroService.Models;

namespace ProveedoresMicroService.Repositories;

/// <summary>
/// Interfaz del repositorio de Proveedores (acceso a MongoDB).
/// </summary>
public interface IProveedorRepository
{
    Task<List<Proveedor>> GetAllAsync();
    Task<Proveedor?> GetByIdAsync(string id);
}
