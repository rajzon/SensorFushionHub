using Contracts.DevicesMetricsGenerator;
using MassTransit;
using NRedisStack;
using NRedisStack.DataTypes;
using NRedisStack.RedisStackCommands;
using StackExchange.Redis;

namespace Devices.API.Consumers;

public sealed class SensorAddedMetricsEventConsumer(
    ILogger<SensorAddedMetricsEventConsumer> logger,
    IMetricRepository metricRepository) : IConsumer<SensorAddedMetricsEvent>
{
    public async Task Consume(ConsumeContext<SensorAddedMetricsEvent> context)
    {
        //TODO add traces and metrics
        //This logs probably not needed if i will add Traces from MassTransit
        logger.LogInformation($"{nameof(SensorAddedMetricsEventConsumer)} started");
        var tsToAdd = context.Message.Metrics.Select(m =>
            new TsSensorMetric(context.Message.SensorId, m.Type, m.CreatedDate, m.Value))
            .ToList();
        foreach (var ts in tsToAdd)
        {
            if (!metricRepository.IsKeyExist(ts.Key))
            {
                await metricRepository.CreateKeyAsync(ts.Key);
            }
        }
        await metricRepository.AddAsync(tsToAdd);
        logger.LogInformation($"{nameof(SensorAddedMetricsEventConsumer)} consumed");
    }
}

public interface IMetricRepository
{
    bool IsKeyExist(string key);
    Task CreateKeyAsync(string key);
    Task<IReadOnlyList<TimeStamp>> AddAsync(IReadOnlyList<TsSensorMetric> metrics);
}

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

//TODO move to Core
public record TsSensorMetric
{
    //TODO needed KeyPrefix?
    public string KeyPrefix { get; }
    public string Key { get; }
    public DateTime CreatedDate { get; }
    public double Value { get; }
    
    public TsSensorMetric(string sensorId, SensorType sensorType, DateTime createdDate, double value)
    {
        Validate(createdDate);
        
        CreatedDate = createdDate;
        Value = value;
        KeyPrefix = sensorType switch
        {
            SensorType.Temperature => "temperature",
            SensorType.Co2 => "co2",
            SensorType.No2 => "no2",
            SensorType.Pm10 => "pm10",
            SensorType.Pm2_5 => "pm2_5",
            SensorType.O3 => "o3",
            _ => throw new Exception($"Unknown type: {sensorType}")
        };

        Key = $"{KeyPrefix}:{sensorId}";
    }

    private void Validate(DateTime createdDate)
    {
        if (createdDate.Kind is not DateTimeKind.Utc)
        {
            //TODO create custom exception
            throw new ArgumentException("Created date must be UTC.", nameof(createdDate));
        }
    }
}