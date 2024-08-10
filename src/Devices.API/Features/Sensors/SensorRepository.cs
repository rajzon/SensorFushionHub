using Devices.API.Core;
using Devices.API.Features.Sensors.Abstract;
using Devices.API.Infrastructure;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Devices.API.Features.Sensors;

internal sealed class SensorRepository : ISensorRepository
{
    private readonly IMongoCollection<Sensor> _sensorsCollection;
    
    public SensorRepository(IMongoClient mongoClient, IOptions<DevicesDatabaseSettings> options)
    {
        var database = mongoClient.GetDatabase(options.Value.DatabaseName);
        _sensorsCollection = database.GetCollection<Sensor>(options.Value.SensorsCollectionName);
    }
    
    public Task CreateAsync(Sensor sensor) =>
        _sensorsCollection.InsertOneAsync(sensor);
    
    public Task<List<Sensor>> GetAllAsync() =>
        _sensorsCollection.Find(_ => true).ToListAsync();
    
    public Task<Sensor?> GetAsync(string id) =>
        _sensorsCollection.Find(s => s.Id == id).FirstOrDefaultAsync();
}