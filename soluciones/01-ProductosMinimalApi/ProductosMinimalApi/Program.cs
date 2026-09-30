using System.Text.Json;
using ProductosMinimalApi.Repositories;
using ProductosMinimalApi.Routes;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

// Singleton: el Dictionary vive en memoria mientras la API está en marcha
builder.Services.AddSingleton<IProductoRepository, ProductoRepository>();

var app = builder.Build();

app.MapProductosRoutes();

app.Run();
