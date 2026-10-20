using MongoDB.Bson;
using MongoDB.Driver;
using ProductosMongo.Models;

namespace ProductosMongo.Repositories.Native;

/// <summary>
/// Implementación del repositorio usando el Driver Nativo de MongoDB.
/// Opera directamente con IMongoCollection y Builders.
/// </summary>
public class ProductoNativeRepository(IMongoCollection<Producto> collection) : IProductoRepository
{
    public IEnumerable<Producto> GetAll() =>
        collection.Find(p => !p.IsDeleted).ToList();

    public Producto? GetById(string id)
    {
        var objectId = ObjectId.Parse(id);
        return collection.Find(p => p.Id == objectId && !p.IsDeleted).FirstOrDefault();
    }

    public Producto Add(Producto producto)
    {
        producto.CreatedAt = DateTime.UtcNow;
        producto.IsDeleted = false;
        collection.InsertOne(producto);
        return producto;
    }

    public Producto? Update(Producto producto)
    {
        producto.UpdatedAt = DateTime.UtcNow;
        var result = collection.ReplaceOne(
            p => p.Id == producto.Id && !p.IsDeleted,
            producto);
        return result.IsAcknowledged ? producto : null;
    }

    public bool Delete(string id)
    {
        var objectId = ObjectId.Parse(id);
        var update = Builders<Producto>.Update
            .Set(p => p.IsDeleted, true)
            .Set(p => p.UpdatedAt, DateTime.UtcNow);
        var result = collection.UpdateOne(
            p => p.Id == objectId && !p.IsDeleted,
            update);
        return result.IsAcknowledged && result.ModifiedCount > 0;
    }

    public bool IsDuplicated(string nombre, string excludeId = "")
    {
        var filterBuilder = Builders<Producto>.Filter;
        var filter = filterBuilder.And(
            filterBuilder.Eq(p => p.Nombre, nombre),
            filterBuilder.Eq(p => p.IsDeleted, false));

        if (!string.IsNullOrEmpty(excludeId))
        {
            var objectId = ObjectId.Parse(excludeId);
            filter = filterBuilder.And(filter,
                filterBuilder.Ne(p => p.Id, objectId));
        }

        return collection.CountDocuments(filter) > 0;
    }

    public IEnumerable<Producto> Search(string query)
    {
        var filter = Builders<Producto>.Filter.Or(
            Builders<Producto>.Filter.Regex(p => p.Nombre, new BsonRegularExpression(query, "i")),
            Builders<Producto>.Filter.Regex(p => p.Categoria.Nombre, new BsonRegularExpression(query, "i")));
        return collection.Find(filter).ToList();
    }

    public IEnumerable<Producto> Filter(string? nombre, decimal? precioMin, decimal? precioMax)
    {
        var filters = new List<FilterDefinition<Producto>>();
        if (!string.IsNullOrWhiteSpace(nombre))
            filters.Add(Builders<Producto>.Filter.Regex(p => p.Nombre, new BsonRegularExpression(nombre, "i")));
        if (precioMin.HasValue)
            filters.Add(Builders<Producto>.Filter.Gte(p => p.Precio, precioMin.Value));
        if (precioMax.HasValue)
            filters.Add(Builders<Producto>.Filter.Lte(p => p.Precio, precioMax.Value));
        var filter = filters.Count > 0 ? Builders<Producto>.Filter.And(filters) : FilterDefinition<Producto>.Empty;
        return collection.Find(filter).ToList();
    }

    public IEnumerable<Producto> GetDeleted()
    {
        return collection.Find(p => p.IsDeleted).ToList();
    }
}
