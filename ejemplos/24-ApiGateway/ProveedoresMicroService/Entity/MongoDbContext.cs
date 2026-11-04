using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;
using ProveedoresMicroService.Models;

namespace ProveedoresMicroService.Entity;

/// <summary>
/// Contexto de base de datos MongoDB para Proveedores.
/// </summary>
public class MongoDbContext : DbContext
{
    public DbSet<Proveedor> Proveedores => Set<Proveedor>();

    public MongoDbContext(DbContextOptions<MongoDbContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Mongo standalone (docker compose) no soporta transacciones
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseMongoDB("mongodb://localhost:27020/proveedores_db");
        }
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Proveedor>().ToCollection("proveedores");
        // Sin transacciones: el servidor standalone de Mongo no las soporta
        Database.AutoTransactionBehavior = Microsoft.EntityFrameworkCore.AutoTransactionBehavior.Never;
    }
}
