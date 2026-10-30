using ProductosCassandra.Dtos;
using ProductosCassandra.Mappers;
using ProductosCassandra.Repositories;

namespace ProductosCassandra.Services;

/// <inheritdoc />
public class ProductoService(
    IProductoRepository productos,
    ICategoriaRepository categorias) : IProductoService
{
    /// <inheritdoc />
    public async Task<ProductoDto?> GetByIdAsync(long id)
    {
        var producto = await productos.GetProductoPorIdAsync(id);
        return producto?.ToDto();
    }

    /// <inheritdoc />
    public async Task<ProductoDto?> CreateAsync(CreateProductoDto dto)
    {
        // La categoria se resuelve aqui: es decision de negocio, y de su nombre
        // depende lo que se copia en las dos tablas (denormalizacion).
        var categoria = await categorias.GetCategoriaAsync(dto.CategoriaId);
        if (categoria is null)
        {
            return null;
        }

        var modelo = dto.ToModel();
        await productos.CrearProductoAsync(modelo, categoria);

        return new ProductoDto(modelo.Id, modelo.Nombre, modelo.Precio,
            categoria.Id, categoria.Nombre);
    }

    /// <inheritdoc />
    public Task DeleteAsync(long id, long categoriaId) =>
        productos.BorrarProductoAsync(id, categoriaId);

    /// <inheritdoc />
    public async Task<IReadOnlyList<HistorialPrecioDto>> GetHistorialPreciosAsync(long id)
    {
        var historial = await productos.GetHistorialPreciosAsync(id);
        return historial.Select(h => h.ToDto()).ToList();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<CambioPrecioDiaDto>> GetCambiosPrecioAsync(DateTime fecha)
    {
        var cambios = await productos.GetCambiosPrecioAsync(fecha);
        return cambios.Select(c => c.ToDto()).ToList();
    }

    /// <inheritdoc />
    public Task<bool> RegistrarCambioPrecioAsync(long id, double precioNuevo) =>
        productos.RegistrarCambioPrecioAsync(id, precioNuevo);
}
