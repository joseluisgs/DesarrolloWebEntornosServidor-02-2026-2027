using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProveedoresMicroService.Models;

/// <summary>
/// Modelo de dominio: Proveedor.
/// Almacena datos de proveedores de productos.
/// </summary>
public class Proveedor
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Telefono { get; set; } = string.Empty;

    public string Direccion { get; set; } = string.Empty;

    public bool Activo { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
