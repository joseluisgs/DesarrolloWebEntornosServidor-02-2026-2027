using AuthMicroService.Infrastructures;
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
    Log.Information("Iniciando AuthMicroService...");

    // ── SERVICIOS ─────────────────────────────────────────────────────────────
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // ── INFRAESTRUCTURA ─────────────────────────────────────────────────────
    builder.Services
        .AddIdentityConfiguration(builder.Configuration)
        .AddServices();

    // ── PIPELINE ─────────────────────────────────────────────────────────────
    var app = builder.Build();

    // ── Seed de roles ────────────────────────────────────────────────────────
    await app.InitializeRolesAsync();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    Log.Information("AuthMicroService http://localhost:5101");
    Log.Information("  Swagger: http://localhost:5101/swagger");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "AuthMicroService termino inesperadamente");
}
finally
{
    Log.CloseAndFlush();
}
