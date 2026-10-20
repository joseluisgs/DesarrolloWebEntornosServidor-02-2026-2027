using MongoDB.Bson;
using MongoDB.Driver;
using ProductosMongo.Entity;
using ProductosMongo.Models;
using Serilog;

namespace ProductosMongo.Seed;

/// <summary>
/// Seeder para MongoDB. Siembra DOS colecciones:
/// 'proveedores' (documentos referenciados) y 'productos' (documentos compuestos).
/// En modo Development se ejecuta SIEMPRE: borra y recrea los datos.
/// </summary>
public static class MongoDbSeeder
{
    // IDs fijos: si fueran aleatorios, los productos referenciarían proveedores
    // que ya no existen tras cada reinicio.
    private static readonly ObjectId LogitechId = ObjectId.Parse("64b000000000000000000001");
    private static readonly ObjectId IkeaId = ObjectId.Parse("64b000000000000000000002");
    private static readonly ObjectId YamahaId = ObjectId.Parse("64b000000000000000000003");
    private static readonly ObjectId DecathlonId = ObjectId.Parse("64b000000000000000000004");
    private static readonly ObjectId AnayaId = ObjectId.Parse("64b000000000000000000005");

    /// <summary>
    /// Siembra proveedores y productos con el Driver Nativo.
    /// Los referenciados se insertan PRIMERO: si faltan, los productos apuntarían a la nada.
    /// </summary>
    /// <param name="database">Base de datos MongoDB.</param>
    public static void Seed(IMongoDatabase database)
    {
        var proveedoresCollection = database.GetCollection<Proveedor>("proveedores");
        var productosCollection = database.GetCollection<Producto>("productos");

        // 1) Referenciados primero
        var proveedores = BuildProveedores();
        proveedoresCollection.DeleteMany(FilterDefinition<Proveedor>.Empty);
        proveedoresCollection.InsertMany(proveedores);
        Log.Information("Coleccion 'proveedores' vaciada y con {Count} documentos.", proveedores.Length);

        // 2) Compuestos después (cada uno apunta a un proveedor ya existente)
        var productos = BuildProductos();
        productosCollection.DeleteMany(FilterDefinition<Producto>.Empty);
        productosCollection.InsertMany(productos);
        Log.Information("Coleccion 'productos' vaciada e insertados {Count} documentos.", productos.Count);
    }

    /// <summary>
    /// Siembra proveedores y productos con Entity Framework Core.
    /// La base de datos debe estar creada y vacía (EnsureDeleted + EnsureCreated).
    /// </summary>
    /// <param name="context">Contexto de EF Core para MongoDB.</param>
    public static void Seed(ProductosMongoDbContext context)
    {
        var proveedores = BuildProveedores();
        var productos = BuildProductos();

        // 1) Referenciados primero
        context.Proveedores.AddRange(proveedores);

        // 2) Compuestos después
        context.Productos.AddRange(productos);

        context.SaveChanges();
        Log.Information("EF Core: insertados {Prov} proveedores y {Prod} productos.",
            proveedores.Length, productos.Count);
    }

    /// <summary>
    /// Los cinco proveedores de la semilla, con ID fijo.
    /// </summary>
    private static Proveedor[] BuildProveedores() =>
    [
        new() { Id = LogitechId, Nombre = "Logitech", Contacto = "exportaciones@logitech.com", Pais = "Suiza" },
        new() { Id = IkeaId, Nombre = "Ikea", Contacto = "proveedores@ikea.com", Pais = "Suecia" },
        new() { Id = YamahaId, Nombre = "Yamaha", Contacto = "distribucion@yamaha.com", Pais = "Japon" },
        new() { Id = DecathlonId, Nombre = "Decathlon", Contacto = "sourcing@decathlon.com", Pais = "Francia" },
        new() { Id = AnayaId, Nombre = "Anaya", Contacto = "pedidos@anayaeditorial.com", Pais = "Espana" }
    ];

    // Los tipos Owned de EF Core NO pueden compartirse entre entidades:
    // si tres productos apuntan al MISMO objeto Categoria, solo uno la guarda.
    // Por eso cada producto recibe su propia instancia (NuevaCategoria).
    private static readonly (string Nombre, string Descripcion)[] CategoriasSemilla =
    [
        ("Electrónica", "Dispositivos y componentes electrónicos"),
        ("Mobiliario", "Muebles para oficina y hogar"),
        ("Instrumentos", "Instrumentos musicales"),
        ("Deportes", "Material deportivo"),
        ("Libros", "Libros y publicaciones")
    ];

    private static Categoria NuevaCategoria(int indice) => new()
    {
        Nombre = CategoriasSemilla[indice].Nombre,
        Descripcion = CategoriasSemilla[indice].Descripcion
    };

    /// <summary>
    /// Los diez productos de la semilla: categoría embebida + proveedor referenciado.
    /// </summary>
    private static List<Producto> BuildProductos() =>
        [
            new()
            {
                Id = ObjectId.GenerateNewId(),
                Nombre = "Teclado Mecánico",
                Precio = 89.99m,
                Imagen = "https://picsum.photos/seed/teclado/300",
                Categoria = NuevaCategoria(0),
                ProveedorId = LogitechId,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = ObjectId.GenerateNewId(),
                Nombre = "Ratón Inalámbrico",
                Precio = 35.50m,
                Imagen = "https://picsum.photos/seed/raton/300",
                Categoria = NuevaCategoria(0),
                ProveedorId = LogitechId,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = ObjectId.GenerateNewId(),
                Nombre = "Monitor 27\"",
                Precio = 299.00m,
                Imagen = "https://picsum.photos/seed/monitor/300",
                Categoria = NuevaCategoria(0),
                ProveedorId = LogitechId,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = ObjectId.GenerateNewId(),
                Nombre = "Silla Ergonómica",
                Precio = 249.99m,
                Imagen = "https://picsum.photos/seed/silla/300",
                Categoria = NuevaCategoria(1),
                ProveedorId = IkeaId,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = ObjectId.GenerateNewId(),
                Nombre = "Escritorio Gaming",
                Precio = 189.00m,
                Imagen = "https://picsum.photos/seed/escritorio/300",
                Categoria = NuevaCategoria(1),
                ProveedorId = IkeaId,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = ObjectId.GenerateNewId(),
                Nombre = "Guitarra Acústica",
                Precio = 159.99m,
                Imagen = "https://picsum.photos/seed/guitarra/300",
                Categoria = NuevaCategoria(2),
                ProveedorId = YamahaId,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = ObjectId.GenerateNewId(),
                Nombre = "Teclado MIDI",
                Precio = 120.00m,
                Imagen = "https://picsum.photos/seed/midi/300",
                Categoria = NuevaCategoria(2),
                ProveedorId = YamahaId,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = ObjectId.GenerateNewId(),
                Nombre = "Balón de Fútbol",
                Precio = 25.99m,
                Imagen = "https://picsum.photos/seed/balon/300",
                Categoria = NuevaCategoria(3),
                ProveedorId = DecathlonId,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = ObjectId.GenerateNewId(),
                Nombre = "Raqueta de Tenis",
                Precio = 79.00m,
                Imagen = "https://picsum.photos/seed/raqueta/300",
                Categoria = NuevaCategoria(3),
                ProveedorId = DecathlonId,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = ObjectId.GenerateNewId(),
                Nombre = "El Arte de Programar",
                Precio = 39.99m,
                Imagen = "https://picsum.photos/seed/libro/300",
                Categoria = NuevaCategoria(4),
                ProveedorId = AnayaId,
                CreatedAt = DateTime.UtcNow
            }
        ];
}
