namespace ProductosMongo.Errors;

/// <summary>
/// Error de dominio base. Patrón Result con CSharpFunctionalExtensions.
/// </summary>
public abstract record DomainError(string Message)
{
    public override string ToString() => $"{GetType().Name}: {Message}";
}

/// <summary>
/// Error de recurso no encontrado.
/// </summary>
public sealed record NotFoundError(string Message) : DomainError(Message)
{
    public static NotFoundError FromId(string id, string resourceType = "Recurso") =>
        new($"{resourceType} con ID {id} no encontrado");
}

/// <summary>
/// Error de validación.
/// </summary>
public sealed record ValidationError(string Message, Dictionary<string, string[]>? Errors = null)
    : DomainError(Message)
{
    public static ValidationError Create(string message) => new(message);
}

/// <summary>
/// Error de conflicto (duplicados, etc.).
/// </summary>
public sealed record ConflictError(string Message) : DomainError(Message)
{
    public static ConflictError Duplicate(string resourceType, string value) =>
        new($"Ya existe un {resourceType} con el valor '{value}'");
}

/// <summary>
/// Error de regla de negocio.
/// </summary>
public sealed record BusinessRuleError(string Message) : DomainError(Message);
