using CSharpFunctionalExtensions;
using MediatR;
using ProductosCQRS.Dtos;
using ProductosCQRS.Errors;
using ProductosCQRS.Mappers;
using ProductosCQRS.Models;
using ProductosCQRS.Services;

namespace ProductosCQRS.Features.Productos.Commands;

public record CreateProductoCommand(CreateProductoDto Dto)
    : IRequest<Result<ProductoDto, DomainError>>;

public class CreateProductoCommandHandler(
    IProductoService service,
    IMediator mediator)
    : IRequestHandler<CreateProductoCommand, Result<ProductoDto, DomainError>>
{
    public async Task<Result<ProductoDto, DomainError>> Handle(
        CreateProductoCommand request, CancellationToken cancellationToken)
    {
        // El servicio valida, guarda en PostgreSQL e invalida caché
        var result = service.Create(request.Dto);
        if (result.IsFailure)
            return Result.Failure<ProductoDto, DomainError>(result.Error);

        var producto = result.Value;

        // Publicar evento CON el objeto completo (relaciones ya cargadas)
        await mediator.Publish(new ProductoCreadoNotification(producto), cancellationToken);

        return Result.Success<ProductoDto, DomainError>(producto.ToDto());
    }
}

public record ProductoCreadoNotification(Producto Producto) : INotification;
