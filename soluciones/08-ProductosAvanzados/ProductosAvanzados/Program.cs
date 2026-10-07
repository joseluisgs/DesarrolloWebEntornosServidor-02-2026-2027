using FluentValidation;
using ProductosAvanzados.Infrastructure;
using ProductosAvanzados.Middleware;

var builder = WebApplication.CreateBuilder(args);

Console.OutputEncoding = System.Text.Encoding.UTF8;

// ── SERVICIOS ────────────────────────────────────────────────────────────────

// MVC + Negociación de contenido: RespetBrowserAcceptHeader permite al cliente
// elegir entre JSON y XML con el header Accept. ReturnHttpNotAcceptable devuelve
// 406 si el cliente pide un formato no soportado (ej: text/csv).
// AddXmlDataContractSerializerFormatters() registra el serializador XML que,
// a diferencia de XmlSerializer, no requiere constructor vacío en los records.
builder.Services.AddControllers(options =>
{
    options.RespectBrowserAcceptHeader = true;
    options.ReturnHttpNotAcceptable = true;
})
.AddXmlDataContractSerializerFormatters();

// Documentación interactiva de la API (solo disponible en desarrollo)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Busca automáticamente todos los AbstractValidator<T> del ensamblado
// y los registra para que la validación se ejecute antes del controller.
// Sin esta línea, FluentValidation NO funciona (los Data Annotations sí).
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Repositorios (Singleton) y Servicios (Scoped) — Infrastructure pattern
builder.Services
    .AddRepositories()
    .AddServices();

// ── PIPELINE DE MIDDLEWARES ──────────────────────────────────────────────────

var app = builder.Build();

// Captura excepciones no controladas y las convierte en ProblemDetails (punto 07).
// Funciona como "red de seguridad": si algo falla inesperadamente, el cliente
// recibe una respuesta coherente en lugar de un 500 genérico.
app.UseGlobalExceptionHandler();

// Swagger solo se muestra en desarrollo (nunca en producción por seguridad)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enrutamiento: asigna cada petición HTTP al método del controller correspondiente
app.MapControllers();

app.Run();
