using System.ComponentModel.DataAnnotations;

namespace ProductosAvanzados.Dtos;

public class NoAdminAttribute : ValidationAttribute
{
    private static readonly string[] Prohibidos = ["admin", "root", "sistema"];

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string texto)
            return ValidationResult.Success;

        if (Prohibidos.Contains(texto.ToLowerInvariant()))
            return new ValidationResult("El nombre no puede ser 'admin', 'root' o 'sistema'.");

        return ValidationResult.Success;
    }
}
