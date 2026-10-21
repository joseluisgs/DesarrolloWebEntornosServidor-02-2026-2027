namespace ProductosEFCore.Errors;

public static class ProveedorError
{
    public static NotFoundError NotFound(string id) =>
        new($"Proveedor con código '{id}' no encontrado");
    public static ValidationError NombreVacio() =>
        ValidationError.Create("El nombre del proveedor es obligatorio");
    public static ConflictError CodigoDuplicado(string id) =>
        ConflictError.Duplicate("proveedor", id);
}
