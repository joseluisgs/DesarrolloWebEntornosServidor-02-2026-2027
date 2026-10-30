using Cassandra;
using Serilog;
using ISession = Cassandra.ISession;

namespace ProductosCassandra.Infrastructures;

/// <summary>
/// Creacion del keyspace y de las tablas. En Cassandra no hay migraciones:
/// el esquema se declara con CREATE ... IF NOT EXISTS y se puede ampliar en caliente.
/// </summary>
public static class CassandraSchemaExtensions
{
    /// <summary>
    /// Espera a que el nodo acepte conexiones y crea keyspace y tablas si no existen.
    /// </summary>
    /// <param name="session">Sesion de Cassandra.</param>
    /// <param name="keyspace">Keyspace de la aplicacion.</param>
    /// <returns>Tarea asincrona.</returns>
    public static async Task InitializeSchemaAsync(this ISession session, string keyspace)
    {
        // La imagen oficial tarda ~30-60 s en estar lista: si el primer CREATE falla
        // por NoHostAvailableException, reintentamos hasta que el nodo despierte.
        for (var intento = 1; ; intento++)
        {
            try
            {
                await session.ExecuteAsync(new SimpleStatement("SELECT now() FROM system.local"));
                break;
            }
            catch (NoHostAvailableException) when (intento < 30)
            {
                Log.Information("Cassandra aun no acepta conexiones (intento {Intento}/30)...", intento);
                await Task.Delay(TimeSpan.FromSeconds(2));
            }
        }

        // SimpleStrategy y factor 1: suficiente para un nodo unico de desarrollo.
        // En produccion iria NetworkTopologyStrategy con replicas por datacenter.
        await session.ExecuteAsync(new SimpleStatement(
            $"CREATE KEYSPACE IF NOT EXISTS {keyspace} " +
            "WITH replication = {'class': 'SimpleStrategy', 'replication_factor': 1}"));

        session.ChangeKeyspace(keyspace);

        // Q3: las categorias sueltas
        await session.ExecuteAsync(new SimpleStatement("""
            CREATE TABLE IF NOT EXISTS categorias (
                id bigint PRIMARY KEY,
                nombre text,
                descripcion text
            )
            """));

        // Q1: la consulta estrella. Partition key = categoria_id (leer una
        // categoria es leer una particion); clustering key = producto_id (orden gratis).
        await session.ExecuteAsync(new SimpleStatement("""
            CREATE TABLE IF NOT EXISTS productos_por_categoria (
                categoria_id bigint,
                producto_id bigint,
                nombre text,
                precio double,
                categoria_nombre text,   -- denormalizacion: el nombre se copia en cada fila
                PRIMARY KEY (categoria_id, producto_id)
            )
            """));

        // Q2: el producto suelto por id
        await session.ExecuteAsync(new SimpleStatement("""
            CREATE TABLE IF NOT EXISTS productos_por_id (
                id bigint PRIMARY KEY,
                nombre text,
                precio double,
                categoria_id bigint,
                categoria_nombre text
            )
            """));

        // Q4: series temporales. El orden temporal esta GUARDADO en la tabla
        await session.ExecuteAsync(new SimpleStatement("""
            CREATE TABLE IF NOT EXISTS historial_precios_por_producto (
                producto_id bigint,
                fecha timestamp,
                precio double,
                precio_anterior double,
                PRIMARY KEY (producto_id, fecha)
            ) WITH CLUSTERING ORDER BY (fecha DESC)
            """));

        // Q5: el informe por dia. La particion es el dia: bucketing de serie
        await session.ExecuteAsync(new SimpleStatement("""
            CREATE TABLE IF NOT EXISTS cambios_precio_por_fecha (
                fecha timestamp,
                producto_id bigint,
                precio double,
                PRIMARY KEY (fecha, producto_id)
            )
            """));

        Log.Information("Esquema de Cassandra listo en el keyspace {Keyspace}", keyspace);
    }
}
