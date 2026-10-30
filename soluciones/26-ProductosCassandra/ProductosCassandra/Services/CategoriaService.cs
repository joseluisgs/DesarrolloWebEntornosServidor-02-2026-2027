using ProductosCassandra.Dtos;
using ProductosCassandra.Mappers;
using ProductosCassandra.Repositories;

namespace ProductosCassandra.Services;

/// <inheritdoc />
public class CategoriaService(
    ICategoriaRepository categorias,
    IProductoRepository productos) : ICategoriaService
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<CategoriaDto>> GetAllAsync()
    {
        var categoriasLista = await categorias.GetCategoriasAsync();
        return categoriasLista.Select(c => c.ToDto()).ToList();
    }

    /// <inheritdoc />
    public async Task<CategoriaDto?> GetByIdAsync(long id)
    {
        var categoria = await categorias.GetCategoriaAsync(id);
        return categoria?.ToDto();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ProductoDto>> GetProductosAsync(long categoriaId)
    {
        // Leer la particion entera de la categoria: orden gratis por clustering key
        var productosLista = await productos.GetProductosPorCategoriaAsync(categoriaId);
        return productosLista.Select(p => p.ToDto()).ToList();
    }
}
