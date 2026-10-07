using ProductosAvanzados.Models;

namespace ProductosAvanzados.Repositories;

public class ProductoRepository : IProductoRepository
{
    private readonly Dictionary<long, Producto> _productos = [];
    private long _nextId = 1;

    public ProductoRepository()
    {
        var productos = new[]
        {
            new Producto { Nombre = "Laptop HP Pavilion", Precio = 899.99m, Categoria = "Electrónica", Imagen = "laptop.jpg" },
            new Producto { Nombre = "Mouse Logitech MX", Precio = 79.99m, Categoria = "Accesorios", Imagen = "mouse.jpg" },
            new Producto { Nombre = "Teclado Mecánico RGB", Precio = 129.50m, Categoria = "Accesorios", Imagen = "teclado.jpg" },
            new Producto { Nombre = "Monitor Samsung 27\"", Precio = 349.00m, Categoria = "Electrónica", Imagen = "monitor.jpg" },
            new Producto { Nombre = "Auriculares Sony WH-1000XM4", Precio = 279.99m, Categoria = "Audio", Imagen = "auriculares.jpg" },
            new Producto { Nombre = "Cable USB-C 2m", Precio = 15.99m, Categoria = "Accesorios", Imagen = "cable.jpg" },
            new Producto { Nombre = "Disco SSD 1TB", Precio = 89.99m, Categoria = "Almacenamiento", Imagen = "ssd.jpg" },
            new Producto { Nombre = "Memoria RAM 16GB", Precio = 59.99m, Categoria = "Componentes", Imagen = "ram.jpg" },
            new Producto { Nombre = "Webcam HD 1080p", Precio = 49.99m, Categoria = "Cámaras", Imagen = "webcam.jpg" },
            new Producto { Nombre = "Silla Gamer Ergonómica", Precio = 199.99m, Categoria = "Muebles", Imagen = "silla.jpg" }
        };

        foreach (var producto in productos)
        {
            Add(producto);
        }
    }

    public IEnumerable<Producto> GetAll() =>
        _productos.Values.Where(p => p.IsActivo);

    public Producto? GetById(long id) =>
        _productos.TryGetValue(id, out var producto) && producto.IsActivo ? producto : null;

    public Producto Add(Producto producto)
    {
        producto.Id = _nextId++;
        producto.CreatedAt = DateTime.UtcNow;
        _productos[producto.Id] = producto;
        return producto;
    }

    public Producto? Update(long id, Producto producto)
    {
        if (!_productos.TryGetValue(id, out var existente) || !existente.IsActivo)
            return null;

        producto.Id = id;
        producto.CreatedAt = existente.CreatedAt;
        producto.UpdatedAt = DateTime.UtcNow;
        _productos[id] = producto;
        return producto;
    }

    public Producto? PatchPrice(long id, decimal precio)
    {
        if (!_productos.TryGetValue(id, out var producto) || !producto.IsActivo)
            return null;

        producto.Precio = precio;
        producto.UpdatedAt = DateTime.UtcNow;
        return producto;
    }

    public bool Delete(long id)
    {
        if (!_productos.TryGetValue(id, out var producto) || !producto.IsActivo)
            return false;

        producto.IsActivo = false;
        producto.DeletedAt = DateTime.UtcNow;
        return true;
    }

    public IEnumerable<Producto> Search(string termino) =>
        _productos.Values.Where(p =>
            p.IsActivo &&
            (p.Nombre.Contains(termino, StringComparison.OrdinalIgnoreCase) ||
             p.Categoria.Contains(termino, StringComparison.OrdinalIgnoreCase)));

    public IEnumerable<Producto> Filter(string? nombre, string? categoria, decimal? precioMin, decimal? precioMax)
    {
        var filtrados = _productos.Values.Where(p => p.IsActivo);

        if (!string.IsNullOrEmpty(nombre))
            filtrados = filtrados.Where(p => p.Nombre.Contains(nombre, StringComparison.OrdinalIgnoreCase));
        if (!string.IsNullOrEmpty(categoria))
            filtrados = filtrados.Where(p => p.Categoria.Equals(categoria, StringComparison.OrdinalIgnoreCase));
        if (precioMin.HasValue)
            filtrados = filtrados.Where(p => p.Precio >= precioMin.Value);
        if (precioMax.HasValue)
            filtrados = filtrados.Where(p => p.Precio <= precioMax.Value);

        return filtrados;
    }

    public (IEnumerable<Producto> Items, int TotalPages, int TotalItems) GetPaged(int page, int pageSize)
    {
        // El techo de paginación vive en la capa de datos: el filtro puede construirse en código
        // (jobs, tests, GraphQL) sin pasar por la validación REST, así que aquí es donde el
        // Skip/Take no sale de 1..100.
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var all = _productos.Values.Where(p => p.IsActivo).ToList();
        var totalItems = all.Count;
        var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
        var items = all.Skip((page - 1) * pageSize).Take(pageSize);

        return (items, totalPages, totalItems);
    }
}
