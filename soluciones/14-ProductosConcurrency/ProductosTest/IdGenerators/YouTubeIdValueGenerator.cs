using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace ProductosEFCore.IdGenerators;

/// <summary>
/// Genera IDs de 11 caracteres estilo YouTube (Base64url).
/// </summary>
public class YouTubeIdValueGenerator : ValueGenerator<string>
{
    private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_";
    private static readonly Random Random = new();

    public override string Next(EntityEntry entry) =>
        new string(Enumerable.Range(0, 11).Select(_ => Chars[Random.Next(64)]).ToArray());

    public override bool GeneratesTemporaryValues => false;
}
