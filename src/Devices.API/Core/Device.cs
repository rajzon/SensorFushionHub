using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Devices.API.Core;

public abstract class Device(DateTime createdDate)
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; init; } = null!;

    public DateTime CreatedDate { get; protected set; } = createdDate;
}