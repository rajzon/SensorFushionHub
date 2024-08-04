using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Devices.API.Core;

public abstract class Device
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; init; }
}

public sealed class Sensor : Device
{
    public string Name { get; private set; }

    public Sensor(string name)
    {
        Name = name;
    }
}