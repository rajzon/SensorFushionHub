using Devices.API.Core;
using NRedisStack;
using NRedisStack.DataTypes;
using NRedisStack.RedisStackCommands;
using StackExchange.Redis;

namespace Devices.API.Consumers;
//TODO move somewhere
public class MetricRepository(IConnectionMultiplexer redis) : IMetricRepository
{
    public bool IsKeyExist(string key)
    {
        var db = redis.GetDatabase();
        return db.KeyExists(key);
    }
    
    public Task CreateKeyAsync(string key)
    {
        var db = redis.GetDatabase();
        var timeSeries = db.TS();
        return timeSeries.CreateAsync(key, new TsCreateParamsBuilder().build());
    }
    
    public Task<IReadOnlyList<TimeStamp>> AddAsync(IReadOnlyList<TsSensorMetric> metrics)
    {
        var db = redis.GetDatabase();
        var timeSeries = db.TS();
        return timeSeries.MAddAsync(metrics
            .Select(m => (m.Key, TimeStamp: new TimeStamp(m.CreatedDate), m.Value))
            .ToList()
        );
    }
}