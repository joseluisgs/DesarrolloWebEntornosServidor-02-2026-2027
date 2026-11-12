using Microsoft.EntityFrameworkCore;
using ProductosCQRS.Entity;
using ProductosCQRS.Models;

namespace ProductosCQRS.Repositories;

public class ProveedorRepository(AppDbContext db) : IProveedorRepository
{
    public IEnumerable<Proveedor> GetAll() =>
        db.Proveedores.AsNoTracking().ToList();

    public Proveedor? GetById(long id) =>
        db.Proveedores.AsNoTracking().FirstOrDefault(p => p.Id == id);

    public Proveedor? GetByNombre(string nombre) =>
        db.Proveedores.AsNoTracking().FirstOrDefault(p => p.Nombre == nombre);
}
