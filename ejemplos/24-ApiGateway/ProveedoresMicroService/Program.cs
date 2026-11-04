using ProveedoresMicroService.Infrastructures;
using Serilog;
using Serilog.Events;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(Log.Logger);

try
{
    Log.Information("Iniciando ProveedoresMicroService...");

    // ── SERVICIOS ─────────────────────────────────────────────────────────────
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // ── INFRAESTRUCTURA ─────────────────────────────────────────────────────
    builder.Services
        .AddDatabase(builder.Configuration)
        .AddRepositories();

    // ── PIPELINE ─────────────────────────────────────────────────────────────
    var app = builder.Build();

    // ── Seed de datos ────────────────────────────────────────────────────────
    await app.InitializeDatabaseAsync();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.MapControllers();

    Log.Information("ProveedoresMicroService http://localhost:5103");
    Log.Information("  Swagger: http://localhost:5103/swagger");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "ProveedoresMicroService termino inesperadamente");
}
finally
{
    Log.CloseAndFlush();
}
