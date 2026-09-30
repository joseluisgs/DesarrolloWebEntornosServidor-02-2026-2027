using ProductosMinimalApi.Models;

namespace ProductosMinimalApi.Routes;

/// <summary>
/// Almacenamiento en memoria con Dictionary y endpoints como extensiones.
/// </summary>
public static class ProductosRoutes
{
    private static readonly Dictionary<long, Producto> _productos = new();
    private static long _nextId = 1;

    public static void MapProductosRoutes(this WebApplication app)
    {
        var g = app.MapGroup("/api/productos").WithTags("Productos");

        g.MapGet("/", () => Results.Ok(_productos.Values.Where(p => p.IsActivo)));

        g.MapGet("/{id:long}", (long id) =>
            _productos.TryGetValue(id, out var p) && p.IsActivo
                ? Results.Ok(p)
                : Results.NotFound());

        g.MapPost("/", (Producto producto) =>
        {
            producto.Id = _nextId++;
            producto.CreatedAt = DateTime.UtcNow;
            _productos[producto.Id] = producto;
            return Results.Created($"/api/productos/{producto.Id}", producto);
        });

        g.MapPut("/{id:long}", (long id, Producto producto) =>
        {
            if (!_productos.TryGetValue(id, out var existente) || !existente.IsActivo)
                return Results.NotFound();

            existente.Nombre = producto.Nombre;
            existente.Precio = producto.Precio;
            existente.Categoria = producto.Categoria;
            existente.Imagen = producto.Imagen;
            existente.UpdatedAt = DateTime.UtcNow;
            return Results.Ok(existente);
        });

        g.MapPatch("/{id:long}", (long id, Dictionary<string, object> cambios) =>
        {
            if (!_productos.TryGetValue(id, out var existente) || !existente.IsActivo)
                return Results.NotFound();

            if (cambios.TryGetValue("precio", out var precio) && precio is System.Text.Json.JsonElement jsonVal)
            {
                existente.Precio = jsonVal.GetDecimal();
                existente.UpdatedAt = DateTime.UtcNow;
                return Results.Ok(existente);
            }

            return Results.BadRequest();
        });

        g.MapDelete("/{id:long}", (long id) =>
        {
            if (!_productos.TryGetValue(id, out var existente) || !existente.IsActivo)
                return Results.NotFound();

            existente.DeletedAt = DateTime.UtcNow;
            return Results.NoContent();
        });

        // ─── Consultas LINQ ─────────────────────────────────────

        g.MapGet("/search", (string? nombre) =>
        {
            var resultados = _productos.Values
                .Where(p => p.IsActivo)
                .Where(p => p.Nombre.Contains(nombre ?? "", StringComparison.OrdinalIgnoreCase))
                .OrderBy(p => p.Nombre)
                .ToList();
            return Results.Ok(resultados);
        });

        g.MapGet("/categoria/{categoria}", (string categoria) =>
        {
            var resultados = _productos.Values
                .Where(p => p.IsActivo)
                .Where(p => p.Categoria.Equals(categoria, StringComparison.OrdinalIgnoreCase))
                .ToList();
            return Results.Ok(resultados);
        });

        g.MapGet("/precio", (decimal? min, decimal? max) =>
        {
            var resultados = _productos.Values
                .Where(p => p.IsActivo)
                .Where(p => p.Precio >= (min ?? 0) && p.Precio <= (max ?? decimal.MaxValue))
                .OrderBy(p => p.Precio)
                .ToList();
            return Results.Ok(resultados);
        });

        g.MapGet("/ordenar", (bool? asc) =>
        {
            var resultados = (asc ?? true)
                ? _productos.Values.Where(p => p.IsActivo).OrderBy(p => p.Precio).ToList()
                : _productos.Values.Where(p => p.IsActivo).OrderByDescending(p => p.Precio).ToList();
            return Results.Ok(resultados);
        });

        g.MapGet("/grupo-categoria", () =>
        {
            var grupos = _productos.Values
                .Where(p => p.IsActivo)
                .GroupBy(p => p.Categoria)
                .ToDictionary(g => g.Key, g => g.ToList());
            return Results.Ok(grupos);
        });

        g.MapGet("/estadisticas", () =>
        {
            var activos = _productos.Values.Where(p => p.IsActivo).ToList();
            return Results.Ok(new
            {
                Total = activos.Count,
                PrecioMedio = activos.Any() ? activos.Average(p => p.Precio) : 0,
                PorCategoria = activos.GroupBy(p => p.Categoria)
                    .ToDictionary(g => g.Key, g => g.Count())
            });
        });
    }
}
