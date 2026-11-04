using Microsoft.EntityFrameworkCore;
using ProductosGraphQL.Entity;
using ProductosGraphQL.Models;

namespace ProductosGraphQL.Repositories;

public class CategoriaRepository(AppDbContext db) : ICategoriaRepository
{
    public IEnumerable<Categoria> GetAll() =>
        db.Categorias.AsNoTracking().ToList();

    public Categoria? GetById(long id) =>
        db.Categorias.AsNoTracking().FirstOrDefault(c => c.Id == id);

    public Categoria? GetByNombre(string nombre) =>
        db.Categorias.AsNoTracking().FirstOrDefault(c => c.Nombre == nombre);
}
