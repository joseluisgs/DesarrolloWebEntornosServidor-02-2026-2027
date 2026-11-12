namespace ProductosMicroService.Dtos;

/// <summary>
/// DTO para crear un producto.
/// ProveedorId es string: los proveedores viven en MongoDB con ObjectId.
/// </summary>
public record CreateProductoDto(
    string Nombre,
    decimal Precio,
    long CategoriaId,
    string ProveedorId,
    string? Imagen = null);

/// <summary>
/// DTO para actualizar un producto.
/// </summary>
public record UpdateProductoDto(
    string Nombre,
    decimal Precio,
    long CategoriaId,
    string ProveedorId,
    string? Imagen = null);

/// <summary>
/// DTO de respuesta de producto (lectura).
/// </summary>
public record ProductoReadDto(
    long Id,
    string Nombre,
    decimal Precio,
    string? Imagen,
    string CategoriaNombre,
    string ProveedorNombre);
