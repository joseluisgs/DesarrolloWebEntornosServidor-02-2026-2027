using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("Iniciando ApiGateway...");

    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();

    // ── JWT Authentication ───────────────────────────────────────────────
    var jwtKey = builder.Configuration["Jwt:Key"] ?? "SuperSecretKeyForJwtToken2024!";
    var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "AuthMicroService";
    var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "ApiGateway";

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
            };
        });

    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("authenticated", policy =>
            policy.RequireAuthenticatedUser());
    });

    // ── YARP Reverse Proxy ───────────────────────────────────────────────
    builder.Services.AddReverseProxy()
        .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseAuthentication();
    app.UseAuthorization();

    // ── Mapeo del Reverse Proxy ──────────────────────────────────────────
    app.MapReverseProxy();

    Log.Information("ApiGateway http://localhost:5100");
    Log.Information("  - Auth:       http://localhost:5100/api/auth/*");
    Log.Information("  - Productos:  http://localhost:5100/api/productos/*");
    Log.Information("  - Proveedores: http://localhost:5100/api/proveedores/*");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "ApiGateway termino inesperadamente");
}
finally
{
    Log.CloseAndFlush();
}
