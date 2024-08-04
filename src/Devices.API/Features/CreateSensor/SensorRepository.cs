using Devices.API.Core;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Devices.API.Features.CreateSensor;

public interface ISensorRepository
{
    Task CreateAsync(Sensor sensor);
    Task<List<Sensor>> GetAllAsync();
}

internal sealed class SensorRepository : ISensorRepository
{
    private readonly IMongoCollection<Sensor> _sensorsCollection;
    
    public SensorRepository(IOptions<DevicesDatabaseSettings> devicesDatabaseSettings)
    {
        var mongoClient = new MongoClient(devicesDatabaseSettings.Value.ConnectionString);
        var database = mongoClient.GetDatabase(devicesDatabaseSettings.Value.DatabaseName);
        
        _sensorsCollection = database.GetCollection<Sensor>(devicesDatabaseSettings.Value.SensorsCollectionName);
    }
    
    public Task CreateAsync(Sensor sensor) =>
        _sensorsCollection.InsertOneAsync(sensor);
    
    public Task<List<Sensor>> GetAllAsync() =>
        _sensorsCollection.Find(_ => true).ToListAsync();
}

internal sealed class DevicesDatabaseSettings
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public string SensorsCollectionName { get; set; } = null!;
}