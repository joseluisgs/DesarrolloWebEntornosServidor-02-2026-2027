namespace ProductosConfigLogging.Errors;

public static class ProductoError
{
    public static NotFoundError NotFound(long id) => NotFoundError.FromId(id, "Producto");
    public static ValidationError NombreVacio() => ValidationError.Create("El nombre del producto es obligatorio");
    public static ValidationError PrecioInvalido(decimal precio) => ValidationError.Create($"El precio {precio} debe ser mayor que cero");
    public static ConflictError NombreDuplicado(string nombre) => ConflictError.Duplicate("producto", nombre);
}
