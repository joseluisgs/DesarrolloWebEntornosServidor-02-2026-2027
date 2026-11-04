using ProductosCQRS.Entity.Mongo;
using ProductosCQRS.Models;

namespace ProductosCQRS.Repositories;

public interface IProductoReadRepository
{
    Task<ProductoRead?> GetByIdAsync(long id);
    Task<List<ProductoRead>> GetAllAsync();
    Task UpsertAsync(ProductoRead producto);
    Task DeleteAsync(long id);
}
