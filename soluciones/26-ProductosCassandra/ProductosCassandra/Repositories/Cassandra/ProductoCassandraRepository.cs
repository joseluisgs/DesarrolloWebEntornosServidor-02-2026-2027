using Cassandra;
using Cassandra.Data.Linq;
using ProductosCassandra.Models;
using ISession = Cassandra.ISession;

namespace ProductosCassandra.Repositories.Cassandra;

/// <inheritdoc />
public class ProductoCassandraRepository(ISession session) : IProductoRepository
{
    // El mapper de DataStax usa los atributos [Table]/[Column]/[PartitionKey] del modelo.
    private readonly Table<ProductoPorCategoria> _productosPorCategoria = new(session);
    private readonly Table<ProductoPorId> _productosPorId = new(session);
    private readonly Table<HistorialPrecioPorProducto> _historialPrecios = new(session);
    private readonly Table<CambioPrecioPorFecha> _cambiosPrecio = new(session);

    /// <inheritdoc />
    public async Task<IReadOnlyList<ProductoPorCategoria>> GetProductosPorCategoriaAsync(long categoriaId)
    {
        // SELECT * FROM productos_por_categoria WHERE categoria_id = ?
        // Es leer una particion ENTERA, que es lo que Cassandra hace rapidisimo.
        // Y sale ordenado por la clustering key (producto_id) de serie: sin ORDER BY.
        var productos = await _productosPorCategoria
            .Where(p => p.CategoriaId == categoriaId)
            .ExecuteAsync();

        return productos.ToList();
    }

    /// <inheritdoc />
    public async Task<ProductoPorId?> GetProductoPorIdAsync(long id)
    {
        // SELECT * FROM productos_por_id WHERE id = ?
        var productos = await _productosPorId.Where(p => p.Id == id).Take(1).ExecuteAsync();
        return productos.FirstOrDefault();
    }

    /// <inheritdoc />
    public async Task CrearProductoAsync(CreateProducto producto, Categoria categoria)
    {
        // Escritura denormalizada: el nombre de la categoria se copia en las DOS tablas.
        // En un relacional el cruce lo pagaba el JOIN en lectura; aqui lo paga quien escribe.
        await _productosPorCategoria.Insert(new ProductoPorCategoria
        {
            CategoriaId = categoria.Id,
            ProductoId = producto.Id,
            Nombre = producto.Nombre,
            Precio = producto.Precio,
            CategoriaNombre = categoria.Nombre
        }).ExecuteAsync();

        await _productosPorId.Insert(new ProductoPorId
        {
            Id = producto.Id,
            Nombre = producto.Nombre,
            Precio = producto.Precio,
            CategoriaId = categoria.Id,
            CategoriaNombre = categoria.Nombre
        }).ExecuteAsync();
    }

    /// <inheritdoc />
    public async Task BorrarProductoAsync(long id, long categoriaId)
    {
        // Dos deletes, uno por tabla. Sin transaccion repartida: Cassandra prioriza
        // la escritura rapida sobre la atomicidad entre tablas (se compensa en la app).
        await session.ExecuteAsync(new SimpleStatement(
            "DELETE FROM productos_por_id WHERE id = ?", id));

        await session.ExecuteAsync(new SimpleStatement(
            "DELETE FROM productos_por_categoria WHERE categoria_id = ? AND producto_id = ?",
            categoriaId, id));
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<HistorialPrecioPorProducto>> GetHistorialPreciosAsync(long productoId)
    {
        // SELECT * FROM historial_precios_por_producto WHERE producto_id = ?
        // Sale por la clustering key con CLUSTERING ORDER BY (fecha DESC): sin ORDER BY
        var historial = await _historialPrecios
            .Where(h => h.ProductoId == productoId)
            .ExecuteAsync();

        return historial.ToList();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<CambioPrecioPorFecha>> GetCambiosPrecioAsync(DateTime fecha)
    {
        // SELECT * FROM cambios_precio_por_fecha WHERE fecha = ?
        // La particion ES el dia: crecimiento acotado por diseno
        var cambios = await _cambiosPrecio
            .Where(c => c.Fecha == fecha.Date)
            .ExecuteAsync();

        return cambios.ToList();
    }

    /// <inheritdoc />
    public async Task<bool> RegistrarCambioPrecioAsync(long productoId, double precioNuevo)
    {
        var producto = await GetProductoPorIdAsync(productoId);
        if (producto is null)
        {
            return false;
        }

        var ahora = DateTime.UtcNow;

        // 1) El historial: una fila mas, con el precio anterior guardado en la propia fila
        await _historialPrecios.Insert(new HistorialPrecioPorProducto
        {
            ProductoId = productoId,
            Fecha = ahora,
            Precio = precioNuevo,
            PrecioAnterior = producto.Precio
        }).ExecuteAsync();

        // 2) El informe del dia: la particion es el dia
        await _cambiosPrecio.Insert(new CambioPrecioPorFecha
        {
            Fecha = ahora.Date,
            ProductoId = productoId,
            Precio = precioNuevo
        }).ExecuteAsync();

        // 3) El precio actual: vive copiado en las tablas de lectura, asi que se
        // actualizan las dos. Un cambio de precio son CUATRO escrituras en total:
        // ese es el coste elegido al denormalizar (13.10.4 del tema).
        await session.ExecuteAsync(new SimpleStatement(
            "UPDATE productos_por_id SET precio = ? WHERE id = ?", precioNuevo, productoId));

        await session.ExecuteAsync(new SimpleStatement(
            "UPDATE productos_por_categoria SET precio = ? WHERE categoria_id = ? AND producto_id = ?",
            precioNuevo, producto.CategoriaId, productoId));

        return true;
    }
}
