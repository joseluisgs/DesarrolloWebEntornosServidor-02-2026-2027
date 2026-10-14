using Microsoft.EntityFrameworkCore;
using ProductosEFCore.Models;

namespace ProductosEFCore.Entity;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Categoria> Categorias => Set<Categoria>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ── Fluent API ────────────────────────────────────────────────────────
        // REGLA: si una anotación y la fluent API se contradicen, MANDA LA FLUENT
        // API, porque es la última en configurar el modelo. Por eso aquí solo vive
        // lo que la entidad no sabe decir: relaciones, índices, precisión, query
        // filters, longitudes y CHECK. Lo básico va en la entidad (Data Annotations,
        // que EFCore.CheckConstraints traduce a CHECK en DatabaseConfig) y la
        // validación de entrada en los DTOs (punto 08). Nada se escribe dos veces.

        modelBuilder.Entity<Producto>(entity =>
        {
            // Relación N:1 → cada Producto pertenece a una Categoría
            entity.HasOne(p => p.Categoria)
                  .WithMany(c => c.Productos)
                  .HasForeignKey(p => p.CategoriaId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Precisión de Precio: 18 dígitos, 2 decimales
            entity.Property(p => p.Precio).HasPrecision(18, 2);

            // Longitud máxima de Imagen: aquí vive porque [StringLength] sin mínimo
            // solo produciría un CHECK siempre verdadero (LENGTH >= 0)
            entity.Property(p => p.Imagen).HasMaxLength(500);

            // Garantía final: la base de datos rechaza ella misma el precio
            // negativo (o nulo, aunque venga SQL escrito a mano).
            // Es CHECK de fluent API y no del paquete EFCore.CheckConstraints
            // porque [Range] sobre decimal no genera CHECK: el atributo lleva
            // dobles y la propiedad es decimal, y el paquete exige tipos idénticos.
            // En PostgreSQL el identificador se delimita con comillas dobles.
            entity.ToTable(t => t.HasCheckConstraint(
                "CK_Productos_Precio", "\"Precio\" >= 0.01"));

            // Índices
            entity.HasIndex(p => p.Nombre);
            entity.HasIndex(p => p.CategoriaId);

            // Query filter: solo productos no borrados lógicamente
            entity.HasQueryFilter(p => !p.IsDeleted);
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            // Longitud máxima de Descripcion (mismo motivo que Imagen)
            entity.Property(c => c.Descripcion).HasMaxLength(200);

            entity.HasIndex(c => c.Nombre).IsUnique();
        });

        // ── Seed Data ─────────────────────────────────────────────────────────
        modelBuilder.Entity<Categoria>().HasData(
            new Categoria { Id = 1, Nombre = "Electrónica", Descripcion = "Dispositivos y componentes electrónicos" },
            new Categoria { Id = 2, Nombre = "Mobiliario", Descripcion = "Muebles para oficina y hogar" },
            new Categoria { Id = 3, Nombre = "Instrumentos", Descripcion = "Instrumentos musicales" },
            new Categoria { Id = 4, Nombre = "Deportes", Descripcion = "Material deportivo" },
            new Categoria { Id = 5, Nombre = "Libros", Descripcion = "Libros y publicaciones" }
        );

        modelBuilder.Entity<Producto>().HasData(
            new Producto { Id = 1, Nombre = "Teclado Mecánico", Precio = 89.99m, Imagen = "https://picsum.photos/seed/teclado/300", CategoriaId = 1, CreatedAt = DateTime.UtcNow },
            new Producto { Id = 2, Nombre = "Ratón Inalámbrico", Precio = 35.50m, Imagen = "https://picsum.photos/seed/raton/300", CategoriaId = 1, CreatedAt = DateTime.UtcNow },
            new Producto { Id = 3, Nombre = "Monitor 27\"", Precio = 299.00m, Imagen = "https://picsum.photos/seed/monitor/300", CategoriaId = 1, CreatedAt = DateTime.UtcNow },
            new Producto { Id = 4, Nombre = "Silla Ergonómica", Precio = 249.99m, Imagen = "https://picsum.photos/seed/silla/300", CategoriaId = 2, CreatedAt = DateTime.UtcNow },
            new Producto { Id = 5, Nombre = "Escritorio Gaming", Precio = 189.00m, Imagen = "https://picsum.photos/seed/escritorio/300", CategoriaId = 2, CreatedAt = DateTime.UtcNow },
            new Producto { Id = 6, Nombre = "Guitarra Acústica", Precio = 159.99m, Imagen = "https://picsum.photos/seed/guitarra/300", CategoriaId = 3, CreatedAt = DateTime.UtcNow },
            new Producto { Id = 7, Nombre = "Teclado MIDI", Precio = 120.00m, Imagen = "https://picsum.photos/seed/midi/300", CategoriaId = 3, CreatedAt = DateTime.UtcNow },
            new Producto { Id = 8, Nombre = "Balón de Fútbol", Precio = 25.99m, Imagen = "https://picsum.photos/seed/balon/300", CategoriaId = 4, CreatedAt = DateTime.UtcNow },
            new Producto { Id = 9, Nombre = "Raqueta de Tenis", Precio = 79.00m, Imagen = "https://picsum.photos/seed/raqueta/300", CategoriaId = 4, CreatedAt = DateTime.UtcNow },
            new Producto { Id = 10, Nombre = "El Arte de Programar", Precio = 39.99m, Imagen = "https://picsum.photos/seed/libro/300", CategoriaId = 5, CreatedAt = DateTime.UtcNow }
        );
    }
}
