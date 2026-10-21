using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using ProductosEFCore.Entity;
using ProductosEFCore.Errors;
using ProductosEFCore.Models;
using Serilog;

namespace ProductosEFCore.Services;

/// <summary>
/// Implementa las tres políticas de concurrencia: optimista, pesimista y mixta.
/// </summary>
public class ConcurrencyService(AppDbContext db) : IConcurrencyService
{
    public async Task<Result<Producto, DomainError>> UpdateOptimisticAsync(long id, int newStock)
    {
        Log.Information("Concurrencia optimista: intentando actualizar producto {Id} a stock {Stock}", id, newStock);

        var producto = await db.Productos.FindAsync(id);
        if (producto is null)
            return Result.Failure<Producto, DomainError>(ProductoError.NotFound(id));

        producto.Stock = newStock;

        try
        {
            await db.SaveChangesAsync();
            Log.Information("Concurrencia optimista: producto {Id} actualizado correctamente", id);
            return Result.Success<Producto, DomainError>(producto);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            Log.Warning("Conflicto de concurrencia optimista en producto {Id}: {Message}", id, ex.Message);

            var entry = db.Entry(producto);
            var databaseValues = await entry.GetDatabaseValuesAsync();

            if (databaseValues is null)
                return Result.Failure<Producto, DomainError>(ProductoError.NotFound(id));

            var conflictProducto = (Producto)databaseValues.ToObject();

            return Result.Failure<Producto, DomainError>(
                new ConflictError($"Conflicto de concurrencia. Stock actual en BD: {conflictProducto.Stock}. " +
                                  $"Tu valor: {newStock}. El producto fue modificado por otro usuario."));
        }
    }

    public async Task<Result<Producto, DomainError>> UpdatePessimisticAsync(long id, int newStock)
    {
        Log.Information("Concurrencia pesimista: bloqueando producto {Id} para actualización a stock {Stock}", id, newStock);

        await using var transaction = await db.Database.BeginTransactionAsync(
            System.Data.IsolationLevel.Serializable);

        try
        {
            var sql = "SELECT * FROM \"Productos\" WHERE \"Id\" = @id FOR UPDATE";
            var productos = await db.Productos
                .FromSqlRaw(sql, new Npgsql.NpgsqlParameter("@id", id))
                .ToListAsync();

            var producto = productos.FirstOrDefault();
            if (producto is null)
            {
                await transaction.RollbackAsync();
                return Result.Failure<Producto, DomainError>(ProductoError.NotFound(id));
            }

            producto.Stock = newStock;
            await db.SaveChangesAsync();
            await transaction.CommitAsync();

            Log.Information("Concurrencia pesimista: producto {Id} actualizado correctamente", id);
            return Result.Success<Producto, DomainError>(producto);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Log.Error(ex, "Error en concurrencia pesimista para producto {Id}", id);
            return Result.Failure<Producto, DomainError>(
                new BusinessRuleError($"Error en bloqueo pesimista: {ex.Message}"));
        }
    }

    public async Task<Result<Producto, DomainError>> UpdateMixedAsync(long id, int cantidad)
    {
        Log.Information("Concurrencia mixta: intentando decrementar stock de producto {Id} por {Cantidad}", id, cantidad);

        await using var transaction = await db.Database.BeginTransactionAsync();

        try
        {
            var producto = await db.Productos
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (producto is null)
            {
                await transaction.RollbackAsync();
                return Result.Failure<Producto, DomainError>(ProductoError.NotFound(id));
            }

            if (producto.Stock < cantidad)
            {
                await transaction.RollbackAsync();
                return Result.Failure<Producto, DomainError>(
                    new BusinessRuleError($"Stock insuficiente. Stock actual: {producto.Stock}, solicitado: {cantidad}"));
            }

            var rowsAffected = await db.Database.ExecuteSqlRawAsync(
                "UPDATE \"Productos\" SET \"Stock\" = \"Stock\" - @cantidad WHERE \"Id\" = @id AND \"Stock\" >= @cantidad",
                new Npgsql.NpgsqlParameter("@cantidad", cantidad),
                new Npgsql.NpgsqlParameter("@id", id));

            if (rowsAffected == 0)
            {
                await transaction.RollbackAsync();
                return Result.Failure<Producto, DomainError>(
                    new BusinessRuleError("No se pudo actualizar el stock. Otro usuario modificó el producto."));
            }

            await db.SaveChangesAsync();
            await transaction.CommitAsync();

            var actualizado = await db.Productos.FindAsync(id);
            Log.Information("Concurrencia mixta: producto {Id} actualizado correctamente, nuevo stock: {Stock}", id, actualizado?.Stock);

            return Result.Success<Producto, DomainError>(actualizado!);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Log.Error(ex, "Error en concurrencia mixta para producto {Id}", id);
            return Result.Failure<Producto, DomainError>(
                new BusinessRuleError($"Error en actualización mixta: {ex.Message}"));
        }
    }
}
