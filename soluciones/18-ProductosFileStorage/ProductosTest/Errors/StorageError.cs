namespace ProductosEFCore.Errors;

public static class StorageError
{
    public static DomainError ArchivoVacio() =>
        new BusinessRuleError("El archivo esta vacio o no se ha proporcionado");

    public static DomainError ArchivoMuyGrande() =>
        new BusinessRuleError("El archivo supera el tamano maximo permitido (5 MB)");

    public static DomainError ExtensionNoPermitida() =>
        new BusinessRuleError("La extension del archivo no esta permitida. Use: .jpg, .jpeg, .png, .gif");

    public static DomainError TipoContenidoNoPermitido() =>
        new BusinessRuleError("El tipo de contenido no esta permitido. Solo se permiten imagenes");

    public static DomainError NombreArchivoInvalido() =>
        new BusinessRuleError("El nombre del archivo contiene caracteres no validos");

    public static DomainError ErrorGuardando() =>
        new InternalError("Error al guardar el archivo en el servidor");

    public static DomainError ErrorEliminando() =>
        new InternalError("Error al eliminar el archivo del servidor");
}
