using ProductosMicroService.Repositories;

namespace ProductosMicroService.Repositories;

/// <summary>
/// Implementacion del repositorio remoto de Proveedores via Refit.
/// </summary>
public class ProveedorRemoteRepository(IProveedorApi api) : IProveedorRemoteRepository
{
    /// <inheritdoc />
    public async Task<ProveedorReadDto?> GetByIdAsync(string id)
    {
        try
        {
            return await api.GetById(id);
        }
        catch
        {
            // Si el proveedor no existe o hay error de conexion, retornamos null
            return null;
        }
    }
}
