using Microsoft.EntityFrameworkCore;
using ProductosEFCore.IdGenerators;
using ProductosEFCore.Models;

namespace ProductosEFCore.Entity;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Proveedor> Proveedores => Set<Proveedor>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ── Fluent API ────────────────────────────────────────────────────────
        // Solo para relaciones, índices, precisión y query filters.
        // Los atributos de validación van en las entidades (Data Annotations).

        modelBuilder.Entity<Producto>(entity =>
        {
            // Relación N:1 → cada Producto pertenece a una Categoría
            entity.HasOne(p => p.Categoria)
                  .WithMany(c => c.Productos)
                  .HasForeignKey(p => p.CategoriaId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Precisión de Precio: 18 dígitos, 2 decimales
            entity.Property(p => p.Precio).HasPrecision(18, 2);

            // Concurrency token
            entity.Property(p => p.RowVersion).IsRowVersion();

            // Índices
            entity.HasIndex(p => p.Nombre);
            entity.HasIndex(p => p.CategoriaId);

            // Query filter: solo productos no borrados lógicamente
            entity.HasQueryFilter(p => !p.IsDeleted);
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.HasIndex(c => c.Nombre).IsUnique();
        });

        modelBuilder.Entity<Proveedor>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Id).HasMaxLength(11);

            entity.HasIndex(p => p.Nombre).IsUnique();
            entity.HasQueryFilter(p => !p.IsDeleted);
        });

        // ── Seed Data ─────────────────────────────────────────────────────────
        var cat1 = Guid.NewGuid();
        var cat2 = Guid.NewGuid();
        var cat3 = Guid.NewGuid();
        var cat4 = Guid.NewGuid();
        var cat5 = Guid.NewGuid();

        modelBuilder.Entity<Categoria>().HasData(
            new Categoria { Id = cat1, Nombre = "Electrónica", Descripcion = "Dispositivos y componentes electrónicos" },
            new Categoria { Id = cat2, Nombre = "Mobiliario", Descripcion = "Muebles para oficina y hogar" },
            new Categoria { Id = cat3, Nombre = "Instrumentos", Descripcion = "Instrumentos musicales" },
            new Categoria { Id = cat4, Nombre = "Deportes", Descripcion = "Material deportivo" },
            new Categoria { Id = cat5, Nombre = "Libros", Descripcion = "Libros y publicaciones" }
        );

        modelBuilder.Entity<Producto>().HasData(
            new Producto { Id = 1, Nombre = "Teclado Mecánico", Precio = 89.99m, Imagen = "https://picsum.photos/seed/teclado/300", Stock = 50, CategoriaId = cat1, CreatedAt = DateTime.UtcNow },
            new Producto { Id = 2, Nombre = "Ratón Inalámbrico", Precio = 35.50m, Imagen = "https://picsum.photos/seed/raton/300", Stock = 120, CategoriaId = cat1, CreatedAt = DateTime.UtcNow },
            new Producto { Id = 3, Nombre = "Monitor 27\"", Precio = 299.00m, Imagen = "https://picsum.photos/seed/monitor/300", Stock = 30, CategoriaId = cat1, CreatedAt = DateTime.UtcNow },
            new Producto { Id = 4, Nombre = "Silla Ergonómica", Precio = 249.99m, Imagen = "https://picsum.photos/seed/silla/300", Stock = 15, CategoriaId = cat2, CreatedAt = DateTime.UtcNow },
            new Producto { Id = 5, Nombre = "Escritorio Gaming", Precio = 189.00m, Imagen = "https://picsum.photos/seed/escritorio/300", Stock = 20, CategoriaId = cat2, CreatedAt = DateTime.UtcNow },
            new Producto { Id = 6, Nombre = "Guitarra Acústica", Precio = 159.99m, Imagen = "https://picsum.photos/seed/guitarra/300", Stock = 25, CategoriaId = cat3, CreatedAt = DateTime.UtcNow },
            new Producto { Id = 7, Nombre = "Teclado MIDI", Precio = 120.00m, Imagen = "https://picsum.photos/seed/midi/300", Stock = 40, CategoriaId = cat3, CreatedAt = DateTime.UtcNow },
            new Producto { Id = 8, Nombre = "Balón de Fútbol", Precio = 25.99m, Imagen = "https://picsum.photos/seed/balon/300", Stock = 100, CategoriaId = cat4, CreatedAt = DateTime.UtcNow },
            new Producto { Id = 9, Nombre = "Raqueta de Tenis", Precio = 79.00m, Imagen = "https://picsum.photos/seed/raqueta/300", Stock = 35, CategoriaId = cat4, CreatedAt = DateTime.UtcNow },
            new Producto { Id = 10, Nombre = "El Arte de Programar", Precio = 39.99m, Imagen = "https://picsum.photos/seed/libro/300", Stock = 60, CategoriaId = cat5, CreatedAt = DateTime.UtcNow }
        );

        base.OnModelCreating(modelBuilder);
    }

    private void GenerateProveedorIds()
    {
        foreach (var entry in ChangeTracker.Entries<Proveedor>()
            .Where(e => e.State == EntityState.Added))
        {
            if (string.IsNullOrEmpty(entry.Entity.Id))
            {
                entry.Entity.Id = new YouTubeIdValueGenerator().Next(entry);
            }
        }
    }

    public override int SaveChanges()
    {
        GenerateProveedorIds();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        GenerateProveedorIds();
        return base.SaveChangesAsync(cancellationToken);
    }
}
