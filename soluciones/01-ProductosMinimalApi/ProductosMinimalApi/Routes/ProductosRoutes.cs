using System.Text.Json;
using ProductosMinimalApi.Models;
using ProductosMinimalApi.Repositories;

namespace ProductosMinimalApi.Routes;

/// <summary>
/// Endpoints de productos. El almacenamiento vive en <see cref="IProductoRepository"/>;
/// aquí solo queda el mapa de rutas y la traducción a resultados HTTP.
/// </summary>
public static class ProductosRoutes
{
    public static void MapProductosRoutes(this WebApplication app)
    {
        // El repositorio está registrado como singleton: lo resolvemos una vez al mapear.
        var repo = app.Services.GetRequiredService<IProductoRepository>();

        var g = app.MapGroup("/api/productos").WithTags("Productos");

        // ─── CRUD ─────────────────────────────────────────────

        g.MapGet("/", () => Results.Ok(repo.GetAll()));

        g.MapGet("/{id:long}", (long id) =>
            repo.GetById(id) is { } producto
                ? Results.Ok(producto)
                : Results.NotFound());

        g.MapPost("/", (Producto producto) =>
        {
            var creado = repo.Add(producto);
            return Results.Created($"/api/productos/{creado.Id}", creado);
        });

        g.MapPut("/{id:long}", (long id, Producto producto) =>
            repo.Update(id, producto) is { } actualizado
                ? Results.Ok(actualizado)
                : Results.NotFound());

        g.MapPatch("/{id:long}", (long id, Dictionary<string, object> cambios) =>
        {
            if (cambios.TryGetValue("precio", out var precio) && precio is JsonElement json)
            {
                return repo.UpdatePrecio(id, json.GetDecimal()) is { } actualizado
                    ? Results.Ok(actualizado)
                    : Results.NotFound();
            }

            return Results.BadRequest();
        });

        g.MapDelete("/{id:long}", (long id) =>
            repo.SoftDelete(id)
                ? Results.NoContent()
                : Results.NotFound());

        // ─── Consultas ────────────────────────────────────────

        g.MapGet("/search", (string? nombre) => Results.Ok(repo.Search(nombre)));

        g.MapGet("/categoria/{categoria}", (string categoria) =>
            Results.Ok(repo.GetByCategoria(categoria)));

        g.MapGet("/precio", (decimal? min, decimal? max) =>
            Results.Ok(repo.GetByPrecio(min, max)));

        g.MapGet("/ordenar", (bool? asc) => Results.Ok(repo.GetOrdered(asc ?? true)));

        g.MapGet("/grupo-categoria", () => Results.Ok(repo.GroupByCategoria()));

        g.MapGet("/estadisticas", () => Results.Ok(repo.GetEstadisticas()));
    }
}
