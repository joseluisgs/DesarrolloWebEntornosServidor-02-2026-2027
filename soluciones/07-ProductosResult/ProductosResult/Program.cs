using ProductosResult.Infrastructure;
using ProductosResult.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddRepositories()
    .AddServices();

var app = builder.Build();

// Safety net: captura excepciones no controladas
// Con Result Pattern, las excepciones de dominio NO deberían llegar aquí,
// pero es bueno tenerlo por si acaso (bugs, errores de BD, etc.)
app.UseGlobalExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
