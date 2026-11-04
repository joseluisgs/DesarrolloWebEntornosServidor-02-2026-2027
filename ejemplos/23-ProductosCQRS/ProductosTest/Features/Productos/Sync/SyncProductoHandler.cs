using MediatR;
using ProductosCQRS.Features.Productos.Commands;
using ProductosCQRS.Models;
using ProductosCQRS.Repositories;
using Serilog;

namespace ProductosCQRS.Features.Productos.Sync;

public class SyncProductoHandler(
    IProductoReadRepository readRepository) :
    INotificationHandler<ProductoCreadoNotification>,
    INotificationHandler<ProductoActualizadoNotification>,
    INotificationHandler<ProductoEliminadoNotification>
{
    public async Task Handle(ProductoCreadoNotification notification, CancellationToken cancellationToken)
    {
        Log.Information("Sync: Producto creado {Id}, sincronizando a MongoDB...", notification.Producto.Id);

        var producto = notification.Producto;

        var read = new ProductoRead
        {
            Id = producto.Id,
            Nombre = producto.Nombre,
            Precio = producto.Precio,
            Categoria = new CategoriaRead { Id = producto.Categoria?.Id ?? 0, Nombre = producto.Categoria?.Nombre ?? "" },
            Proveedor = new ProveedorRead { Id = producto.Proveedor?.Id ?? 0, Nombre = producto.Proveedor?.Nombre ?? "" },
            SyncAt = DateTime.UtcNow
        };

        await readRepository.UpsertAsync(read);

        Log.Information("Sync: Producto {Id} sincronizado a MongoDB", notification.Producto.Id);
    }

    public async Task Handle(ProductoActualizadoNotification notification, CancellationToken cancellationToken)
    {
        Log.Information("Sync: Producto actualizado {Id}, sincronizando a MongoDB...", notification.Producto.Id);

        var producto = notification.Producto;

        var read = new ProductoRead
        {
            Id = producto.Id,
            Nombre = producto.Nombre,
            Precio = producto.Precio,
            Categoria = new CategoriaRead { Id = producto.Categoria?.Id ?? 0, Nombre = producto.Categoria?.Nombre ?? "" },
            Proveedor = new ProveedorRead { Id = producto.Proveedor?.Id ?? 0, Nombre = producto.Proveedor?.Nombre ?? "" },
            SyncAt = DateTime.UtcNow
        };

        await readRepository.UpsertAsync(read);

        Log.Information("Sync: Producto {Id} actualizado en MongoDB", notification.Producto.Id);
    }

    public async Task Handle(ProductoEliminadoNotification notification, CancellationToken cancellationToken)
    {
        Log.Information("Sync: Producto eliminado {Id}, eliminando de MongoDB...", notification.ProductoId);

        await readRepository.DeleteAsync(notification.ProductoId);

        Log.Information("Sync: Producto {Id} eliminado de MongoDB", notification.ProductoId);
    }
}
