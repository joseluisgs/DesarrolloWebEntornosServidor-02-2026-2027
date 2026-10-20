using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;
using ProductosMongo.Models;

namespace ProductosMongo.Entity;

/// <summary>
/// Contexto de Entity Framework Core para MongoDB.
/// Usa el provider MongoDB.EntityFrameworkCore.
/// </summary>
public class ProductosMongoDbContext(DbContextOptions<ProductosMongoDbContext> options) : DbContext(options)
{
    public DbSet<Producto> Productos => Set<Producto>();

    public DbSet<Proveedor> Proveedores => Set<Proveedor>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // MongoDB standalone (docker-compose) no soporta transacciones, pero el
        // provider EF Core las exige en SaveChanges con varios documentos.
        Database.AutoTransactionBehavior = AutoTransactionBehavior.Never;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Dos colecciones: una por tipo de documento. No hay JOINs ni Include().
        modelBuilder.Entity<Producto>().ToCollection("productos");
        modelBuilder.Entity<Proveedor>().ToCollection("proveedores");
    }
}
