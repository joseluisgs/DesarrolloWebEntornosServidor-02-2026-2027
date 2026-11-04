namespace ProductosMicroService.Errors;

/// <summary>
/// Error de dominio base usando ROP (Railway-Oriented Programming).
/// </summary>
public abstract record DomainError(string Message)
{
    public override string ToString() => $"{GetType().Name}: {Message}";
}

/// <summary>
/// Error cuando un recurso no se encuentra.
/// </summary>
public sealed record NotFoundError(string Message) : DomainError(Message)
{
    public static NotFoundError FromId(string id, string resourceType = "Recurso") =>
        new($"{resourceType} con ID '{id}' no encontrado");
}

/// <summary>
/// Error de validación de negocio.
/// </summary>
public sealed record BusinessRuleError(string Message) : DomainError(Message);
