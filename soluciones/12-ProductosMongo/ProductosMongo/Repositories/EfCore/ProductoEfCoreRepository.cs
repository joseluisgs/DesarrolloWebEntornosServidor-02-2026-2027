using System.Globalization;
using Microsoft.EntityFrameworkCore;
using ProductosMongo.Entity;
using ProductosMongo.Models;

namespace ProductosMongo.Repositories.EfCore;

/// <summary>
/// Implementación del repositorio usando Entity Framework Core con MongoDB.
/// Usa ProductosMongoDbContext y SaveChanges para persistencia.
/// </summary>
public class ProductoEfCoreRepository(ProductosMongoDbContext context) : IProductoRepository
{
    public IEnumerable<Producto> GetAll() =>
        context.Productos.Where(p => !p.IsDeleted).ToList();

    public Producto? GetById(string id) =>
        context.Productos.FirstOrDefault(p => p.Id.ToString() == id && !p.IsDeleted);

    public Producto Add(Producto producto)
    {
        producto.CreatedAt = DateTime.UtcNow;
        producto.IsDeleted = false;
        context.Productos.Add(producto);
        context.SaveChanges();
        return producto;
    }

    public Producto? Update(Producto producto)
    {
        producto.UpdatedAt = DateTime.UtcNow;
        context.Productos.Update(producto);
        var affected = context.SaveChanges();
        return affected > 0 ? producto : null;
    }

    public bool Delete(string id)
    {
        var producto = context.Productos.FirstOrDefault(p => p.Id.ToString() == id && !p.IsDeleted);
        if (producto is null) return false;

        producto.IsDeleted = true;
        producto.UpdatedAt = DateTime.UtcNow;
        var affected = context.SaveChanges();
        return affected > 0;
    }

    public bool IsDuplicated(string nombre, string excludeId = "")
    {
        var query = context.Productos.Where(p => p.Nombre == nombre && !p.IsDeleted);

        if (!string.IsNullOrEmpty(excludeId))
            query = query.Where(p => p.Id.ToString() != excludeId);

        return query.Any();
    }

    public IEnumerable<Producto> Search(string query)
    {
        var lowerQuery = query.ToLower(CultureInfo.CurrentCulture);
        return context.Productos
            .Where(p => !p.IsDeleted &&
                (p.Nombre.ToLower(CultureInfo.CurrentCulture).Contains(lowerQuery) ||
                 p.Categoria.Nombre.ToLower(CultureInfo.CurrentCulture).Contains(lowerQuery)))
            .ToList();
    }

    public IEnumerable<Producto> Filter(string? nombre, decimal? precioMin, decimal? precioMax)
    {
        var query = context.Productos.Where(p => !p.IsDeleted);

        if (!string.IsNullOrWhiteSpace(nombre))
        {
            var lowerNombre = nombre.ToLower(CultureInfo.CurrentCulture);
            query = query.Where(p => p.Nombre.ToLower(CultureInfo.CurrentCulture).Contains(lowerNombre));
        }
        if (precioMin.HasValue)
            query = query.Where(p => p.Precio >= precioMin.Value);
        if (precioMax.HasValue)
            query = query.Where(p => p.Precio <= precioMax.Value);

        return query.ToList();
    }

    public IEnumerable<Producto> GetDeleted()
    {
        return context.Productos.Where(p => p.IsDeleted).ToList();
    }
}
