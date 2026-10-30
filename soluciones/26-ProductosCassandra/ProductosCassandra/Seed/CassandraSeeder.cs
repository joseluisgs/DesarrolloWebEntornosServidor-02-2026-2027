using Cassandra;
using Cassandra.Data.Linq;
using ProductosCassandra.Models;
using ISession = Cassandra.ISession;

namespace ProductosCassandra.Seed;

/// <summary>
/// Datos de ejemplo para desarrollo: 2 categorias y 5 productos, repartidos
/// en las dos tablas con el nombre de categoria copiado en cada fila.
/// </summary>
public static class CassandraSeeder
{
    /// <summary>
    /// Siembra categorias y productos si el keyspace esta vacio.
    /// </summary>
    /// <param name="session">Sesion de Cassandra.</param>
    /// <returns>Tarea asincrona.</returns>
    public static async Task SeedAsync(ISession session)
    {
        var categorias = new Table<Categoria>(session);
        if ((await categorias.Select(c => c).ExecuteAsync()).Any())
        {
            return; // ya hay datos: no se siembra dos veces
        }

        await categorias.Insert(new Categoria { Id = 1, Nombre = "Periféricos", Descripcion = "Teclados, ratones y demas" }).ExecuteAsync();
        await categorias.Insert(new Categoria { Id = 2, Nombre = "Monitores", Descripcion = "Pantallas para todo tipo de uso" }).ExecuteAsync();

        // Cada producto se escribe en las DOS tablas, con el nombre de la categoria copiado.
        await InsertarAsync(session, 101, "Teclado Mecánico", 89.99, 1, "Periféricos");
        await InsertarAsync(session, 102, "Ratón Inalámbrico", 29.99, 1, "Periféricos");
        await InsertarAsync(session, 103, "Alfombrilla XL", 19.99, 1, "Periféricos");
        await InsertarAsync(session, 201, "Monitor 27\" QHD", 249.99, 2, "Monitores");
        await InsertarAsync(session, 202, "Monitor 32\" 4K", 449.99, 2, "Monitores");

        // Historico de precios de ejemplo: el Teclado ha bajado dos veces
        await InsertarPrecioAsync(session, 101, new DateTime(2026, 10, 1, 9, 0, 0, DateTimeKind.Utc), 99.99, 109.99);
        await InsertarPrecioAsync(session, 101, new DateTime(2026, 10, 15, 9, 0, 0, DateTimeKind.Utc), 89.99, 99.99);
    }

    private static async Task InsertarPrecioAsync(
        ISession session, long productoId, DateTime fecha, double precio, double precioAnterior)
    {
        await session.ExecuteAsync(new SimpleStatement(
            "INSERT INTO historial_precios_por_producto (producto_id, fecha, precio, precio_anterior) VALUES (?, ?, ?, ?)",
            productoId, fecha, precio, precioAnterior));

        await session.ExecuteAsync(new SimpleStatement(
            "INSERT INTO cambios_precio_por_fecha (fecha, producto_id, precio) VALUES (?, ?, ?)",
            fecha.Date, productoId, precio));
    }

    private static async Task InsertarAsync(
        ISession session, long id, string nombre, double precio, long categoriaId, string categoriaNombre)
    {
        await session.ExecuteAsync(new SimpleStatement(
            "INSERT INTO productos_por_categoria (categoria_id, producto_id, nombre, precio, categoria_nombre) VALUES (?, ?, ?, ?, ?)",
            categoriaId, id, nombre, precio, categoriaNombre));

        await session.ExecuteAsync(new SimpleStatement(
            "INSERT INTO productos_por_id (id, nombre, precio, categoria_id, categoria_nombre) VALUES (?, ?, ?, ?, ?)",
            id, nombre, precio, categoriaId, categoriaNombre));
    }
}
