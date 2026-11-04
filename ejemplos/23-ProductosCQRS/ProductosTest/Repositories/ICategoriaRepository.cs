using ProductosCQRS.Entity.Mongo;
using ProductosCQRS.Models;

namespace ProductosCQRS.Repositories;

public interface ICategoriaRepository
{
    IEnumerable<Categoria> GetAll();
    Categoria? GetById(long id);
    Categoria? GetByNombre(string nombre);
}
