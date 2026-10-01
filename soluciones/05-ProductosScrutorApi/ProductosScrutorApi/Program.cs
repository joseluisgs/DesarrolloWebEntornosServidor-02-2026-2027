using Scrutor;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.Scan(scan => scan
    .FromAssemblyOf<Program>()
    // Singleton porque el repositorio usa Dictionary en memoria.
    // Si fuera Scoped, cada request perdería los datos.
    // En producción con BD real, sería WithScopedLifetime().
    .AddClasses(classes => classes.Where(t => t.Name.EndsWith("Repository")))
        .AsImplementedInterfaces()
        .WithSingletonLifetime()
    .AddClasses(classes => classes.Where(t => t.Name.EndsWith("Service")))
        .AsImplementedInterfaces()
        .WithScopedLifetime()
);

var app = builder.Build();

app.MapControllers();

app.Run();
