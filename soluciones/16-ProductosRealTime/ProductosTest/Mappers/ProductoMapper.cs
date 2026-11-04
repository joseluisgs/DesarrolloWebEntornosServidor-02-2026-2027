using ProductosEFCore.Dtos;
using ProductosEFCore.Models;

namespace ProductosEFCore.Mappers;

public static class ProductoMapper
{
    public static ProductoDto ToDto(this Producto producto) =>
        new(producto.Id, producto.Nombre, producto.Precio,
            producto.Categoria?.Nombre ?? string.Empty,
            producto.Imagen, producto.IsDeleted,
            producto.CreatedAt, producto.UpdatedAt);

    public static Producto ToModel(this CreateProductoDto dto) => new()
    {
        Nombre = dto.Nombre,
        Precio = dto.Precio,
        CategoriaId = dto.CategoriaId,
        Imagen = dto.Imagen ?? string.Empty
    };

    public static Producto ToModel(this UpdateProductoDto dto) => new()
    {
        Nombre = dto.Nombre,
        Precio = dto.Precio,
        CategoriaId = dto.CategoriaId,
        Imagen = dto.Imagen ?? string.Empty
    };

    public static CategoriaDto ToDto(this Categoria categoria) =>
        new(categoria.Id, categoria.Nombre, categoria.Descripcion);
}
