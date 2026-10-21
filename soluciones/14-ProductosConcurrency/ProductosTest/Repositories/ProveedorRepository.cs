using Microsoft.EntityFrameworkCore;
using ProductosEFCore.Entity;
using ProductosEFCore.Models;

namespace ProductosEFCore.Repositories;

public class ProveedorRepository(AppDbContext db) : IProveedorRepository
{
    public IEnumerable<Proveedor> GetAll() =>
        db.Proveedores.AsNoTracking().ToList();

    public Proveedor? GetById(string id) =>
        db.Proveedores.AsNoTracking()
            .FirstOrDefault(p => p.Id == id);

    public Proveedor Add(Proveedor proveedor)
    {
        proveedor.CreatedAt = DateTime.UtcNow;
        db.Proveedores.Add(proveedor);
        db.SaveChanges();
        return proveedor;
    }

    public Proveedor? Update(Proveedor proveedor)
    {
        var existente = db.Proveedores.Find(proveedor.Id);
        if (existente is null) return null;

        existente.Nombre = proveedor.Nombre;
        existente.Contacto = proveedor.Contacto;
        existente.UpdatedAt = DateTime.UtcNow;

        db.SaveChanges();
        return existente;
    }

    public bool Delete(string id)
    {
        var proveedor = db.Proveedores.Find(id);
        if (proveedor is null) return false;

        proveedor.IsDeleted = true;
        db.SaveChanges();
        return true;
    }

    public bool IsDuplicated(string nombre, string excludeId = "") =>
        db.Proveedores.Any(p =>
            p.Nombre == nombre && p.Id != excludeId);
}
