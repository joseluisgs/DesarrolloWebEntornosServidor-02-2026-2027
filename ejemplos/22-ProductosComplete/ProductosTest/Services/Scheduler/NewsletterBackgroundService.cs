using ProductosEFCore.Repositories;
using ProductosEFCore.Services.Email;

namespace ProductosEFCore.Services.Scheduler;

// Primary constructor (C#): los parámetros de DI quedan disponibles en toda
// la clase. No hacen falta campos privados ni asignaciones en el constructor.
// TimeProvider desacopla el reloj: en tests se inyecta un FakeTimeProvider y
// la espera hasta el proximo lunes ocurre en milisegundos (seccion 22.3.4).
/// <summary>
/// Tarea programada que envia un email semanal con los productos nuevos de los ultimos 7 dias.
/// Ejecuta todos los lunes a las 8:00 AM.
/// </summary>
public class NewsletterBackgroundService(
    IServiceProvider serviceProvider,
    ILogger<NewsletterBackgroundService> logger,
    TimeProvider timeProvider)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("NewsletterBackgroundService iniciado");

        while (!stoppingToken.IsCancellationRequested)
        {
            var now = timeProvider.GetUtcNow().UtcDateTime;
            var nextMonday = GetNextMonday(now);
            var delay = nextMonday - now;

            logger.LogInformation("Proximo envio de newsletter: {NextMonday}", nextMonday);
            await Task.Delay(delay, timeProvider, stoppingToken);

            if (stoppingToken.IsCancellationRequested) break;

            try
            {
                await SendNewsletterAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error enviando newsletter");
            }
        }
    }

    private async Task SendNewsletterAsync()
    {
        using var scope = serviceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IProductoRepository>();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

        var productos = repository.GetAll()
            .Where(p => !p.IsDeleted && p.CreatedAt >= timeProvider.GetUtcNow().UtcDateTime.AddDays(-7))
            .ToList();

        if (productos.Count == 0)
        {
            logger.LogInformation("No hay productos nuevos en los ultimos 7 dias");
            return;
        }

        var content = string.Join("", productos.Select(p => $@"
            <tr>
                <td style='padding: 8px;'>{p.Id}</td>
                <td style='padding: 8px;'>{p.Nombre}</td>
                <td style='padding: 8px;'>{p.Precio:N2} EUR</td>
            </tr>"));

        var body = EmailTemplates.CreateBase("Newsletter Semanal - Productos Nuevos", $@"
            <p>Se han detectado <strong>{productos.Count}</strong> productos nuevos en los ultimos 7 dias:</p>
            <table style='width: 100%; border-collapse: collapse; margin: 20px 0;'>
                <tr style='background: #f8f9fa;'>
                    <th style='padding: 8px;'>ID</th>
                    <th style='padding: 8px;'>Nombre</th>
                    <th style='padding: 8px;'>Precio</th>
                </tr>
                {content}
            </table>");

        await emailService.SendEmailAsync(new EmailMessage
        {
            To = "admin@productos.com",
            Subject = $"Newsletter Semanal - {productos.Count} productos nuevos",
            Body = body,
            IsHtml = true
        });

        logger.LogInformation("Newsletter enviado: {Count} productos nuevos", productos.Count);
    }

    private static DateTime GetNextMonday(DateTime now)
    {
        var daysUntilMonday = ((int)DayOfWeek.Monday - (int)now.DayOfWeek + 7) % 7;
        if (daysUntilMonday == 0) daysUntilMonday = 7;
        return now.Date.AddDays(daysUntilMonday).AddHours(8);
    }
}
