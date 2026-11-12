using ProductosMicroService.Models;

namespace ProductosMicroService.Repositories;

/// <summary>
/// Interfaz del repositorio de Categorias (PostgreSQL).
/// </summary>
public interface ICategoriaRepository
{
    IEnumerable<Categoria> GetAll();
    Categoria? GetById(long id);
}
