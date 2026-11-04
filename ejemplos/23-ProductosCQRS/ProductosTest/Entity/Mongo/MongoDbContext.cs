using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;
using ProductosCQRS.Models;

namespace ProductosCQRS.Entity.Mongo;

public class MongoDbContext : DbContext
{
    public DbSet<ProductoRead> Productos => Set<ProductoRead>();

    public MongoDbContext(DbContextOptions<MongoDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductoRead>().ToCollection("productos_read");
    }
}
