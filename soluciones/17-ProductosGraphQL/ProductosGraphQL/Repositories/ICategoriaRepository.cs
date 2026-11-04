using ProductosGraphQL.Models;

namespace ProductosGraphQL.Repositories;

public interface ICategoriaRepository
{
    IEnumerable<Categoria> GetAll();
    Categoria? GetById(long id);
    Categoria? GetByNombre(string nombre);
}
