using System.Text.Json;
using System.Text.Json.Serialization;
using MongoDB.Bson;

namespace ProductosMongo.Converters;

/// <summary>
/// Convierte ObjectId de MongoDB a/string en JSON.
/// Sin esto, ObjectId se serializa como {"timestamp":..., "creationTime":...}.
/// </summary>
public class ObjectIdJsonConverter : JsonConverter<ObjectId>
{
    public override ObjectId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return string.IsNullOrEmpty(value) ? ObjectId.Empty : ObjectId.Parse(value);
    }

    public override void Write(Utf8JsonWriter writer, ObjectId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
