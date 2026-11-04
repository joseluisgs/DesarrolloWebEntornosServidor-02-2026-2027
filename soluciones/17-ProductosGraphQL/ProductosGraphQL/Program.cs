using ProductosGraphQL.Infrastructures;
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
        path: "logs/productos-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(Log.Logger);

try
{
    Log.Information("Iniciando la aplicacion...");

    var services = builder.Services;
    var configuration = builder.Configuration;
    var environment = builder.Environment;

    // ── AUTH ────────────────────────────────────────────────────────────────
    services.AddAuthConfiguration(configuration);

    // ── DATABASE + REPOS + SERVICES ────────────────────────────────────────
    services
        .AddDatabases(configuration)
        .AddRepositories()
        .AddServices();

    // ── MVC (AuthController: /api/auth/login y /api/auth/signup) ───────────
    services.AddControllers();

    // ── GRAPHQL ────────────────────────────────────────────────────────────
    services.AddGraphQLConfig(environment);

    // ── PIPELINE ───────────────────────────────────────────────────────────
    var app = builder.Build();
    var isDevelopment = app.Environment.IsDevelopment();

    await app.InitializeDatabaseAsync(isDevelopment);

    app.UseSerilogRequestLogging();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapGraphQLEndpoints();
    app.MapControllers();

    Console.WriteLine("========================================");
    Console.WriteLine("  Productos GraphQL API");
    Console.WriteLine("  http://localhost:5110/graphql");
    Console.WriteLine($"  Entorno: {environment.EnvironmentName}");
    Console.WriteLine("========================================");

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
