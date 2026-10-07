using FluentValidation;
using ProductosConfigLogging.Dtos;

namespace ProductosConfigLogging.Validators;

public class CreateProductoValidator : AbstractValidator<CreateProductoDto>
{
    private static readonly string[] CategoriasValidas = ["Electrónica", "Mobiliario", "Instrumentos", "Deportes", "Libros"];

    public CreateProductoValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es obligatorio")
            .MaximumLength(100).WithMessage("El nombre no puede tener más de 100 caracteres")
            .MinimumLength(2).WithMessage("El nombre debe tener al menos 2 caracteres");

        RuleFor(x => x.Precio)
            .GreaterThan(0).WithMessage("El precio debe ser mayor que 0")
            .LessThan(1000000).WithMessage("El precio no puede superar 1.000.000");

        RuleFor(x => x.Categoria)
            .NotEmpty().WithMessage("La categoría es obligatoria")
            .Must(c => CategoriasValidas.Contains(c))
            .WithMessage($"Categorías válidas: {string.Join(", ", CategoriasValidas)}");

        RuleFor(x => x.Imagen)
            .Must(url => string.IsNullOrEmpty(url) || Uri.TryCreate(url, UriKind.Absolute, out _))
            .When(x => !string.IsNullOrEmpty(x.Imagen))
            .WithMessage("La imagen debe ser una URL válida");
    }
}
