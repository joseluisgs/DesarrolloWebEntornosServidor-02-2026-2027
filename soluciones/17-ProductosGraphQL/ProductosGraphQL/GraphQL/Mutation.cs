using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Subscriptions;
using ProductosGraphQL.Dtos;
using ProductosGraphQL.GraphQL.Inputs;
using ProductosGraphQL.Models;
using ProductosGraphQL.Services;

namespace ProductosGraphQL.GraphQL;

// Mutation delgado y sin constructor: HotChocolate registra el tipo como singleton,
// por lo que las dependencias scoped (servicios con DbContext) se inyectan por
// parámetro con [Service], resueltas del scope de cada petición.
public class ProductoMutation
{
    [Authorize(Policy = "AdminOnly")]
    public Producto? CrearProducto(
        CreateProductoInput input,
        [Service] IProductoService service,
        [Service] ITopicEventSender eventSender)
    {
        var dto = new CreateProductoDto
        {
            Nombre = input.Nombre,
            Precio = input.Precio,
            CategoriaId = input.CategoriaId,
            Imagen = input.Imagen
        };

        var result = service.Create(dto);
        if (result.IsFailure) return null;

        _ = eventSender.SendAsync("ProductoCreado", result.Value);
        return result.Value;
    }

    [Authorize(Policy = "AdminOnly")]
    public Producto? ActualizarProducto(
        long id,
        UpdateProductoInput input,
        [Service] IProductoService service,
        [Service] ITopicEventSender eventSender)
    {
        var dto = new UpdateProductoDto
        {
            Nombre = input.Nombre,
            Precio = input.Precio,
            CategoriaId = input.CategoriaId,
            Imagen = input.Imagen
        };

        var result = service.Update(id, dto);
        if (result.IsFailure) return null;

        _ = eventSender.SendAsync("ProductoActualizado", result.Value);
        return result.Value;
    }

    [Authorize(Policy = "AdminOnly")]
    public bool EliminarProducto(
        long id,
        [Service] IProductoService service,
        [Service] ITopicEventSender eventSender)
    {
        var result = service.Delete(id);
        if (result.IsSuccess)
        {
            _ = eventSender.SendAsync("ProductoEliminado", id);
        }
        return result.IsSuccess;
    }
}
