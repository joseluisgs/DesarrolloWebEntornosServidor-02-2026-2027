using Microsoft.EntityFrameworkCore;
using ProductosMicroService.Entity;
using ProductosMicroService.Models;

namespace ProductosMicroService.Repositories;

/// <summary>
/// Implementacion del repositorio de Productos con EF Core (PostgreSQL).
/// </summary>
public class ProductoRepository(AppDbContext context) : IProductoRepository
{
    /// <inheritdoc />
    public IEnumerable<Producto> GetAll() =>
        context.Productos.Include(p => p.Categoria).AsNoTracking().ToList();

    /// <inheritdoc />
    public Producto? GetById(long id) =>
        context.Productos.Include(p => p.Categoria).FirstOrDefault(p => p.Id == id);

    /// <inheritdoc />
    public Producto Add(Producto producto)
    {
        context.Productos.Add(producto);
        context.SaveChanges();
        return producto;
    }

    /// <inheritdoc />
    public Producto? Update(Producto producto)
    {
        var existente = context.Productos.Find(producto.Id);
        if (existente is null) return null;

        existente.Nombre = producto.Nombre;
        existente.Precio = producto.Precio;
        existente.CategoriaId = producto.CategoriaId;
        existente.ProveedorId = producto.ProveedorId;
        existente.Imagen = producto.Imagen;
        context.SaveChanges();
        return existente;
    }

    /// <inheritdoc />
    public bool Delete(long id)
    {
        var existente = context.Productos.Find(id);
        if (existente is null) return false;

        context.Productos.Remove(existente);
        context.SaveChanges();
        return true;
    }

    /// <inheritdoc />
    public bool IsDuplicated(string nombre, long excludeId = 0) =>
        context.Productos.Any(p => p.Nombre == nombre && p.Id != excludeId);

    /// <inheritdoc />
    public async Task LoadRelationsAsync(Producto producto)
    {
        await context.Entry(producto)
            .Reference(p => p.Categoria)
            .LoadAsync();
    }
}
