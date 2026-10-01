using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using ProductosControllersApi.Models;
using ProductosControllersApi.Repositories;

namespace ProductosControllersApi.Controllers;

/// <summary>
/// CRUD y consultas de productos delegando en <see cref="IProductoRepository"/>.
/// </summary>
/// <remarks>
/// El repositorio se crea aquí mismo, <b>sin inyección de dependencias</b>, y es <see langword="static"/>:
/// los controladores se instancian en cada petición, y el <c>Dictionary</c> debe ser único para
/// compartirse entre todas las peticiones.
/// </remarks>
[ApiController]
[Route("api/[controller]")]
public class ProductosController : ControllerBase
{
    private static readonly IProductoRepository Repo = new ProductoRepository();

    /// <summary>Devuelve todos los productos activos.</summary>
    [HttpGet]
    public ActionResult<List<Producto>> GetAll() => Ok(Repo.GetAll());

    /// <summary>Devuelve un producto por su identificador.</summary>
    [HttpGet("{id:long}")]
    public ActionResult<Producto> GetById(long id) =>
        Repo.GetById(id) is { } producto ? Ok(producto) : NotFound();

    /// <summary>Crea un producto y responde con la localización del nuevo recurso.</summary>
    [HttpPost]
    public ActionResult<Producto> Create(Producto producto)
    {
        var creado = Repo.Add(producto);
        return CreatedAtAction(nameof(GetById), new { id = creado.Id }, creado);
    }

    /// <summary>Sustituye los datos de un producto existente.</summary>
    [HttpPut("{id:long}")]
    public ActionResult<Producto> Update(long id, Producto producto) =>
        Repo.Update(id, producto) is { } actualizado ? Ok(actualizado) : NotFound();

    /// <summary>Actualiza el precio de un producto a partir de <c>{ "precio": number }</c>.</summary>
    [HttpPatch("{id:long}")]
    public ActionResult<Producto> Patch(long id, Dictionary<string, object> cambios)
    {
        if (cambios.TryGetValue("precio", out var precio) && precio is JsonElement json)
        {
            return Repo.UpdatePrecio(id, json.GetDecimal()) is { } actualizado
                ? Ok(actualizado)
                : NotFound();
        }

        return BadRequest();
    }

    /// <summary>Elimina lógicamente un producto (fija <c>DeletedAt</c>).</summary>
    [HttpDelete("{id:long}")]
    public ActionResult Delete(long id) =>
        Repo.SoftDelete(id) ? NoContent() : NotFound();

    // ─── Consultas ─────────────────────────────────────────────

    /// <summary>Busca productos cuyo nombre contenga <paramref name="nombre"/>.</summary>
    [HttpGet("search")]
    public ActionResult<List<Producto>> Search([FromQuery] string? nombre) =>
        Ok(Repo.Search(nombre));

    /// <summary>Devuelve los productos de una categoría.</summary>
    [HttpGet("categoria/{categoria}")]
    public ActionResult<List<Producto>> FilterByCategoria(string categoria) =>
        Ok(Repo.GetByCategoria(categoria));

    /// <summary>Devuelve los productos con precio entre <paramref name="min"/> y <paramref name="max"/>.</summary>
    [HttpGet("precio")]
    public ActionResult<List<Producto>> FilterByPrecio([FromQuery] decimal? min, [FromQuery] decimal? max) =>
        Ok(Repo.GetByPrecio(min, max));

    /// <summary>Devuelve los productos ordenados por precio.</summary>
    [HttpGet("ordenar")]
    public ActionResult<List<Producto>> OrderByPrecio([FromQuery] bool? asc) =>
        Ok(Repo.GetOrdered(asc ?? true));

    /// <summary>Devuelve los productos agrupados por categoría.</summary>
    [HttpGet("grupo-categoria")]
    public ActionResult<Dictionary<string, List<Producto>>> GroupByCategoria() =>
        Ok(Repo.GroupByCategoria());

    /// <summary>Devuelve total, precio medio y desglose por categoría.</summary>
    [HttpGet("estadisticas")]
    public ActionResult<ProductoEstadisticas> GetEstadisticas() =>
        Ok(Repo.GetEstadisticas());
}
