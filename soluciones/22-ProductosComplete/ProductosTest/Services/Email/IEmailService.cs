namespace ProductosEFCore.Services.Email;

/// <summary>
/// Mensaje de correo electronico.
/// </summary>
public class EmailMessage
{
    /// <summary>Destinatario.</summary>
    public string To { get; set; } = string.Empty;

    /// <summary>Asunto.</summary>
    public string Subject { get; set; } = string.Empty;

    /// <summary>Cuerpo del mensaje.</summary>
    public string Body { get; set; } = string.Empty;

    /// <summary>Indica si el cuerpo es HTML.</summary>
    public bool IsHtml { get; set; } = true;
}

/// <summary>
/// Contrato del servicio de email.
/// </summary>
public interface IEmailService
{
    /// <summary>Envia un email de forma inmediata.</summary>
    Task SendEmailAsync(EmailMessage message);

    /// <summary>Encola un email para envio posterior.</summary>
    Task EnqueueEmailAsync(EmailMessage message);
}
