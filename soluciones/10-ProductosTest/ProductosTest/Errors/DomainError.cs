namespace ProductosTest.Errors;

public abstract record DomainError(string Message)
{
    public override string ToString() => $"{GetType().Name}: {Message}";
}

public sealed record NotFoundError(string Message) : DomainError(Message)
{
    public static NotFoundError FromId(long id, string resourceType = "Recurso") =>
        new($"{resourceType} con ID {id} no encontrado");
}

public sealed record ValidationError(string Message, Dictionary<string, string[]>? Errors = null)
    : DomainError(Message)
{
    public static ValidationError Create(string message) => new(message);
    public static ValidationError WithFieldErrors(Dictionary<string, string[]> errors) =>
        new("Errores de validación", errors);
}

public sealed record ConflictError(string Message) : DomainError(Message)
{
    public static ConflictError Duplicate(string resourceType, string value) =>
        new($"Ya existe un {resourceType} con el valor '{value}'");
}

public sealed record BusinessRuleError(string Message) : DomainError(Message);
