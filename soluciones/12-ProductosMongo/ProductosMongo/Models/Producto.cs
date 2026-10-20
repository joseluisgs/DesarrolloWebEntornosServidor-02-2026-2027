using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProductosMongo.Models;

/// <summary>
/// Modelo de producto con dos formas de relacionarse:
/// <c>Categoria</c> embebida (composición) y <c>ProveedorId</c> referenciado.
/// </summary>
public class Producto
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [BsonElement("precio")]
    public decimal Precio { get; set; }

    [BsonElement("imagen")]
    public string? Imagen { get; set; }

    // Composición: la categoría vive DENTRO del producto (un solo documento)
    [BsonElement("categoria")]
    public Categoria Categoria { get; set; } = null!;

    // Referencia: el proveedor vive en OTRA colección; solo guardamos su _id
    [BsonElement("proveedor_id")]
    public ObjectId ProveedorId { get; set; }

    [BsonElement("is_deleted")]
    public bool IsDeleted { get; set; }

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Categoría embebida dentro de cada producto.
/// [Owned] es lo que le dice a EF Core que esto va DENTRO del documento,
/// en una colección aparte. El Driver Nativo ignora el atributo: serializa
/// cualquier objeto anidado como subdocumento.
/// </summary>
[Owned]
public class Categoria
{
    [BsonElement("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [BsonElement("descripcion")]
    public string Descripcion { get; set; } = string.Empty;
}
