namespace ProductosExcepciones.Errors;

public static class ProductoException
{
    public static NotFoundException NotFound(long id) => new($"Producto con ID {id} no encontrado");
    public static ValidationException NombreVacio() => new("El nombre del producto es obligatorio");
    public static ValidationException PrecioInvalido(decimal precio) => new($"El precio {precio} debe ser mayor que cero");
    public static ConflictException NombreDuplicado(string nombre) => new($"Ya existe un producto con el nombre '{nombre}'");
}
