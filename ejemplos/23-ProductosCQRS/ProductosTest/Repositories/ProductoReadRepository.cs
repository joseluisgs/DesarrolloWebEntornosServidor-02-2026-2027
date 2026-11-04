using Microsoft.EntityFrameworkCore;
using ProductosCQRS.Entity.Mongo;
using ProductosCQRS.Models;

namespace ProductosCQRS.Repositories;

public class ProductoReadRepository(MongoDbContext mongoContext) : IProductoReadRepository
{
    public async Task<ProductoRead?> GetByIdAsync(long id) =>
        await mongoContext.Productos
            .Where(p => p.Id == id)
            .FirstOrDefaultAsync();

    public async Task<List<ProductoRead>> GetAllAsync() =>
        await mongoContext.Productos.ToListAsync();

    public async Task UpsertAsync(ProductoRead producto)
    {
        var existente = await mongoContext.Productos
            .Where(p => p.Id == producto.Id)
            .FirstOrDefaultAsync();

        if (existente is not null)
        {
            existente.Nombre = producto.Nombre;
            existente.Precio = producto.Precio;
            existente.Categoria = producto.Categoria;
            existente.Proveedor = producto.Proveedor;
            existente.SyncAt = DateTime.UtcNow;
        }
        else
        {
            mongoContext.Productos.Add(producto);
        }
        await mongoContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var existente = await mongoContext.Productos
            .Where(p => p.Id == id)
            .FirstOrDefaultAsync();

        if (existente is not null)
        {
            mongoContext.Productos.Remove(existente);
            await mongoContext.SaveChangesAsync();
        }
    }
}
