using FluentValidation;
using Microsoft.Extensions.Options;
using ProductosEFCore.Config;
using ProductosEFCore.Infrastructures;
using ProductosEFCore.Middleware;
using ProductosEFCore.Services.Scheduler;
using Serilog;
using Serilog.Events;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "ProductosAPI")
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File(
        path: "logs/productos-.log",
        rollingInterval: RollingInterval.Day,
        rollOnFileSizeLimit: true,
        fileSizeLimitBytes: 10_000_000,
        retainedFileCountLimit: 7,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File(
        path: "logs/productos-json-.log",
        rollingInterval: RollingInterval.Hour,
        formatter: new Serilog.Formatting.Json.JsonFormatter())
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(Log.Logger);

try
{
    Log.Information("Iniciando la aplicacion...");

    // ── CONFIGURACION TIPADA ─────────────────────────────────────────────────
    builder.Services.Configure<ApiConfig>(
        builder.Configuration.GetSection("Api"));

    // ── IDENTITY + JWT ──────────────────────────────────────────────────────
    builder.Services.AddIdentityConfiguration(builder.Configuration, builder.Environment);

    // ── SERVICIOS ────────────────────────────────────────────────────────────
    builder.Services.AddControllers(options =>
    {
        options.RespectBrowserAcceptHeader = true;
        options.ReturnHttpNotAcceptable = true;
    })
    .AddXmlDataContractSerializerFormatters();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new()
        {
            Title = "Productos API",
            Version = "v1",
            Description = "API REST completa para gestion de productos con Identity, File Storage, Email, Logging, y Optimizaciones.",
            Contact = new() { Name = "DAW", Email = "daw@ies.com" }
        });

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

        // Incluir comentarios XML solo si el fichero existe (evita FileNotFoundException)
        var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
        if (File.Exists(xmlPath))
        {
            options.IncludeXmlComments(xmlPath);
        }
    });

    builder.Services.AddValidatorsFromAssemblyContaining<Program>();

    // ── INFRAESTRUCTURA ──────────────────────────────────────────────────────
    builder.Services
        .AddDatabases(builder.Configuration)
        .AddRepositories()
        .AddServices()
        .AddCache(builder.Configuration, builder.Environment)
        .AddStorage()
        .AddEmail(builder.Environment);

    // ── OPTIMIZACIONES ───────────────────────────────────────────────────────
    builder.Services
        .AddResponseCompressionConfig()
        .AddRateLimitingConfig()
        .AddResilienceConfig()
        .AddHealthCheckConfig(builder.Configuration);

    // ── TAREAS PROGRAMADAS ───────────────────────────────────────────────────
    // ⚠️ TimeProvider NO viene registrado en DI por defecto (comprobado en .NET 10).
    builder.Services.AddSingleton(TimeProvider.System);
    builder.Services.AddHostedService<NewsletterBackgroundService>();

    // ── PIPELINE DE MIDDLEWARES ──────────────────────────────────────────────

    var app = builder.Build();
    var isDevelopment = app.Environment.IsDevelopment();

    await app.InitializeDatabaseAsync(isDevelopment);
    app.InitializeStorage(isDevelopment);

    app.UseSerilogRequestLogging();
    app.UseResponseCompression();
    app.UseStaticFiles();
    app.UseGlobalExceptionHandler();

    // ── AUTHENTICATION + AUTHORIZATION ───────────────────────────────────────
    app.UseAuthentication();
    app.UseAuthorization();

    // ── RATE LIMITING ────────────────────────────────────────────────────────
    app.UseRateLimiter();

    // ── SWAGGER (solo desarrollo) ────────────────────────────────────────────
    if (isDevelopment)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Productos API V1");
            options.RoutePrefix = "swagger";
        });
    }

    app.MapControllers();

    // ── HEALTH CHECKS ────────────────────────────────────────────────────────
    app.MapHealthCheckEndpoints();

    // ── CONFIGURACION TIPADA ─────────────────────────────────────────────────
    var apiConfig = app.Services.GetRequiredService<IOptions<ApiConfig>>().Value;

    var port = builder.Configuration["ASPNETCORE_URLS"]?.Split(':').LastOrDefault() ?? "5000";
    Console.WriteLine("========================================");
    Console.WriteLine($"  {apiConfig.Nombre}");
    Console.WriteLine($"  {apiConfig.Mensaje}");
    Console.WriteLine($"  http://localhost:{port}");
    Console.WriteLine($"  Swagger: http://localhost:{port}/swagger");
    Console.WriteLine($"  Health: http://localhost:{port}/health");
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
