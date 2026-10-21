using Microsoft.EntityFrameworkCore;
using ProductosEFCore.Entity;
using ProductosEFCore.Models;

namespace ProductosEFCore.Repositories;

public class ProductoRepository(AppDbContext db) : IProductoRepository
{
    public IEnumerable<Producto> GetAll() =>
        db.Productos.AsNoTracking().Include(p => p.Categoria).ToList();

    public IEnumerable<Producto> GetDeleted() =>
        db.Productos.IgnoreQueryFilters()
            .AsNoTracking()
            .Include(p => p.Categoria)
            .Where(p => p.IsDeleted)
            .ToList();

    public Producto? GetById(long id) =>
        db.Productos.AsNoTracking().Include(p => p.Categoria)
            .FirstOrDefault(p => p.Id == id);

    public Producto Add(Producto producto)
    {
        producto.CreatedAt = DateTime.UtcNow;
        db.Productos.Add(producto);
        db.SaveChanges();
        return producto;
    }

    public Producto? Update(Producto producto)
    {
        var existente = db.Productos.Find(producto.Id);
        if (existente is null) return null;

        existente.Nombre = producto.Nombre;
        existente.Precio = producto.Precio;
        existente.Imagen = producto.Imagen;
        existente.CategoriaId = producto.CategoriaId;
        existente.Stock = producto.Stock;
        existente.UpdatedAt = DateTime.UtcNow;

        db.SaveChanges();
        return existente;
    }

    public bool Delete(long id)
    {
        var producto = db.Productos.Find(id);
        if (producto is null) return false;

        producto.IsDeleted = true;
        db.SaveChanges();
        return true;
    }

    public bool Restore(long id)
    {
        var producto = db.Productos.IgnoreQueryFilters()
            .FirstOrDefault(p => p.Id == id && p.IsDeleted);
        if (producto is null) return false;

        producto.IsDeleted = false;
        db.SaveChanges();
        return true;
    }

    public bool IsDuplicated(string nombre, long excludeId = 0) =>
        db.Productos.Any(p =>
            p.Nombre == nombre && p.Id != excludeId);
}
