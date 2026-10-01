using ProductosMinimalApiDI.Models;
using ProductosMinimalApiDI.Services;

namespace ProductosMinimalApiDI.Routes;

/// <summary>
/// Rutas de la API de productos usando Inyección de Dependencias.
/// </summary>
public static class ProductosRoutes
{
    /// <summary>
    /// Registra los endpoints de productos.
    /// </summary>
    /// <param name="app">Aplicación web.</param>
    /// <returns>Ruta de la aplicación para encadenamiento.</returns>
    public static void MapProductosRoutes(this WebApplication app)
    {
        var g = app.MapGroup("/api/productos").WithTags("Productos");

        g.MapGet("/", (IProductoService service) =>
            Results.Ok(service.GetAll()));

        g.MapGet("/{id:long}", (long id, IProductoService service) =>
            service.GetById(id) is { } producto
                ? Results.Ok(producto)
                : Results.NotFound());

        g.MapPost("/", (Producto producto, IProductoService service) =>
        {
            try
            {
                var creado = service.Add(producto);
                return Results.Created($"/api/productos/{creado.Id}", creado);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        g.MapPut("/{id:long}", (long id, Producto producto, IProductoService service) =>
        {
            try
            {
                var actualizado = service.Update(id, producto);
                return actualizado is not null
                    ? Results.Ok(actualizado)
                    : Results.NotFound();
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        g.MapPatch("/{id:long}", (long id, Dictionary<string, object> cambios, IProductoService service) =>
        {
            if (!cambios.TryGetValue("precio", out var precio) || precio is not System.Text.Json.JsonElement jsonVal)
                return Results.BadRequest();

            try
            {
                var actualizado = service.PatchPrice(id, jsonVal.GetDecimal());
                return actualizado is not null
                    ? Results.Ok(actualizado)
                    : Results.NotFound();
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        g.MapDelete("/{id:long}", (long id, IProductoService service) =>
            service.Delete(id)
                ? Results.NoContent()
                : Results.NotFound());

        // ─── Consultas LINQ ─────────────────────────────────────

        g.MapGet("/search", (string? nombre, IProductoService service) =>
            Results.Ok(service.Search(nombre)));

        g.MapGet("/categoria/{categoria}", (string categoria, IProductoService service) =>
            Results.Ok(service.FilterByCategoria(categoria)));

        g.MapGet("/precio", (decimal? min, decimal? max, IProductoService service) =>
            Results.Ok(service.FilterByPrecio(min, max)));

        g.MapGet("/ordenar", (bool? asc, IProductoService service) =>
            Results.Ok(service.OrderByPrecio(asc ?? true)));

        g.MapGet("/grupo-categoria", (IProductoService service) =>
            Results.Ok(service.GroupByCategoria()));

        g.MapGet("/estadisticas", (IProductoService service) =>
            Results.Ok(service.GetEstadisticas()));
    }
}
