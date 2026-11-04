using CSharpFunctionalExtensions;
using MediatR;
using ProductosCQRS.Dtos;
using ProductosCQRS.Errors;
using ProductosCQRS.Services;

namespace ProductosCQRS.Features.Productos.Queries;

public record GetAllProductosQuery()
    : IRequest<Result<List<ProductoReadDto>, DomainError>>;

public class GetAllProductosQueryHandler(
    IProductoService service)
    : IRequestHandler<GetAllProductosQuery, Result<List<ProductoReadDto>, DomainError>>
{
    public async Task<Result<List<ProductoReadDto>, DomainError>> Handle(
        GetAllProductosQuery request, CancellationToken cancellationToken)
    {
        // El servicio consulta MongoDB con caché
        return await service.GetAllReadAsync();
    }
}
