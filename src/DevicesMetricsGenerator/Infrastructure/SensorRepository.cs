using DevicesMetricsGenerator.Core;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace DevicesMetricsGenerator.Infrastructure;

internal sealed class SensorRepository : ISensorRepository
{
    private readonly IMongoCollection<Sensor> _sensorsCollection;
    
    public SensorRepository(IMongoClient mongoClient, IOptions<DevicesMetricsGeneratorDatabaseSettings> options)
    {
        var database = mongoClient.GetDatabase(options.Value.DatabaseName);
        _sensorsCollection = database.GetCollection<Sensor>(options.Value.SensorsCollectionName);
    }
    
    public Task CreateAsync(Sensor sensor) =>
        _sensorsCollection.InsertOneAsync(sensor);
    
    public Task<List<Sensor>> GetAllAsync() =>
        _sensorsCollection.Find(_ => true).ToListAsync();
}