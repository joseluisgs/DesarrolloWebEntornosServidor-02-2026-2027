using CSharpFunctionalExtensions;
using MediatR;
using ProductosCQRS.Dtos;
using ProductosCQRS.Errors;
using ProductosCQRS.Services;

namespace ProductosCQRS.Features.Productos.Queries;

public record GetProductoByIdQuery(long Id)
    : IRequest<Result<ProductoReadDto, DomainError>>;

public class GetProductoByIdQueryHandler(
    IProductoService service)
    : IRequestHandler<GetProductoByIdQuery, Result<ProductoReadDto, DomainError>>
{
    public async Task<Result<ProductoReadDto, DomainError>> Handle(
        GetProductoByIdQuery request, CancellationToken cancellationToken)
    {
        // El servicio consulta MongoDB con caché
        return await service.GetByIdReadAsync(request.Id);
    }
}
