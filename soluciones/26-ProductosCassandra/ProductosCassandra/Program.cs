using Cassandra;
using ProductosCassandra.Config;
using ProductosCassandra.Infrastructures;
using ProductosCassandra.Seed;
using Serilog;
using Serilog.Events;
using ISession = Cassandra.ISession;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File(
        path: "logs/productos-cassandra-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(Log.Logger);

try
{
    Log.Information("Iniciando la aplicacion...");

    // ── CONTROLADORES (MVC, mismo patron que el resto de la UD) ──────────────
    builder.Services.AddControllers();

    // ── SWAGGER ──────────────────────────────────────────────────────────────
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new()
        {
            Title = "Productos Cassandra API",
            Version = "v1",
            Description = "Modelado por consultas (query-first) sobre Apache Cassandra: " +
                          "cada endpoint responde a una tabla diseñada para el.",
            Contact = new() { Name = "DAW", Email = "daw@ies.com" }
        });

        // Incluir comentarios XML solo si el fichero existe (evita FileNotFoundException)
        var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
        if (File.Exists(xmlPath))
        {
            options.IncludeXmlComments(xmlPath);
        }
    });

    // ── INFRAESTRUCTURA (estilo Tienda) ──────────────────────────────────────
    builder.Services
        .AddCassandra(builder.Configuration)
        .AddRepositories()
        .AddServices();

    var app = builder.Build();

    // Esquema y datos: en Cassandra no hay migraciones, solo CREATE IF NOT EXISTS.
    var settings = app.Services.GetRequiredService<CassandraSettings>();
    await app.Services.GetRequiredService<ISession>().InitializeSchemaAsync(settings.Keyspace);

    if (app.Environment.IsDevelopment())
    {
        await CassandraSeeder.SeedAsync(app.Services.GetRequiredService<ISession>());
    }

    // ── PIPELINE ─────────────────────────────────────────────────────────────
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.MapControllers();

    // ── BANNER ───────────────────────────────────────────────────────────────
    var port = builder.Configuration["ASPNETCORE_URLS"]?.Split(':').LastOrDefault() ?? "5000";
    var nombre = builder.Configuration["Api:Nombre"] ?? "Productos Cassandra API";
    Console.WriteLine("========================================");
    Console.WriteLine($"  {nombre}");
    Console.WriteLine($"  http://localhost:{port}");
    Console.WriteLine($"  Swagger: http://localhost:{port}/swagger");
    Console.WriteLine($"  Entorno: {app.Environment.EnvironmentName}");
    Console.WriteLine("========================================");

    Log.Information("API {Nombre} lista", nombre);

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
