using Microsoft.EntityFrameworkCore;
using ProductosMicroService.Entity;
using ProductosMicroService.Models;

namespace ProductosMicroService.Repositories;

/// <summary>
/// Implementacion del repositorio de Categorias con EF Core.
/// </summary>
public class CategoriaRepository(AppDbContext context) : ICategoriaRepository
{
    /// <inheritdoc />
    public IEnumerable<Categoria> GetAll() => context.Categorias.ToList();

    /// <inheritdoc />
    public Categoria? GetById(long id) => context.Categorias.Find(id);
}
