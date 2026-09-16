namespace Retrofit.Console.Models;

/// <summary>
/// Modelo para comentarios de JSONPlaceholder
/// </summary>
public class Comment
{
    public int PostId { get; set; }
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;

    public override string ToString() => $"[{Id}] {Name} - {Email}";
}
