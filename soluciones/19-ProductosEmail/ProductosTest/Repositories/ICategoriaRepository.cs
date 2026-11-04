using ProductosEFCore.Models;

namespace ProductosEFCore.Repositories;

public interface ICategoriaRepository
{
    IEnumerable<Categoria> GetAll();
    Categoria? GetById(long id);
    Categoria? GetByNombre(string nombre);
}
