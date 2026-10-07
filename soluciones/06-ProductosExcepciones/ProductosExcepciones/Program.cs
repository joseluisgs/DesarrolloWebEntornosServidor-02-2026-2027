using System.Text.Json;
using ProductosExcepciones.Infrastructure;
using ProductosExcepciones.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

builder.Services.AddControllers();

builder.Services
    .AddRepositories()
    .AddServices();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.MapControllers();

app.Run();
