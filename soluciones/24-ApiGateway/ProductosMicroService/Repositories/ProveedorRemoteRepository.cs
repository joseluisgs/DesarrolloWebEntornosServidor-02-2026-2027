using ProductosMicroService.Repositories;
using Refit;
using Serilog;

namespace ProductosMicroService.Repositories;

/// <summary>
/// Implementacion del repositorio remoto de Proveedores via Refit.
/// Cada llamada queda registrada en consola: es el salto entre microservicios
/// que se quiere ver en la demostracion.
/// </summary>
public class ProveedorRemoteRepository(IProveedorApi api) : IProveedorRemoteRepository
{
    /// <inheritdoc />
    public async Task<ProveedorReadDto?> GetByIdAsync(string id)
    {
        Log.Information("[Productos] → necesita el proveedor {Id}: llamada HTTP al microservicio de Proveedores", id);

        try
        {
            var proveedor = await api.GetById(id);
            if (proveedor is null)
            {
                Log.Information("[Productos] ← Proveedores no devuelve nada para {Id}", id);
                return null;
            }

            Log.Information("[Productos] ← {Nombre} ({Id}) recuperado de Proveedores", proveedor.Nombre, id);
            return proveedor;
        }
        catch (ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            // Refit convierte el 404 del otro microservicio en ApiException
            Log.Information("[Productos] ← Proveedores responde 404: el proveedor {Id} no existe", id);
            return null;
        }
        catch (ApiException ex)
        {
            Log.Warning("[Productos] ← Proveedores responde {Status} al pedir {Id}: no se puede validar",
                (int)ex.StatusCode, id);
            return null;
        }
        catch (Exception ex)
        {
            // Si el proveedor no existe o hay error de conexion, retornamos null
            Log.Warning("[Productos] ← Proveedores no responde ({Motivo}): no se puede validar el proveedor {Id}",
                ex.Message, id);
            return null;
        }
    }
}
