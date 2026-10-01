using System.Text.Json;
using ProductosMinimalApiDI.Infrastructure;
using ProductosMinimalApiDI.Routes;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

builder.Services
    .AddRepositories()
    .AddServices();

var app = builder.Build();

app.MapProductosRoutes();

app.Run();
