using System.Text.Json;
using ProductosControllersApiDI.Infrastructure;

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

app.MapControllers();

app.Run();
