using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ProductosMicroService.Infrastructures;
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
    Log.Information("Iniciando ProductosMicroService...");

    // ── SERVICIOS ─────────────────────────────────────────────────────────────
    builder.Services.AddControllers(options =>
    {
        options.RespectBrowserAcceptHeader = true;
        options.ReturnHttpNotAcceptable = true;
    })
    .AddXmlDataContractSerializerFormatters();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // ── JWT (valida roles ADMIN en escrituras; la firma la emite AuthMicroService) ──
    var jwtKey = builder.Configuration["Jwt:Key"]
        ?? "SuperSecretKeyForJwtToken2024!SuperSecretKeyForJwtToken2024!";
    var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "AuthMicroService";
    var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "ApiGateway";

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            // El token usa ClaimTypes.Role (URI largo); mapearlo a roles de ASP.NET Core
            options.MapInboundClaims = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                RoleClaimType = System.Security.Claims.ClaimTypes.Role
            };
        });
    builder.Services.AddAuthorization();

    // ── INFRAESTRUCTURA ─────────────────────────────────────────────────────
    builder.Services
        .AddDatabase(builder.Configuration)
        .AddRepositories(builder.Configuration)
        .AddServices();

    // ── PIPELINE ─────────────────────────────────────────────────────────────
    var app = builder.Build();

    // ── Seed de datos ────────────────────────────────────────────────────────
    await app.InitializeDatabaseAsync();

    app.UseSerilogRequestLogging();
    app.UseAuthentication();
    app.UseAuthorization();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.MapControllers();

    Log.Information("ProductosMicroService http://localhost:5102");
    Log.Information("  Swagger: http://localhost:5102/swagger");
    Log.Information("  Entorno: {Entorno}", app.Environment.EnvironmentName);

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "ProductosMicroService termino inesperadamente");
}
finally
{
    Log.CloseAndFlush();
}
