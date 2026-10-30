using Cassandra;
using NUnit.Framework;
using ProductosCassandra.Infrastructures;
using ProductosCassandra.Repositories;
using ProductosCassandra.Repositories.Cassandra;
using ProductosCassandra.Services;
using Testcontainers.Cassandra;

namespace ProductosCassandra.Test;

/// <summary>
/// Base para tests de integracion con Cassandra real usando TestContainers.
/// El contenedor tarda ~1 minuto en estar listo: es una JVM, no un binario de 10 MB.
/// </summary>
public abstract class IntegrationTestBase
{
    // NUnit1032: el contenedor no se disposea en el teardown del fixture porque es
    // estatico y compartido por los tres; lo reclama Ryuk al terminar el proceso de tests.
#pragma warning disable NUnit1032
    private static readonly CassandraContainer Contenedor = new CassandraBuilder("cassandra:5.0")
        .WithEnvironment("MAX_HEAP_SIZE", "1G")
        .WithEnvironment("HEAP_NEWSIZE", "256M")
        .Build();
#pragma warning restore NUnit1032

#pragma warning disable NUnit1032
    protected ISession Session { get; private set; } = null!;
#pragma warning restore NUnit1032

    /// <summary>Repositorio de productos bajo prueba, apuntando al contenedor efimero.</summary>
    protected IProductoRepository ProductoRepositorio { get; private set; } = null!;

    /// <summary>Repositorio de categorias bajo prueba.</summary>
    protected ICategoriaRepository CategoriaRepositorio { get; private set; } = null!;

    /// <summary>Servicio de productos bajo prueba (capa de negocio).</summary>
    protected IProductoService ProductoServicio { get; private set; } = null!;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        await Contenedor.StartAsync();

        var cluster = Cluster.Builder()
            .AddContactPoints(Contenedor.Hostname)
            .WithPort(Contenedor.GetMappedPublicPort(9042))
            .Build();

        Session = cluster.Connect();
        await Session.InitializeSchemaAsync("productos");

        // Los servicios se montan sobre los MISMOS repositorios reales:
        // aqui no se mockea nada, se prueba la pila completa contra el contenedor.
        ProductoRepositorio = new ProductoCassandraRepository(Session);
        CategoriaRepositorio = new CategoriaCassandraRepository(Session);
        ProductoServicio = new ProductoService(ProductoRepositorio, CategoriaRepositorio);

        await SembrarAsync();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        // Solo se cierra la SESION de este fixture. El contenedor es estatico y
        // COMPARTIDO entre los tres fixtures: si se disposeara aqui, el siguiente
        // fixture arrancaria sobre un objeto muerto (ObjectDisposedException).
        // El contenedor lo reclama el propio Testcontainers (Ryuk) al salir del proceso.
        Session.Dispose();
    }

    /// <summary>Ejecuta CQL a pelo con parametros posicionales (?).</summary>
    protected async Task EjecutarCqlAsync(string cql, params object[] valores)
    {
        await Session.ExecuteAsync(new SimpleStatement(cql, valores));
    }

    /// <summary>
    /// Datos fijos: 2 categorias y 5 productos. IDs fijos para no depender de aleatoriedad.
    /// </summary>
    private async Task SembrarAsync()
    {
        await EjecutarCqlAsync(
            "INSERT INTO categorias (id, nombre, descripcion) VALUES (?, ?, ?)",
            1L, "Periféricos", "Teclados, ratones y demás");
        await EjecutarCqlAsync(
            "INSERT INTO categorias (id, nombre, descripcion) VALUES (?, ?, ?)",
            2L, "Monitores", "Pantallas para todo tipo de uso");

        await InsertarProductoAsync(101, "Teclado Mecánico", 89.99, 1, "Periféricos");
        await InsertarProductoAsync(102, "Ratón Inalámbrico", 29.99, 1, "Periféricos");
        await InsertarProductoAsync(103, "Alfombrilla XL", 19.99, 1, "Periféricos");
        await InsertarProductoAsync(201, "Monitor 27\" QHD", 249.99, 2, "Monitores");
        await InsertarProductoAsync(202, "Monitor 32\" 4K", 449.99, 2, "Monitores");

        // Historico de precios: el Teclado (101) tuvo dos cambios
        await InsertarPrecioAsync(101, new DateTime(2026, 10, 1, 9, 0, 0, DateTimeKind.Utc), 99.99, 109.99);
        await InsertarPrecioAsync(101, new DateTime(2026, 10, 15, 9, 0, 0, DateTimeKind.Utc), 89.99, 99.99);
    }

    private async Task InsertarProductoAsync(
        long id, string nombre, double precio, long categoriaId, string categoriaNombre)
    {
        await EjecutarCqlAsync(
            "INSERT INTO productos_por_categoria (categoria_id, producto_id, nombre, precio, categoria_nombre) VALUES (?, ?, ?, ?, ?)",
            categoriaId, id, nombre, precio, categoriaNombre);

        await EjecutarCqlAsync(
            "INSERT INTO productos_por_id (id, nombre, precio, categoria_id, categoria_nombre) VALUES (?, ?, ?, ?, ?)",
            id, nombre, precio, categoriaId, categoriaNombre);
    }

    private async Task InsertarPrecioAsync(
        long productoId, DateTime fecha, double precio, double precioAnterior)
    {
        await EjecutarCqlAsync(
            "INSERT INTO historial_precios_por_producto (producto_id, fecha, precio, precio_anterior) VALUES (?, ?, ?, ?)",
            productoId, fecha, precio, precioAnterior);

        await EjecutarCqlAsync(
            "INSERT INTO cambios_precio_por_fecha (fecha, producto_id, precio) VALUES (?, ?, ?)",
            fecha.Date, productoId, precio);
    }
}
