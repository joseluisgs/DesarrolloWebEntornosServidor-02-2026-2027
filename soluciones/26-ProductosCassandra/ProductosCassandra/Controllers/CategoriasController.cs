using Microsoft.AspNetCore.Mvc;
using ProductosCassandra.Dtos;
using ProductosCassandra.Services;

namespace ProductosCassandra.Controllers;

/// <summary>
/// Endpoints de categorías. La ruta sale de [Route("api/[controller]")]: /api/Categorias.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CategoriasController(ICategoriaService service) : ControllerBase
{
    /// <summary>Consulta Q3: el catálogo de categorías.</summary>
    /// <returns>Lista de categorías.</returns>
    [HttpGet]
    public async Task<ActionResult<List<CategoriaDto>>> GetAll()
    {
        var categorias = await service.GetAllAsync();
        return Ok(categorias.ToList());
    }

    /// <summary>Una categoría por su id.</summary>
    /// <param name="id">Id de la categoría.</param>
    /// <returns>La categoría, o 404 si no existe.</returns>
    [HttpGet("{id:long}")]
    public async Task<ActionResult<CategoriaDto>> GetById(long id)
    {
        var categoria = await service.GetByIdAsync(id);
        return categoria is not null
            ? Ok(categoria)
            : NotFound(new { message = $"Categoría con ID {id} no encontrada" });
    }

    /// <summary>
    /// Consulta Q1: la consulta estrella de Cassandra. Leer los productos de una
    /// categoría es leer una partición entera, ordenada por la clustering key de serie.
    /// </summary>
    /// <param name="id">Id de la categoría (partition key).</param>
    /// <returns>Los productos de esa categoría.</returns>
    [HttpGet("{id:long}/productos")]
    public async Task<ActionResult<List<ProductoDto>>> GetProductos(long id)
    {
        var productos = await service.GetProductosAsync(id);
        return Ok(productos.ToList());
    }
}
