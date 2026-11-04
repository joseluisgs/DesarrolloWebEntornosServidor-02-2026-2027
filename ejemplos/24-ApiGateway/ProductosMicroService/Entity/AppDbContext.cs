using Microsoft.EntityFrameworkCore;
using ProductosMicroService.Models;

namespace ProductosMicroService.Entity;

/// <summary>
/// Contexto de base de datos PostgreSQL para Productos.
/// </summary>
public class AppDbContext : DbContext
{
    public DbSet<Producto> Productos => Set<Producto>();

    public DbSet<Categoria> Categorias => Set<Categoria>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Precio).HasPrecision(18, 2);
            entity.HasOne(e => e.Categoria)
                  .WithMany()
                  .HasForeignKey(e => e.CategoriaId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).HasMaxLength(100).IsRequired();
        });
    }
}
