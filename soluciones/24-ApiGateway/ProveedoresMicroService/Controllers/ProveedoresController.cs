using Microsoft.AspNetCore.Mvc;
using ProveedoresMicroService.Repositories;

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
        var proveedores = await repository.GetAllAsync();
        return Ok(proveedores);
    }

    /// <summary>
    /// Obtiene un proveedor por su ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var proveedor = await repository.GetByIdAsync(id);
        if (proveedor is null)
            return NotFound(new { error = $"Proveedor '{id}' no encontrado" });

        return Ok(proveedor);
    }
}
