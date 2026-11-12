using Microsoft.EntityFrameworkCore;
using ProveedoresMicroService.Entity;
using ProveedoresMicroService.Models;

namespace ProveedoresMicroService.Repositories;

/// <summary>
/// Implementación del repositorio de Proveedores con MongoDB.
/// </summary>
public class ProveedorRepository(MongoDbContext context) : IProveedorRepository
{
    /// <inheritdoc />
    public async Task<List<Proveedor>> GetAllAsync() =>
        await context.Proveedores.Where(p => p.Activo).ToListAsync();

    /// <inheritdoc />
    public async Task<Proveedor?> GetByIdAsync(string id) =>
        await context.Proveedores.FirstOrDefaultAsync(p => p.Id == id);
}
