using FluentValidation;
using Microsoft.Extensions.Options;
using ProductosEFCore.Config;
using ProductosEFCore.Infrastructures;
using ProductosEFCore.Middleware;
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
    Log.Information("Iniciando la aplicación...");

    // ── CONFIGURACIÓN TIPADA ─────────────────────────────────────────────────
    builder.Services.Configure<ApiConfig>(
        builder.Configuration.GetSection("Api"));

    // ── IDENTITY + JWT ─────────────────────────────────────────────────────
    builder.Services.AddIdentityConfiguration(builder.Configuration, builder.Environment);

    // ── SERVICIOS ─────────────────────────────────────────────────────────────
    builder.Services.AddControllers(options =>
    {
        options.RespectBrowserAcceptHeader = true;
        options.ReturnHttpNotAcceptable = true;
    })
    .AddXmlDataContractSerializerFormatters();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Description = "Introduce el token JWT"
        });
        options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            {
                new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Reference = new Microsoft.OpenApi.Models.OpenApiReference
                    {
                        Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    });

    builder.Services.AddValidatorsFromAssemblyContaining<Program>();

    // ── INFRAESTRUCTURA (estilo Tienda) ───────────────────────────────────────
    builder.Services
        .AddDatabases(builder.Configuration)
        .AddRepositories()
        .AddServices()
        .AddCache(builder.Configuration, builder.Environment)
        .AddStorage();

    // ── PIPELINE DE MIDDLEWARES ───────────────────────────────────────────────

    var app = builder.Build();
    var isDevelopment = app.Environment.IsDevelopment();

    await app.InitializeDatabaseAsync(isDevelopment);
    app.InitializeStorage(isDevelopment);

    app.UseSerilogRequestLogging();
    app.UseStaticFiles();

    app.UseGlobalExceptionHandler();

    // ── AUTHENTICATION + AUTHORIZATION ────────────────────────────────────────
    app.UseAuthentication();
    app.UseAuthorization();

    if (isDevelopment)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.MapControllers();

    // ── CONFIGURACIÓN TIPADA ──────────────────────────────────────────────────
    var apiConfig = app.Services.GetRequiredService<IOptions<ApiConfig>>().Value;

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
    Log.Fatal(ex, "La aplicación terminó de forma inesperada");
}
finally
{
    Log.CloseAndFlush();
}
