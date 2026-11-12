using CSharpFunctionalExtensions;
using MediatR;
using ProductosCQRS.Dtos;
using ProductosCQRS.Errors;
using ProductosCQRS.Mappers;
using ProductosCQRS.Models;
using ProductosCQRS.Services;

namespace ProductosCQRS.Features.Productos.Commands;

public record UpdateProductoCommand(long Id, UpdateProductoDto Dto)
    : IRequest<Result<ProductoDto, DomainError>>;

public class UpdateProductoCommandHandler(
    IProductoService service,
    IMediator mediator)
    : IRequestHandler<UpdateProductoCommand, Result<ProductoDto, DomainError>>
{
    public async Task<Result<ProductoDto, DomainError>> Handle(
        UpdateProductoCommand request, CancellationToken cancellationToken)
    {
        // El servicio valida, actualiza en PostgreSQL e invalida caché
        var result = service.Update(request.Id, request.Dto);
        if (result.IsFailure)
            return Result.Failure<ProductoDto, DomainError>(result.Error);

        var producto = result.Value;

        // Publicar evento CON el objeto completo (relaciones ya cargadas)
        await mediator.Publish(new ProductoActualizadoNotification(producto), cancellationToken);

        return Result.Success<ProductoDto, DomainError>(producto.ToDto());
    }
}

public record ProductoActualizadoNotification(Producto Producto) : INotification;
