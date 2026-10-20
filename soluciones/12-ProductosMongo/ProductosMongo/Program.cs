using ProductosMongo.Config;
using ProductosMongo.Infrastructures;
using Serilog;
using Serilog.Events;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File(
        path: "logs/productos-mongo-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(Log.Logger);

try
{
    Log.Information("Iniciando la aplicacion...");

    // ── CONFIGURACION TIPADA ──────────────────────────────────────────────────
    builder.Services.Configure<ApiConfig>(
        builder.Configuration.GetSection("Api"));

    // ── SERVICIOS ─────────────────────────────────────────────────────────────
    builder.Services.AddControllers(options =>
    {
        options.RespectBrowserAcceptHeader = true;
        options.ReturnHttpNotAcceptable = true;
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new ProductosMongo.Converters.ObjectIdJsonConverter());
    })
    .AddXmlDataContractSerializerFormatters();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // ── INFRAESTRUCTURA (estilo Tienda) ───────────────────────────────────────
    builder.Services
        .AddDatabases(builder.Configuration)
        .AddRepositories(builder.Configuration)
        .AddServices();

    // ── PIPELINE DE MIDDLEWARES ───────────────────────────────────────────────

    var app = builder.Build();
    var isDevelopment = app.Environment.IsDevelopment();

    await app.InitializeDatabaseAsync(isDevelopment);

    if (isDevelopment)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.MapControllers();

    // ── CONFIGURACION TIPADA ──────────────────────────────────────────────────
    var apiConfig = app.Services.GetRequiredService<Microsoft.Extensions.Options.IOptions<ApiConfig>>().Value;

    var port = builder.Configuration["ASPNETCORE_URLS"]?.Split(':').LastOrDefault() ?? "5000";
    Console.WriteLine("========================================");
    Console.WriteLine($"  {apiConfig.Nombre}");
    Console.WriteLine($"  {apiConfig.Mensaje}");
    Console.WriteLine($"  http://localhost:{port}");
    Console.WriteLine($"  Swagger: http://localhost:{port}/swagger");
    Console.WriteLine($"  Entorno: {app.Environment.EnvironmentName}");
    Console.WriteLine("========================================");

    Log.Information("API {Nombre}: {Mensaje}", apiConfig.Nombre, apiConfig.Mensaje);

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "La aplicacion termino de forma inesperada");
}
finally
{
    Log.CloseAndFlush();
}
