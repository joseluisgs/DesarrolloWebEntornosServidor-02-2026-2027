using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProductosMongo.Models;

/// <summary>
/// Proveedor como documento independiente de la colección 'proveedores'.
/// A diferencia de <see cref="Categoria"/>, NO se embebe dentro de Producto:
/// Producto solo guarda su identificador en <c>proveedor_id</c>.
/// </summary>
public class Proveedor
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [BsonElement("contacto")]
    public string Contacto { get; set; } = string.Empty;

    [BsonElement("pais")]
    public string Pais { get; set; } = string.Empty;
}
