using ProductosAvanzados.Dtos;
using ProductosAvanzados.Models;

namespace ProductosAvanzados.Mappers;

public static class ProductoMapper
{
    public static ProductoDto ToDto(this Producto producto) =>
        new(producto.Id, producto.Nombre, producto.Precio, producto.Categoria,
            producto.Imagen, producto.CreatedAt, producto.UpdatedAt, producto.IsActivo);

    public static Producto ToModel(this CreateProductoDto dto) => new()
    {
        Nombre = dto.Nombre,
        Precio = dto.Precio,
        Categoria = dto.Categoria,
        Imagen = dto.Imagen ?? string.Empty
    };

    public static Producto ToModel(this UpdateProductoDto dto) => new()
    {
        Nombre = dto.Nombre,
        Precio = dto.Precio,
        Categoria = dto.Categoria,
        Imagen = dto.Imagen ?? string.Empty
    };
}
