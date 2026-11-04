namespace AuthMicroService.Errors;

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
/// Error de validacion de negocio.
/// </summary>
public sealed record BusinessRuleError(string Message) : DomainError(Message);

/// <summary>
/// Error de conflicto (recurso duplicado).
/// </summary>
public sealed record ConflictError(string Message) : DomainError(Message)
{
    public static ConflictError Duplicate(string resourceType, string value) =>
        new($"Ya existe un {resourceType} con el valor '{value}'");
}

/// <summary>
/// Error de autenticacion (HTTP 401): credenciales invalidas.
/// </summary>
public sealed record UnauthorizedError(string Message) : DomainError(Message)
{
    public static UnauthorizedError InvalidCredentials() => new("Credenciales invalidas");
}
