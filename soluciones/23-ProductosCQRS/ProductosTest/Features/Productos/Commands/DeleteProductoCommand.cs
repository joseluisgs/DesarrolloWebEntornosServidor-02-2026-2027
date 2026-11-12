using CSharpFunctionalExtensions;
using MediatR;
using ProductosCQRS.Dtos;
using ProductosCQRS.Errors;
using ProductosCQRS.Services;

namespace ProductosCQRS.Features.Productos.Commands;

public record DeleteProductoCommand(long Id)
    : IRequest<Result<bool, DomainError>>;

public class DeleteProductoCommandHandler(
    IProductoService service,
    IMediator mediator)
    : IRequestHandler<DeleteProductoCommand, Result<bool, DomainError>>
{
    public async Task<Result<bool, DomainError>> Handle(
        DeleteProductoCommand request, CancellationToken cancellationToken)
    {
        // El servicio elimina de PostgreSQL e invalida caché
        var result = service.Delete(request.Id);
        if (result.IsFailure)
            return Result.Failure<bool, DomainError>(result.Error);

        await mediator.Publish(new ProductoEliminadoNotification(request.Id), cancellationToken);

        return Result.Success<bool, DomainError>(true);
    }
}

public record ProductoEliminadoNotification(long ProductoId) : INotification;
