using CSharpFunctionalExtensions;
using ProductosCQRS.Dtos;
using ProductosCQRS.Errors;
using ProductosCQRS.Models;

namespace ProductosCQRS.Services;

public interface IProductoService
{
    // ── Commands (escritura → PostgreSQL) ────────────────────────────────────
    Result<Producto, DomainError> Create(CreateProductoDto dto);
    Result<Producto, DomainError> Update(long id, UpdateProductoDto dto);
    UnitResult<DomainError> Delete(long id);
    UnitResult<DomainError> Restore(long id);

    // ── Queries (lectura → MongoDB + caché) ──────────────────────────────────
    Task<Result<List<ProductoReadDto>, DomainError>> GetAllReadAsync();
    Task<Result<ProductoReadDto, DomainError>> GetByIdReadAsync(long id);

    // ── Utilidades ───────────────────────────────────────────────────────────
    IEnumerable<Producto> GetAll();
    IEnumerable<Producto> GetDeleted();
    Result<Producto, DomainError> GetById(long id);
    IEnumerable<Producto> Search(string termino);
    IEnumerable<Producto> Filter(string? nombre, long? categoriaId, decimal? precioMin, decimal? precioMax);
    (IEnumerable<Producto> Items, int TotalPages, int TotalItems) GetPaged(int page, int pageSize);
    IEnumerable<Categoria> GetAllCategorias();
    Categoria? GetCategoriaById(long id);
}
