using ProductosEFCore.Services.Email;
using Serilog;

namespace ProductosEFCore.Infrastructures;

/// <summary>
/// Configuracion del servicio de email.
/// Desarrollo: MemoryEmailService (no envia realmente).
/// Produccion: MailKitEmailService (envia emails reales).
/// </summary>
public static class EmailConfig
{
    public static IServiceCollection AddEmail(this IServiceCollection services, IWebHostEnvironment environment)
    {
        if (environment.IsDevelopment())
        {
            Log.Information("Configurando servicio de email en memoria (desarrollo)...");
            services.AddScoped<IEmailService, MemoryEmailService>();
        }
        else
        {
            Log.Information("Configurando servicio de email con MailKit (produccion)...");
            // services.AddScoped<IEmailService, MailKitEmailService>();
            services.AddScoped<IEmailService, MemoryEmailService>();
        }

        return services;
    }
}
