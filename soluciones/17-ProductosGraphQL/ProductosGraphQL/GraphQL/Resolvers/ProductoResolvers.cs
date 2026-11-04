using HotChocolate;
using HotChocolate.Types;
using ProductosGraphQL.Models;

namespace ProductosGraphQL.GraphQL.Resolvers;

[ExtendObjectType(typeof(Producto))]
public class ProductoResolvers
{
    /// <summary>
    /// Campo calculado: disponible si hay stock mayor a 0.
    /// </summary>
    public bool GetEstaDisponible([Parent] Producto producto)
    {
        return !producto.IsDeleted;
    }
}
