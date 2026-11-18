using Microsoft.AspNetCore.Mvc;
using ProveedoresMicroService.Repositories;
using Serilog;

namespace ProveedoresMicroService.Controllers;

/// <summary>
/// API de Proveedores (solo lectura).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProveedoresController(IProveedorRepository repository) : ControllerBase
{
    /// <summary>
    /// Obtiene todos los proveedores activos.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        Log.Information("[Proveedores] GET /api/proveedores · consultando MongoDB...");
        var proveedores = await repository.GetAllAsync();
        Log.Information("[Proveedores] ← {Total} proveedores leídos de MongoDB", proveedores.Count);
        return Ok(proveedores);
    }

    /// <summary>
    /// Obtiene un proveedor por su ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        Log.Information("[Proveedores] GET /api/proveedores/{Id} · consultando MongoDB...", id);
        var proveedor = await repository.GetByIdAsync(id);
        if (proveedor is null)
        {
            Log.Information("[Proveedores] ← {Id} no está en MongoDB → 404", id);
            return NotFound(new { error = $"Proveedor '{id}' no encontrado" });
        }

        Log.Information("[Proveedores] ← {Nombre} ({Id}) leído de MongoDB → 200", proveedor.Nombre, id);
        return Ok(proveedor);
    }
}
