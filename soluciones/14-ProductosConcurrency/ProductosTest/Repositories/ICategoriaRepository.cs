using ProductosEFCore.Models;

namespace ProductosEFCore.Repositories;

public interface ICategoriaRepository
{
    IEnumerable<Categoria> GetAll();
    Categoria? GetById(Guid id);
    Categoria? GetByNombre(string nombre);
}
