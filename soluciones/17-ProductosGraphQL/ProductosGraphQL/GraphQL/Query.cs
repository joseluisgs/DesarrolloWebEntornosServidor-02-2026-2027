using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;
using ProductosGraphQL.Models;
using ProductosGraphQL.Repositories;

namespace ProductosGraphQL.GraphQL;

public class ProductosQuery
{
    [UsePaging(MaxPageSize = 50, DefaultPageSize = 10)]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Producto> GetProductos(
        [Service] IProductoRepository productoRepository) =>
        productoRepository.GetAll().AsQueryable();

    public async Task<Producto?> GetProducto(
        long id,
        [Service] IProductoRepository productoRepository) =>
        productoRepository.GetById(id);

    [UsePaging]
    public IQueryable<Categoria> GetCategorias(
        [Service] ICategoriaRepository categoriaRepository) =>
        categoriaRepository.GetAll().AsQueryable();
}
