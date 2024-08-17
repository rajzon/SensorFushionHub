using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DevicesMetricsGenerator.Core;

public sealed class Sensor
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; }

    public string SensorId { get; private set; }

    public Sensor(string sensorId)
    {
        SensorId = sensorId;
    }
}