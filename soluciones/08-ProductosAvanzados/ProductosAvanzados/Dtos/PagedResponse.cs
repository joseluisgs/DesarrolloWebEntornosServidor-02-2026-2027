namespace ProductosAvanzados.Dtos;

/// <summary>
/// DTO tipado para respuestas paginadas (mismo papel que <c>PagedResult&lt;T&gt;</c> en Tienda).
/// </summary>
/// <remarks>
/// Existe como tipo concreto -y no como objeto anonimo- para que el controlador pueda
/// firmar con <c>ActionResult&lt;PagedResponse&lt;T&gt;&gt;</c> y Swagger documente la forma real.
/// El JSON serializado es identico al del objeto anonimo original:
/// <c>{"data":[...],"pagination":{"page":1,"pageSize":10,"totalPages":1,"totalItems":0}}</c>
/// </remarks>
/// <typeparam name="T">Tipo de elemento de la pagina actual.</typeparam>
public sealed record PagedResponse<T>
{
    /// <summary>Elementos de la pagina actual.</summary>
    public IEnumerable<T> Data { get; init; } = Enumerable.Empty<T>();

    /// <summary>Metadatos de la paginacion.</summary>
    public PageMetadata Pagination { get; init; } = new(0, 0, 0, 0);
}

/// <summary>
/// Metadatos de la paginacion de una respuesta <see cref="PagedResponse{T}"/>.
/// </summary>
/// <param name="Page">Pagina actual (base 1).</param>
/// <param name="PageSize">Tamano de pagina solicitado.</param>
/// <param name="TotalPages">Numero total de paginas.</param>
/// <param name="TotalItems">Numero total de elementos.</param>
public sealed record PageMetadata(int Page, int PageSize, int TotalPages, int TotalItems);