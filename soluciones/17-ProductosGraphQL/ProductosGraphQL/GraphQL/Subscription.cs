using HotChocolate;
using HotChocolate.Types;
using ProductosGraphQL.Models;

namespace ProductosGraphQL.GraphQL;

public class Subscription
{
    /// <summary>
    /// Notifica cuando se crea un nuevo producto.
    /// </summary>
    [Subscribe]
    [Topic("ProductoCreado")]
    public Producto ProductoCreado([EventMessage] Producto producto)
    {
        return producto;
    }

    /// <summary>
    /// Notifica cuando se actualiza un producto.
    /// </summary>
    [Subscribe]
    [Topic("ProductoActualizado")]
    public Producto ProductoActualizado([EventMessage] Producto producto)
    {
        return producto;
    }

    /// <summary>
    /// Notifica cuando se elimina un producto.
    /// </summary>
    [Subscribe]
    [Topic("ProductoEliminado")]
    public long ProductoEliminado([EventMessage] long id)
    {
        return id;
    }
}
